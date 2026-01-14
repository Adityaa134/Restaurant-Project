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
                await _next(httpContext); // calling next or subsequent middleware
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    _logger.LogError("{ExceptionType} {ExceptionMeassage}", ex.InnerException.GetType().ToString(), ex.InnerException.Message);
                }
                else
                {
                    _logger.LogError("{ExceptionType} {ExceptionMeassage}", ex.GetType().ToString(), ex.Message);
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
