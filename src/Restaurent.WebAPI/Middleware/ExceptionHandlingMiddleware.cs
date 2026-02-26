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
                if (ex is ArgumentException)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
                else if (ex.InnerException != null)
                {
                    _logger.LogError("{ExceptionType} {ExceptionMeassage}", ex.InnerException.GetType().ToString(), ex.InnerException.Message);
                    httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                }
                else
                {
                    _logger.LogError("{ExceptionType} {ExceptionMeassage}", ex.GetType().ToString(), ex.Message);
                    httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                }

                await httpContext.Response.WriteAsync("OOPS! and error occured please refresh");
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
