using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading.Tasks;

namespace VehicleTracker.ServiceBus
{
    public class RabbitMqListener : IServiceBusListener, IDisposable
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly ILogger _logger;

        public RabbitMqListener(IOptions<RabbitMqOptions> options, ILogger logger)
        {
            _logger = logger;
            CreateChannel(options);
        }

        private void CreateChannel(IOptions<RabbitMqOptions> options)
        {
            _connection = options.Value.CreateConnection();
            try
            {
                _channel = _connection.CreateModel();
            }
            catch
            {
                _connection.Close();
                throw;
            }
        }

        public void Subscribe<T>(string address, Func<T, Task> callback, InteractionType interactionType)
        {
            Subscribe(address, message => callback(message.Deserialize<T>()), interactionType);
            _logger.LogInformation($"Subscribed on message channel {address}, {interactionType}");
        }

        private void Subscribe(string address, Func<byte[], Task> receive, InteractionType queueType) 
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
