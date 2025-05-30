﻿using Chato.Automation.Infrastructure.Instruction;
using Chato.Server.DataAccess.Models;
using Chato.Server.Hubs;
using Chato.Server.Services;
using Chatto.Shared;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;

namespace Chato.Automation.Scenario;

public abstract class InstructionScenarioBase : ChatoRawDataScenarioBase
{
    protected readonly CounterSignal _counterSignal;
    private readonly Dictionary<string, Func<UserInstructionExecuter, InstructionNode, Task>> _actionMapper;
    private readonly CancellationTokenSource _cancellationTokenSource;

    protected readonly Dictionary<string, UserInstructionExecuter> Users;
    private readonly Dictionary<string, HashSet<string>> _groupUsers;

    public InstructionScenarioBase(ILogger logger, ScenarioConfig config) : base(logger, config)
    {
        Users = new Dictionary<string, UserInstructionExecuter>();
        _groupUsers = new Dictionary<string, HashSet<string>>();

        _counterSignal = new CounterSignal(Logger, 2);

        _actionMapper = new Dictionary<string, Func<UserInstructionExecuter, InstructionNode, Task>>();



        Initialize();

    }

    public virtual async Task RegisterUsers(params string[] users)
    {
        foreach (var user in users)
        {
            var registrationRequest = new RegistrationRequest { UserName = user, Age = 20, Description = $"Description_{user}", Gender = "male" };
            var registrationInfo = await RunPostCommand<RegistrationRequest, ResponseWrapper<RegistrationResponse>>(RegisterAuthControllerUrl, registrationRequest);

            var executer = new UserInstructionExecuter(registrationInfo.Body, HubUrl, Logger, _counterSignal);
            //await executer.RegisterAsync();

            Users.Add(user, executer);
        }
    }

    private async Task StartSignalR(UserInstructionExecuter userExecuter, int amountMessages)
    {
        //var message2 = Encoding.UTF8.GetString(message);
        await userExecuter.StartSignalRAsync(amountMessages);

    }

    private async Task SendMessageToOthersInGroup(UserInstructionExecuter userExecuter, string groupName, string userNameFrom, string message, SenderInfoType messageType, string? imageName, string? description)
    {
        //var message2 = Encoding.UTF8.GetString(message);
        await userExecuter.SendMessageToOthersInGroup(chatName: groupName, userNameFrom: userNameFrom, message: message, messageType: messageType, imageName, description);

    }



