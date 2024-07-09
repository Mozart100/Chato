using Chatto.Shared;

namespace Chato.Server.Services;

public interface IPreloadDataLoader
{
    Task ExecuteAsync();
    static string[] StaticRooms { get; } = { IUsersPreload.DefaultRoom };

}

public interface IUsersPreload 
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
        foreach (var room in IPreloadDataLoader.StaticRooms)
        {
            for (int j = 0; j < 3; j++)
            {
                var request = new RegistrationRequest()
                {
                    UserName = $"{room}__User{j + 1}",
                    Description = $"Description_{room}",
                    Gender = "male",
                    Age = 20,
                };

                var token = await _assignmentService.RegisterUserAndAssignToRoom(request, room);
            }
        }

    }
}
