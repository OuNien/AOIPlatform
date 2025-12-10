using AOI.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace GrabWorkerService;

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

                // RabbitMQ Host
                var host = config["RabbitMQ:Host"] ?? "localhost";
                services.AddSingleton<IMessageBus>(new RabbitMqMessageBus(host));

                // ¸ü¤J GrabWorker °Ñ¼Æ
                services.Configure<GrabWorkerOptions>(config.GetSection("GrabWorker"));

                services.AddHostedService<Worker>();
            });
}
