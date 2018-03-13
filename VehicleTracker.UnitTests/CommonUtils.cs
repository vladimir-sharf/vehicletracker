using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using VehicleTracker.ServiceBus;

namespace VehicleTracker.UnitTests
{
    public static class CommonUtils
    {
        public static ILogger<T> CreateLogger<T>()
        {
            var logger = new Mock<ILogger<T>>();
            return logger.Object;
        }

        public static async Task StopOnCondition(this TimeSpan maxTime, TimeSpan interval, Func<bool> condition)
        {
            var start = DateTime.Now;
            while (!condition())
            {
                if ((DateTime.Now - start) > maxTime) break;
                await Task.Delay(interval);
            }
        }

        public static IServiceBus CreateServiceBus<T>(Action<string, T, InteractionType> publishStatusCallback)
        {
            var bus = new Mock<IServiceBus>();
            bus.Setup(x => x.Publish(It.IsAny<string>(),
                It.IsAny<T>(),
                It.IsAny<InteractionType>()))
                .Callback(publishStatusCallback)
                .Returns<string, T, InteractionType>((a, m, t) => Task.FromResult(m));
            return bus.Object;
        }


    }
}
