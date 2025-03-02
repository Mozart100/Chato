using Chato.Server.Services;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chato.Server.BackgroundTasks
{
    public class ChatEvictionBackgroundTask : BackgroundService
    {
        private readonly IChatCleanerService _chatCleaner;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ChatEvictionBackgroundTask(IChatCleanerService chatCleaner, IServiceScopeFactory serviceScopeFactory)
        {
            _chatCleaner = chatCleaner;
            this._serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var chatDto = _chatCleaner.Dequeue();
                if (chatDto is not null)
                {
                    using (IServiceScope scope = _serviceScopeFactory.CreateScope())
                    {
                        var chatService = scope.ServiceProvider.GetRequiredService<IChatService>();

                        chatDto = await chatService.GetRoomByNameOrIdAsync(chatDto.RoomName);
                        if (chatDto is not null)
                        {
                            var lastMessage = chatDto.Messages.LastOrDefault();
                            if (lastMessage is not null)
                            {
                                var dateTime = DateTimeOffset.FromUnixTimeSeconds(lastMessage.TimeStemp).UtcDateTime;
                                var passedTime = DateTime.UtcNow - dateTime;
                                if (passedTime >TimeSpan.FromMinutes(30))
                                {
                                    await chatService.RemoveRoomByNameOrIdAsync(chatDto.RoomName);
                                    continue;
                                }
                            }
                        }
                    }

                    //continue;
                }
                await Task.Delay(1000, stoppingToken);
            }

        }
    }
}