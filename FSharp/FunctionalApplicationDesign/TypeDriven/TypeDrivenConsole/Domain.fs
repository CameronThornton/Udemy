module Domain

open System

type UnsafeProduct = { Name: string; Price: decimal }

[<CustomEquality; CustomComparison>]
type Price =
    private
    | Price of decimal

    static member (+)(Price priceOne, Price priceTwo) = Price(priceOne + priceTwo)

    static member (-)(Price priceOne, Price priceTwo) = Price(priceOne - priceTwo)

    static member (<**>)(Price price, d: decimal) = Decimal.Round(price * d, 2) |> Price

    static member (<//>)(Price priceOne, d: decimal) = Decimal.Round(priceOne / d, 2) |> Price

    static member (<**>)(d: decimal, price) = price * d

    interface IComparable with
        member x.CompareTo y =
            match y with
            | :? Price as y ->
                let (Price pOne) = x
                let (Price pTwo) = y
                compare pOne pTwo
            | _ -> 0

    override x.Equals(yObj) =
        match yObj with
        | :? Price as y ->
            let (Price pOne) = x
            let (Price pTwo) = y
            pOne = pTwo
        | _ -> false

    override x.GetHashCode() =
        let (Price p) = x
        hash p


type Product = { Name: string; Price: Price }

module Result =
    let apply fRes xRes =
        match fRes, xRes with
        | Ok f, Ok x -> f x |> Ok
        | _, Error e -> Error e
        | Error e, _ -> Error e

    let (<!>) = Result.map
    let (<*>) = apply

module Price =
    let private validIntegerForPrice intForPrice =
        match intForPrice with
        | intForPrice when intForPrice >= 0 -> Ok intForPrice
        | _ -> Error "price can not be negative."

    let private validIntegerForDecimal intForDecimal =
        match intForDecimal with
        | intForDecimal when intForDecimal >= 0 && intForDecimal < 100 -> Ok intForDecimal
        | _ -> Error "decimal points must be between 0 and 99 for a price."

    let private convert integer intForDecimal =
        (decimal integer)
        + (decimal intForDecimal) / decimal 100

    let toValue (Price decimal) = decimal

    open Result

    let create (integer, intForDecimal) =
        Price
        <!> (convert <!> (validIntegerForPrice integer)
             <*> (validIntegerForDecimal intForDecimal))

    let create' (integer, intForDecimal) =
        let validPrice = validIntegerForPrice integer
        let validDecimal = validIntegerForDecimal intForDecimal
        let liftedConvert = Ok convert
        //let result = liftedConvert validPrice validDecimal
        let firstParameterApplied = liftedConvert <*> validPrice
        let secondParamterApplied = firstParameterApplied <*> validDecimal

        // a -> b -> c
        //E<a -> b -> c>    : Applied return
        //E<a> -> E<b -> c> : Applied apply

        //a -> b -> c
        //E<a> -> E<b -> c> : Applied map

        Result.map Price secondParamterApplied

module Product =
    let create name price = { Name = name; Price = price }
