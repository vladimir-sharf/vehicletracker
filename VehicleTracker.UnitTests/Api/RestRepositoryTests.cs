using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using VehicleTracker.Api.Model;
using VehicleTracker.Api.Storage.Rest;
using Xunit;

namespace VehicleTracker.UnitTests
{
    public class RestRepositoryTests
    {
        [Fact]
        public async Task RestRepository_Get_Test()
        {
            var httpClient = new Mock<HttpClient>();
            httpClient.Object.BaseAddress = new Uri("http://localhost/");

            var transformer = new QueryStringVehicleFilterTransformer();
            var repo = new RestRepository<string, Vehicle, VehicleFilter>(httpClient.Object, transformer);

            var result = await repo.Get("1");

            result.ShouldNotBeNull();
        }
    }
}
