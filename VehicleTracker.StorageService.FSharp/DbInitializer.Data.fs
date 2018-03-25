module VehicleTracker.StorageService.FSharp.DbInitializerData
open VehicleTracker.StorageService.FSharp.DbInitializerDataHelper

let private createKalles : ContextTransformer = 
    addCustomer "Kalles Grustransporter AB" "Cementvägen 8, 111 11 Södertälje"
    >|> (addVehicle "YS2R4X20005399401" "ABC123"
        <+> addVehicle "VLUR4X20009093588" "DEF456"
        <+> addVehicle "VLUR4X20009048066" "GHI789")

let private createJohans : ContextTransformer = 
    addCustomer "Johans Bulk AB" "Balkvägen 12, 222 22 Stockholm"
    >|> (addVehicle "YS2R4X20005388011" "JKL012"
        <+> addVehicle "YS2R4X20005387949" "MNO345")

let private createHaralds : ContextTransformer = 
    addCustomer "Haralds Värdetransporter AB" "BalkvägenBudgetvägen 1, 333 33 Uppsala"
    >|> (addVehicle "YS2R4X20005387765" "PQR678"
        <+> addVehicle "YS2R4X20005387055" "STU901")

let create : ContextTransformer = createKalles >> createJohans >> createHaralds

