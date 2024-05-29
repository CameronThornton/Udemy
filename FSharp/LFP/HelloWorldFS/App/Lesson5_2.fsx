open System

type Int32 with
    static member ParseAsOption(str: string) =
        match Int32.TryParse str with
        | false, _ -> None
        | true, x -> Some x

let (>>=) m f = Option.bind f m

let readInput s = s |> Int32.ParseAsOption

let retn x = Some x

let addition =
    readInput "1"
    >>= (fun x ->
        readInput "2"
        >>= (fun y ->
            let sum = x + y
            retn sum))

type MaybeBuilder() =
    member o.Bind(m, f) = Option.bind f m
    member o.Return x = Some x

let maybe = MaybeBuilder()

let additionTwo =
    maybe {
        let! x = readInput "1"
        let! y = readInput "2"
        let sum = x + y

        return sum
    }

let additionThree =
    maybe {
        let! x = additionTwo
        let! y = readInput "5"
        let sum = x + y

        return sum
    }

let map' f opt = maybe { return f opt }

let apply' fo xo =
    maybe {
        let! f = fo
        let! x = xo

        return f x
    }

let f1 x = Some(x + 1)
let f2 x = Some(x * 2)

let (>=>) f g x = (f x) >>= g

let c x = (f1 >=> f2) x

let e = c 5
