namespace WebAPI.Middleware;

public class PerformanceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public PerformanceMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
        _logger = loggerFactory.CreateLogger<PerformanceMiddleware>();
    }

    public async Task Invoke(HttpContext httpContext)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        await _next(httpContext);
        watch.Stop();

        var millisec = watch.ElapsedMilliseconds;
        _logger.LogInformation($"Request {httpContext.Request.Method} {httpContext.Request.Path} executed in {millisec} ms");
    }
}
