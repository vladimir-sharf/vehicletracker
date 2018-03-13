using RabbitMQ.Client;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace VehicleTracker.ServiceBus
{
    public class RabbitMqBus : IServiceBus
    {
        private readonly IOptions<RabbitMqOptions> _options;
        private readonly ILogger _logger;

        public RabbitMqBus(IOptions<RabbitMqOptions> options, ILogger<RabbitMqBus> logger)
        {
            _options = options;
            _logger = logger;
        }

        private void PublishMessage(string exchange, string routingKey, IModel channel, byte[] body)
            => channel.BasicPublish(exchange: exchange,
                                        routingKey: routingKey,
                                        basicProperties: null,
                                        body: body);

        public Task Publish<T>(string address, T message, InteractionType interactionType)
        {
            switch (interactionType) 
            {
                case InteractionType.CompetingConsumers:
                    return Publish(address, message);
                case InteractionType.PublishSubscribe:
                    return Broadcast(address, message);
            }

            throw new NotImplementedException();
        }

        public Task Publish<T>(string queueName, T message)
        {
            using (var connection = _options.Value.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

                var body = message.SerializeBytes();
                channel.BasicPublish(exchange: "",
                                        routingKey: queueName,
                                        basicProperties: null,
                                        body: body);

                _logger.LogTrace($"Sent message {message.Serialize()} to {queueName}");

                return Task.CompletedTask;
            }
        }

        public Task Broadcast<T>(string exchange, T message) 
        {
            using (var connection = _options.Value.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange, "fanout");

                var body = message.SerializeBytes();
                channel.BasicPublish(exchange: exchange,
                                        routingKey: "",
                                        basicProperties: null,
                                        body: body);

                _logger.LogTrace($"Broadcasted message {message.Serialize()} to {exchange}");

                return Task.CompletedTask;
            }
        }
    }
}
