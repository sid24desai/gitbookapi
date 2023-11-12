namespace BookkeeperAPI.Controllers
{
    #region usings
    using BookkeeperAPI.Entity;
    using BookkeeperAPI.Exceptions;
    using BookkeeperAPI.Model;
    using BookkeeperAPI.Repository.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;
    using System.ComponentModel.DataAnnotations;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;
    #endregion

    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public AuthenticationController(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        [HttpPost("/api/oauth2/token")]
        [ProducesResponseType(StatusCodes.Status200OK, Type =typeof(JwtAccessToken))]
        [ProducesErrorResponseType(typeof(ErrorResponseModel))]
        public async Task<ActionResult<string>> GetToken([FromBody][Required] LoginCredential credential)
        {
            if (credential == null || credential.Email == null || credential.Password == null)
            {
                throw new HttpOperationException(400, "Bad Request");
            }

            User? userInfo = await _userRepository.GetUserByEmailAsync(credential);

            if (userInfo == null)
            {
                throw new HttpOperationException(400, "Invalid credentials");
            }

            Claim[] claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("user_id", userInfo.Id.ToString()),
                new Claim("display_name", userInfo.Credential!.DisplayName!),
                new Claim("user_name", userInfo.Credential.Email!),
                new Claim("nbf", ToUnixEpoch(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64),
                new Claim("iat", ToUnixEpoch(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64)
            };

            DateTime expiresAt = DateTime.UtcNow.AddMinutes(30);
            RSA rsa = RSA.Create();
            rsa.FromXmlString(Encoding.UTF8.GetString(Convert.FromBase64String(_configuration[_configuration["RSA:Key:Private"]!]!)));
            SigningCredentials singIn = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuration[_configuration["Jwt:Iss"]!]!,
                audience: _configuration[_configuration["Jwt:Aud"]!]!,
                claims: claims,
                signingCredentials: singIn,
                expires: expiresAt
            );

            string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new JwtAccessToken()
            {
                AccessToken = accessToken,
                ExpiresAt = expiresAt,
                TokenId = Guid.NewGuid()
            });
        }

        private static long ToUnixEpoch(DateTime date)
        {   
            return (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
        }
    }
}
