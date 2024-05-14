using Chato.Server.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Chato.Server.BackgroundTasks
{


    public class TestBackgroundTask : BackgroundService
    {
        public const string MessageTemplate = "message-{0}";

        private IHubContext<ChatHub> _hubContext;


        public TestBackgroundTask(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int index = 0;

            while (index < 50)
            {
                var message = string.Format(MessageTemplate, "xxx");
            //    await _hubContext.Clients.All.SendAsync(ChatHub.TOPIC_MESSAGE_RECEIVED, $"server", $"{message}");
                await Task.Delay(2000);

                index++;
            }
        }
    }
}
