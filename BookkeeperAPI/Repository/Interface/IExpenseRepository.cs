using BookkeeperAPI.Constants;
using BookkeeperAPI.Entity;
using BookkeeperAPI.Model;

namespace BookkeeperAPI.Repository.Interface
{
    public interface IExpenseRepository
    {
        public Task<List<ExpenseView>> GetExpensesAsync(Guid userId, int pageNumber, int pageSize, ExpenseCategory? category, string? name, DateTime? from, DateTime? to);

        public Task<int> GetExpenseCountAsync(Guid userId, ExpenseCategory? category, string? name, DateTime? from, DateTime? to);

        public Task<Expense?> GetExpenseByIdAsync(Guid expenseId);

        public Task SaveExpenseAsync(Expense expense);

        public Task SaveChangesAsync();

        public Task DeleteExpenseAsync(Expense expense);
    }
}
