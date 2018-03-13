using System;
using System.Threading.Tasks;

namespace VehicleTracker.ServiceBus
{
    public interface IServiceBusListenerFactory 
    {
        Task<IServiceBusListener> Create();
    }
}