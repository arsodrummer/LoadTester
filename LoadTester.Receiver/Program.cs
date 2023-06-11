using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using NServiceBus.Persistence.Sql;
using LoadTester.Shared.Commands;

[assembly: SqlPersistenceSettings(
    MsSqlServerScripts = true)]

namespace Receiver;

public class Program
{
    private static IEndpointInstance _endpointInstance;
    private static string _connection;

    public static void Main(string[] args)
    {
        CreateHostBuilder(args, "receiver").Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args, string schemaName)
    {
        var builder = Host.CreateDefaultBuilder(args);

        builder.UseNServiceBus(ctx =>
        {
            IConfiguration configuration = ctx.Configuration;

            _connection = configuration.GetConnectionString("BusConnection");
            var endpointConfiguration = new EndpointConfiguration("Receiver");

            int.TryParse(configuration["ControlParameters:MaxConcurrency"], out int maxConcurrency);

            if (maxConcurrency > 1)
            {
                endpointConfiguration.LimitMessageProcessingConcurrencyTo(maxConcurrency);
            }

            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");
            endpointConfiguration.EnableInstallers();

            var metrics = endpointConfiguration.EnableMetrics();
            metrics.SendMetricDataToServiceControl(
                serviceControlMetricsAddress: "Particular.Monitoring",
                interval: TimeSpan.FromSeconds(2));

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(type => type.Namespace == typeof(LoadTestingCommand).Namespace);
            endpointConfiguration.RegisterComponents(registration: configureComponent =>
            {
                configureComponent.ConfigureComponent<IMessageSession>(_ => _endpointInstance, DependencyLifecycle.SingleInstance);
            });

            var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
            transport.ConnectionString(_connection);
            transport.DefaultSchema(schemaName);
            transport.UseSchemaForQueue("error", "dbo");
            transport.UseSchemaForQueue("audit", "dbo");
            transport.UseSchemaForQueue("LoadTester", "sender");
            transport.Transactions(TransportTransactionMode.SendsAtomicWithReceive);

            transport.Routing().RouteToEndpoint(typeof(LoadTestingCommand), "LoadTester");

            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            var dialect = persistence.SqlDialect<SqlDialect.MsSqlServer>();
            dialect.Schema(schemaName);
            persistence.ConnectionBuilder(() => new SqlConnection(_connection));

            SqlHelper.CreateSchema(_connection, "receiver");

            return endpointConfiguration;
        });

        return builder;
    }
}