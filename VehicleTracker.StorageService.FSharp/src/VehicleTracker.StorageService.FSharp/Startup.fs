module VehicleTracker.StorageService.FSharp.Startup

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open VehicleTracker.StorageService.FSharp.HttpHandlers
open VehicleTracker.StorageService.FSharp.Models
open Microsoft.EntityFrameworkCore
open VehicleTracker.StorageService.FSharp.DataAccess.Customers
open VehicleTracker.StorageService.FSharp.DataAccess.Vehicles
open VehicleTracker.StorageService.FSharp.Routing
open Microsoft.Extensions.Configuration

// ---------------------------------
// Web app
// ---------------------------------
let webApp =
    choose [
        subRoute "/vehicles"
            (choose [
                GET >=> routefs (handleGet getVehicle)
                DELETE >=> routefs (handleDelete deleteVehicle)
                PUT >=> routefs (handlePut updateVehicle)
                GET >=> route "/" >=> handleList listVehicles
                POST >=> route "/" >=> handlePost addVehicle
            ])
        subRoute "/customers"
            (choose [
                GET >=> routefo (handleGet getCustomer)
                DELETE >=> routefo (handleDelete deleteCustomer)
                PUT >=> routefo (handlePut updateCustomer)
                GET >=> route "/" >=> handleList listCustomers
                POST >=> route "/" >=> handlePost addCustomer
            ])                
        setStatusCode 404 >=> text "Not Found" ]

// ---------------------------------
// Error handler
// ---------------------------------
let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(EventId(), ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

// ---------------------------------
// Config
// ---------------------------------
let configureCors (builder : CorsPolicyBuilder) =
    builder.WithOrigins("http://localhost:8093")
           .AllowAnyMethod()
           .AllowAnyHeader()
           |> ignore

let configureApp (app : IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IHostingEnvironment>()
    (match env.IsDevelopment() with
    | true  -> app.UseGiraffeErrorHandler errorHandler //app.UseDeveloperExceptionPage()
    | false -> app.UseGiraffeErrorHandler errorHandler)
        .UseCors(configureCors)
        .UseGiraffe(webApp)

let configureServices (context : WebHostBuilderContext)  (services : IServiceCollection) =
    services.AddCors()    |> ignore
    services.AddGiraffe() |> ignore
    
    services.AddDbContext<VehiclesContext>(
        fun (options : DbContextOptionsBuilder) -> 
            options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")) |> ignore
        ) |> ignore

    