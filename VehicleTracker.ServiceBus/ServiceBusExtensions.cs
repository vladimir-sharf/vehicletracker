using System;
using System.Threading.Tasks;

namespace VehicleTracker.ServiceBus 
{
    public static class ServiceBusExtensions 
    {
        public static async Task<IServiceBusListener> Subscribe<T>(this Task<IServiceBusListener> factoryTask, string address, Func<T, Task> callback, InteractionType interactionType) 
        {
            var listener = await factoryTask;
            listener.Subscribe(address, callback, interactionType);
            return listener;
        }
    }
}