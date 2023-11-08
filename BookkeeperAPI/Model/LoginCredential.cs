namespace BookkeeperAPI.Model
{
    #region usings
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    #endregion

    public class LoginCredential
    {
        [EmailAddress]
        [Required]
        public string? Email { get; set; }

        [PasswordPropertyText(true)]
        [Required]
        public string? Password { get; set; }
    }
}
