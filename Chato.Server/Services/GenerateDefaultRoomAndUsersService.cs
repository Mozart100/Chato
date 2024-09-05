using Chatto.Shared;

namespace Chato.Server.Services;

public interface IPreloadDataLoader
{
    Task ExecuteAsync();
}

public interface IPersistentUsers
{
    public const string AdultRoom = "Adults";
    public const string OnlyGirlsRoom = "OnlyGrils";
    public const string SchoolRoom = "School";
    
    
    public const string ToRemoveRoom = "ToRemove";


    static string[] PersistentUsers { get; } = { AdultRoom, OnlyGirlsRoom, SchoolRoom };
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
        var room = IPersistentUsers.AdultRoom;
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


        room = IPersistentUsers.OnlyGirlsRoom;
        for (int j = 0; j < 5; j++)
        {
            var request = new RegistrationRequest()
            {
                UserName = $"{room}__User{j + 1}",
                Description = $"{room}=> I love roses.",
                Gender = "female",
                Age = 20,
            };

            var token = await _assignmentService.RegisterUserAndAssignToRoom(request, room);
        }


        room = IPersistentUsers.SchoolRoom;
        for (int j = 0; j < 7; j++)
        {
            var request = new RegistrationRequest()
            {
                UserName = $"{room}__User{j + 1}",
                Description = $"{room}=> I hate school.",
                Gender = "male",
                Age = 20,
            };

            var token = await _assignmentService.RegisterUserAndAssignToRoom(request, room);
        }

        room = IPersistentUsers.ToRemoveRoom;
        for (int j = 0; j < 10; j++)
        {
            var request = new RegistrationRequest()
            {
                UserName = $"{room}__User{j + 1}",
                Description = $"{room}=> I hate school.",
                Gender = "male",
                Age = 20,
            };

            var token = await _assignmentService.RegisterUserAndAssignToRoom(request, room);
        }

    }
}
