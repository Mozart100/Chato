using Chato.Server.Services;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chato.Server.BackgroundTasks
{
    public class ChatEvictionBackgroundTask : BackgroundService
    {
        private readonly IChatCleaner _chatCleaner;

        public ChatEvictionBackgroundTask(IChatCleaner chatCleaner)
        {
            _chatCleaner = chatCleaner;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var chatDto = _chatCleaner.Dequeue();
                if (chatDto is not null)
                {

                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
