using System.Text.Json.Serialization;

namespace BookkeeperAPI.Model
{

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ExpenseCategory
    {
        Shopping,
        Food,
        Housing,
        Transportation,
        Vehicle,
        Entertainment,
        Communication,
        FinancialExpenses,
        Investment
    }

    public class Expense
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public string Name { get; set; }

        public ExpenseCategory Category { get; set; }

        public double Amount { get ; set; }

        public string GetCategory(ExpenseCategory category)
        {
            switch (category)
            {
                case ExpenseCategory.Shopping:
                    return "Shopping";
                case ExpenseCategory.Food:
                    return "Food & Beverages";
                case ExpenseCategory.Housing:
                    return "Housing";
                case ExpenseCategory.Transportation:
                    return "Transportation";
                case ExpenseCategory.Vehicle:
                    return "Vehicle";
                case ExpenseCategory.Entertainment:
                    return "Entertainment";
                case ExpenseCategory.Communication:
                    return "Communication";
                case ExpenseCategory.FinancialExpenses:
                    return "Financial Expenses";
                case ExpenseCategory.Investment:
                    return "Investment";
                default:
                    throw new ArgumentOutOfRangeException(nameof(category));
            }
        }
    }
}
