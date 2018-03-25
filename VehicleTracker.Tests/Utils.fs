module VehicleTracker.Tests.Utils
open Microsoft.Extensions.Configuration
open VehicleTracker.StorageService.FSharp.App
open Microsoft.AspNetCore.TestHost
open VehicleTracker.StorageService.FSharp.DbInitializerHelper
open VehicleTracker.StorageService.FSharp.Models
open System.Data.SqlClient
open System
open VehicleTracker.StorageService.FSharp.DbInitializer

let updateDbName update conn =
    let builder = new SqlConnectionStringBuilder(conn)
    builder.InitialCatalog <- update builder.InitialCatalog
    builder.ConnectionString

let dbNameFunc testName name = 
    let result = sprintf "%s_%s_%s" name testName (DateTime.Now.ToString("yyyyMMdd_HHmmssF"))
    if result.Length > 120 
        then result.Substring(0, 120)
        else result

let readConnectionString (config : IConfiguration) = config.GetConnectionString("DefaultConnection")

let updateConnectionString testName = dbNameFunc testName |> updateDbName

let substituteConnectionString testName config = 
    let conn = (readConnectionString >> updateConnectionString testName) config
    let builder = new ConfigurationBuilder()
    builder.AddInMemoryCollection(config.AsEnumerable()) |> ignore
    builder.AddInMemoryCollection(dict [("ConnectionStrings:DefaultConnection", conn)]) |> ignore
    builder.Build()

let readConfig = 
    let builder = new ConfigurationBuilder() 
    builder.AddJsonFile "appSettings.StorageService.json" |> ignore
    builder.AddEnvironmentVariables() |> ignore
    builder.Build()

let ensureDbDeleted (context : VehiclesContext) = context.Database.EnsureDeleted()

let withServer f testName = 
    async {
        let config = readConfig |> substituteConnectionString testName |> configureWebHost
        use server = new TestServer(config)
        return! f server
    }

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

let setupTest name init deinit f = 
    f |> withClient |> dbContext init deinit |> withServer <| name

let initializer dataInit = wrap ensureDbExists >=> checkEmpty >=!> dataInit >=+> saveChanges
let emptyDb ctx = initializer id ctx

let dropDb context = wrap ensureDbDeleted context
let saveDb = (fun context -> Skip "Database should be saved")
let defaultDeinitializer = dropDb

let runTheory<'T> (paramsSeq : obj array seq) f =
    async {
        for v in paramsSeq do
            printfn "%A" v
            match v.[0] with
            | :? 'T as p -> do! f(p)
            | _ -> ()
    }    
   