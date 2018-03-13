using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleTracker.ServiceBus;
using VehicleTracker.ServiceBus.Messages;
using VehicleTracker.TrackerManager.Configuration;
using VehicleTracker.TrackerManager.Model;
using VehicleTracker.TrackerManager.Services;
using VehicleTracker.TrackerManager.Storage;
using Xunit;

namespace VehicleTracker.UnitTests
{
    public class TrackerManagerTests
    {
        [Fact]
        public async Task VehicleRepositoryInMemory_Test()
        {
            var repository = new VehicleRepositoryInMemory();

            var items = new[]
            {
                new VehicleSubscription("1", DateTime.UtcNow),
                new VehicleSubscription("2", DateTime.UtcNow),
            };

            foreach (var item in items)
            {
                await repository.Add(item);
            }

            var result = await repository.Get();
            result.ShouldBe(items, true);

            await repository.Remove(items[0].Id);

            result = await repository.Get();

            result.ShouldBe(new[] { items[1] }, true);
        }

        [Fact]
        public async Task VehicleRepositoryInMemory_Duplicates_Test()
        {
            var repository = new VehicleRepositoryInMemory();

            var items = new[]
            {
                new VehicleSubscription("1", DateTime.UtcNow.AddDays(-1)),
                new VehicleSubscription("1", DateTime.UtcNow),
            };

            foreach (var item in items)
            {
                await repository.Add(item);
            }

            var result = await repository.Get();
            result.ShouldBe(new[] { items[1] }, true);
        }

        [Fact]
        public async Task VehicleService_QueryVehicles_Test()
        {
            var vehicleRepository = new Mock<IVehicleRepository>();
            vehicleRepository.Setup(x => x.Get())
                .Returns(() => Task.FromResult((IEnumerable<VehicleSubscription>)new[] {
                    new VehicleSubscription("1", DateTime.Now),
                    new VehicleSubscription("2", DateTime.Now),
                }));

            var messageList = new List<VehicleTrackRequest>();
            var serviceBus = new Mock<IServiceBus>();
            var (address, interactionType) = ((string)null, InteractionType.CompetingConsumers);
            var bus = CommonUtils.CreateServiceBus((string a, VehicleTrackRequest m, InteractionType t) =>
            {
                address = a;
                interactionType = t;
                messageList.Add(m);
            });

            var options = new Mock<IOptions<TrackingOptions>>();
            options.Setup(x => x.Value)
                .Returns(new TrackingOptions
                {
                    TrackingTimeout = 10
                });

            var logger = CommonUtils.CreateLogger<VehicleService>();

            var vehicleService = new VehicleService(vehicleRepository.Object, bus, options.Object, logger);

            await vehicleService.QueryVehicles();

            messageList.Count.ShouldBe(2);
            messageList.Select(x => x.Id).ShouldBe(new[] { "1", "2" }, true);
        }
    }
}
