using AOI.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SchedulerService;
using GrabControlService;
using GrabWorkerService;
using MappingService;

public class Program
{
    public static async Task Main(string[] args)
    {
        var bus = new RabbitMqMessageBus("InMemory");

        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddSingleton<IMessageBus>(bus);

                // 1. 排程站
                services.AddHostedService<SchedulerService.Worker>();
                // 2. 取像控制站
                services.AddHostedService<GrabControlService.Worker>();
                // 3. 取像站
                services.AddHostedService<GrabWorkerService.Worker>();
                // 4. 辨識站
                services.AddHostedService<MappingService.Worker>();
                // 5. 檢測控制站
                services.AddHostedService<InspectControlService.Worker>();
                // 6. 檢測站
                services.AddHostedService<InspectWorkerService.Worker>();
            })
            .Build();

        await host.RunAsync();
    }
}
