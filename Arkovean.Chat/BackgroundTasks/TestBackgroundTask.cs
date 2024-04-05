using Arkovean.Chat.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Arkovean.Chat.BackgroundTasks
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

            while(index++ < 50)
            {
                await _hubContext.Clients.All.SendAsync(ChatHub.TOPIC_MESSAGE_RECEIVED, $"user-{index}", $"message-{index}");
                await Task.Delay(2000);
            }
        }
    }
}
