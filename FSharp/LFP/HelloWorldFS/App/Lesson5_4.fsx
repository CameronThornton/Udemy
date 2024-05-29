open System

type QueueMsg =
    | Add of int
    | Fetch of AsyncReplyChannel<int>

type Queue() =
    let innerQueue =
        MailboxProcessor.Start (fun inbox ->
            let rec loop list =
                async {
                    printfn "number of elements in list is %A" (list |> List.length)

                    match list with
                    | [] ->
                        let! msg =
                            inbox.Scan (function
                                | Add _ as m -> Some <| async.Return m
                                | _ -> None)

                        match msg with
                        | Add x -> return! (loop (x :: list))
                        | _ -> return! loop list
                    | _ ->
                        let! msg = inbox.Receive()

                        match msg with
                        | Add x -> return! loop (x :: list)
                        | Fetch (reply) ->
                            reply.Reply(list.Head)
                            return! loop list.Tail
                }

            loop [])

    member this.Add x = innerQueue.Post <| Add x
    member this.Fetch() = innerQueue.PostAndReply Fetch

let queue = Queue()

let producer =
    async {
        let rand = Random()

        while true do
            let r = rand.Next(100)
            r |> queue.Add
            do! Async.Sleep(100 * r)
    }

let consumer =
    async {
        let rand = Random()

        while true do
            printfn "%i" <| queue.Fetch()
            do! Async.Sleep(100 * rand.Next(100))
    }

producer |> Async.Start
consumer |> Async.Start
