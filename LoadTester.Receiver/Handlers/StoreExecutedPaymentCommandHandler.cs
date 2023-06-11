using LoadTester.Shared;
using Microsoft.Extensions.Configuration;
using NServiceBus;
using LoadTester.Shared.Commands;
using LoadTester.Shared.Commands.Responses;
using Microsoft.Extensions.Logging;

namespace Receiver.Handlers
{
    public class StoreExecutedPaymentCommandHandler : IHandleMessages<StoreExecutedPaymentCommand>
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public StoreExecutedPaymentCommandHandler(IConfiguration configuration, ILogger<StoreExecutedPaymentCommandHandler> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task Handle(StoreExecutedPaymentCommand message, IMessageHandlerContext context)
        {
            _logger.LogInformation($"[{DateTime.Now:HH:mm:ss}] Handling {nameof(StoreExecutedPaymentCommandHandler)}");

            int highestDuration = int.Parse(_configuration["ControlParameters:StoreExecutedPayment:HighestDuration"]);
            int lowestDuration = int.Parse(_configuration["ControlParameters:StoreExecutedPayment:LowestDuration"]);
            bool randomLoad = bool.Parse(_configuration["ControlParameters:RandomLoad"]);

            await Task.Delay(LoadHelper.GetDelayDuration(lowestDuration, highestDuration, randomLoad), context.CancellationToken);

            await context.Reply(new StoreExecutedPaymentResponse());
        }
    }
}
