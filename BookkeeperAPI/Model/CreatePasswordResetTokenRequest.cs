namespace BookkeeperAPI.Model
{
    #region usings
    using System.ComponentModel.DataAnnotations;
    #endregion

    public class CreatePasswordResetTokenRequest
    {
        [EmailAddress]
        public string? Email { get; set; }

        public string? Token { get; set; }
    }
}
