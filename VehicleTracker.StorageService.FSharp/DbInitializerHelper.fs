namespace VehicleTracker.StorageService.FSharp

open Microsoft.Extensions.Logging
open System.Threading
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open System
open VehicleTracker.StorageService.FSharp.Models

module DbInitializerHelper = 
    let seedDatabaseTry (services : IServiceProvider) (logger : ILogger)= 
        try 
            logger.LogInformation("Trying to connect to the database...")
            let context = services.GetRequiredService<VehiclesContext>()
            Some (DbInitializer.init logger context)
        with
            | e -> logger.LogError("An error occurred while seeding the database: " + e.Message); None

    let rec seedDatabaseLoop (services : IServiceProvider) =
        let logger = services.GetRequiredService<ILogger<VehiclesContext>>()
        match seedDatabaseTry services logger with
        | Some (Success result) -> 
            logger.LogInformation("Database has been seeded") |> ignore
            Some result
        | Some Skip -> 
            None
        | None -> 
            logger.LogInformation("Waiting 10 sec...")
            Thread.Sleep(10000)
            seedDatabaseLoop services

    let seedDatabase (host : IWebHost) =
        use scope = host.Services.CreateScope()
        let services = scope.ServiceProvider
        seedDatabaseLoop services

