namespace VehicleTracker.Tests

open Xunit
open VehicleTracker.Tests.Utils
open System.Net
open VehicleTracker.StorageService.FSharp.DbInitializer
open VehicleTracker.StorageService.FSharp.DbInitializerDataHelper
open Newtonsoft.Json
open VehicleTracker.StorageService.FSharp.Models

module SampleTest =
    let defaultInitializer dataInit = Some (wrap ensureDbExists >=> checkEmpty >=+> dataInit >=+> saveChanges)

    let emptyInitializer = defaultInitializer id

    [<Fact>]
    let ``Connection test``() = 
        withServer emptyInitializer
            (fun client -> async {
                let! result = Async.AwaitTask (client.GetAsync("/vehicles/"))
                Assert.Equal(HttpStatusCode.OK, result.StatusCode)
            })

    let createVehicles : ContextTransformer = 
        addCustomer "Name 1" "Address 1"
            >|> addVehicle "1" "222222"
            >|> addVehicle "2" "222222"
            |> returnContext
            >> addCustomer "Name 2" "Address 2"
            >|> addVehicle "3" "333333"
            >|> addVehicle "4" "444444"
            |> returnContext
        
    [<Fact>]
    let ``Get test``() = 
        withServer (defaultInitializer createVehicles)
            (fun client -> async {
                let! result = Async.AwaitTask (client.GetAsync("/vehicles/"))
                Assert.Equal(HttpStatusCode.OK, result.StatusCode)
                let! content = Async.AwaitTask (result.Content.ReadAsStringAsync())
                let result = JsonConvert.DeserializeObject<seq<Vehicle>>(content)
                Assert.Equal(4, Seq.length result)
            })

    [<EntryPoint>]
    let main _ =
        Async.RunSynchronously (``Connection test``())
        Async.RunSynchronously (``Get test``())
        0