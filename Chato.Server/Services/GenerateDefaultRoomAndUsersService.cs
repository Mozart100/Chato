namespace Chato.Server.Services;

public interface IPreloadDataLoader
{
    public const string DefaultRoom = "xxx_room";

    Task ExecuteAsync();
}

public class GenerateDefaultRoomAndUsersService : IPreloadDataLoader
{
    private readonly IAssignmentService _assignmentService;
    private readonly IAuthenticationService _authenticationService;

    public GenerateDefaultRoomAndUsersService(IAuthenticationService authenticationService,
        IAssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
        this._authenticationService = authenticationService;
    }

    public async Task ExecuteAsync()
    {
        for (int j = 0; j < 3; j++)
        {
            var userName = $"{IPreloadDataLoader.DefaultRoom}__User{j + 1}";
            var password = userName;

            var token = await _assignmentService.RegisterUserAndAssignToRoom(userName, password, IPreloadDataLoader.DefaultRoom);
        }
    }
}
