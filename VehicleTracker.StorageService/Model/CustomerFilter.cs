using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VehicleTracker.StorageService.Model
{
    public class CustomerFilter
    {
        public CustomerFilter(Guid? id) 
        {
            Id = id;
        }
        
        public Guid? Id { get; set;}
    }
}
