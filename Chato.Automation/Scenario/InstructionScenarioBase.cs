using Chato.Automation.Infrastructure.Instruction;
using Chato.Server.Hubs;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR.Client;

namespace Chato.Automation.Scenario;

public record HubMessageRecieved(string From, string Message);

public abstract class InstructionScenarioBase : ScenarioBase
{
    protected const string Hub_Send_Message_Topic = nameof(ChatHub.SendMessageAllUsers);


    protected HubConnection Connection;
    //protected SemaphoreSlim FinishedSignal;
    //protected SemaphoreSlim StartListeningSignal;
    protected HashSet<string> IgnoreUsers;

    private readonly CancellationTokenSource _cancellationTokenSource;
    private Queue<HubMessageRecieved> _receivedMessages;

    public InstructionScenarioBase(string baseUrl) : base(baseUrl)
    {
        _receivedMessages = new Queue<HubMessageRecieved>();
        IgnoreUsers = new HashSet<string>();

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



    protected async Task InstructionExecuter(InstructionGraph graph)
    {
        var instructions = await graph.Pulse();

        while (instructions.Any())
        {
            foreach (var instruction in instructions)
            {
                if (instruction.Instruction.Equals(UserHubInstruction.Publish_Instrauction))
                {
                    await SendMessageToAllUSers(userNameFrom: instruction.UserName, message: instruction.Message);
                    await Task.Delay(3000);
                }
                else
                {
                    if (instruction.Instruction.Equals(UserHubInstruction.Received_Instrauction))
                    {
                        var messageReceived = _receivedMessages.Dequeue();

                        messageReceived.From.Should().Be(instruction.FromArrived);
                        messageReceived.Message.Should().Be(instruction.Message);
                    }
                }
            }

            instructions = await graph.Pulse();
        }
    }

    public async Task SendMessageToAllUSers(string userNameFrom, string message)
    {
        await Connection.SendAsync(Hub_Send_Message_Topic, userNameFrom, message);
    }

    protected async Task Listen()
    {
        Connection.On<string, string>(ChatHub.TOPIC_MESSAGE_RECEIVED, async (user, message) =>
        {
            if (IgnoreUsers.Contains(user) == false)
            {
                Logger.Info($"Message received [{message}] from [{user}].");

                _receivedMessages.Enqueue(new HubMessageRecieved(user, message));
            }
            else
            {
                Logger.Info($"Message [{message}] ignored from [{user}].");
            }
        });
    }
}
