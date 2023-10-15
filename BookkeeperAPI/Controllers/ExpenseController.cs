namespace BookkeeperAPI.Controllers
{
    #region usings
    using BookkeeperAPI.Data;
    using BookkeeperAPI.Entity;
    using BookkeeperAPI.Model;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using BookkeeperAPI.Constants;
    using BookkeeperAPI.Utility;
    #endregion

    [ApiController]
    [Produces("application/json")]
    public class ExpenseController : ControllerBase
    {
        private readonly BookkeeperContext _context;
        public ExpenseController(BookkeeperContext context)
        {
            _context = context;
        }

        [HttpGet("/api/me/expenses")]
        public async Task<ActionResult<List<ExpenseView>>> GetExpense(Guid userId)
        {
            if(userId == Guid.Empty)
            {
                return BadRequest();
            }

            return Ok(
                await _context.Expenses
                .Where(x => x.UserId.Equals(userId))
                .Select(x => new ExpenseView()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Amount = x.Amount,
                    Category = x.Category.ToString(),
                    Date = x.Date,
                }).ToListAsync());
        }

        [HttpGet("/api/expenses/{expenseId}")]
        public async Task<ActionResult<ExpenseView>> GetExpenseById(Guid expenseId)
        {
            return Ok(
                await _context.Expenses
                .Where(x => x.Id.Equals(expenseId))
                .Select(x => new ExpenseView()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Amount = x.Amount,
                    Category = x.Category.ToString(),
                    Date = x.Date,
                })
                .FirstOrDefaultAsync());
        }

        [HttpPost("/api/expenses")]
        public async Task<ActionResult<ExpenseView>> CreateExpense(Guid userId, CreateExpenseRequest expense)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            Expense _expense = new Expense();
            _expense.UserId = userId;
            _expense.Name = expense.Name;
            _expense.Amount = expense.Amount;
            _expense.Category = expense.Category.ToEnum<ExpenseCategory>();
            _expense.Date = expense.Date;

            await _context.Expenses.AddAsync(_expense);

            await _context.SaveChangesAsync();

            ExpenseView result = new ExpenseView()
            {
                Id = _expense.Id,
                Name = _expense.Name,
                Category = _expense.Category.ToString(),
                Amount = _expense.Amount,
                Date = _expense.Date,
            };

            return Ok(result);
        }

        [HttpPatch("/api/expenses/{expenseId}")]
        public async Task<ActionResult<ExpenseView>> UpdateExpense(Guid expenseId, UpdateExpenseRequest expense)
        {
            Expense? _expense = await _context.Expenses
                .Where(x => x.Id.Equals(expenseId))
                .FirstOrDefaultAsync();

            if (_expense == null)
            {
                return NotFound();
            }

            _expense.Amount = expense?.Amount ?? _expense.Amount;
            _expense.Category = expense?.Category ?? _expense.Category;
            _expense.Name = expense?.Name ?? _expense.Name;
            _expense.Date = expense?.Date ?? _expense.Date;

            await _context.SaveChangesAsync();

            ExpenseView result = new ExpenseView()
            {
                Id = _expense.Id,
                Name = _expense.Name,
                Category = _expense.Category.ToString(),
                Amount = _expense.Amount,
                Date = _expense.Date
            };

            return Ok(result);
        }

        [HttpDelete("/api/expenses/{expenseId}")]
        public async Task<IActionResult> DeleteExpense(Guid expenseId)
        {
            Expense? _expense = await _context.Expenses
                .Where(x => x.Id.Equals(expenseId))
                .FirstOrDefaultAsync();

            if (_expense == null)
            {
                return NotFound();
            }

            _context.Expenses.Remove(_expense);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
