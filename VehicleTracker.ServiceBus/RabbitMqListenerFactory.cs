using RabbitMQ.Client;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace VehicleTracker.ServiceBus
{
    public class RabbitMqListenerFactory : IServiceBusListenerFactory, IDisposable
    {
        private readonly Lazy<Task<IServiceBusListener>> _listener;
        private readonly ILogger _logger;
        private readonly IOptions<RabbitMqOptions> _options;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public RabbitMqListenerFactory(IOptions<RabbitMqOptions> options, ILogger<RabbitMqListenerFactory> logger)
        {
            _options = options;
            _logger = logger;
            _cancellationTokenSource = new CancellationTokenSource();
            _listener = new Lazy<Task<IServiceBusListener>>(() => Create(_cancellationTokenSource.Token));
        }

        public Task<IServiceBusListener> Create()
            => _listener.Value;

        private async Task<IServiceBusListener> Create(CancellationToken token)
        {
            RabbitMqListener result;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    result = new RabbitMqListener(_options, _logger);;
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
        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            if (_listener.IsValueCreated && _listener.Value != null && _listener.Value.IsCompleted) 
            {
                var listener = _listener.Value.Result as IDisposable;
                if (listener != null) listener.Dispose();
            }
        }
    }
}
