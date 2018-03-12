using System;
using System.Threading.Tasks;

namespace VehicleTracker.ServiceBus
{
    public interface IServiceBus : IDisposable
    {
        Task Publish<T>(string address, T message, InteractionType interactionType);
        Task Subscribe<T>(string address, Func<T, Task> callback, InteractionType interactionType);
    }
}
