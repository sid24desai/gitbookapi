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
    using BookkeeperAPI.Service.Interface;
    using BookkeeperAPI.Service;
    using System.ComponentModel.DataAnnotations;
    using BookkeeperAPI.Exceptions;
    using Microsoft.AspNetCore.Authorization;
    #endregion

    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class ExpenseController : ControllerBase
    {
        private readonly BookkeeperContext _context;
        private readonly IExpenseService _expenseService;
        public ExpenseController(BookkeeperContext context, IExpenseService expenseService)
        {
            _context = context;
            _expenseService = expenseService;
        }

        [HttpGet("/api/me/expenses")]
        [ProducesDefaultResponseType(typeof(PaginatedResult<ExpenseView>))]
        [ProducesErrorResponseType(typeof(ErrorResponseModel))]
        public async Task<ActionResult<PaginatedResult<ExpenseView>>> GetExpense([FromHeader] [Required] Guid userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25, [FromQuery] ExpenseCategory? category = null, [FromQuery] string? name = null, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            if (userId == Guid.Empty)
            {
                throw new HttpOperationException(StatusCodes.Status400BadRequest, "UserId provided is invalid");
            }

            string domain = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.Path;

            PaginatedResult<ExpenseView> result = await _expenseService.GetPaginatedExpensesAsync(userId, domain, pageNumber, pageSize, category, name, from, to);

            return result;
        }

        [HttpGet("/api/expenses/{expenseId}")]
        [ProducesDefaultResponseType(typeof(ExpenseView))]
        [ProducesErrorResponseType(typeof(ErrorResponseModel))]
        public async Task<ActionResult<ExpenseView>> GetExpenseById(Guid expenseId)
        {
            return Ok(await _expenseService.GetExpenseByIdAsync(expenseId));
        }

        [HttpPost("/api/expenses")]
        [ProducesDefaultResponseType(typeof(ExpenseView))]
        [ProducesErrorResponseType(typeof(ErrorResponseModel))]
        public async Task<ActionResult<ExpenseView>> CreateExpense([FromQuery][Required] Guid userId, [FromBody][Required] CreateExpenseRequest expense)
        {
            if (userId == Guid.Empty)
            {
                throw new HttpOperationException(StatusCodes.Status400BadRequest, "UserId provided is invalid");
            }

            ExpenseView result = await _expenseService.CreateExpenseAsync(userId, expense);

            return Ok(result);
        }

        [HttpPatch("/api/expenses/{expenseId}")]
        [ProducesDefaultResponseType(typeof(ExpenseView))]
        [ProducesErrorResponseType(typeof(NotFoundResult))]
        public async Task<ActionResult<ExpenseView>> UpdateExpense(Guid expenseId, UpdateExpenseRequest expense)
        {
            ExpenseView result = await _expenseService.UpdateExpenseAsync(expenseId, expense);

            return Ok(result);
        }

        [HttpDelete("/api/expenses/{expenseId}")]
        [ProducesDefaultResponseType(typeof(NoContentResult))]
        [ProducesErrorResponseType(typeof(NotFoundResult))]
        public async Task<ActionResult<ResponseModel>> DeleteExpense(Guid expenseId)
        {
            await _expenseService.DeleteExpenseAsync(expenseId);

            return Ok(new ResponseModel()
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Expense deleted successfully"
            });
        }

    }
}
