using AOI.Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SchedulerService;

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

                // RabbitMQ Host (可從設定讀取)
                var host = config["RabbitMQ:Host"] ?? "localhost";
                services.AddSingleton<IMessageBus>(new RabbitMqMessageBus(host));

                // Scheduler 設定
                services.Configure<SchedulerOptions>(config.GetSection("Scheduler"));

                services.AddHostedService<Worker>();
            });
}
