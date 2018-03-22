namespace VehicleTracker.Tests

open Xunit
open Microsoft.AspNetCore.TestHost
open VehicleTracker.StorageService.FSharp.App
open System.Net

module SampleTest =
    [<Fact>]
    let Test1() = 
        let server = new TestServer(configureWebHost())
        let client = server.CreateClient()
        async {
            let! result = Async.AwaitTask (client.GetAsync("/vehicles/"))
            Assert.Equal(HttpStatusCode.OK, result.StatusCode)
        }