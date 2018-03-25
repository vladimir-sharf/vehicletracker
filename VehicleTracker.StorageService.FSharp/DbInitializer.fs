module VehicleTracker.StorageService.FSharp.DbInitializer

open VehicleTracker.StorageService.FSharp.Models
open System.Linq

type Result<'a> = Success of 'a | Skip of string | Error of string | Fatal of string

let saveChanges (context : VehiclesContext) = context.SaveChanges()

let ensureDbExists (context : VehiclesContext) = 
    context.Database.EnsureCreated() |> ignore
    context

let checkEmpty (context : VehiclesContext) = 
    try 
        match context.Vehicles.Any() with
        | true -> Skip "DB is not empty"
        | false -> Success context
    with
        | e -> Error e.Message

let private bind f g context =
    try
        let result = f context
        match result with
        | Success c -> g c
        | Error e -> Error e
        | Skip m -> Skip m
        | Fatal m -> Fatal m
    with
        | e -> Error e.Message

let (>=>) f g = bind f g

let wrap f a = 
    try
        Success (f a)
    with
        | e -> Error e.Message

let (>=+>) f g = bind f (wrap g)

let critical f a = 
    try
        Success (f a)
    with
        | e -> Fatal e.Message

let (>=!>) f g = bind f (critical g)

