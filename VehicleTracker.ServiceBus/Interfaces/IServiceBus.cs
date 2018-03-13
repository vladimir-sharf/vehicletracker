using System;
using System.Threading.Tasks;

namespace VehicleTracker.ServiceBus
{
    public interface IServiceBus
    {
        Task Publish<T>(string address, T message, InteractionType interactionType);
    }
}
