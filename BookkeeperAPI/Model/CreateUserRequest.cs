namespace BookkeeperAPI.Model
{
    #region usings
    using BookkeeperAPI.Entity;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    #endregion

    public class CreateUserRequest
    {
        [MaxLength(100)]
        public string? DisplayName { get; set; }

        public UserPreference? UserPreference { get; set; }

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [PasswordPropertyText(true)]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }
}
