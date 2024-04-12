using Chato.Automation.Infrastructure.Instruction;
using Chato.Server.Hubs;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR.Client;

namespace Chato.Automation.Scenario;

public abstract class InstructionScenarioBase : ScenarioBase
{
    protected const string Hub_Send_Message_Topic = nameof(ChatHub.SendMessageAllUsers);

    protected Dictionary<UserHubChat, bool> Users;

    protected HubConnection Connection;
    //protected SemaphoreSlim FinishedSignal;
    //protected SemaphoreSlim StartListeningSignal;

    private readonly CancellationTokenSource _cancellationTokenSource;
    private Queue<HubMessageRecieved> _receivedMessages;

    public InstructionScenarioBase(string baseUrl) : base(baseUrl)
    {
        Users = new Dictionary<UserHubChat, bool>();
        //FinishedSignal = new SemaphoreSlim(0, 1);
        //StartListeningSignal = new SemaphoreSlim(0, 1);

        //_cancellationTokenSource = new CancellationTokenSource();

        //Task.Run(async () => await ListeningThread(_cancellationTokenSource.Token));


        _receivedMessages = new Queue<HubMessageRecieved>();

        Connection = new HubConnectionBuilder()
         .WithUrl(BaseUrl)
         .WithAutomaticReconnect()
         .Build();

        Connection.Reconnecting += (sender) =>
        {
            Logger.Info("Connection reconnecting");
            return Task.CompletedTask;
        };

        Connection.Reconnected += (sender) =>
        {
            Logger.Info($"Hub Connected.");
            return Task.CompletedTask;

        };
    }

    public async Task InstructionExecuter(InstructionGraph graph)
    {
        var instructions = await graph.Pulse();

        foreach (var instruction in instructions)
        {
            if (instruction.Instruction.Equals(UserHubInstruction.Publish_Instrauction))
            {
                await SendMessageToAllUSers("xxx", instruction.Message);
                continue;
            }

            if (instruction.Instruction.Equals(UserHubInstruction.Received_Instrauction))
            {
                var nessageReceived  = _receivedMessages.Dequeue();

                nessageReceived.From.Should().Be(instruction.FromArrived);
                nessageReceived.Message.Should().Be(instruction.Message);
            }
        }
    }

    public async Task SendMessageToAllUSers(string userNameFrom, string message)
    {
        var user = Users.FirstOrDefault(x => x.Key.Name == userNameFrom).Key;
        //user.

        await Connection.SendAsync(Hub_Send_Message_Topic, userNameFrom, message);
    }



    protected async Task Listen()
    {
        Connection.On<string, string>(ChatHub.TOPIC_MESSAGE_RECEIVED, async (user, message) =>
        {
            _receivedMessages.Enqueue(new HubMessageRecieved(user, message));
        });
    }
}
