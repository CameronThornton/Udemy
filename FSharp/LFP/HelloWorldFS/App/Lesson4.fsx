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

[<Struct>]
type Person = { FirstName: string; LastName: string }

let john =
    { FirstName = "John"
      LastName = "Connor" }

let sarah = { john with FirstName = "Sarah" }

let alterFirstName person firstName = { person with FirstName = firstName }

let newJohn = alterFirstName john "John"

let x = newJohn = john

let { FirstName = name } = sarah

type Color =
    | White
    | Black
    | Rgb of string

type PieceKind =
    | King
    | Queen
    | Rook
    | Knight
    | Bishop
    | Pawn

type Piece = Piece of Color: Color * Kind: PieceKind

type 'a MyList =
    | Empty
    | ListNode of 'a * 'a MyList

let someList = ListNode(1, ListNode(2, Empty))

let getCount list =
    let rec innerCount list count =
        match list with
        | Empty -> count
        | ListNode (_, subList) -> innerCount subList (count + 1)

    innerCount list 0

let piece = Piece(White, King)

let outputPiece piece =
    let (Piece (Color = c; Kind = k)) = piece
    (c, k)

let zz = outputPiece piece

let a = getCount someList

let users = Dictionary<int, string>()
users.Add(5, "onur")

let getUser id =
    match users.TryGetValue id with
    | true, username -> Some username
    | _ -> None

let someUser = getUser 5

let printNumberOfChars (userName: string) = printf "%i" userName.Length

match someUser with
| Some userName -> printNumberOfChars userName
| _ -> printf "%A" "No user found"
