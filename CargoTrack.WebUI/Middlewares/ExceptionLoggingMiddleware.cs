namespace CargoTrack.WebUI.Middlewares
{
    public class ExceptionLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionLoggingMiddleware> _logger;

        public ExceptionLoggingMiddleware(RequestDelegate next, ILogger<ExceptionLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Yakalanmamış istisna: {Method} {Path}", context.Request.Method, context.Request.Path);
                throw; // Loglamadan sonra fırlatmaya devam et — asıl kullanıcıya gösterilecek yanıtı UseExceptionHandler üretir
            }
        }
    }
}