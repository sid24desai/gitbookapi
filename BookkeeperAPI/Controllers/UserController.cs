namespace BookkeeperAPI.Controllers
{

    #region usings
    using BookkeeperAPI.Data;
    using BookkeeperAPI.Entity;
    using BookkeeperAPI.Model;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    #endregion

    [ApiController]
    [Route("/api/users")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        // TODO(BOOKA-31): Move business logic to service layer and Database logic to Repository layer
        private BookkeeperContext _context;
        public UserController(BookkeeperContext context)
        {
            _context = context;
        }

        [HttpGet("/api/me/account")]
        public IActionResult GetUser([FromQuery] Guid userId)
        {
            UserView? user = _context.Users.Where(x => x.Id.Equals(userId)).Select(x => new UserView()
            {
                Id = x.Id,
                DisplayName = x.Credential.DisplayName,
                Email = x.Credential.Email,
                Preferences = x.Preferences!
            }).FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost("/api/users/new")]
        public IActionResult CreateUser([FromBody] CreateUserRequest request)
        {
            User user = new User();
            user.Preferences = request.UserPreference;
            EntityEntry<User> u = _context.Users.Add(user);
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
            _context.Credentials.Add(user.Credential);
            _context.SaveChanges();
            return StatusCode(201, u.Entity);
        }

        [HttpPatch("/api/me/preference")]
        public IActionResult UpdateUserPreference([FromQuery] Guid userId, UserPreference preference)
        {
            UserView? user = _context.Users
                .Where(x => x.Id.Equals(userId))
                .Select(x => new UserView()
                {
                    Id = x.Id,
                    DisplayName = x.Credential.DisplayName,
                    Email = x.Credential.Email,
                    Preferences = x.Preferences
                })
                .FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            user.Preferences = preference;
            _context.SaveChanges();

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPatch("/api/me/account/password")]
        public IActionResult CreatePasswordResetToken([FromBody] CreatePasswordResetTokenRequest request)
        {
            User? user = _context.Users
                .Where(x => x.Credential.Email == request.Email)
                .Select(x => new User()
                {
                    Id = x.Id,
                    Credential = x.Credential
                }).ToList().FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            // TODO(BOOKA-25): Update logic to create a redirection link with token as query parameter
            user.Credential.Password = "PasswordReset";
            user.Credential.LastUpdated = DateTime.UtcNow;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("/api/me/account")]
        public IActionResult DeleteUser()
        {
            // TODO(BOOKA-29): Update logic to select user based on user id found in access token
            _context.Users.Remove(_context.Users.FirstOrDefault());
            _context.SaveChanges();

            return NoContent();
        }
    }
}
