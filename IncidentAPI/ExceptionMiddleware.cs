using Incident.Application.Exceptions; 
using System.Net;
using System.Text.Json;

namespace IncidentAPI.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Catch ANY error from Controller/Service/Repo
                _logger.LogError(ex, "Global Exception Caught: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            
            context.Response.StatusCode = ex switch
            {
                HeaderValidationException => (int)HttpStatusCode.BadRequest,
                FileTooLargeException => (int)HttpStatusCode.BadRequest,
                UnsupportedFormatException => (int)HttpStatusCode.BadRequest,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = _env.IsDevelopment() ? ex.Message : "An internal server error occurred.",
                Details = _env.IsDevelopment() ? ex.StackTrace : null 
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}