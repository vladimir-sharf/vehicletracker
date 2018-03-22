namespace VehicleTracker.StorageService.FSharp

open Giraffe

module Routing =
    let routefs handler = routef "/%s" handler
    let routefo handler = routef "/%O" handler
