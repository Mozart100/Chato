namespace Chato.Server.Services;

public interface IPreloadDataLoader
{
    Task ExecuteAsync();
}

public interface IUsersPreload : IPreloadDataLoader
{
    public const string DefaultRoom = "Adults";
}


public class GenerateDefaultRoomAndUsersService : IPreloadDataLoader
{
    private readonly IAssignmentService _assignmentService;

    public GenerateDefaultRoomAndUsersService(IAssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    public async Task ExecuteAsync()
    {
        for (int j = 0; j < 3; j++)
        {
            var userName = $"{IUsersPreload.DefaultRoom}__User{j + 1}";
            var password = userName;

            var token = await _assignmentService.RegisterUserAndAssignToRoom(userName, password, IUsersPreload.DefaultRoom);
        }
    }
}
