using Chato.Server.Models.Dtos;

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
            //var userName = $"{IUsersPreload.DefaultRoom}__User{j + 1}";
            //var password = userName;
            //var description = $"Description_{IUsersPreload.DefaultRoom}";
            //var gender = "male";
            //var age = 20;

            var request = new RegistrationRequest()
            {
                UserName = $"{IUsersPreload.DefaultRoom}__User{j + 1}",
                //Password = $"{IUsersPreload.DefaultRoom}__Password{j + 1}",
                Description = $"Description_{IUsersPreload.DefaultRoom}",
                Gender = "male",
                Age = 20,
            };

            var token = await _assignmentService.RegisterUserAndAssignToRoom(request, IUsersPreload.DefaultRoom);
            //var token = await _assignmentService.RegisterUserAndAssignToRoom(userName, password, IUsersPreload.DefaultRoom, description, gender, age);
        }
    }
}
