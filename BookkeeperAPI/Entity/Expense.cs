namespace BookkeeperAPI.Entity
{
    #region usings
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text.Json.Serialization;
    using BookkeeperAPI.Constants;
    #endregion

    [Table("expense")]
    public class Expense
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("category")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ExpenseCategory Category { get; set; }

        [Column("amount")]
        public double Amount { get ; set; }


        [Column("user_id")]
        public Guid UserId { get; set; }

        public User User { get; set; }
    }
}
