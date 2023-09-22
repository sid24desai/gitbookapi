namespace BookkeeperAPI.Entity
{
    #region usings
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    #endregion

    [Table("users")]
    public class User
    {
        [Column("id")]
        [Key]
        public Guid Id { get; set; }

        [Column(name: "preference", TypeName = "jsonb")]
        public UserPreference? Preferences { get; set; }


        public UserCredential Credential { get; set; }
        
        public IEnumerable<Expense>? Expenses { get; set; }
    }
}
