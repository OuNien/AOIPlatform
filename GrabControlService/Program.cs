using AOI.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace GrabControlService;

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
                var bus = new RabbitMqMessageBus(host);
                services.AddSingleton<IMessageBus>(bus);

                // 註冊 GrabControl 設定
                services.Configure<GrabControlOptions>(config.GetSection("GrabControl"));

                services.AddHostedService<Worker>();
            });
}
