using LoadTester.Shared;
using Microsoft.Extensions.Configuration;
using NServiceBus;
using LoadTester.Shared.Commands;
using LoadTester.Shared.Commands.Responses;
using Microsoft.Extensions.Logging;

namespace Receiver.Handlers
{
    public class LogPaymentCommandHandler : IHandleMessages<LogPaymentCommand>
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public LogPaymentCommandHandler(IConfiguration configuration, ILogger<LogPaymentCommandHandler> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task Handle(LogPaymentCommand message, IMessageHandlerContext context)
        {
            _logger.LogInformation($"[{DateTime.Now}] Handling {nameof(LogPaymentCommand)}");

            int highestDuration = int.Parse(_configuration["ControlParameters:LogPayment:HighestDuration"]);
            int lowestDuration = int.Parse(_configuration["ControlParameters:LogPayment:LowestDuration"]);
            bool randomLoad = bool.Parse(_configuration["ControlParameters:RandomLoad"]);

            await Task.Delay(LoadHelper.GetDelayDuration(lowestDuration, highestDuration, randomLoad), context.CancellationToken);

            await context.Reply(new LogPaymentResponse());
        }
    }
}
