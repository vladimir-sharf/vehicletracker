namespace VehicleTracker.StorageService.FSharp.Models

open System
open System.ComponentModel.DataAnnotations 
open Microsoft.EntityFrameworkCore

type CustomerId = Guid
type CustomerName = string
type CustomerAddress = string
type VIN = string
type RegNr = string

type Customer() = 
    let mutable _id: Guid = Guid.Empty
    let mutable _name: string = ""
    let mutable _address: string = ""

    [<Key>]
    member public this.Id with get() = _id and set v = _id <- v
    member public this.Name with get() = _name and set v = _name <- v
    member public this.Address with get() = _address and set v = _address <- v

type Vehicle() = 
    let mutable _id: string = ""
    let mutable _regNr: string = ""
    let mutable _customerId: Guid = Guid.Empty

    [<Key>]
    member public this.Id with get() = _id and set v = _id <- v
    member public this.RegNr with get() = _regNr and set v = _regNr <- v
    member public this.CustomerId with get() = _customerId and set v = _customerId <- v

[<CLIMutable>]
type VehicleFilter = 
    {
        Id: VIN option
        CustomerId: CustomerId option        
    }

[<CLIMutable>]
type CustomerFilter = 
    {
        Id: CustomerId option
    }
    
type VehiclesContext(options : DbContextOptions<VehiclesContext>) = 
    inherit DbContext(options)
    
    [<DefaultValue>]
    val mutable _customers: DbSet<Customer>
    [<DefaultValue>]
    val mutable _vehicles: DbSet<Vehicle>

    member public this.Customers with get() = this._customers
                                 and set v = this._customers <- v
    member public this.Vehicles with get() = this._vehicles
                                 and set v = this._vehicles <- v    