module VehicleTracker.StorageService.FSharp.DbInitializerDataHelper
open VehicleTracker.StorageService.FSharp.Models
open System

type ContextTransformer = VehiclesContext -> VehiclesContext

let addCustomer name address (context : VehiclesContext) =
    let c = new Customer()
    c.Id <- Guid.NewGuid()
    c.Name <- name
    c.Address <- address
    context.Customers.Add(c) |> ignore
    c

let addVehicle vin regNr (context : VehiclesContext) (customer : Customer) =
    let v = new Vehicle()
    v.Id <- vin
    v.CustomerId <- customer.Id
    v.RegNr <- regNr
    context.Vehicles.Add(v) |> ignore
    v

let private bind f g context =
    let c = f context 
    g context c |> ignore
    context

let (>|>) f g = bind f g

let private plus f g context c = 
    f context c |> ignore
    g context c

let (<+>) f g = plus f g
