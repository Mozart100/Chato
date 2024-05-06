﻿using Chato.Automation.Infrastructure.Instruction;
using Chato.Automation.Responses;
using Chato.Server;
using Chato.Server.Controllers;
using Microsoft.Extensions.Logging;
using System;
using System.Text;

namespace Chato.Automation.Scenario;

public abstract class InstructionScenarioBase : ScenarioBase
{
    private readonly CounterSignal _counterSignal;
    private readonly Dictionary<string, Func<UserInstructionExecuter, InstructionNode, Task>> _actionMapper;
    private readonly CancellationTokenSource _cancellationTokenSource;

    private readonly Dictionary<string, UserInstructionExecuter> _users;
    private readonly Dictionary<string, List<string>> _groupUsers;

    public InstructionScenarioBase(ILogger logger, ScenarioConfig config) : base(logger, config)
    {
        _users = new Dictionary<string, UserInstructionExecuter>();
        _groupUsers = new Dictionary<string, List<string>>();

        _counterSignal = new CounterSignal(2);

        _actionMapper = new Dictionary<string, Func<UserInstructionExecuter, InstructionNode, Task>>();

        HubUrl = $"{BaseUrl}/chat";
        AuthControllerUrl = $"{BaseUrl}/api/auth";
        RegisterAuthControllerUrl = $"{AuthControllerUrl}/register";
        LoginAuthControllerUrl = $"{AuthControllerUrl}/login";

        Initialize();

    }

    protected string HubUrl { get; }
    protected string AuthControllerUrl { get; }
    protected string RegisterAuthControllerUrl { get; }
    protected string LoginAuthControllerUrl { get; }

    public async Task AssignUserToGroupAsync(string groupName, params string[] users)
    {
        var registrationRequest = new RegisterAndLoginRequest { Password = "string", Username = "string" };
        var registrationInfo = await RunPostCommand<RegisterAndLoginRequest, RegisterResponse>(RegisterAuthControllerUrl, registrationRequest);
        var tokenResponse = await RunPostCommand<RegisterAndLoginRequest, LoginResponse>(LoginAuthControllerUrl, registrationRequest);

        var stacked = default(List<string>);
        if (_groupUsers.TryGetValue(groupName, out stacked) == false)
        {
            _groupUsers[groupName] = stacked ??= new List<string>();
        }


        foreach (var user in users)
        {
            var executer = new UserInstructionExecuter(registrationInfo, tokenResponse, HubUrl, Logger, _counterSignal);
            await executer.InitializeWithGroupAsync(groupName);

            _users.Add(user, executer);
            stacked.Add(user);
        }
    }


    private async Task SendMessage(UserInstructionExecuter userExecuter, string groupName, string userNameFrom, byte[] message)
    {
        //var message2 = Encoding.UTF8.GetString(message);
        if (groupName == null)
        {
            await userExecuter.SendMessageToAllUSers(userNameFrom: userNameFrom, message: message);
        }
        else
        {
            await userExecuter.SendMessageToOthersInGroup(groupName: groupName, userNameFrom: userNameFrom, message: message);
        }
    }

    private void Initialize()
    {

        _actionMapper.Add(UserHubInstructions.Publish_Instrauction, async (userExecuter, instruction) =>
        {
            await _counterSignal.SetThrasholdAsync(instruction.Children.Where(x => x.Instruction != UserHubInstructions.Not_Received_Instrauction).Count());
            await SendMessage(userExecuter: userExecuter, groupName: instruction.GroupName, userNameFrom: instruction.UserName, message: instruction.Message);

            if (await _counterSignal.WaitAsync(timeoutInSecond: 5) == false)
            {
                throw new Exception("Not all users receved their messages");
            }
        });

        _actionMapper.Add(UserHubInstructions.Received_Instrauction, async (userExecuter, instruction) => await userExecuter.ListenToStringCheckAsync(instruction.FromArrived, instruction.Message));
        _actionMapper.Add(UserHubInstructions.Not_Received_Instrauction, async (userExecuter, instruction) => await userExecuter.NotReceivedCheckAsync());
        _actionMapper.Add(UserHubInstructions.Run_Download_Instrauction, async (userExecuter, instruction) => await userExecuter.DownloadStream(instruction.Message));

        //_actionMapper.Add(UserHubInstructions.Run_Verify_Instrauction, async (userExecuter, instruction) => await userExecuter.VerifyHistoryAsync(new Queue<byte[]>()));
    }


    protected async Task InstructionExecuter(InstructionGraph graph)
    {
        var instructions = await graph.MoveNext();

        while (instructions.Any())
        {
            foreach (var instruction in instructions)
            {
                if (instruction.Instruction.Equals(UserHubInstructions.Run_Operation_Instrauction))
                {
                    await instruction.Operation(instruction);
                    continue;
                }

                var userExecuter = _users[instruction.UserName];
                await _actionMapper[instruction.Instruction]?.Invoke(userExecuter, instruction);
            }

            instructions = await graph.MoveNext();
        }
    }

    protected async Task UsersCleanup()
    {
        foreach (var user in _users.Values)
        {
            await user.Close();
        }

        _users.Clear();
    }

    public async Task GroupUsersCleanup(params string[] groupNames)
    {
        foreach (var groupName in groupNames)
        {
            foreach (var user in _groupUsers[groupName].ToArray())
            {
                await _users[user].GroupClose(groupName);
            }
        }

        foreach (var user in _users.Values)
        {
            await user.KillConnection();
        }

        _users.Clear();
        _groupUsers.Clear();
    }
}