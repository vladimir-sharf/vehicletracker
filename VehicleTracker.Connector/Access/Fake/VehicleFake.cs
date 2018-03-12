using System;
using System.Threading.Tasks;
using VehicleTracker.ServiceBus.Messages;

namespace VehicleTracker.Connector
{
    public class VehicleFake 
    {
        private readonly double _connectChance;
        private readonly double _disconnectChance;
        private readonly int _minDelay;
        private readonly int _maxDelay;
        private readonly Random _rand = new Random();
        public VehicleFake(double connectChance, double disconnectChance, int minDelay, int maxDelay)
        {
            _connectChance = connectChance;
            _disconnectChance = disconnectChance;
            _minDelay = minDelay;
            _maxDelay = maxDelay;
        }

        private VehicleStatus _status = VehicleStatus.Disconnected;
        public async Task<VehicleStatus> Next() 
        {
            await RandomDelay();
            _status = NextStatus();
            return _status;
        }

        private Task RandomDelay()
        {
            var seconds = _rand.Next(_minDelay, _maxDelay);
            return Task.Delay(TimeSpan.FromSeconds(seconds));
        }

        private VehicleStatus NextStatus() 
        {
            var r = _rand.NextDouble();
            switch (_status)
            {
                case VehicleStatus.Connected:
                    if (r < _disconnectChance)
                        return VehicleStatus.Disconnected;
                    break;
                case VehicleStatus.Disconnected:
                    if (r < _connectChance)
                        return VehicleStatus.Connected;
                    break;
            }
            return _status;
        }
    }
}