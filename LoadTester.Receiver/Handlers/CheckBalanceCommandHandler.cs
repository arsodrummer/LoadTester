using LoadTester.Shared;
using Microsoft.Extensions.Configuration;
using NServiceBus;
using LoadTester.Shared.Commands;
using LoadTester.Shared.Commands.Responses;
using Microsoft.Extensions.Logging;

namespace Receiver.Handlers
{
    public class CheckBalanceCommandHandler : IHandleMessages<CheckBalanceCommand>
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public CheckBalanceCommandHandler(IConfiguration configuration, ILogger<CheckBalanceCommandHandler> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task Handle(CheckBalanceCommand message, IMessageHandlerContext context)
        {
            _logger.LogInformation($"[{DateTime.Now}] Handling {nameof(CheckBalanceCommandHandler)}");

            int highestDuration = int.Parse(_configuration["ControlParameters:CheckBalance:HighestDuration"]);
            int lowestDuration = int.Parse(_configuration["ControlParameters:CheckBalance:LowestDuration"]);
            bool randomLoad = bool.Parse(_configuration["ControlParameters:RandomLoad"]);

            await Task.Delay(LoadHelper.GetDelayDuration(lowestDuration, highestDuration, randomLoad), context.CancellationToken);

            await context.Reply(new CheckBalanceResponse());
        }
    }
}
