using AOI.Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GrabWorkerService;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                var config = context.Configuration;

                string host = config["RabbitMQ:Host"] ?? "localhost";
                services.AddSingleton<IMessageBus>(new RabbitMqMessageBus(host));

                services.Configure<GrabWorkerOptions>(config.GetSection("GrabWorker"));

                services.AddHostedService<Worker>();
            });
}
