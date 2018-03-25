namespace VehicleTracker.StorageService.FSharp.DataAccess

open VehicleTracker.StorageService.FSharp.Models
open VehicleTracker.StorageService.FSharp.DataAccess.Common
open System.Linq

module Customers = 
    let customers (context : VehiclesContext) = context.Customers
    let updateCustomerFields (item : Customer) (newItem : Customer) = 
        item.Name <- newItem.Name
        item.Address <- newItem.Address
    let filterById id (q : IQueryable<Customer>) =
        query { 
            for item in q do 
            where (item.Id = id) 
            select item
        }

    let private customerFilter = 
        wrap (fun x -> x.Id) filterById 

    let listCustomers filter = list customers (Some customerFilter) filter
    let getCustomer id = get customers filterById id
    let addCustomer customer = add customers customer
    let updateCustomer item = update customers filterById updateCustomerFields item
    let deleteCustomer id = delete customers filterById id
    let extractCustomerId (x : Customer) = x.Id
