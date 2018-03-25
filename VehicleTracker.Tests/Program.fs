module VehicleTracker.Tests.App

open VehicleTracker.Tests.CustomerServiceTest
open VehicleTracker.Tests.VehicleServiceTest
open VehicleTracker.Tests.Utils

[<EntryPoint>]
let main _ =
    // Async.RunSynchronously (``Customer service connection test``())        
    // Async.RunSynchronously(runTheory customerListParams ``List customers test``)
    // Async.RunSynchronously (runTheory customerGetParams ``Get customer test``)
    // Async.RunSynchronously (``Add customer test``())        
    // Async.RunSynchronously (``Add duplicate customer test``())        
    // Async.RunSynchronously (``Delete customer test``())        
    // Async.RunSynchronously (``Delete nonexistent customer test``())        
    // Async.RunSynchronously (``Update customer test``())        
    // Async.RunSynchronously (``Update nonexistent customer test``())        
    // Async.RunSynchronously (``Update customer bad request test``())        
    Async.RunSynchronously (``Customer readonly tests``())

    // Async.RunSynchronously (``Vehicle service connection test``())        
    // Async.RunSynchronously(runTheory vehicleListParams ``List vehicles test``)
    // Async.RunSynchronously (runTheory vehicleGetParams ``Get vehicle test``)
    // Async.RunSynchronously (``Add vehicle test``())        
    // Async.RunSynchronously (``Add duplicate vehicle test``())        
    // Async.RunSynchronously (``Delete vehicle test``())        
    // Async.RunSynchronously (``Delete nonexistent vehicle test``())        
    // Async.RunSynchronously (``Update vehicle test``())        
    // Async.RunSynchronously (``Update nonexistent vehicle test``())        
    // Async.RunSynchronously (``Update vehicle bad request test``())        
    Async.RunSynchronously (``Vehicle readonly tests``())
    0