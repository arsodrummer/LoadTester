using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NBomber.CSharp;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LoadTester
{
    public class NServiceBusLoader : BackgroundService
    {
        private readonly MessageService _messageService;
        private readonly ILogger _logger;

        public NServiceBusLoader(MessageService messageService, ILogger<MessageService> logger)
        {
            _messageService = messageService;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var scenario = Scenario.Create("LoadTestNServiceBus", async context =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                try
                {
                    await _messageService.SendMessageAsync(cancellationToken);
                    return Response.Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Failed to SendMessageAsync: ", ex);
                    return Response.Fail();
                }
            })
            .WithLoadSimulations(
                Simulation.Inject(rate: 10,
                                  interval: TimeSpan.FromSeconds(1),
                                  during: TimeSpan.FromSeconds(30))
            );

            NBomberRunner
                .RegisterScenarios(scenario)
                .LoadConfig("./NBomberConfig.json")
                .Run();

            return Task.CompletedTask;
        }
    }
}
