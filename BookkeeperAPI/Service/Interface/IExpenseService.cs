namespace BookkeeperAPI.Service.Interface
{
    #region usings
    using BookkeeperAPI.Constants;
    using BookkeeperAPI.Model;
    #endregion

    public interface IExpenseService
    {
        public Task<PaginatedResult<ExpenseView>> GetPaginatedExpensesAsync(Guid userId, string domain, int pageNumber, int pageSize, ExpenseCategory? category, string? name, DateTime? from, DateTime? to);

        public Task<ExpenseView> GetExpenseByIdAsync(Guid expenseId);

        public Task<ExpenseView> CreateExpenseAsync(Guid userId, CreateExpenseRequest request);

        public Task<ExpenseView> UpdateExpenseAsync(Guid expenseId, UpdateExpenseRequest request);

        public Task DeleteExpenseAsync(Guid expenseId);
    }
}
