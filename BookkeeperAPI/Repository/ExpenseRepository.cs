namespace BookkeeperAPI.Repository
{
    #region usings
    using BookkeeperAPI.Constants;
    using BookkeeperAPI.Data;
    using BookkeeperAPI.Entity;
    using BookkeeperAPI.Exceptions;
    using BookkeeperAPI.Model;
    using BookkeeperAPI.Repository.Interface;
    using Microsoft.EntityFrameworkCore;
    #endregion
    public class ExpenseRepository : IExpenseRepository
    {
        private BookkeeperContext _context;

        public ExpenseRepository(BookkeeperContext context)
        {
            _context = context;
        }

        public async Task<List<ExpenseView>> GetExpensesAsync(Guid userId, int pageNumber, int pageSize, ExpenseCategory? category, string? name, DateTime? from, DateTime? to)
        {
            List<ExpenseView> data = await _context.Expenses
                .Where(x => (
                    x.UserId.Equals(userId) &&
                    (x.Category == category || category == null) &&
                    (name == null || x.Name.Contains(name)) &&
                    (from == null || x.Date >= from) &&
                    (to == null || x.Date <= to)
                    )
                )
                .Select(x => new ExpenseView()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Amount = x.Amount,
                    Category = x.Category.ToString(),
                    Date = x.Date,
                })
                .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
                .ToListAsync();

            return data;
        }

        public async Task<int> GetExpenseCountAsync(Guid userId, ExpenseCategory? category, string? name, DateTime? from, DateTime? to)
        {
            int totalCount = await _context.Expenses
                .Where(x => (
                    x.UserId.Equals(userId) &&
                    (x.Category == category || category == null) &&
                    (name == null || x.Name.Contains(name)) &&
                    (from == null || x.Date >= from) &&
                    (to == null || x.Date <= to)
                    )
                )
                .CountAsync();

            return totalCount;
        }

        public async Task<Expense?> GetExpenseByIdAsync(Guid expenseId)
        {
            Expense? expense = await _context.Expenses
                .Where(x => x.Id.Equals(expenseId))
                .FirstOrDefaultAsync();

            return expense;
        }


        public async Task SaveExpenseAsync(Expense expense)
        {
            await _context.Expenses.AddAsync(expense);

            int result =  await _context.SaveChangesAsync();

            if(result == 0 )
            {
                throw new HttpOperationException("Something went wrong");
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteExpenseAsync(Expense expense)
        {
            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
        }
    }
}
