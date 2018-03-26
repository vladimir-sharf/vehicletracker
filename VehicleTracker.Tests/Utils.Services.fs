module VehicleTracker.Tests.UtilsServices

open Microsoft.Extensions.Configuration
open Microsoft.AspNetCore.TestHost
open VehicleTracker.StorageService.FSharp.DbInitializerHelper
open System.Data.SqlClient
open System

let private updateDbName update conn =
    let builder = new SqlConnectionStringBuilder(conn)
    builder.InitialCatalog <- update builder.InitialCatalog
    builder.ConnectionString

let private dbNameFunc testName name = 
    let result = sprintf "%s_%s_%s" name testName (DateTime.Now.ToString("yyyyMMdd_HHmmssF"))
    if result.Length > 120 
        then result.Substring(0, 120)
        else result

let private readConnectionString connName (config : IConfiguration) = config.GetConnectionString connName

let private updateConnectionString testName = dbNameFunc testName |> updateDbName

let substituteConnectionString connName testName config = 
    let conn = (readConnectionString connName >> updateConnectionString testName) config
    let builder = new ConfigurationBuilder()
    builder.AddInMemoryCollection(config.AsEnumerable()) |> ignore
    builder.AddInMemoryCollection(dict [(connName, conn)]) |> ignore
    builder.Build()

let withClient f (server : TestServer) = 
    async {
        use client = server.CreateClient()
        return! f client
    }

let dbContext init deinit f (server : TestServer) = 
    async {
        seedDatabase "Test data init" server.Host init |> ignore
        try 
            return! f server
        finally
            seedDatabase "Removing test DB" server.Host deinit |> ignore
    }

let readConfig (name : string) = 
    let builder = new ConfigurationBuilder() 
    builder.AddJsonFile name |> ignore
    builder.AddEnvironmentVariables() |> ignore
    builder.Build()

let withServer f webHostBuilder =
    async {
        use server = new TestServer(webHostBuilder)
        return! f server
    }

let private combine f g next x =
    f (g next) x

let (>=>) = combine
