namespace BookkeeperAPI.Controllers
{

    #region usings
    using BookkeeperAPI.Data;
    using BookkeeperAPI.Entity;
    using BookkeeperAPI.Model;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    #endregion

    [ApiController]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        // TODO(BOOKA-31): Move business logic to service layer and Database logic to Repository layer
        private readonly BookkeeperContext _context;
        public UserController(BookkeeperContext context)
        {
            _context = context;
        }

        [HttpGet("/api/me/account")]
        public async Task<ActionResult<UserView>> GetUser([FromQuery] Guid userId)
        {
            if(userId == Guid.Empty)
            {
                return BadRequest();
            }

            UserView? user = await _context.Users.Where(x => x.Id.Equals(userId)).Select(x => new UserView()
            {
                Id = x.Id,
                DisplayName = x.Credential.DisplayName,
                Email = x.Credential.Email,
                Preferences = x.Preferences!
            }).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost("/api/users/new")]
        public async Task<ActionResult<UserView>> CreateUser([FromBody] CreateUserRequest request)
        {
            User user = new User();
            user.Preferences = request.UserPreference;
            EntityEntry<User> u = await _context.Users.AddAsync(user);
            user.Credential = new UserCredential()
            {
                User = u.Entity,
                UserId = u.Entity.Id,
                DisplayName = request.DisplayName,
                Password = request.Password,
                Email = request.Email,
                LastUpdated = DateTime.UtcNow,
                CreatedTime = DateTime.UtcNow,
            };

            await _context.Credentials.AddAsync(user.Credential);

            await _context.SaveChangesAsync();

            return StatusCode(201, new UserView
                {
                    Id = u.Entity.Id,
                    Preferences = u.Entity.Preferences!,
                    DisplayName = u.Entity.Credential.DisplayName,
                    Email = u.Entity.Credential.Email,
                }
            );
        }

        [HttpPatch("/api/me/preference")]
        public async Task<ActionResult<UserView>> UpdateUserPreference([FromQuery] Guid userId, UserPreference preference)
        {
            UserView? user = await _context.Users
                .Where(x => x.Id.Equals(userId))
                .Select(x => new UserView()
                {
                    Id = x.Id,
                    DisplayName = x.Credential.DisplayName,
                    Email = x.Credential.Email,
                    Preferences = x.Preferences!
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            user.Preferences = preference;
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPatch("/api/me/account/password")]
        public async Task<IActionResult> CreatePasswordResetToken([FromBody] CreatePasswordResetTokenRequest request)
        {
            User? user = await _context.Users
                .Where(x => x.Credential.Email == request.Email)
                .Select(x => new User()
                {
                    Id = x.Id,
                    Credential = x.Credential
                }).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            // TODO(BOOKA-25): Update logic to create a redirection link with token as query parameter
            user.Credential.Password = "PasswordReset";
            user.Credential.LastUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("/api/me/account")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            // TODO(BOOKA-29): Update logic to select user based on user id found in access token
            User? _user = await _context.Users.Where(x => x.Id.Equals(userId)).FirstOrDefaultAsync();

            if(_user == null) 
            {
                return NotFound();
            }

            _context.Users.Remove(_user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
