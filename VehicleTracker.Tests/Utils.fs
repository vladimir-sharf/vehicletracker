module VehicleTracker.Tests.Utils
open Microsoft.Extensions.Configuration
open VehicleTracker.StorageService.FSharp.App
open Microsoft.AspNetCore.TestHost
open VehicleTracker.StorageService.FSharp.DbInitializerHelper
open VehicleTracker.StorageService.FSharp.Models
open VehicleTracker.StorageService.FSharp.DbInitializer

let readConfig() = 
    let builder = new ConfigurationBuilder() 
    builder.AddJsonFile "appSettings.StorageService.json" |> ignore
    builder.AddEnvironmentVariables() |> ignore
    builder.Build()

let configureWebHost() = 
    configureWebHost (readConfig())

let createServer() = new TestServer(configureWebHost())

let createClient (server : TestServer) = server.CreateClient()

let private ensureDbDeleted (context : VehiclesContext) = context.Database.EnsureDeleted()

let withServer initializer f = 
    async {
        use server = createServer()
        use client = createClient server
        try 
            match initializer with
            | None -> ()
            | Some initializer -> seedDatabase "Test data init" server.Host initializer |> ignore
            return! f client
        finally
            seedDatabase "Removing test DB" server.Host (wrap ensureDbDeleted) |> ignore
    }

