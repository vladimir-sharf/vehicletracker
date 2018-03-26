module VehicleTracker.Tests.App

open VehicleTracker.Tests.CustomerServiceTest
open VehicleTracker.Tests.VehicleServiceTest

[<EntryPoint>]
let main _ =
    // Async.RunSynchronously (``Customer readonly tests``())
    // Async.RunSynchronously (``Add customer test``())        
    Async.RunSynchronously (``Add duplicate customer test``())        
    // Async.RunSynchronously (``Delete customer test``())        
    // Async.RunSynchronously (``Delete nonexistent customer test``())        
    // Async.RunSynchronously (``Update customer test``())        
    // Async.RunSynchronously (``Update nonexistent customer test``())        
    // Async.RunSynchronously (``Update customer bad request test``())        

    // Async.RunSynchronously (``Vehicle readonly tests``())
    // Async.RunSynchronously (``Add vehicle test``())        
    // Async.RunSynchronously (``Add duplicate vehicle test``())        
    // Async.RunSynchronously (``Delete vehicle test``())        
    // Async.RunSynchronously (``Delete nonexistent vehicle test``())        
    // Async.RunSynchronously (``Update vehicle test``())        
    // Async.RunSynchronously (``Update nonexistent vehicle test``())        
    // Async.RunSynchronously (``Update vehicle bad request test``())        
    0