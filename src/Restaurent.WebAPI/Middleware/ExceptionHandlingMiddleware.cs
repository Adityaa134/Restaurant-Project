namespace Restaurent.WebAPI.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext); 
            }
            catch (Exception ex)
            {
                int statusCode;
                string title;
                string detail;

                switch (ex)
                {
                    case ArgumentException argEx:
                        statusCode = StatusCodes.Status400BadRequest;
                        title = "Invalid Request";
                        detail = argEx.Message;
                        break;

                    case InvalidOperationException invalidOpEx:
                        statusCode = StatusCodes.Status409Conflict;
                        title = "Operation Not Allowed";
                        detail = invalidOpEx.Message;
                        break;

                    default:
                        statusCode = StatusCodes.Status500InternalServerError;
                        title = "Internal Server Error";
                        detail = "OOPS! An error occurred. Please refresh.";

                        if (ex.InnerException != null)
                            _logger.LogError("{ExceptionType} {ExceptionMessage}", ex.InnerException.GetType().ToString(), ex.InnerException.Message);
                        else
                            _logger.LogError("{ExceptionType} {ExceptionMessage}", ex.GetType().ToString(), ex.Message);
                        break;
                }

                httpContext.Response.StatusCode = statusCode;
                httpContext.Response.ContentType = "application/problem+json";

                var problemDetails = new
                    {
                        title,
                        status = statusCode,
                        detail
                    };

                await httpContext.Response.WriteAsJsonAsync(problemDetails);
            }
        }
    }

    
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
