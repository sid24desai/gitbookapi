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
    using System.ComponentModel;
    using System;
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
        public async Task<ActionResult<PaginatedResult<ExpenseView>>> GetExpense([FromHeader] Guid userId, int pageNumber = 1, int pageSize = 25, ExpenseCategory? category = null, string? name = null, DateTime? from = null, DateTime? to = null)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            string domain = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.Path;

            string filterQuery = "";
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>()
            {
                { "pageNumber", pageNumber },
                { "pageSize", pageSize }
            };

            if (category != null)
            {
                filterQuery += $"&category={category}";
                parameters.Add("category", category.ToString()!);
            }

            if (name != null)
            {
                filterQuery += $"&name={name}";
                parameters.Add("name", name);
            }

            if (from != null)
            {
                filterQuery += $"&from={from}";
                parameters.Add("from", from);
            }

            if (to != null)
            {
                filterQuery += $"&to={to}";
                parameters.Add("to", to);
            }

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

            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            PaginatedResult<ExpenseView> result = new PaginatedResult<ExpenseView>()
            {
                PageCount = totalPages,
                PageNumber = pageNumber,
                PageSize = pageSize,
                FirstPage = domain + "?pageNumber=1&pageSize=" + pageSize + filterQuery,
                LastPage = domain + "?pageNumber=" + totalPages + "&pageSize=" + pageSize + filterQuery,
                TotalCount = totalCount,
                NextPage = pageNumber == totalPages ? null : (domain + "?pageNumber=" + (pageNumber + 1) + "&pageSize=" + pageSize + filterQuery),
                PreviousPage = pageNumber == 1 ? null : (domain + "?pageNumber=" + (pageNumber - 1) + "&pageSize=" + pageSize + filterQuery),
                Data = data,
            };

            return result;
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
            _expense.Category = expense.Category;
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
