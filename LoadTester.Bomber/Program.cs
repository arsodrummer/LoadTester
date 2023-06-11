using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using LoadTester.Shared.Commands;

namespace LoadTester;

public class Program
{
    private static IEndpointInstance _endpointInstance;
    private static string _connection;
    private static string _endpointNameReceiver = "LoadTester";

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

            _connection = configuration.GetConnectionString("DefaultConnection");
            var endpointConfiguration = new EndpointConfiguration(_endpointNameReceiver);

            endpointConfiguration.SendFailedMessagesTo("error");
            //endpointConfiguration.AuditProcessedMessagesTo("audit");
            endpointConfiguration.EnableInstallers();


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
            //transport.UseSchemaForQueue("LoadTester", "sender");
            //transport.Transactions(TransportTransactionMode.SendsAtomicWithReceive);

            //transport.Routing().RouteToEndpoint(typeof(LoadTestingCommand), "LoadTester");

            //var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            //var dialect = persistence.SqlDialect<SqlDialect.MsSqlServer>();
            //dialect.Schema(schemaName);
            //persistence.ConnectionBuilder(() => new SqlConnection(_connection));

            SqlHelper.CreateSchema(_connection, "sender");

            return endpointConfiguration;
        });

        return builder.ConfigureServices(services =>
        {
            services.AddScoped<MessageService>();
            services.AddHostedService<NServiceBusLoader>();
        });
    }
}