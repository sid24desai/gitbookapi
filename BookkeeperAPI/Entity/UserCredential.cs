namespace BookkeeperAPI.Entity
{
    #region usings
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    #endregion

    [Table("user_credential")]
    public class UserCredential
    {
        [Column("id")]
         
        public Guid Id { get; set; }

        [Column("display_name")]
        [MaxLength(100)]
        public string? DisplayName { get; set; }

        [Column("email")]
        [EmailAddress]
        public string? Email { get; set; }

        [Column("password")]
        [PasswordPropertyText(true)]
        [MinLength(8)]
        public string? Password { get; set; }

        [Column("last_updated")]
        public DateTime LastUpdated { get; set; } 

        [Column("created_time")]
        public DateTime CreatedTime { get; set; }



        [Column("user_id")]
        public Guid UserId { get; set; }

        public User? User { get; set; }
    }
}
