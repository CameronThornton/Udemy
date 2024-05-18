module Lesson3

open System

[<AbstractClass>]
type IManager() =
    abstract member Authorize: unit -> unit
    default __.Authorize() = ()

type Employee =
    inherit IManager

    [<DefaultValue>]
    val mutable private age: int

    val fullName: string
    val mutable department: string

    member employee.Age
        with get () = employee.age
        and private set (age) = employee.age <- age

    new(fullName, age) = Employee(fullName, age, "Other")

    new(fullName, age, department) as this =
        { fullName = fullName
          //age = age
          department = department }
        then this.department <- "some department"

type Employee2<'T>(fullName: string, age: int, department: string) =
    inherit IManager()
    let x = 4

    let f x y = x

    do printfn "%i" x
    new(fullName, age) = Employee2(fullName, age, "Other")

    member val Age = 35 with get, set

    member __.SomeMethod(a: 'T) = a

    member this.SomeInteger(i: 'T) = this.SomeMethod i
    override __.Authorize() = ()

let emp = Employee("person", 33, "hamburger")
let emp2 = Employee2<int>("person2", 332, "hamburger")
