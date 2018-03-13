using System;
using System.Threading.Tasks;

namespace VehicleTracker.ServiceBus
{
    public interface IServiceBusListener 
    {
        void Subscribe<T>(string address, Func<T, Task> callback, InteractionType interactionType);
    }
}