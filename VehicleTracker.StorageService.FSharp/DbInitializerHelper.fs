namespace VehicleTracker.StorageService.FSharp

open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open System
open VehicleTracker.StorageService.FSharp.Models
open VehicleTracker.StorageService.FSharp.DbInitializer
open System.Threading

module DbInitializerHelper = 
    let createContext (services : IServiceProvider) = services.GetRequiredService<VehiclesContext>()

    let log (logger : ILogger<VehiclesContext>) level message a =
        match level with
        | LogLevel.Error -> logger.LogError(message) |> ignore
        | LogLevel.Warning -> logger.LogWarning(message) |> ignore
        | LogLevel.Information -> logger.LogInformation(message) |> ignore
        | _ -> logger.LogDebug(message) |> ignore
        a

    let rec seedDatabaseLoop name (logger : ILogger<VehiclesContext>) initializer =
        match initializer with
        | Success result -> 
            logger.LogWarning(sprintf "%s completed" name) |> ignore
            Success result
        | Skip m -> 
            logger.LogInformation(sprintf "%s skipped, reason: %s" name m) |> ignore
            Skip m
        | Error e -> 
            logger.LogError(e)
            logger.LogInformation("Waiting 10 sec...")
            Thread.Sleep(10000)
            seedDatabaseLoop name logger initializer

    let seedDatabase name (host : IWebHost) initializer =
        use scope = host.Services.CreateScope()
        let services = scope.ServiceProvider
        let logger = services.GetRequiredService<ILogger<VehiclesContext>>()
        services 
            |> (
                wrap (log logger LogLevel.Debug "Connecting to DB")
                >=+> createContext 
                >=+> log logger LogLevel.Debug "Connected"
                >=> initializer
            ) 
            |> seedDatabaseLoop name logger

