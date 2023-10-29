namespace BookkeeperAPI.Controllers
{

    #region usings
    using BookkeeperAPI.Data;
    using BookkeeperAPI.Entity;
    using BookkeeperAPI.Model;
    using BookkeeperAPI.Service.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using Microsoft.Identity.Client;
    #endregion

    [ApiController]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("/api/me/account")]
        public async Task<ActionResult<UserView>> GetUser([FromQuery] Guid userId)
        {
            if(userId == Guid.Empty)
            {
                return BadRequest();
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
        public async Task<ActionResult<UserView>> UpdateUserPreference([FromQuery] Guid userId, UserPreference preference)
        {
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
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            // TODO(BOOKA-29): Update logic to select user based on user id found in access token
            await _userService.DeleteUserAsync(userId);

            return StatusCode(StatusCodes.Status204NoContent, null);
        }
    }
}
