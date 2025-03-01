using Chatto.Shared;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Chato.Server.Services
{
    public interface IChatCleaner
    {
        ChatDto Dequeue();
        void Enqueue(ChatDto chatDto);
    }

    public class ChatCleaner : IChatCleaner, IDisposable
    {
        private readonly Queue<ChatDto> _queue = new Queue<ChatDto>();
        private bool _disposed;
        private readonly object _lock = new();
        private readonly ILogger<ChatCleaner> _logger;

        public ChatCleaner(ILogger<ChatCleaner> logger)
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

                    return _queue.Count > 0 ? _queue.Dequeue() : null;
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

                _logger.LogInformation($"{nameof(ChatCleaner)} disposed, queue cleared.");
            }
        }
    }
}
