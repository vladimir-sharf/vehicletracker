using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading.Tasks;

namespace VehicleTracker.ServiceBus
{
    public class RabbitMqListener : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger _logger;

        public RabbitMqListener(IConnection connection, IModel channel, ILogger logger)
        {
            _connection = connection;
            _channel = channel;
            _logger = logger;
        }

        public void Listen(string address, Func<byte[], Task> receive, InteractionType queueType) 
        {
            switch (queueType) 
            {
                case InteractionType.CompetingConsumers:
                    _channel.QueueDeclare(queue: address,
                                            durable: false,
                                            exclusive: false,
                                            autoDelete: false,
                                            arguments: null);
                    Listen(address, receive);
                    break;
                case InteractionType.PublishSubscribe:
                    _channel.ExchangeDeclare(address, "fanout");
                    var tmpQueueName = _channel.QueueDeclare().QueueName;
                    _channel.QueueBind(tmpQueueName, address, "");
                    Listen(tmpQueueName, receive);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        
        private void Listen(string queueName, Func<byte[], Task> receive)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                _logger.LogTrace($"Received message on channel {queueName}: {ea.Body.Stringify()}");
                var body = ea.Body;
                receive(body)
                    .ContinueWith(t => _channel.BasicAck(ea.DeliveryTag, false), TaskContinuationOptions.OnlyOnRanToCompletion);
            };
            _channel.BasicConsume(queueName, false, consumer);
        }

        public void Dispose()
        {
            if (_channel != null)
            {
                _channel.Dispose();
            }

            if (_connection != null)
            {
                _connection.Dispose();
            }
        }
    }
}
