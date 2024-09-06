using Chatto.Shared;
using Microsoft.AspNetCore.SignalR.Client;
using System.Data.Common;
using System;
using System.Text;

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
    private readonly IRoomService _roomService;

    public GenerateDefaultRoomAndUsersService(IAssignmentService assignmentService,
        IRoomService  roomService
        )
    {
        _assignmentService = assignmentService;
        this._roomService = roomService;
    }

    public async Task ExecuteAsync()
    {
        var room = IPersistentUsers.AdultRoom;
        var requests = new List<(string UserName, string Token)>();
        for (int j = 0; j < 3; j++)
        {
            var request = new RegistrationRequest()
            {
                UserName = $"{room}__User{j + 1}",
                Description = $"Description_{room}",
                Gender = "male",
                Age = 20,
            };


            var message = $"{request.UserName} has registered";
            var token = await _assignmentService.RegisterUserAndAssignToRoom(request, room);
            await _roomService.SendMessageAsync(room, request.UserName, message);
        
            requests.Add((request.UserName, token));
        }
        var hi = $"{requests[1].UserName} say hi";
        await _roomService.SendMessageAsync(room, requests[1].UserName, hi);

        hi = $"{requests[2].UserName} say hi back";
        await _roomService.SendMessageAsync(room, requests[2].UserName, hi);


        requests.Clear();

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
            requests.Add((request.UserName, token));
        }

        var talk = $"{requests[3].UserName} say hello you";
        await _roomService.SendMessageAsync(room, requests[3].UserName, talk);

        talk = $"{requests[4].UserName} say whats up";
        await _roomService.SendMessageAsync(room, requests[4].UserName, talk);




        requests.Clear();
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
            requests.Add((request.UserName, token));
        }

        talk = $"{requests[3].UserName} when school starts";
        await _roomService.SendMessageAsync(room, requests[3].UserName, talk);

        talk = $"{requests[4].UserName} say tomorrow";
        await _roomService.SendMessageAsync(room, requests[4].UserName, talk);






        //room = IPersistentUsers.ToRemoveRoom;
        //for (int j = 0; j < 10; j++)
        //{
        //    var request = new RegistrationRequest()
        //    {
        //        UserName = $"{room}__User{j + 1}",
        //        Description = $"{room}=> I hate school.",
        //        Gender = "male",
        //        Age = 20,
        //    };

        //    var token = await _assignmentService.RegisterUserAndAssignToRoom(request, room);
        //    requests.Add((request.UserName, token));
        //}

    }
}
