namespace VehicleTracker.Tests

open Xunit
open Microsoft.AspNetCore.TestHost
open VehicleTracker.StorageService.FSharp.App
open System.Net
open Microsoft.Extensions.Configuration

module SampleTest =
    let readConfig() = 
        let builder = new ConfigurationBuilder() 
        builder.AddJsonFile "appSettings.StorageService.json" |> ignore
        builder.AddEnvironmentVariables() |> ignore
        builder.AddInMemoryCollection(
            [ "ConnectionStrings:DefaultConnection", "Server=localhost;Database=VehicleDb_Test;Integrated Security=True" ] 
            |> Map.ofList) |> ignore
        builder.Build()

    let configureWebHost() = 
        configureWebHost (readConfig())

    let createServer() = new TestServer(configureWebHost())

    let createClient (server : TestServer) = server.CreateClient()

    [<Fact>]
    let Test1() = 
        async {
            use server = createServer()
            use client = createClient server
            let! result = Async.AwaitTask (client.GetAsync("/vehicles/"))
            let! content = Async.AwaitTask (result.Content.ReadAsStringAsync())
            printfn "%s" content
            Assert.Equal(HttpStatusCode.OK, result.StatusCode)
        }

    [<EntryPoint>]
    let main _ = 
        Async.RunSynchronously (Test1()) |> ignore
        0