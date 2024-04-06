using Chato.Server.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Chato.Server.BackgroundTasks
{
    public class TestBackgroundTask : BackgroundService
    {
        private IHubContext<ChatHub> _hubContext;

        public TestBackgroundTask(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int index = 0;

            while (index++ < 50)
            {
                await _hubContext.Clients.All.SendAsync(ChatHub.TOPIC_MESSAGE_RECEIVED, $"user-{index}", $"message-{index}");
                await Task.Delay(2000);
            }
        }
    }
}
