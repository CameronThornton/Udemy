module MyOption

open System

let map f a' =
    match a' with
    | Some x -> f x |> Some
    | _ -> None

let ret x = Some x

let apply fOpt aOpt =
    match fOpt, aOpt with
    | Some f, Some x -> Some(f x)
    | _ -> None
