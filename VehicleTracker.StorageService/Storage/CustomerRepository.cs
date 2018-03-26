using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracker.StorageService.Context;
using VehicleTracker.StorageService.ErrorHandling;
using VehicleTracker.StorageService.Model;

namespace VehicleTracker.StorageService.Storage
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
            try
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (e.IsDuplicateError())
                    throw new ConflictException();
            }
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
            var result = await _context.Customers
                .Filter(filter)
                .ToListAsync();
            return result;
        }

        public async Task<Customer> Get(Guid id)
            => await _context.Customers.FirstOrDefaultAsync(x => x.Id == id) ?? throw new ObjectNotFoundException();

        public async Task<Customer> Update(Guid id, Customer customer)
        {
            if (id != customer.Id)
                throw new BadRequestException("Ids in url and in payload are different");

            var existing = await Get(id);
            existing.Name = customer.Name;
            existing.Address = customer.Address;
            await _context.SaveChangesAsync();
            return existing;
        }
    }
}
