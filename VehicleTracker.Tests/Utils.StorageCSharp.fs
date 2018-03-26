module VehicleTracker.Tests.UtilsStorageCSharp

open VehicleTracker.Tests.UtilsServices
open VehicleTracker.StorageService
open Microsoft.AspNetCore.Hosting
open Serilog
open System
open Autofac.Extensions.DependencyInjection
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Configuration
open VehicleTracker.StorageService.FSharp.Models
open Microsoft.EntityFrameworkCore

let configureLogging (hostingContext : WebHostBuilderContext) (loggerConfiguration : LoggerConfiguration) =
     loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration) |> ignore
     
let configureWebHost config =     
    WebHostBuilder()
        .UseConfiguration(config)
        .UseKestrel()
        .ConfigureServices(Action<IServiceCollection> (fun services -> 
            services.AddAutofac() |> ignore
            services.AddDbContext<VehiclesContext>(
                fun (options : DbContextOptionsBuilder) -> 
                    options.UseSqlServer(config.GetConnectionString("DefaultConnection")) |> ignore
                ) |> ignore
        ))
        .UseSerilog(Action<WebHostBuilderContext, LoggerConfiguration> configureLogging)
        .UseStartup<Startup>()

let webHostBuilder testName nxt = fun () -> 
    let builder = readConfig "appSettings.StorageService.json" 
                    |> substituteConnectionString "DefaultConnection" testName 
                    |> configureWebHost
    nxt builder

let setupTest testName init deinit f = 
    webHostBuilder testName >=> withServer >=> dbContext init deinit >=> withClient <| f <| ()
