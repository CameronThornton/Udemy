open System
open System.Collections.Generic

let cons a b = List.Cons(a, b)

let rec reduce f x y =
    match y with
    | [] -> x
    | head :: tail -> f head <| (reduce f x) tail


let add x y = x + y
let multiply x y = x * y
let mul = reduce multiply 1
let anyTrue = reduce (||) false
let allTrue = reduce (&&) true
let append a b = reduce cons b a
let sum = reduce add 0
let doubleAndCons num list = cons (2 * num) list
let doubleAll = reduce doubleAndCons []

let fAndCons f = f >> cons
let double n = 2 * n

let doubleAndCons2 = fAndCons double
let map f = reduce (f >> cons) []
let doubleAll2 = map double

// printfn "%A" <| sum [ 1; 2; 3 ]
// printfn "%A" <| mul [ 1; 2; 3; 4 ]

// printfn "%A"
// <| anyTrue [ true; false; true; true ]

// printfn "%A" <| allTrue [ false ]

// printfn "%A"
// <| append [ true; true ] [ false; false ]

// printfn "%A" <| doubleAll [ 1; 2; 3 ]
// printfn "%A" <| doubleAll2 [ 1; 2; 3 ]


let add' x y = x + y
let inc = add 1
let tt = inc 2

let runDBQuery (connString: string) (query: string) = "dummy result"
let localDb = runDBQuery "myLocalConnection"
let aQuery = localDb "select * from table"

let myList = [ 1; 2; 3 ]

let another =
    myList
    |> List.filter (fun x -> x = 1)
    |> List.map (fun x -> x + 1)

let myArray = [| 1; 2; 3 |]
let z = myArray.[1]

let yetAnother =
    [ for x = 1 to 3 do
          yield! myArray ]
    |> Set.ofList

let someSeq = { 1..3 } |> Seq.toList

let newList = 1 :: myList
