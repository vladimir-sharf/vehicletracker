using Microsoft.Extensions.Caching.Memory;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracker.ServiceBus;
using VehicleTracker.ServiceBus.Messages;
using VehicleTracker.TrackerService;
using Xunit;

namespace VehicleTracker.UnitTests
{
    public class TrackerServiceTests
    {
        [Fact]
        public async Task VehicleService_PindById_Test()
        {
            var pingCount = 0;
            var connector = CreateVehicleConnector(() => pingCount++);

            var (msgCount, address, msg, interactionType) = 
                (0, (string)null, (VehicleStatusMessage)null, InteractionType.CompetingConsumers);
            var bus = CommonUtils.CreateServiceBus((string a, VehicleStatusMessage m, InteractionType t) =>
            {
                msgCount++;
                address = a;
                msg = m;
                interactionType = t;
            });

            var logger = CommonUtils.CreateLogger<VehicleService>();

            var vehicleService = new VehicleService(connector, bus, logger);
            var result = await vehicleService.Ping("1");

            result.ShouldNotBeNull();
            result.Id.ShouldBe("1");

            pingCount.ShouldBe(1);
            msgCount.ShouldBe(1);

            msg.Id.ShouldBe("1");
            msg.Status.ShouldBe(VehicleStatus.Connected);

            address.ShouldBe(QueueNames.StatusQueueName);
            interactionType.ShouldBe(InteractionType.PublishSubscribe);
        }

        [Fact]
        public async Task CachingVehicleService_SkipOutdated_Test()
        {
            var memoryCache = new Mock<IMemoryCache>();
            var underlying = new Mock<IVehicleService>();
            underlying
                .Setup(x => x.Ping(It.IsAny<string>()))
                .Throws(new Exception("Underlying ping is called"));
            var logger = CommonUtils.CreateLogger<CachingVehicleService>();

            var vehicleService = new CachingVehicleService(underlying.Object, memoryCache.Object, logger);

            var request = new VehicleTrackRequest("1")
            {
                TimeUtc = DateTime.UtcNow.AddDays(-1)
            };
            var result = await vehicleService.Ping(request);

            result.ShouldBeNull();
        }

        [Fact]
        public async Task CachingVehicleService_SkipRepeated_Test()
        {
            var memoryCache = CreateCache();
            var pingCount = 0;
            var underlying = new Mock<IVehicleService>();
            underlying
                .Setup(x => x.Ping(It.IsAny<string>()))
                .Returns<string>((id) => Task.FromResult(new VehicleStatusMessage(id, VehicleStatus.Connected)))
                .Callback(() => pingCount++);

            var logger = CommonUtils.CreateLogger<CachingVehicleService>();

            var vehicleService = new CachingVehicleService(underlying.Object, memoryCache, logger);

            var request = new VehicleTrackRequest("1");
            var result = await vehicleService.Ping(request);
            var result2 = await vehicleService.Ping(request);

            pingCount.ShouldBe(1);

            result.ShouldNotBeNull();
            result.Id.ShouldBe("1");

            result2.ShouldBeNull();
        }

        private IVehicleConnector CreateVehicleConnector(Action onPing)
        {
            var connector = new Mock<IVehicleConnector>();
            connector
                .Setup(x => x.Ping(It.IsAny<string>()))
                .Returns<string>(id => Task.FromResult(new VehicleStatusMessage(id, VehicleStatus.Connected)))
                .Callback(onPing);
            return connector.Object;
        }

        private IMemoryCache CreateCache()
        {
            var dictionary = new Dictionary<object, object>();
            var cache = new Mock<IMemoryCache>();
            cache.Setup(x => x.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny))
                .Returns(new TryGetValueDelegate((object key, out object b) => dictionary.TryGetValue(key, out b)));

            cache.Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Callback((object key) => dictionary[key] = true)
                .Returns(new Mock<ICacheEntry>().Object);
                
            return cache.Object;
        }

        private delegate bool TryGetValueDelegate(object key, out object value);
    }
}
