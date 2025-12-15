using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AOI.Infrastructure.Messaging
{
    public class RabbitMqMessageBus : IMessageBus
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        private readonly string _exchange = "aoi.bus";

        public RabbitMqMessageBus(string host)
        {
            var factory = new ConnectionFactory
            {
                HostName = host,
                DispatchConsumersAsync = true,

                // 設備級必要：自動恢復
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Exchange 永續化
            _channel.ExchangeDeclare(
                exchange: _exchange,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false
            );

            // Publish Confirm（設備等級必須）
            _channel.ConfirmSelect();
        }

        public async Task PublishAsync<T>(T message, string routingKey)
        {
            var body = JsonSerializer.SerializeToUtf8Bytes(message);

            var props = _channel.CreateBasicProperties();
            props.DeliveryMode = 2; // persistent

            _channel.BasicPublish(
                exchange: _exchange,
                routingKey: routingKey,
                basicProperties: props,
                body: body
            );

            // 保證訊息已送達 broker
            _channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(3));
        }

        public Task SubscribeAsync<T>(string queueName, Func<T, Task> handler)
        {
            // Queue 永續
            _channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            // 綁定 routing key = queue 名稱
            _channel.QueueBind(
                queue: queueName,
                exchange: _exchange,
                routingKey: queueName
            );

            // QoS: 每次 1 個（特別重要：GPU/GrabWorker）
            _channel.BasicQos(0, 1, false);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var msg = JsonSerializer.Deserialize<T>(json);

                    await handler(msg);

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch
                {
                    // 發生錯誤 → 將訊息 requeue
                    _channel.BasicNack(ea.DeliveryTag, false, requeue: true);
                }
            };

            _channel.BasicConsume(
                queue: queueName,
                autoAck: false,
                consumer
            );

            return Task.CompletedTask;
        }
    }

}
