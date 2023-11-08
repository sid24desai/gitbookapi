namespace BookkeeperAPI.Controllers
{
    #region usings
    using BookkeeperAPI.Entity;
    using BookkeeperAPI.Model;
    using BookkeeperAPI.Repository.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;
    using System.ComponentModel.DataAnnotations;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
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
                return BadRequest();
            }

            User? userInfo = await _userRepository.GetUserByEmailAsync(credential);

            if (userInfo == null)
            {
                return BadRequest("Invalid credentials");
            }

            Claim[] claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("user_id", userInfo.Id.ToString()),
                new Claim("display_name", userInfo.Credential!.DisplayName!),
                new Claim("user_name", userInfo.Credential.Email!)
            };

            DateTime expiresAt = DateTime.UtcNow.AddMinutes(20);

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration[_configuration["Jwt:Key"]!]!));
            SigningCredentials singIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuration[_configuration["Jwt:Iss"]!]!,
                audience: _configuration[_configuration["Jwt:Aud"]!]!,
                claims: claims,
                signingCredentials: singIn,
                expires: expiresAt,
                notBefore: DateTime.UtcNow
            );

            string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new JwtAccessToken()
            {
                AccessToken = accessToken,
                ExpiresAt = expiresAt,
                TokenId = Guid.NewGuid()
            });
        }
    }
}
