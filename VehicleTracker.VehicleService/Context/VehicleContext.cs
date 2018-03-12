using Microsoft.EntityFrameworkCore;
using VehicleTracker.VehicleService.Model;

namespace VehicleTracker.VehicleService.Context
{
    public class VehicleContext : DbContext
    {
        public VehicleContext(DbContextOptions<VehicleContext> options) : base(options)
        {
        }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Customer> Customers { get; set; }
    }
}
