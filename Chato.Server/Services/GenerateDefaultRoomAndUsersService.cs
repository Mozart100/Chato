using Chato.Server.DataAccess.Models;

namespace Chato.Server.Services
{

    public interface IPreloadDataLoader
    {
        public const string DefaultRoom = "xxx_room";

        Task ExecuteAsync();

    }

    public class GenerateDefaultRoomAndUsersService : IPreloadDataLoader
    {
        private readonly IAssignmentService assignmentService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserService _userService;

        public GenerateDefaultRoomAndUsersService(IAuthenticationService authenticationService, IUserService userService, IAssignmentService _assignmentService)
        {
            _assignmentService = _assignmentService;
            this._authenticationService = authenticationService;
            this._userService = userService;
        }

        public async Task ExecuteAsync()
        {
            for (int j = 0; j < 3; j++)
            {
                var userName = $"{IPreloadDataLoader.DefaultRoom}__User{j + 1}";
                var password = userName;

                var token = await _authenticationService.RegisterAsync(userName, password);
            }
        }
    }
}
