namespace VehicleTracker.StorageService.FSharp
open VehicleTracker.StorageService.FSharp.Models

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

    let handleGet getFunc id (next : HttpFunc) (ctx : HttpContext) =
        let dataContext = ctx.GetService<VehiclesContext>()
        task {
            let! result = getFunc id dataContext
            return! json result next ctx
        }

    let handlePost addFunc (next : HttpFunc) (ctx : HttpContext) =
        let dataContext = ctx.GetService<VehiclesContext>()
        task {
            let! result = ctx.BindJsonAsync<'T>()
            let! status = addFunc result dataContext
            status |> ignore
            return! Successful.OK "Success" next ctx
        }

    let handlePut updateFunc id (next : HttpFunc) (ctx : HttpContext) =
        let dataContext = ctx.GetService<VehiclesContext>()
        task {
            let! item = ctx.BindJsonAsync<'T>()
            let! result = updateFunc item id dataContext 
            result |> ignore
            return! Successful.OK "Success" next ctx
        }

    let handleDelete deleteFunc id (next : HttpFunc) (ctx : HttpContext) =
        let dataContext = ctx.GetService<VehiclesContext>()
        task {
            let! result = deleteFunc id dataContext 
            result |> ignore
            return! Successful.OK "Success" next ctx
        }
     
            
        