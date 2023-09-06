namespace BookkeeperAPI.Controllers
{
    #region usings
    using BookkeeperAPI.Model;
    using Microsoft.AspNetCore.Mvc;
    #endregion

    [ApiController]
    [Route("/api/[controller]")]
    [Produces("application/json")]
    public class ExpenseController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Expense>> GetExpense()
        {
            return new List<Expense>() { new Expense() { Id = Guid.NewGuid(), Date = DateTime.Now, Name = "test expense", Category = ExpenseCategory.FinancialExpenses, Amount = 5450.00 } };
        }
    }
}
