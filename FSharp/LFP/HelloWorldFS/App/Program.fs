open System
open HelloWorldLibrary

type VowelCount =
    { A: int
      E: int
      I: int
      O: int
      U: int }

let rec countVowels acc index (str: string) =
    if index = str.Length then
        acc
    else
        let c = str.[index]

        let acc =
            match c with
            | 'a' -> { acc with A = acc.A + 1 }
            | 'e' -> { acc with E = acc.E + 1 }
            | 'i' -> { acc with I = acc.I + 1 }
            | 'o' -> { acc with O = acc.O + 1 }
            | 'u' -> { acc with U = acc.U + 1 }
            | _ -> acc

        countVowels acc (index + 1) str

let text = "FSharp simply rocks."

let accInitVal = { A = 0; E = 0; I = 0; O = 0; U = 0 }

printfn "%A" <| countVowels accInitVal 0 text
printfn "%A" Lesson2.Scratch.o
