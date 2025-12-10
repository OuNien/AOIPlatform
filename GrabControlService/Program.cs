using AOI.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                // »P SchedulerService ¦@¥Î InMemoryMessageBus
                var bus = new RabbitMqMessageBus("localhost");
                services.AddSingleton<IMessageBus>(bus);


                services.AddHostedService<Worker>();
            });
}
