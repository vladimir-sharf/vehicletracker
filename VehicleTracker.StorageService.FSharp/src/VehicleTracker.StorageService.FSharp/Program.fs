module VehicleTracker.StorageService.FSharp.App

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Serilog
open VehicleTracker.StorageService.FSharp.Startup
open Microsoft.Extensions.Configuration

let readConfig() =
    let builder = new ConfigurationBuilder()
    builder.AddJsonFile "appSettings.json" |> ignore
    builder.AddEnvironmentVariables() |> ignore
    builder.Build()

let configureLogging (hostingContext : WebHostBuilderContext) (loggerConfiguration : LoggerConfiguration) =
         loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration) |> ignore

let buildWebHost args =     
    WebHostBuilder()
        .UseConfiguration(readConfig())
        .UseKestrel()
        .UseIISIntegration()
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureServices(Action<WebHostBuilderContext, IServiceCollection> configureServices)
        .UseSerilog(Action<WebHostBuilderContext, LoggerConfiguration> configureLogging)
        .Build()

[<EntryPoint>]
let main args =
    let host = buildWebHost args
    DbInitializerHelper.seedDatabase host |> ignore
    host.Run()
    0