namespace WebAPI.Middleware;

public class IpAddressMiddleware
{
    private readonly RequestDelegate _next;

    public IpAddressMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var ipAddress = httpContext.Connection.RemoteIpAddress.ToString();
        httpContext.Items["IpAddress"] = ipAddress;

        using (StreamWriter writer = File.AppendText("Logs/ipaddress.txt"))
        {
            writer.WriteLine($"{DateTime.UtcNow} - {ipAddress}");
        }

        await _next(httpContext);
    }
}