namespace VehicleTracker.StorageService.FSharp.DataAccess

open Microsoft.EntityFrameworkCore
open VehicleTracker.StorageService.FSharp.Models
open System.Linq
open Giraffe.Common
open System.Collections.Generic

module Common = 
    let wrap extr f filter q = 
        match extr filter with
        | None -> q
        | Some v -> f v q
    
    let (>+>) f g filter = f filter >> g filter

    let private toListAsync (queryable : IQueryable<'a>) = 
        task {
            let! result = queryable.ToListAsync()
            return result :> IEnumerable<'a>
        }

    let private firstAsync (queryable : IQueryable<'a>) = queryable.FirstAsync()

    let get sourceF filterF id = sourceF >> filterF id >> firstAsync
    
    let list sourceF (filterF : ('Filter -> IQueryable<'Item> -> IQueryable<'Item>) option) filter =
        match filterF with
        | Some F -> sourceF >> F filter >> toListAsync
        | None -> sourceF >> toListAsync

    let add<'a when 'a : not struct> (sourceF : VehiclesContext -> DbSet<'a>) (item: 'a) (context : VehiclesContext) =
        let source = sourceF context
        source.Add(item) |> ignore
        context.SaveChangesAsync()        

    let update (sourceF : VehiclesContext -> DbSet<'Item>) filterF updF item id context = 
        task {
            let! upd = get sourceF filterF id context
            updF upd item
            return! context.SaveChangesAsync()
        }
        
    let delete (sourceF : VehiclesContext -> DbSet<'Item>) filterF id context = 
        task {
            let source = sourceF context
            let! item = get sourceF filterF id context
            source.Remove(item) |> ignore
            return! context.SaveChangesAsync()
        }
