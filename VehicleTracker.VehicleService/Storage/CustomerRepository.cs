using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracker.VehicleService.Context;
using VehicleTracker.VehicleService.Model;

namespace VehicleTracker.VehicleService.Storage
{
    public class CustomerRepository : IRepository<Guid, Customer, CustomerFilter>
    {
        private readonly VehicleContext _context;

        public CustomerRepository(VehicleContext context)
        {
            _context = context;
        }

        public async Task<Customer> Create(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task Delete(Guid id)
        {
            var customer = await Get(id);
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Customer>> List(CustomerFilter filter)
        {
            var result = await _context.Customers.ToListAsync();
            return result;
        }

        public async Task<Customer> Get(Guid id)
            => await _context.Customers.FirstOrDefaultAsync(x => x.Id == id) ?? throw new KeyNotFoundException();

        public async Task<Customer> Update(Guid id, Customer customer)
        {
            var existing = await Get(id);
            existing.Name = customer.Name;
            existing.Address = customer.Address;
            await _context.SaveChangesAsync();
            return existing;
        }
    }
}
