namespace BookkeeperAPI.Service
{
    #region usings
    using BookkeeperAPI.Constants;
    using BookkeeperAPI.Data;
    using BookkeeperAPI.Entity;
    using BookkeeperAPI.Exceptions;
    using BookkeeperAPI.Model;
    using BookkeeperAPI.Repository.Interface;
    using BookkeeperAPI.Service.Interface;
    using Microsoft.EntityFrameworkCore;
    #endregion

    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;

        public ExpenseService(IExpenseRepository expenseRepository) 
        { 
            _expenseRepository = expenseRepository;
        }

        public async Task<PaginatedResult<ExpenseView>> GetPaginatedExpensesAsync(Guid userId, string domain, int pageNumber, int pageSize, ExpenseCategory? category, string? name, DateTime? from, DateTime? to)
        {
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

            List<ExpenseView> data = await _expenseRepository.GetExpensesAsync(userId, pageNumber, pageSize, category, name, from, to);

            int totalCount = await _expenseRepository.GetExpenseCountAsync(userId, category, name, from, to);

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
    
        public async Task<ExpenseView> GetExpenseByIdAsync(Guid expenseId)
        {
            Expense? result = await _expenseRepository.GetExpenseByIdAsync(expenseId);

            if(result != null)
            {
                return new ExpenseView()
                {
                    Name = result.Name,
                    Id = result.Id,
                    Amount = result.Amount,
                    Category = result.Category.ToString(),
                    Date = result.Date,
                };
            }

            throw new HttpOperationException(StatusCodes.Status404NotFound, "Invalid expense id");
        }

        public async Task<ExpenseView> CreateExpenseAsync(Guid userId, CreateExpenseRequest request)
        {
            Expense expense = new Expense();
            expense.UserId = userId;
            expense.Name = request.Name;
            expense.Amount = request.Amount;
            expense.Category = request.Category;
            expense.Date = request.Date;

            await _expenseRepository.SaveExpenseAsync(expense);

            ExpenseView result = new ExpenseView()
            {
                Id = expense.Id,
                Name = expense.Name,
                Category = expense.Category.ToString(),
                Amount = expense.Amount,
                Date = expense.Date,
            };

            return result;
        }

        public async Task<ExpenseView> UpdateExpenseAsync(Guid expenseId, UpdateExpenseRequest request)
        {
            Expense? _expense = await _expenseRepository.GetExpenseByIdAsync(expenseId);

            if (_expense == null)
            {
                throw new HttpOperationException(StatusCodes.Status404NotFound, $"Expense with id '{expenseId}' does not exist");
            }

            _expense.Amount = request?.Amount ?? _expense.Amount;
            _expense.Category = request?.Category ?? _expense.Category;
            _expense.Name = request?.Name ?? _expense.Name;
            _expense.Date = request?.Date ?? _expense.Date;

            await _expenseRepository.SaveChangesAsync();

            ExpenseView result = new ExpenseView()
            {
                Id = _expense.Id,
                Name = _expense.Name,
                Category = _expense.Category.ToString(),
                Amount = _expense.Amount,
                Date = _expense.Date
            };

            return result;
        }

        public async Task DeleteExpenseAsync(Guid expenseId)
        {
            Expense? expense = await _expenseRepository.GetExpenseByIdAsync(expenseId);

            if(expense == null)
            {
                throw new HttpOperationException(StatusCodes.Status404NotFound, $"Expense with id '{expenseId}' does not exist");
            }

            await _expenseRepository.DeleteExpenseAsync(expense);
        }
    }
}
