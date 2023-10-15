namespace BookkeeperAPI.Model
{
    public class CreateExpenseRequest
    {
        public string Name { get; set; }

        public string Category { get; set; }

        public double Amount { get; set; }

        public DateTime Date { get; set; }
    }
}
