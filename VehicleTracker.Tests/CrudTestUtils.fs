module VehicleTracker.Tests.CrudTestUtils

open System.Net
open Newtonsoft.Json
open Shouldly
open System.Net.Http
open System.Text

type ExpectedList<'a> = {
    Status : HttpStatusCode
    Content : 'a list
}

type ExpectedItem<'a> = {
    Status : HttpStatusCode
    Content : 'a
}

type ExpectedString = {
    Status : HttpStatusCode
    Content : string
}

type Expected<'a> = Item of 'a ExpectedItem | List of 'a ExpectedList | String of ExpectedString | None of HttpStatusCode | Status of HttpStatusCode


let private readingContent f (response : HttpResponseMessage) =
    async {
        let! content = Async.AwaitTask (response.Content.ReadAsStringAsync())
        return f response.StatusCode content
    }
    
let checkItem<'a> = 
    readingContent <| fun status content ->
        Item { 
            Content = JsonConvert.DeserializeObject<'a>(content)
            Status = status
        }

let checkList<'a> = 
    readingContent <| fun status content ->
    List { 
        Content = JsonConvert.DeserializeObject<seq<'a>>(content) |> Seq.toList
        Status = status
    }
    
let checkString message =
    (readingContent <| fun status content ->
        String {
            Status = status
            Content = content
        }) message

let check transform (url : string) (expected : Expected<'a>) (result : HttpResponseMessage) = 
    async {
        match expected with
        | None _ ->
            let converted = Status (result.StatusCode)
            converted.ShouldBe(expected, url)
            let! content = Async.AwaitTask (result.Content.ReadAsStringAsync())
            content.ShouldBeNullOrEmpty(url)
        | Status _ -> 
            let converted = Status (result.StatusCode)
            converted.ShouldBe(expected, url)
        | Item _ ->
            let! converted = checkItem result
            converted.ShouldBe(expected, url)
        | List _ ->
            let! converted = checkList result
            let transformed = 
                match converted with
                | List l -> List (transform l)
                | _ -> converted
            transformed.ShouldBe(expected, url)
        | String _ ->
            let! converted = checkString result
            converted.ShouldBe(expected, url)            
    }

let listTest (transform : ExpectedList<'a> -> ExpectedList<'a>) (url : string) (expected : Expected<'a>) (client : HttpClient) =
    async {
        let! result = Async.AwaitTask (client.GetAsync(url))
        do! check transform url expected result
    }

let getTest (url : string) (expected : Expected<'a>) = 
    listTest id url expected

type GetTestParams<'a> = {
    TestName: string
    Url: string
    Expected: Expected<'a>
}

let addTest (url : string) item (expected : Expected<'a>) (client : HttpClient) = 
    async {
        let content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json")
        let! resp = Async.AwaitTask (client.PostAsync (url, content))
        do! check id url expected resp
    }

let deleteTest (url : string) (expected : Expected<'a>) (client : HttpClient) = 
    async {
        let! resp = Async.AwaitTask (client.DeleteAsync (url))
        do! check id url expected resp
    }

let putTest (url : string) item (expected : Expected<'a>) (client : HttpClient) = 
    async {
        let content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json")
        let! resp = Async.AwaitTask (client.PutAsync (url, content))
        do! check id url expected resp
    }

let (<+>) f g client =
    async {
        do! f client
        do! g client
    }
