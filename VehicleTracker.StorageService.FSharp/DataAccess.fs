namespace VehicleTracker.StorageService.FSharp.DataAccess

open Microsoft.EntityFrameworkCore
open VehicleTracker.StorageService.FSharp.Models
open System.Linq
open Giraffe.Common
open System.Collections.Generic
open System

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

    let private firstAsync (queryable : IQueryable<'a>) = 
        task {
            try 
                let! result = queryable.FirstAsync()
                return Some result
            with :? InvalidOperationException -> return None
        }

    let get sourceF filterF id = sourceF >> filterF id >> firstAsync
    
    let list sourceF (filterF : ('Filter -> IQueryable<'Item> -> IQueryable<'Item>) option) filter =
        match filterF with
        | Some F -> sourceF >> F filter >> toListAsync
        | None -> sourceF >> toListAsync

    type AddResult = Success of int | Duplicate

    let add<'a when 'a : not struct> (sourceF : VehiclesContext -> DbSet<'a>) (item: 'a) (context : VehiclesContext) =
        let source = sourceF context
        source.Add(item) |> ignore
        task {
            try
                let! result = context.SaveChangesAsync()
                return Success result
            with :? DbUpdateException -> return Duplicate
        }

    let update (sourceF : VehiclesContext -> DbSet<'Item>) filterF updF item id context = 
        task {
            let! upd = get sourceF filterF id context
            match upd with
            | Some upd ->
                updF upd item
                let! result = context.SaveChangesAsync()
                return Some result
            | None -> return None
        }
        
    let delete (sourceF : VehiclesContext -> DbSet<'Item>) filterF id context = 
        task {
            let source = sourceF context
            let! item = get sourceF filterF id context
            match item with
            | Some item ->
                source.Remove(item) |> ignore
                let! result = context.SaveChangesAsync()
                return Some result
            | None -> return None
        }