    private void Initialize()
    {

        _actionMapper.Add(UserInstructions.logout_Chat_Instruction, async (userExecuter, instruction) =>
        {
            await _counterSignal.SetThrasholdAsync(1);
            await userExecuter.UserDisconnectingAsync();


            if (await _counterSignal.WaitAsync(timeoutInSecond: 5) == false)
            {
                throw new Exception("Not all users received their messages");
            }

            await UsersCleanup(userExecuter.UserName);
        });


        _actionMapper.Add(UserInstructions.JoinOrCreate_Chat_Instruction, async (userExecuter, instruction) =>
        {
            if (userExecuter.UserName == "nathan")
            {

            }

            if (userExecuter.UserName == "olessya")
            {

            }



            if (userExecuter.UserName == "anatoliy")
            {

            }

            int amountMessage = -1;
            ChatType chatType = ChatType.Public;
            SenderInfoType? senderInfoType = null;
            string? description = null;

            if (instruction.Instruction is JoinOrCreateChatInstruction instance)
            {
                amountMessage = instance.AmountMessages;
                chatType = instance.ChatType;
                description = instance.Description;
            }

            await _counterSignal.SetThrasholdAsync(1);


            await userExecuter.JoinOrCreateChat(instruction.ChatName, chatType, description);
            await userExecuter.DownloadHistory(instruction.ChatName, amountMessage);

            //if(amountNotified > 0)
            //{

            //}


            if (await _counterSignal.WaitAsync(timeoutInSecond: 5) == false)
            {
                throw new Exception("Not all users received their messages");
            }

        });

        _actionMapper.Add(UserInstructions.GetHistory_Chat_Instruction, async (userExecuter, instruction) =>
        {
            int amountMessage = -1;

            if (instruction.Instruction is GetHistoryChatInstruction instance)
            {
                amountMessage = instance.AmountMessages;
            }

            await _counterSignal.SetThrasholdAsync(1);

            await userExecuter.DownloadHistory(instruction.ChatName, amountMessage);


            if (await _counterSignal.WaitAsync(timeoutInSecond: 5) == false)
            {
                throw new Exception("Not all users received their messages");
            }

        });

        //_actionMapper.Add(UserInstructions.Notify_User_Instruction, async (userExecuter, instruction) =>
        //{
        //    await _counterSignal.SetThrasholdAsync(1);

        //    await Task.Delay(100);

        //    //if (await _counterSignal.WaitAsync(timeoutInSecond: 5) == false)
        //    //{
        //    //    throw new Exception("Not all users received their messages");
        //    //}


        //    SenderInfoType? senderInfoType = null;
        //    if ( instruction.Instruction is NotifyUserInstruction notifyUserInstruction)
        //    {
        //        senderInfoType = notifyUserInstruction.ExpectedSenderInfoType;
        //    }

        //    await userExecuter.ShouldBeNotified(instruction.ChatName);

        //});


        _actionMapper.Add(UserInstructions.User_RegisterLobi_Instruction, async (userExecuter, instruction) =>
        {
            int amountMessages = -1;
            if (instruction.Instruction is UserRegisterLobiInstruction instance)
            {
                amountMessages = instance.AmountMessages;
            }


            await _counterSignal.SetThrasholdAsync(2);

            await StartSignalR(userExecuter: userExecuter, amountMessages: amountMessages);


            if (await _counterSignal.WaitAsync(timeoutInSecond: 5) == false)
            {
                var any = await _counterSignal.AnyRelease();
                if (!any)
                {
                    throw new Exception($"{UserInstructions.User_RegisterLobi_Instruction} - Not all users received their messages");
                }
            }
            else
            {
                var message = string.Format(ChattoHub.User_Connected_Message_Template, "lobi");
                await userExecuter.MessageShouldBe(IChatService.Lobi, instruction.UserName, "server", message, imagePath: null);
            }
        });




        _actionMapper.Add(UserInstructions.Publish_ToRestRoom_Instruction, async (userExecuter, instruction) =>
        {
            if (userExecuter.UserName == "olessya")
            {

            }

            int awaitAmount = -1;
            if (instruction.Instruction is UserSendStringMessageRestRoomInstruction instance)
            {
                awaitAmount = instance.AmountAwaits;
            }
            await _counterSignal.SetThrasholdAsync(awaitAmount);
            await SendMessageToOthersInGroup(userExecuter: userExecuter, groupName: instruction.ChatName, userNameFrom: instruction.UserName, message: instruction.Message, messageType: instruction.messageType, imageName: instruction.ImageName, string.Empty);

            if (await _counterSignal.WaitAsync(timeoutInSecond: 5) == false)
            {
                throw new Exception("Not all users received their messages");
            }
        });


        _actionMapper.Add(UserInstructions.Received_Instruction, async (userExecuter, instruction) =>
        {
            if (userExecuter.UserName == "nathan")
            {

            }

            if (instruction.messageType == SenderInfoType.Image)
            {

            }

            await userExecuter.MessageShouldBe(instruction.ChatName, instruction.UserName, instruction.FromArrived, instruction.Message, instruction.ImageName);
        });
        _actionMapper.Add(UserInstructions.Not_Received_Instruction, async (userExecuter, instruction) => await userExecuter.NotReceivedCheckAsync());
        _actionMapper.Add(UserInstructions.Leave_Room_Instruction, async (userExecuter, instruction) => await userExecuter.LeaveChatInfo(instruction.ChatName));
    }


    protected async Task InstructionExecuter(InstructionGraph graph)
    {
        var instructions = await graph.MoveNext();

        while (instructions.Any())
        {
            foreach (var instruction in instructions)
            {
                if (instruction.Instruction.InstructionName.Equals(UserInstructions.Run_Operation_Instruction))
                {
                    if (instruction.Instruction is UserRunOperationInstruction instance)
                    {
                        var registerInfo = default(RegistrationResponse);
                        if (Users.TryGetValue(instruction.UserName, out var ins))
                        {
                            registerInfo = ins.RegisterResponse;
                        }

                        await instance.Operation(new UserInfo(instruction, registerInfo));
                    }

                    continue;
                }

                var userExecuter = Users[instruction.UserName];
                await _actionMapper[instruction.Instruction.InstructionName]?.Invoke(userExecuter, instruction);
            }

            instructions = await graph.MoveNext();
        }
    }

    public async Task UsersCleanup(params string[] users)
    {
        foreach (var user in users)
        {
            foreach (var group in _groupUsers.Keys.ToArray())
            {
                if (_groupUsers[group].Contains(user))
                {
                    _groupUsers[group].Remove(user);
                    if (_groupUsers[group].Any() == false)
                    {
                        _groupUsers[group] = null;
                        _groupUsers.Remove(group);
                    }
                }
            }

            await Users[user].KillConnectionAsync();
            Users.Remove(user);
        }
    }
}