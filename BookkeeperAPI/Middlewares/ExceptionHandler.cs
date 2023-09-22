using System.Net;
using System.Text.Json;

namespace BookkeeperAPI.Middlewares
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;
        public ExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try 
            { 
                await _next(context);
            }
            catch (Exception e)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = e.Message, statusCode = StatusCodes.Status500InternalServerError }));
                return;
            }
        }
    }
}
