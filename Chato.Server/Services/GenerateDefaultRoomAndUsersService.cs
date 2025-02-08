using Chatto.Shared;
using Microsoft.AspNetCore.SignalR.Client;
using System.Data.Common;
using System;
using System.Text;
using Chato.Server.DataAccess.Models;

namespace Chato.Server.Services;

public interface IPreloadDataLoader
{
    Task ExecuteAsync();
}

public interface IPersistentChatAndUsers
{
    public const string AdultRoom = "Adults";
    public const string OnlyGirlsRoom = "OnlyGirls";
    public const string SchoolRoom = "School";


    public const string Supervisor = "supervisor";
      

    public const string ToRemoveRoom = "ToRemove";


    static string[] PersistentChats { get; } = { IChatService.Lobi, AdultRoom, OnlyGirlsRoom, SchoolRoom };
}


public class GenerateDefaultRoomAndUsersService : IPreloadDataLoader
{
    private readonly IAssignmentService _assignmentService;
    private readonly IChatService _roomService;

    public GenerateDefaultRoomAndUsersService(IAssignmentService assignmentService,
        IChatService roomService
        )
    {
        _assignmentService = assignmentService;
        this._roomService = roomService;
    }

    public async Task ExecuteAsync()
    {
        await _assignmentService.CreateLobi();


        //var registrationRequest = new RegistrationRequest()
        //{
        //    UserName = $"{IPersistentChatAndUsers.Supervisor}",
        //    Description = $"I am the supervisor",
        //    Gender = "male",
        //    Age = 20,
        //};

        //var token = await _assignmentService.RegisterUserAndAssignToRoom(registrationRequest, IChatService.Lobi);


        var chat = IPersistentChatAndUsers.AdultRoom;
        var requests = new List<(string UserName, string Token)>();
        for (int j = 0; j < 3; j++)
        {
            var request = new RegistrationRequest()
            {
                UserName = $"{chat}__User{j + 1}",
                Description = $"Description_{chat}",
                Gender = "male",
                Age = 20,
            };


            var message = $"{request.UserName} has registered";
            var token = await _assignmentService.RegisterUserAndAssignToRoom(request, chat,ChatType.Public);
            await _roomService.SendMessageAsync(chat, request.UserName, message,null,SenderInfoType.TextMessage);

            requests.Add((request.UserName, token));
        }
        var hi = $"{requests[1].UserName} say hi";
        await _roomService.SendMessageAsync(chat, requests[1].UserName, hi,null, SenderInfoType.TextMessage);

        hi = $"{requests[2].UserName} say hi back";
        await _roomService.SendMessageAsync(chat, requests[2].UserName, hi,null, SenderInfoType.TextMessage);


        requests.Clear();

        chat = IPersistentChatAndUsers.OnlyGirlsRoom;
        for (int j = 0; j < 5; j++)
        {
            var request = new RegistrationRequest()
            {
                UserName = $"{chat}__User{j + 1}",
                Description = $"{chat}=> I love roses.",
                Gender = "female",
                Age = 20,
            };

            var token = await _assignmentService.RegisterUserAndAssignToRoom(request, chat,ChatType.Public);
            requests.Add((request.UserName, token));
        }

        var talk = $"{requests[3].UserName} say hello you";
        await _roomService.SendMessageAsync(chat, requests[3].UserName, talk,null, SenderInfoType.TextMessage);

        talk = $"{requests[4].UserName} say whats up";
        await _roomService.SendMessageAsync(chat, requests[4].UserName, talk,null, SenderInfoType.TextMessage);




        requests.Clear();
        chat = IPersistentChatAndUsers.SchoolRoom;
        for (int j = 0; j < 7; j++)
        {
            var request = new RegistrationRequest()
            {
                UserName = $"{chat}__User{j + 1}",
                Description = $"{chat}=> I hate school.",
                Gender = "male",
                Age = 20,
            };

            var token = await _assignmentService.RegisterUserAndAssignToRoom(request, chat, ChatType.Public);
            requests.Add((request.UserName, token));
        }

        talk = $"{requests[3].UserName} when school starts";
        await _roomService.SendMessageAsync(chat, requests[3].UserName, talk,null, SenderInfoType.TextMessage);

        talk = $"{requests[4].UserName} say tomorrow";
        await _roomService.SendMessageAsync(chat, requests[4].UserName, talk,null, SenderInfoType.TextMessage);






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
