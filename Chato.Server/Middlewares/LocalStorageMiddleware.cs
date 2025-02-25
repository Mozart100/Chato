using AutoMapper;
using Chato.Server.DataAccess.Models;
using Chato.Server.Hubs;
using Chato.Server.Services;
using Chatto.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Chato.Server.Middlewares;

public class LocalStorageMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LocalStorageMiddleware> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public LocalStorageMiddleware(RequestDelegate next, ILogger<LocalStorageMiddleware> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _next = next;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip SignalR requests
        if (context.Request.Path.StartsWithSegments($"{ChattoHub.HubMapUrl}", StringComparison.OrdinalIgnoreCase) ||
            context.Request.Path.StartsWithSegments("/hub", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var userName = context.User?.Identity?.Name;

        if (!string.IsNullOrEmpty(userName))
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

                var user = await userService.GetUserByNameOrIdGetOrDefaultAsync(userName);
                if (user is not null)
                {
                    context.Items[User.User_Key] = user;
                }

                _logger.LogInformation($"User = {userName}");
            }
        }

        await _next(context);
    }
}
