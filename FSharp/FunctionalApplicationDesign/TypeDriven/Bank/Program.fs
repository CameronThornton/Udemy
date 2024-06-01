open System.Reflection
open System.IO
open Microsoft.Extensions.Configuration
open Akka.Configuration
open Domain
open Akka.Actor
open Commands
open System

// let dllPath = Assembly.GetEntryAssembly().Location
// let dir = FileInfo(dllPath).Directory

// let config =
//     ConfigurationBuilder()
//         .SetBasePath(dir.FullName)
//         .AddJsonFile("appsettings.json")
//         .Build()

let s = Config.akka
let c = ConfigurationFactory.ParseString(s)

let test = c.Root.Values
let props = CommandHandler.Props()
let sys = ActorSystem.Create("Bank", c)

let commandHandler = sys.ActorOf(props, "CommandHandler")
commandHandler.Tell(SetBalance("1", 2000M))
commandHandler.Tell(SetBalance("2", 3000M))
commandHandler.Tell(TransferMoney("1", "2", 100M, Guid.NewGuid().ToString()))
commandHandler.Tell(TransferMoney("1", "2", 1950M, Guid.NewGuid().ToString()))

Console.ReadKey() |> ignore
