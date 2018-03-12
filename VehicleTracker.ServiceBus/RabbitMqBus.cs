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
        private Lazy<Task<RabbitMqListener>> _listener;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly IOptions<RabbitMqOptions> _options;
        private ILogger _logger;

        public RabbitMqBus(IOptions<RabbitMqOptions> options, ILogger<RabbitMqBus> logger)
        {
            _options = options;
            _cancellationTokenSource = new CancellationTokenSource();
            _listener = new Lazy<Task<RabbitMqListener>>(() => CreateListenerSafe(_cancellationTokenSource.Token));
            _logger = logger;
        }

        private async Task<RabbitMqListener> CreateListenerSafe(CancellationToken token)
        {
            RabbitMqListener result;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    result = CreateListener();
                    _logger.LogInformation($"RabbitMq message listener created");
                    return result;
                }
                catch(Exception e)
                {
                    _logger.LogWarning(e, $"Create RabbitMq message listener failed. Retry after 10 seconds");
                    await Task.Delay(TimeSpan.FromSeconds(10), token);
                }
            }
            _logger.LogWarning($"Creation of RabbitMq message listener was cancelled");
            return null;
        }

        private RabbitMqListener CreateListener()
        {
            var connection = CreateConnection();
            try
            {
                var channel = connection.CreateModel();
                return new RabbitMqListener(connection, channel, _logger);
            }
            catch
            {
                connection.Close();
                throw;
            }
        }

        private IConnection CreateConnection()
        {
            var options = _options.Value;
            var factory = new ConnectionFactory()
            {
                HostName = options.HostName,
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password,
                AutomaticRecoveryEnabled = true
            };
            return factory.CreateConnection();
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
            using (var connection = CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

                var body = message.SerializeBytes();
                PublishMessage("", queueName, channel, body);
                _logger.LogTrace($"Sent message {message.Serialize()} to {queueName}");

                return Task.FromResult(true);
            }
        }

        public Task Broadcast<T>(string exchange, T message) 
        {
            using (var connection = CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange, "fanout");

                var body = message.SerializeBytes();
                PublishMessage(exchange, "", channel, body);

                _logger.LogTrace($"Broadcasted message {message.Serialize()} to {exchange}");

                return Task.FromResult(true);
            }
        }

        public async Task Subscribe<T>(string address, Func<T, Task> callback, InteractionType interactionType)
        {
            var listener = await _listener.Value;
            if (listener != null)
            {
                listener.Listen(address, message => callback(message.Deserialize<T>()), interactionType);
                _logger.LogInformation($"Subscribed on message channel {address}, {interactionType}");
            }
        }
            
        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            if (_listener.IsValueCreated && _listener.Value != null)
                _listener.Value.Dispose();
        }
    }
}
