namespace VehicleTracker.StorageService.FSharp.DataAccess

open VehicleTracker.StorageService.FSharp.Models
open VehicleTracker.StorageService.FSharp.DataAccess.Common
open System.Linq

module Vehicles = 
    let private vehicles (context : VehiclesContext) = context.Vehicles
    let private updateVehicleFields (item : Vehicle) (newItem : Vehicle) = 
        item.CustomerId <- newItem.CustomerId
        item.RegNr <- newItem.RegNr

    let private filterByCustomerId customerId (q : IQueryable<Vehicle>) = 
        query {
            for vehicle in q do
            where (vehicle.CustomerId = customerId)
            select vehicle
        }

    let private filterById id (q : IQueryable<Vehicle>) =
        query { 
            for item in q do 
            where (item.Id = id) 
            select item
        }

    let private vehicleFilter = 
        wrap (fun x -> x.CustomerId) filterByCustomerId  
        >+> wrap (fun x -> x.Id) filterById 

    let listVehicles filter = list vehicles (Some vehicleFilter) filter
    let getVehicle id = get vehicles filterById id
    let addVehicle vehicle =  add vehicles vehicle
    let updateVehicle item id = update vehicles filterById updateVehicleFields item id
    let deleteVehicle id = delete vehicles filterById id
