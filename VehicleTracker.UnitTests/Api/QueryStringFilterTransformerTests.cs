using Shouldly;
using System;
using System.Collections.Generic;
using VehicleTracker.Api.Model;
using VehicleTracker.Api.Storage.Rest;
using Xunit;

namespace VehicleTracker.UnitTests
{
    public class QueryStringFilterTransformerTests
    {
        public static Guid CustomerId = new Guid("8E3E9B7A-A96B-4240-B62F-1EEF1176CA9F");

        public static IEnumerable<object[]> QueryStringVehicleFilterTransformer_Test_Data = new[] {
                new object[]
                {
                    new VehicleFilter { CustomerId = CustomerId },
                    $"?customerId={CustomerId}"
                },
                new object[]
                {
                    new VehicleFilter { CustomerId = null },
                    ""
                },
                new object[]
                {
                    null,
                    ""
                }
            };

        [Theory]
        [MemberData(nameof(QueryStringVehicleFilterTransformer_Test_Data))]
        public void QueryStringVehicleFilterTransformer_Test(VehicleFilter filter, string qs)
            => new QueryStringVehicleFilterTransformer()
                .Transform(filter)
                .ShouldBe(qs);

        public static IEnumerable<object[]> QueryStringFilterTransformerDefault_Test_Data = new[] {
                new object[]
                {
                    new VehicleFilter { CustomerId = CustomerId },
                    ""
                },
                new object[]
                {
                    new VehicleFilter { CustomerId = null },
                    ""
                },
                new object[]
                {
                    null,
                    ""
                }
            };

        [Theory]
        [MemberData(nameof(QueryStringFilterTransformerDefault_Test_Data))]
        public void QueryStringFilterTransformerDefault_Test(VehicleFilter filter, string qs)
            => new QueryStringFilterTransformerDefault<VehicleFilter>()
                .Transform(filter)
                .ShouldBe(qs);
    }
}
