using AutoMapper;
using Chato.Server.DataAccess.Models;
using Chato.Server.Hubs;
using Chato.Server.Infrastracture;
using Chato.Server.Services;
using Chatto.Shared;

namespace Chato.Server.Middlewares;

public class LocalStorageMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LocalStorageMiddleware> _logger;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    //private readonly ILocalStorage _localStorage;

    public LocalStorageMiddleware(RequestDelegate next, ILogger<LocalStorageMiddleware> logger, 
        IUserService userService,
        IMapper mapper
        //, ILocalStorage localStorage
        )
    {
        _next = next;
        _logger = logger;
        this._userService = userService;
        this._mapper = mapper;
        //this._localStorage = localStorage;
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

        //context.Items.Add()
        var userName = context.User?.Identity?.Name;

        if (userName.IsNotEmpty())
        {
            User user = await _userService.GetUserByNameOrIdGetOrDefaultAsync(userName);
            if (user is not null)
            {
                var dto = _mapper.Map<UserDto>(user);
                context.Items[User.User_Key] = dto;
            }

            _logger.LogInformation("zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz");
            _logger.LogInformation("zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz");
            _logger.LogInformation("zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz");
            _logger.LogInformation("zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz");

            _logger.LogInformation($"User = {userName}");

            _logger.LogInformation("zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz");
            _logger.LogInformation("zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz");
            _logger.LogInformation("zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz");
            _logger.LogInformation("zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz");
        }

        await _next(context);
    }
}
