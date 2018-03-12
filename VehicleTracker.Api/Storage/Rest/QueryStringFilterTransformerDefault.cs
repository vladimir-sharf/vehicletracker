using VehicleTracker.Api.Storage.Rest.Model;

namespace VehicleTracker.Api.Storage.Rest
{
    public class QueryStringVehicleTransformerDefault<TFilter> : IQueryStringFilterTransformer<TFilter>
    {
        public string Transform(TFilter filter)
            => "";
    }
}