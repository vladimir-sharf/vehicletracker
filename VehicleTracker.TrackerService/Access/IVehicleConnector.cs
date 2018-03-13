using System.Threading.Tasks;
using VehicleTracker.ServiceBus.Messages;

namespace VehicleTracker.TrackerService 
{
    public interface IVehicleConnector
    {
        Task<VehicleStatusMessage> Ping(string id);
    }
}