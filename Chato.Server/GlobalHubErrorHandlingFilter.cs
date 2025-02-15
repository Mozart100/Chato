using Microsoft.AspNetCore.SignalR;

namespace Chato.Server;

public class GlobalHubErrorHandlingFilter : IHubFilter
{
    private readonly ILogger<GlobalHubErrorHandlingFilter> _logger;

    public GlobalHubErrorHandlingFilter(ILogger<GlobalHubErrorHandlingFilter> logger)
    {
        _logger = logger;
    }

    public async ValueTask<object> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object>> next)
    {
        try
        {

            _logger.LogInformation($"{invocationContext.HubMethod.Name} -- started");
            var result = await next(invocationContext);
            _logger.LogInformation($"{invocationContext.HubMethod.Name} -- finished");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in Hub Method: {Method}", invocationContext.HubMethodName);
            throw new HubException("An internal error occurred in SignalR."); // Custom error message for clients
        }
    }
}
