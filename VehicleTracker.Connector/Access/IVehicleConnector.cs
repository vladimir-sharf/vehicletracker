using System.Threading.Tasks;
using VehicleTracker.ServiceBus.Messages;

namespace VehicleTracker.Connector 
{
    public interface IVehicleConnector
    {
        Task<VehicleStatusMessage> Ping(string id);
    }
}