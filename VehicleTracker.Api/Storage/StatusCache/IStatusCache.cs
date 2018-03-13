using System.Threading.Tasks;
using VehicleTracker.ServiceBus.Messages;

namespace VehicleTracker.Api.Storage.StatusCache
{
    public interface IStatusCache
    {
        Task<VehicleStatusMessage> Get(string id);
        Task<VehicleStatusMessage> Update(string id, VehicleStatusMessage vehicle);
    }
}
