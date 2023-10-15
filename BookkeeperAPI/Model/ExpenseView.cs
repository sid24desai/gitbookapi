namespace BookkeeperAPI.Model
{
    public class ExpenseView
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public double Amount { get; set; }
    }
}
