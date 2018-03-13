namespace VehicleTracker.Api.Storage.Rest
{
    public class QueryStringFilterTransformerDefault<TFilter> : IQueryStringFilterTransformer<TFilter>
    {
        public string Transform(TFilter filter)
            => "";
    }
}