module VehicleTracker.Tests.VehicleServiceTest

open Xunit
open VehicleTracker.Tests.UtilsTesting
open VehicleTracker.Tests.UtilsStorageCSharp
open VehicleTracker.Tests.DataHelper
open System.Net
open System
open VehicleTracker.Tests.CrudTestUtils

(* ------------------------------------------------
    Test Data
--------------------------------------------------- *)
let private customers = List.map (fun i -> Guid.NewGuid()) [1..2]

let private vehicles = 
    [ addVehicle "1" "111111" customers.[0]
      addVehicle "2" "222222" customers.[0]
      addVehicle "3" "333333" customers.[1]
      addVehicle "4" "444444" customers.[1]
    ]

(* ------------------------------------------------
    Utils
--------------------------------------------------- *)
let orderById = fun (v : Vehicle) -> v.Id :> IComparable

let expectedVehicles filter = 
    let result = vehicles |> Seq.filter(filter) |> Seq.sortBy orderById |> Seq.toList
    List { Content = result; Status = HttpStatusCode.OK } 

let private vehicleListTransform x : ExpectedList<Vehicle> = { x with Content = List.sortBy orderById x.Content }

let findVehicle id = vehicles |> Seq.filter(fun c -> c.Id = id) |> Seq.exactlyOne

let expectedVehicle = findVehicle >> (fun result -> Item { Content = result; Status = HttpStatusCode.OK })

let notFound : Expected<Vehicle> = String { Content = "Not Found"; Status = HttpStatusCode.NotFound }

(* ------------------------------------------------
    Tests
--------------------------------------------------- *)
let vehicleListParams : obj array seq = 
    Seq.ofList [ 
        [| { 
            TestName = "NoFilters"
            Url = "/vehicles/"
            Expected = expectedVehicles <| fun v -> true
        } |];
        [| { 
            TestName = "CustomerId"
            Url = (sprintf "/vehicles/?customerId=%A" customers.[0])
            Expected = expectedVehicles <| fun v -> v.CustomerId = customers.[0]
        } |];
        [| { 
            TestName = "Id"
            Url = "/vehicles/?id=1"
            Expected = expectedVehicles <| fun (v : Vehicle) -> v.Id = "1"
        } |];
        [| {
            TestName = "CustomerId_Id"
            Url = (sprintf "/vehicles/?customerId=%A&id=1" customers.[0])
            Expected = expectedVehicles <| fun v -> v.CustomerId = customers.[0] && v.Id = "1"
        } |];
        [| {
            TestName = "Empty"
            Url = (sprintf "/vehicles/?customerId=%A" (Guid.NewGuid()))
            Expected = expectedVehicles <| fun v -> false
        } |]            
    ]

let vehicleGetParams : obj array seq = 
    Seq.ofList [ 
        [| { TestName = "1"; Url = "/vehicles/1"; Expected = expectedVehicle "1" } |];
        [| { TestName = "4"; Url = "/vehicles/4"; Expected = expectedVehicle "4" } |];
        [| { TestName = "NonEx"; Url = "/vehicles/NonEx"; Expected = notFound } |];
    ]

[<Fact>]
let ``Vehicle readonly tests``() =
    setupTest "VehicleReadTests" (initializer (createVehicles vehicles)) defaultDeinitializer
    <| fun (client) -> async {
        do! getTest "/vehicles/" (Status HttpStatusCode.OK) client
        do! runTheory vehicleListParams (fun ({ Url = url; Expected = expected }) ->
            listTest vehicleListTransform url expected client)
        do! runTheory vehicleGetParams (fun ({ Url = url; Expected = expected } : GetTestParams<Vehicle>) ->
            getTest url expected client)
    }
    
[<Fact>]
let ``Add vehicle test``() =
    setupTest "VehicleAddTest" emptyDb defaultDeinitializer
    <| (addTest "/vehicles/" (findVehicle "1") (Status HttpStatusCode.OK) 
        <+> getTest "/vehicles/1" (expectedVehicle "1")
    )

[<Fact>]
let ``Add duplicate vehicle test``() =
    setupTest "VehicleAddDupTest" (initializer (createVehicles vehicles)) defaultDeinitializer
    <| (addTest "/vehicles/" (findVehicle "1") (String { Status = HttpStatusCode.Conflict; Content = "Entity with specified id already exists" })
        <+> listTest vehicleListTransform "/vehicles/" (expectedVehicles <| fun v -> true) 
    )

[<Fact>]
let ``Delete vehicle test``() =
    setupTest "VehicleDeleteTest" (initializer (createVehicles vehicles)) defaultDeinitializer
    <| (deleteTest "/vehicles/1" (Status HttpStatusCode.OK)
        <+> getTest "/vehicles/1" notFound
    )

[<Fact>]
let ``Delete nonexistent vehicle test``() =
    setupTest "VehicleDeleteNonExTest" (initializer (createVehicles vehicles)) defaultDeinitializer
    <| (deleteTest "/vehicles/nonex" notFound
        <+> listTest vehicleListTransform "/vehicles/" (expectedVehicles <| fun v -> true) 
    )

[<Fact>]
let ``Update vehicle test``() =
    let vehicle = {
        Id = "1"
        CustomerId = Guid.NewGuid()
        RegNr = "Updated"
    }
    setupTest "VehicleUpdateTest" (initializer (createVehicles vehicles)) defaultDeinitializer
    <| (putTest "/vehicles/1" vehicle (Status HttpStatusCode.OK)
        <+> getTest "/vehicles/1" (Item { Status = HttpStatusCode.OK; Content = vehicle })
    )

[<Fact>]
let ``Update nonexistent vehicle test``() =
    let vehicle = {
        Id = "nonex"
        CustomerId = Guid.NewGuid()
        RegNr = "Updated"
    }
    setupTest "VehicleUpdateNonExTest" (initializer (createVehicles vehicles)) defaultDeinitializer
    <| (putTest "/vehicles/nonex" vehicle notFound
        <+> listTest vehicleListTransform "/vehicles/" (expectedVehicles <| fun v -> true) 
    )

[<Fact>]
let ``Update vehicle bad request test``() =
    let vehicle = {
        Id = "2"
        CustomerId = Guid.NewGuid()
        RegNr = "Updated"
    }
    setupTest "VehicleUpdateBadReqTest" (initializer (createVehicles vehicles)) defaultDeinitializer
    <| (putTest "/vehicles/1" vehicle (String { Status = HttpStatusCode.BadRequest; Content = "Ids in url and in payload are different" })
        <+> listTest vehicleListTransform "/vehicles/" (expectedVehicles <| fun v -> true) 
    )