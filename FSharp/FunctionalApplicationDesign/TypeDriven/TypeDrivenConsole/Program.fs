open Domain
open Domain.Result

let p = Price.create (10, 50)
let pTwo = Price.create (10, 99)

printfn "%A" (p = pTwo)

let product = (Product.create "Phone") <!> p

match product with
| Ok p -> printf "Product price with tax: %A" (p.Price <**> 1.15M |> Price.toValue)
| Error err -> printf "%A" <| err
