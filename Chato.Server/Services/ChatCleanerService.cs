using Chatto.Shared;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Chato.Server.Services
{
    public interface IChatCleanerService
    {
        ChatDto Dequeue();
        void Enqueue(ChatDto chatDto);
    }

    public class ChatCleanerService : IChatCleanerService, IDisposable
    {
        private readonly Queue<ChatDto> _queue = new Queue<ChatDto>();
        private bool _disposed;
        private readonly object _lock = new();
        private readonly ILogger<ChatCleanerService> _logger;

        public ChatCleanerService(ILogger<ChatCleanerService> logger)
        {
            this._logger = logger;
        }

        public void Enqueue(ChatDto result)
        {
            lock (_lock)
            {
                if (!_disposed)
                {
                    _queue.Enqueue(result);
                }
            }
        }

        public ChatDto Dequeue()
        {
            lock (_lock)
            {
                if (!_disposed)
                {
                    if (_queue.Count > 0)
                    {
                        var chat = _queue.Dequeue();
                        return chat;
                    }
                }
            }

            return null;
        }

        public void Dispose()
        {
            lock (_lock)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                _queue.Clear(); // Ensure all items are removed.

                _logger.LogInformation($"{nameof(ChatCleanerService)} disposed, queue cleared.");
            }
        }
    }
}
