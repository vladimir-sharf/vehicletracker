module VehicleTracker.Tests.UtilsTesting

let runTheory<'T> (paramsSeq : obj array seq) f =
    paramsSeq 
    |> Seq.fold (fun (r : Async<unit> list) v -> 
        match v.[0] with
        | :? 'T as p -> 
            let a = async {
                try 
                    do! f p
                    printfn "SUCCESS\nCASE: %A" v.[0]
                with e -> 
                    printfn "FAILED\nCASE:\n%A\nERROR:\n%s" v.[0] e.Message
            } 
            a :: r
        | _ -> r) []
    |> Async.Parallel
    |> Async.Ignore
   