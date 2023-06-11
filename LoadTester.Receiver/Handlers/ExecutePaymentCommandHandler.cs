using LoadTester.Shared;
using Microsoft.Extensions.Configuration;
using NServiceBus;
using LoadTester.Shared.Commands;
using LoadTester.Shared.Commands.Responses;
using Microsoft.Extensions.Logging;

namespace Receiver.Handlers
{
    public class ExecutePaymentCommandHandler : IHandleMessages<ExecutePaymentCommand>
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public ExecutePaymentCommandHandler(IConfiguration configuration, ILogger<StoreExecutedPaymentCommandHandler> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task Handle(ExecutePaymentCommand message, IMessageHandlerContext context)
        {
            _logger.LogInformation($"[{DateTime.Now}] Handling {nameof(ExecutePaymentCommandHandler)}");

            int highestDuration = int.Parse(_configuration["ControlParameters:ExecutePayment:HighestDuration"]);
            int lowestDuration = int.Parse(_configuration["ControlParameters:ExecutePayment:LowestDuration"]);
            bool randomLoad = bool.Parse(_configuration["ControlParameters:RandomLoad"]);

            await Task.Delay(LoadHelper.GetDelayDuration(lowestDuration, highestDuration, randomLoad), context.CancellationToken);

            await context.Reply(new ExecutePaymentResponse());
        }
    }
}
