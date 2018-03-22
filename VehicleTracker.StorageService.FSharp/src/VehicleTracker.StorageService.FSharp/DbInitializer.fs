namespace VehicleTracker.StorageService.FSharp

open VehicleTracker.StorageService.FSharp.Models
open System
open System.Linq
open Microsoft.Extensions.Logging

type Result<'a> = Success of 'a | Skip

module DbInitializer = 
    let private addCustomer name address (context : VehiclesContext) =
        let c = new Customer()
        c.Id <- Guid.NewGuid()
        c.Name <- name
        c.Address <- address
        context.Customers.Add(c) |> ignore
        c

    let private addVehicle vin regNr (context : VehiclesContext) (customer : Customer) =
        let v = new Vehicle()
        v.Id <- vin
        v.CustomerId <- customer.Id
        v.RegNr <- regNr
        context.Vehicles.Add(v) |> ignore
        v

    let private bind f g context =
        let c = f context 
        g context c |> ignore
        c

    let private (>|>) f g = bind f g

    let private returnContext f context =
        f context |> ignore
        context

    let private createKalles = 
        addCustomer "Kalles Grustransporter AB" "Cementvägen 8, 111 11 Södertälje"
            >|> addVehicle "YS2R4X20005399401" "ABC123"
            >|> addVehicle "VLUR4X20009093588" "DEF456"
            >|> addVehicle "VLUR4X20009048066" "GHI789"
            |> returnContext

    let private createJohans = 
        addCustomer "Johans Bulk AB" "Balkvägen 12, 222 22 Stockholm"
            >|> addVehicle "YS2R4X20005388011" "JKL012"
            >|> addVehicle "YS2R4X20005387949" "MNO345"
            |> returnContext

    let private createHaralds = 
        addCustomer "Haralds Värdetransporter AB" "BalkvägenBudgetvägen 1, 333 33 Uppsala"
            >|> addVehicle "YS2R4X20005387765" "PQR678"
            >|> addVehicle "YS2R4X20005387055" "STU901"
            |> returnContext

    let private saveChanges (logger : ILogger) (context : VehiclesContext) = 
        logger.LogInformation("New records saved to database")
        context.SaveChanges()

    let private ensureDbExists (context : VehiclesContext) = 
        context.Database.EnsureCreated() |> ignore
        context

    let private check (logger : ILogger) (context : VehiclesContext) = 
        match context.Vehicles.Any() with
        | true -> logger.LogInformation("Database is already created. Skipping"); Skip
        | false -> logger.LogInformation("Seeding the database"); Success context

    let private bind2 f g context =
        let result = f context
        match result with
        | Success c -> Success (g c)
        | _ -> Skip

    let (>=>) f g = bind2 f g

    let init (logger : ILogger) = ensureDbExists >> check logger >=> createKalles >=> createJohans >=> createHaralds >=> saveChanges logger
