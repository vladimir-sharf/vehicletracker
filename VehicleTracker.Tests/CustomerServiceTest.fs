module VehicleTracker.Tests.CustomerServiceTest

open Xunit
open VehicleTracker.Tests.Utils
open VehicleTracker.Tests.DataHelper
open System.Net
open System
open VehicleTracker.Tests.CrudTestUtils

(* ------------------------------------------------
    Test Data
--------------------------------------------------- *)
let private customers =
    [ addCustomer "Name 1" "Address 1"
      addCustomer "Name 2" "Address 2"
      addCustomer "Name 3" "Address 3"
    ]

(* ------------------------------------------------
    Utils
--------------------------------------------------- *)
let orderById = fun (v : Customer) -> v.Id :> IComparable

let expectedCustomers filter = 
    let result = customers |> Seq.filter(filter) |> Seq.sortBy orderById |> Seq.toList
    List { Content = result; Status = HttpStatusCode.OK } 

let private customerListTransform x : ExpectedList<Customer> = { x with Content = List.sortBy orderById x.Content }

let findCustomer id = customers |> Seq.filter(fun c -> c.Id = id) |> Seq.exactlyOne

let expectedCustomer c = Item { Content = c; Status = HttpStatusCode.OK }

let notFound : Expected<Customer> = String { Content = "Not Found"; Status = HttpStatusCode.NotFound }

let getUrlId id = sprintf "/customers/%A" id

let getUrlCustomer c = getUrlId c.Id

let getUrl n = getUrlId customers.[n].Id

let nonExUrl = getUrlId <| Guid.NewGuid()

(* ------------------------------------------------
    Tests
--------------------------------------------------- *)
// [<Fact>]
let ``Customer service connection test``() = 
    setupTest "CustomersConnectionTest" emptyDb defaultDeinitializer
    <| getTest "/customers/" (Status HttpStatusCode.OK)

let customerListParams : obj array seq = 
    Seq.ofList [ 
        [| { 
            TestName = "NoFilters"
            Url = "/customers/"
            Expected = expectedCustomers <| fun v -> true
        } |];
        [| { 
            TestName = "Id"
            Url = sprintf "/customers/?id=%A" customers.[0].Id
            Expected = expectedCustomers <| fun (v : Customer) -> v.Id = customers.[0].Id
        } |];
        [| {
            TestName = "Empty"
            Url = sprintf "/customers/?id=%A" (Guid.NewGuid())
            Expected = expectedCustomers <| fun v -> false
        } |]            
    ]

// [<Theory; MemberData("customerListParams")>]
let ``List customers test``({ TestName = testName; Url = url; Expected = expected }) =
    setupTest (sprintf "CustomerListTest_%s" testName) (initializer (createCustomers customers)) defaultDeinitializer
    <| listTest customerListTransform url expected

let customerGetParams : obj array seq = 
    Seq.ofList [ 
        [| { TestName = "1"; Url = getUrl 0; Expected = expectedCustomer customers.[0] } |];
        [| { TestName = "NonEx"; Url = nonExUrl; Expected = notFound } |];
        [| { TestName = "NotGuid"; Url = "/customers/1"; Expected = notFound } |];
    ]

// [<Theory; MemberData("customerGetParams")>]
let ``Get customer test`` ({ TestName = testName; Url = url; Expected = expected } : GetTestParams<Customer>) =
    setupTest (sprintf "CustomerGetTest_%s" testName) (initializer (createCustomers customers)) defaultDeinitializer
    <| getTest url expected

[<Fact>]
let ``Customer readonly tests``() =
    setupTest "CustomerReadTests" (initializer (createCustomers customers)) defaultDeinitializer
    <| fun (client) -> async {
        do! getTest "/customers/" (Status HttpStatusCode.OK) client
        do! runTheory customerListParams (fun ({ Url = url; Expected = expected }) ->
            listTest customerListTransform url expected client)
        do! runTheory customerGetParams (fun ({ Url = url; Expected = expected } : GetTestParams<Customer>) ->
            getTest url expected client)
    }
    
[<Fact>]
let ``Add customer test``() =
    setupTest "CustomerAddTest" emptyDb defaultDeinitializer
    <| (addTest "/customers/" customers.[0] (Status HttpStatusCode.OK) 
        <+> getTest (getUrl 0) (expectedCustomer customers.[0])
    )

[<Fact>]
let ``Add duplicate customer test``() =
    setupTest "CustomerAddDupTest" (initializer (createCustomers customers)) defaultDeinitializer
    <| (addTest "/customers/" customers.[0] (String { Status = HttpStatusCode.Conflict; Content = "Entity with specified id already exists" })
        <+> listTest customerListTransform "/customers/" (expectedCustomers <| fun v -> true) 
    )

[<Fact>]
let ``Delete customer test``() =
    setupTest "CustomerDeleteTest" (initializer (createCustomers customers)) defaultDeinitializer
    <| (deleteTest (getUrl 0) (Status HttpStatusCode.OK)
        <+> getTest (getUrl 0) notFound
    )

[<Fact>]
let ``Delete nonexistent customer test``() =
    setupTest "CustomerDeleteNonExTest" (initializer (createCustomers customers)) defaultDeinitializer
    <| (deleteTest nonExUrl notFound
        <+> listTest customerListTransform "/customers/" (expectedCustomers <| fun v -> true) 
    )

[<Fact>]
let ``Update customer test``() =
    let customer = 
        { customers.[0] with
            Name = "Updated name"
            Address = "Updated customer"
        }
    setupTest "CustomerUpdateTest" (initializer (createCustomers customers)) defaultDeinitializer
    <| (putTest (getUrl 0) customer (Status HttpStatusCode.OK)
        <+> getTest (getUrl 0) (Item { Status = HttpStatusCode.OK; Content = customer })
    )

[<Fact>]
let ``Update nonexistent customer test``() =
    let customer = {
        Id = Guid.NewGuid()
        Name = "Updated name"
        Address = "Updated customer"
    }
    setupTest "CustomerUpdateNonExTest" (initializer (createCustomers customers)) defaultDeinitializer
    <| (putTest (getUrlCustomer customer) customer notFound
        <+> listTest customerListTransform "/customers/" (expectedCustomers <| fun v -> true) 
    )

[<Fact>]
let ``Update customer bad request test``() =
    let customer = {
        Id = Guid.NewGuid()
        Name = "Updated name"
        Address = "Updated customer"
    }
    setupTest "CustomerUpdateBadReqTest" (initializer (createCustomers customers)) defaultDeinitializer
    <| (putTest nonExUrl customer (String { Status = HttpStatusCode.BadRequest; Content = "Ids in url and in payload are different" })
        <+> listTest customerListTransform "/customers/" (expectedCustomers <| fun v -> true) 
    )