using VehicleFilter = VehicleTracker.Api.Model.VehicleFilter;

namespace VehicleTracker.Api.Storage.Rest
{
    public class QueryStringVehicleFilterTransformer : IQueryStringFilterTransformer<VehicleFilter>
    {
        public string Transform(VehicleFilter filter)
            => filter?.CustomerId != null ? $"?customerId={filter.CustomerId}" : "";
    }
}