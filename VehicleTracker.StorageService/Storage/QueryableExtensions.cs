
using VehicleTracker.StorageService.Storage;
using System.Linq;

namespace VehicleTracker.StorageService.Model 
{
    public static class QueryableExtensions 
    {
        public static IQueryable<Vehicle> FilterById (this IQueryable<Vehicle> q, VehicleFilter filter) =>
            filter.Id != null ? q.Where(x => x.Id == filter.Id) : q;

        public static IQueryable<Vehicle> FilterByCustomerId (this IQueryable<Vehicle> q, VehicleFilter filter) =>
            filter.CustomerId != null ? q.Where(x => x.CustomerId == filter.CustomerId) : q;

        public static IQueryable<Vehicle> Filter(this IQueryable<Vehicle> q, VehicleFilter filter) =>
            q.FilterById(filter)
            .FilterByCustomerId(filter);

        public static IQueryable<Customer> FilterById (this IQueryable<Customer> q, CustomerFilter filter) =>
            filter.Id != null ? q.Where(x => x.Id == filter.Id) : q;

        public static IQueryable<Customer> Filter(this IQueryable<Customer> q, CustomerFilter filter) =>
            q.FilterById(filter);
    }
}