using LoadTester.Shared;
using Microsoft.Extensions.Configuration;
using NServiceBus;
using LoadTester.Shared.Commands;
using LoadTester.Shared.Commands.Responses;
using Microsoft.Extensions.Logging;

namespace Receiver.Handlers
{
    public class CheckPermissionsCommandHadnler : IHandleMessages<CheckPermissionsCommand>
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public CheckPermissionsCommandHadnler(IConfiguration configuration, ILogger<CheckPermissionsCommandHadnler> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task Handle(CheckPermissionsCommand message, IMessageHandlerContext context)
        {
            _logger.LogInformation($"[{DateTime.Now}] Handling {nameof(CheckPermissionsCommandHadnler)}");

            int highestDuration = int.Parse(_configuration["ControlParameters:CheckPermissions:HighestDuration"]);
            int lowestDuration = int.Parse(_configuration["ControlParameters:CheckPermissions:LowestDuration"]);
            bool randomLoad = bool.Parse(_configuration["ControlParameters:RandomLoad"]);

            await Task.Delay(LoadHelper.GetDelayDuration(lowestDuration, highestDuration, randomLoad), context.CancellationToken);

            await context.Reply(new CheckPermissionsResponse());
        }
    }
}
