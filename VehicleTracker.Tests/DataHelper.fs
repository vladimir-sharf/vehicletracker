module VehicleTracker.Tests.DataHelper

open VehicleTracker.StorageService.FSharp.Models
open System
open VehicleTracker.StorageService.FSharp.DbInitializer

type Vehicle = {
    Id: VIN
    RegNr: RegNr
    CustomerId: CustomerId
} 

type Customer = {
    Id: CustomerId
    Name: CustomerName
    Address: CustomerAddress
}

let addCustomer name address = {
        Id = Guid.NewGuid()
        Name = name
        Address = address
    }

let addVehicle vin regNr customerId = {
        Id = vin
        RegNr = regNr
        CustomerId = customerId
    }

let rec createVehicles (vehicles : Vehicle list) (context : VehicleTracker.StorageService.FSharp.Models.VehiclesContext) = 
    match vehicles with
    | [] -> context
    | v::rest -> 
        let vehicle = new VehicleTracker.StorageService.FSharp.Models.Vehicle();
        vehicle.Id <- v.Id
        vehicle.RegNr <- v.RegNr
        vehicle.CustomerId <- v.CustomerId
        context.Vehicles.Add(vehicle) |> ignore
        createVehicles rest context 

let rec createCustomers customers context = 
    match customers with
    | [] -> context
    | c::rest -> 
        let mc = VehicleTracker.StorageService.FSharp.DbInitializerDataHelper.addCustomer c.Name c.Address context
        mc.Id <- c.Id
        createCustomers rest context


let ensureDbDeleted (context : VehiclesContext) = context.Database.EnsureDeleted()
let initializer dataInit = wrap ensureDbExists >=> checkEmpty >=!> dataInit >=+> saveChanges
let emptyDb ctx = initializer id ctx

let dropDb context = wrap ensureDbDeleted context
let saveDb = (fun context -> Skip "Database should be saved")
let defaultDeinitializer = dropDb
