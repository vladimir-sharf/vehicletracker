namespace VehicleTracker.Api.Storage.Rest
{
    public interface IQueryStringFilterTransformer<TFilter>
    {
        string Transform(TFilter filter);
    }
}