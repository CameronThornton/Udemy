module Lesson2.Scratch

let add x y = x + y

let inc = add 1

let double x = x * 2

let incDouble = (List.map inc) >> (List.map double)

let l = [ 1; 2; 3 ]
let n = l |> List.map inc |> List.map double
let o = l |> incDouble
