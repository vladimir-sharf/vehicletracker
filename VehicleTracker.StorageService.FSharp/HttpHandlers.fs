namespace VehicleTracker.StorageService.FSharp
open VehicleTracker.StorageService.FSharp.Models
open DataAccess.Common
open System.Threading.Tasks

module HttpHandlers =

    open Microsoft.AspNetCore.Http
    open Giraffe

    let handleList listFunc (next : HttpFunc) (ctx : HttpContext) = 
        let dataContext = ctx.GetService<VehiclesContext>()
        task {
            let filter = ctx.BindQueryString<'TFilter>()
            let! result = listFunc filter dataContext
            return! json result next ctx
        }

    let finish = Some >> Task.FromResult

    let private handle404 result funcSuccess next context  =
        match result with
        | Some r -> funcSuccess r next context
        | None -> (setStatusCode 404 >=> text "Not Found") finish context

    let private handle404Success result = (fun r -> Successful.OK "Success") |> handle404 result

    let handleGet getFunc id (next : HttpFunc) (ctx : HttpContext) =
        let dataContext = ctx.GetService<VehiclesContext>()
        task {
            let! result = getFunc id dataContext
            return! handle404 result json next ctx
        }

    let handlePost (addFunc : 'T -> VehiclesContext -> AddResult Task) (next : HttpFunc) (ctx : HttpContext) =
        let dataContext = ctx.GetService<VehiclesContext>()
        task {
            let! result = ctx.BindJsonAsync<'T>()
            let! status = addFunc result dataContext
            match status with
            | Duplicate -> return! (setStatusCode 409 >=> text "Entity with specified id already exists") finish ctx
            | Success _ -> return! Successful.OK "Success" next ctx
        }

    let handlePut updateFunc extractFunc id (next : HttpFunc) (ctx : HttpContext) =
        task {
            let! item = ctx.BindJsonAsync<'T>()
            match extractFunc item = id with
            | true -> 
                let dataContext = ctx.GetService<VehiclesContext>()
                let! status = updateFunc item id dataContext 
                return! handle404Success status next ctx
            | false -> return! (setStatusCode 400 >=> text "Ids in url and in payload are different") finish ctx
        }

    let handleDelete deleteFunc id (next : HttpFunc) (ctx : HttpContext) =
        let dataContext = ctx.GetService<VehiclesContext>()
        task {
            let! status = deleteFunc id dataContext 
            return! handle404Success status next ctx
        }