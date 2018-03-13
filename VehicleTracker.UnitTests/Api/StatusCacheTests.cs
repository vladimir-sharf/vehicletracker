using Xunit;
using VehicleTracker.ServiceBus.Messages;
using Shouldly;
using System.Threading.Tasks;
using VehicleTracker.Api.Storage.StatusCache;

namespace VehicleTracker.UnitTests
{
    public class StatusCacheTests
    {
        [Fact]
        public async Task StatusCache_UpdateGet_Test()
        {
            var logger = CommonUtils.CreateLogger<StatusCache>();
            var cache = CommonUtils.CreateMemoryCache();
            var statusCache = new StatusCache(cache, logger);

            await statusCache.Update("1", new VehicleStatusMessage("1", VehicleStatus.Connected));
            var result = await statusCache.Get("1");
            result.ShouldNotBeNull();
            result.Id.ShouldBe("1");
            result.Status.ShouldBe(VehicleStatus.Connected);
        }

        [Fact]
        public async Task StatusCache_GetNonExistent_Test()
        {
            var logger = CommonUtils.CreateLogger<StatusCache>();
            var cache = CommonUtils.CreateMemoryCache();
            var statusCache = new StatusCache(cache, logger);

            var result = await statusCache.Get("1");
            result.ShouldNotBeNull();
            result.Id.ShouldBe("1");
            result.Status.ShouldBe(VehicleStatus.Unknown);
        }
    }
}
