using Microsoft.Extensions.Logging;
using NServiceBus;
using LoadTester.Shared.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LoadTester
{
    public class MessageService
    {
        private readonly IMessageSession _messageSession;
        private readonly ILogger _logger;

        public MessageService(IMessageSession messageSession, ILogger<MessageService> logger)
        {
            _messageSession = messageSession;
            _logger = logger;
        }

        internal async Task SendMessageAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _messageSession.Send("Receiver", new LoadTestingCommand { Id = Guid.NewGuid().ToString() });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error on sending command in {nameof(SendMessageAsync)}: ", ex.ToString());
                throw;
            }
        }
    }
}
