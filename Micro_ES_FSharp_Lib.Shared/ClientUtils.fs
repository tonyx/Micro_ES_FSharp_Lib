namespace Tonyx.EventSourcing.Sample
open FSharpPlus
module ClientUtils =
    let catchErrors f l =
        let (okList, errors) =
            l
            |> List.map f
            |> Result.partition
        if (errors.Length > 0) then
            Result.Error (errors.Head)
        else
            okList |> Result.Ok
    let boolToResult message x =
        match x with
        | true -> x |> Ok
        | false -> Error message
