using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOI.Infrastructure.Messaging
{
    public interface IMessageBus
    {
        Task PublishAsync<T>(T message, string routingKey);

        Task SubscribeAsync<T>(string queueName, Func<T, Task> handler);
    }



}
