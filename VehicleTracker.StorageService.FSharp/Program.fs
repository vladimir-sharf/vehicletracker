module VehicleTracker.StorageService.FSharp.App

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Serilog
open VehicleTracker.StorageService.FSharp.Startup
open Microsoft.Extensions.Configuration
open VehicleTracker.StorageService.FSharp.DbInitializer
open VehicleTracker.StorageService.FSharp.DbInitializerData

let readConfig() =
    let builder = new ConfigurationBuilder()
    builder.AddJsonFile "appSettings.json" |> ignore
    builder.AddEnvironmentVariables() |> ignore
    builder.Build()

let configureLogging (hostingContext : WebHostBuilderContext) (loggerConfiguration : LoggerConfiguration) =
         loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration) |> ignore

let configureWebHost config =     
    WebHostBuilder()
        .UseConfiguration(config)
        .UseKestrel()
        .UseIISIntegration()
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureServices(Action<WebHostBuilderContext, IServiceCollection> configureServices)
        .UseSerilog(Action<WebHostBuilderContext, LoggerConfiguration> configureLogging)

let buildWebHost (builder : IWebHostBuilder) = builder.Build()

let dataInitializer = wrap ensureDbExists >=> checkEmpty >=+> create >=+> saveChanges

[<EntryPoint>]
let main args =
    let config = readConfig()
    let host = buildWebHost (configureWebHost config)
    DbInitializerHelper.seedDatabase "Test data init" host dataInitializer |> ignore
    host.Run()
    0