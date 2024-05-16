module Lesson3

type Employee =
    class
        val age: int
        val fullName: string
        val mutable department: string

        new(fullName, age) = Employee(fullName, age, "Other")

        new(fullName, age, department) as this =
            { fullName = fullName
              age = age
              department = department }
            then this.department <- "some department"
    end

type Employee2(fullName: string, age: int, department: string) =
    let x = 4
    do printfn "%i" x
    new(fullName, age) = Employee2(fullName, age, "Other")

let emp = Employee("person", 33, "hamburger")
let emp2 = Employee2("person2", 332, "hamburger")
