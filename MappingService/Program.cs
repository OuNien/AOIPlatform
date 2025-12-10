using AOI.Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MappingService;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                var config = context.Configuration;

                var host = config["RabbitMQ:Host"] ?? "localhost";
                services.AddSingleton<IMessageBus>(new RabbitMqMessageBus(host));

                services.Configure<MappingOptions>(
                    config.GetSection("Mapping"));

                services.AddHostedService<Worker>();
            });
}
