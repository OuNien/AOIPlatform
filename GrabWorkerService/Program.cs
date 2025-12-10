using AOI.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                // 單獨跑 GrabWorkerService 專案時會用到這個 bus
                var bus = new RabbitMqMessageBus("localhost");
                services.AddSingleton<IMessageBus>(bus);

                services.AddHostedService<Worker>();
            });
}
