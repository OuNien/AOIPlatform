using AOI.Infrastructure.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace InspectControlService;

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

                // 建立 RabbitMQ Bus
                var host = config["RabbitMQ:Host"] ?? "localhost";
                services.AddSingleton<IMessageBus>(new RabbitMqMessageBus(host));

                // 載入 InspectControl 設定
                services.Configure<InspectControlOptions>(
                    config.GetSection("InspectControl"));

                services.AddHostedService<Worker>();
            });
}
