module VehicleTracker.Tests.UtilsStorageFSharp

open VehicleTracker.StorageService.FSharp.App
open VehicleTracker.Tests.UtilsServices

let webHostBuilder testName nxt = 
    let builder = readConfig "appSettings.StorageService.FSharp.json" 
                    |> substituteConnectionString "DefaultConnection" testName 
                    |> configureWebHost
    fun () -> nxt builder
        
let setupTest testName init deinit f = 
    webHostBuilder testName >=> withServer >=> dbContext init deinit >=> withClient <| f <| ()

