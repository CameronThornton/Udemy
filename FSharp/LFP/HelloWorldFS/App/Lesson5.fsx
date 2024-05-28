open System

// let x = 3

// let text =
//     match x with
//     | 0 -> "zero"
//     | 1 -> "one"
//     | 2 -> "two"
//     | 3 -> "three"
//     | _ -> "smaller than 0 greater than 3"

// let (|Fizz|Buzz|FizzBuzz|Other|) =
//     function
//     | 0, 0 -> FizzBuzz
//     | 0, _ -> Fizz
//     | _, 0 -> Buzz
//     | _ -> Other

// let (|Fizz|_|) =
//     function
//     | 0, _ -> Some Fizz
//     | _ -> None

// let (|Buzz|_|) =
//     function
//     | _, 0 -> Some Buzz
//     | _ -> None

// for c in [ 1..15 ] do
//     match c % 3, c % 5 with
//     | Fizz & Buzz -> printfn "fizzbuzz"
//     | Fizz -> printfn "fizz"
//     | Buzz -> printfn "buzz"
//     | _ when c = 10 -> printfn "%i" 10
//     | _ -> printfn "%i" c

type Line = { X1: int; X2: int; Y1: int; Y2: int }

let line = { X1 = 1; X2 = 2; Y1 = 3; Y2 = 4 }

let { X1 = xOne } = line

printfn "%A" xOne

let { Y1 = y; X2 = x2 } = line

printfn "%A" y
printfn "%A" x2

type Circle = Circle of r: int
let circle = Circle 5

let (Circle r) = circle
printfn "%A" r

type Shape =
    | Circle of Circle
    | Line of Line

let shape = Line line

let shapeResult =
    match shape with
    | Circle r -> $"{r}"
    | Line { X1 = x } -> $"this is a line with X1 = {x}"

let printCoordinates { X1 = x1; X2 = x2; Y1 = y1; Y2 = y2 } = $"{x1}, {x2}, {y1}, {y2}"

printCoordinates line
