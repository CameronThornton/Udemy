open System

let cons a b = List.Cons(a, b)

let rec reduce f x =
    fun (y) ->
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


printfn "%A" <| sum [ 1; 2; 3 ]
printfn "%A" <| mul [ 1; 2; 3; 4 ]

printfn "%A"
<| anyTrue [ true; false; true; true ]

printfn "%A" <| allTrue [ false ]

printfn "%A"
<| append [ true; true ] [ false; false ]

printfn "%A" <| doubleAll [ 1; 2; 3 ]
printfn "%A" <| doubleAll2 [ 1; 2; 3 ]
