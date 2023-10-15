namespace BookkeeperAPI.Model
{
    #region usings
    using BookkeeperAPI.Constants;
    #endregion

    public class UpdateExpenseRequest
    {
        public string? Name { get; set; }

        public ExpenseCategory? Category { get; set; }

        public double? Amount { get; set; }

        public DateTime? Date { get; set; }
    }
}
