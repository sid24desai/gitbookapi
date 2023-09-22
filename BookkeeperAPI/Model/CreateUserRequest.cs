using BookkeeperAPI.Entity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookkeeperAPI.Model
{
    public class CreateUserRequest
    {
        [MaxLength(100)]
        public string DisplayName { get; set; }

        public UserPreference UserPreference { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [PasswordPropertyText]
        [MaxLength(25)]
        [MinLength(8)]
        public string Password { get; set; }
    }
}
