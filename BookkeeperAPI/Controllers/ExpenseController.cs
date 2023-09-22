namespace BookkeeperAPI.Controllers
{
    #region usings
    using BookkeeperAPI.Data;
    using BookkeeperAPI.Entity;
    using Microsoft.AspNetCore.Mvc;
    #endregion

    [ApiController]
    [Route("/api/v1/user/{userId}/expenses")]
    [Produces("application/json")]
    public class ExpenseController : ControllerBase
    {
        private BookkeeperContext _context;
        public ExpenseController(BookkeeperContext context) 
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<List<Expense>> GetExpense(Guid userId)
        {
            return Ok(_context.Expenses.Where(x => x.User.Id.Equals(userId)));
        }
    }
}
