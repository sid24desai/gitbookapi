namespace BookkeeperAPI.Controllers
{

    #region usings
    using BookkeeperAPI.Data;
    using BookkeeperAPI.Entity;
    using BookkeeperAPI.Exceptions;
    using BookkeeperAPI.Model;
    using BookkeeperAPI.Service;
    using BookkeeperAPI.Service.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using Microsoft.Identity.Client;
    using System.ComponentModel.DataAnnotations;
    using System.Net.Mail;
    #endregion

    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpGet("/api/me/account")]
        public async Task<ActionResult<UserView>> GetUser()
        {
            Guid userId;
            string userIdClaim = HttpContext.User.Claims.Where(x => x.Type == "user_id").First().Value.ToString();
            bool isValidUserId = Guid.TryParse(userIdClaim, out userId);

            if (!isValidUserId)
            {
                throw new HttpOperationException(401, "Unauthorized");
            }

            UserView user = await _userService.GetUserByIdAsync(userId);

            return Ok(user);
        }

        [HttpPost("/api/users/new")]
        public async Task<ActionResult<UserView>> CreateUser([FromBody] CreateUserRequest request)
        {
            UserView user = await _userService.CreateNewUserAsync(request);

            return StatusCode(StatusCodes.Status201Created, user);
        }

        [HttpPatch("/api/me/preference")]
        public async Task<ActionResult<UserView>> UpdateUserPreference(UserPreference preference)
        {
            Guid userId;
            string userIdClaim = HttpContext.User.Claims.Where(x => x.Type == "user_id").First().Value.ToString();
            bool isValidUserId = Guid.TryParse(userIdClaim, out userId);

            if (!isValidUserId)
            {
                throw new HttpOperationException(401, "Unauthorized");
            }

            UserView user = await _userService.UpdateUserPreferenceAsync(userId, preference);

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPatch("/api/me/account/password")]
        public async Task<ActionResult> CreatePasswordResetToken([FromBody] CreatePasswordResetTokenRequest request)
        {
            await _userService.CreatePasswordResetTokenAsync(request);
            return StatusCode(StatusCodes.Status204NoContent, null);
        }

        [HttpDelete("/api/me/account")]
        public async Task<ActionResult> DeleteUser()
        {
            Guid userId;
            string userIdClaim = HttpContext.User.Claims.Where(x => x.Type == "user_id").First().Value.ToString();
            bool isValidUserId = Guid.TryParse(userIdClaim, out userId);

            if (!isValidUserId)
            {
                throw new HttpOperationException(401, "Unauthorized");
            }

            await _userService.DeleteUserAsync(userId);

            return StatusCode(StatusCodes.Status204NoContent, null);
        }

        [AllowAnonymous]
        [HttpPost("/api/users/new/otp")]
        public async Task<ActionResult> GetOtpForEmail([FromBody] [Required] string email)
        {
            string body = System.IO.File.ReadAllText("EmailTemplates/OtpValidation.htm");
            body = body.Replace("[User]", email);
            body = body.Replace("[OTP_CODE]", OneTimePassword.Generate().ToString());
            MailMessage message = new MailMessage()
            {
                From = new MailAddress(_configuration[_configuration["Email:Address"]!]!, "Bookkeeper"),
                Subject = "OTP for validating Bookkeeper account",
                IsBodyHtml = true,
                Body = body
            };
            message.To.Add(new MailAddress(email));
            await EmailService.SendEmail(new LoginCredential()
            {
                Email = _configuration[_configuration["Email:Address"]!],
                Password = _configuration[_configuration["Email:Password"]!]
            }, message);

            return Ok();
        }
    }
}
