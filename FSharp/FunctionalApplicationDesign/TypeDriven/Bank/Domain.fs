module Domain

open Akka.Persistence
open Akka.Persistence.Fsm
open Commands
open Events
open Akka.Actor
open States
open System

type AccountActor(accountId) as account =
    inherit ReceivePersistentActor()

    let mutable currentBalance = 0M
    let mutable availableBalance = 0M
    let mutable reservations: Map<string, decimal> = Map.empty
    let context = ReceivePersistentActor.Context
    let publish = context.System.EventStream.Publish

    let updateState (event: Event) =
        match event with
        | :? BalanceSet as e ->
            currentBalance <- e.Amount
            availableBalance <- currentBalance
        | :? AmountReserved as e ->
            match reservations.TryFind e.TransactionId with
            | None ->
                reservations <- reservations.Add(e.TransactionId, e.Amount)

                if e.Amount > 0M then
                    availableBalance <- availableBalance - e.Amount
            | _ -> ()

        | :? ReservationConfirmed as e ->
            match reservations.TryFind e.TransactionId with
            | Some amount ->
                reservations <- reservations.Remove e.TransactionId
                currentBalance <- currentBalance - amount

                if amount < 0M then
                    availableBalance <- availableBalance - amount
            | _ -> ()
        | _ -> ()

    let tellToTransactionProcess e transactionId =
        let transactionProcess = "../TransactionProcess_"

        context
            .ActorSelection(transactionProcess + transactionId)
            .Tell(e)

    let passivateIfApplicable () =
        if (reservations = Map.empty) then
            accountId
            |> PassivateAccount
            |> context.Parent.Tell

    let publishSideEffects (event: Event) =
        publish event

        match event with
        | :? TransferRejected as e ->
            tellToTransactionProcess e (e.TransactionId)
            passivateIfApplicable ()

        | :? AmountReserved as e -> tellToTransactionProcess e (e.TransactionId)
        | :? BalanceSet -> passivateIfApplicable ()
        | :? ReservationConfirmed as e ->
            tellToTransactionProcess e (e.TransactionId)
            passivateIfApplicable ()
        | _ -> ()

    let handleEvent (event: Event) =
        account.Persist(
            event,
            fun e ->
                updateState e
                publishSideEffects e
        )

    let transferMoney (command: TransferMoney) =
        if command.Amount > availableBalance then
            handleEvent
            <| TransferRejected(command.TransactionId, "Not enough money")
        else
            handleEvent
            <| AmountReserved(command.Amount, command.TransactionId)

        true

    let setBalance (command: SetBalance) =
        handleEvent <| BalanceSet command.Amount

    let confirmReservation (command: ConfirmReservation) =
        let event = ReservationConfirmed command.TransactionId

        match reservations.TryFind command.TransactionId with
        | Some _ -> handleEvent event
        | _ -> publishSideEffects event

        true

    let recover (message: obj) =
        match message with
        | :? Event as e -> updateState e
        | _ -> ()

    do
        account.Command transferMoney
        account.Command confirmReservation
        account.Command setBalance
        account.Recover recover

    override __.PersistenceId = "Account_" + accountId

    static member Props accountId =
        Props.Create(fun _ -> AccountActor accountId)



type TransactionProcess(command: TransferMoney) as t =
    inherit PersistentFSM<IState, obj, Event>()

    let stay = t.Stay
    let goto = t.GoTo
    let stop: unit -> _ = t.Stop
    let start = Start()
    let context = TransactionProcess.Context
    let sourceCommitting: IState = upcast SourceCommitting()
    let targetCommitting: IState = upcast TargetCommitting()
    let targetApproving: IState = upcast TargetApproving()

    let tellToAccount accountId e =
        let account = "../Account_"

        context
            .ActorSelection(account + accountId)
            .Tell(e)

    do
        ``base``.StartWith(start, null)

        ``base``.When(
            start,
            fun x ->
                fun _ ->
                    match x.FsmEvent with
                    | :? AmountReserved ->
                        goto(targetApproving)
                            .AndThen(fun _ ->
                                TransferMoney(command.Source, command.Target, -command.Amount, command.TransactionId)
                                |> tellToAccount command.Target)
                    | :? TransferRejected ->
                        command.TransactionId
                        |> PassivateTransaction
                        |> context.Parent.Tell

                        stop ()
                    | _ -> stay ()
        )

        ``base``.When(
            targetApproving,
            fun x ->
                fun _ ->
                    match x.FsmEvent with
                    | :? AmountReserved as e ->
                        goto(sourceCommitting)
                            .AndThen(fun _ ->
                                ConfirmReservation e.TransactionId
                                |> tellToAccount command.Source)
                    | _ -> stay ()
        )

        ``base``.When(
            sourceCommitting,
            fun x ->
                fun _ ->
                    match x.FsmEvent with
                    | :? ReservationConfirmed as e ->
                        goto(targetCommitting)
                            .AndThen(fun _ ->
                                ConfirmReservation e.TransactionId
                                |> tellToAccount command.Target)
                    | _ -> stay ()
        )

        ``base``.When(
            targetCommitting,
            fun x ->
                fun _ ->
                    match x.FsmEvent with
                    | :? ReservationConfirmed ->
                        command.TransactionId
                        |> PassivateTransaction
                        |> context.Parent.Tell

                        stop ()
                    | _ -> stay ()
        )

    override __.PersistenceId = "TransactionProcess_" + command.TransactionId
    override __.ApplyEvent(_, currentdata) = currentdata

    override this.OnRecoveryCompleted() =
        let state = this.StateName

        match state with
        | :? Start -> Command |> tellToAccount command.Source
        | :? TargetApproving -> command |> tellToAccount command.Target
        | :? SourceCommitting ->
            ConfirmReservation command.TransactionId
            |> tellToAccount command.Source
        | :? TargetCommitting ->
            ConfirmReservation command.TransactionId
            |> tellToAccount command.Target
        | _ -> ()

    static member Props command =
        Props.Create(fun _ -> TransactionProcess command)

type CommandHandler() as commandHandler =
    inherit ReceivePersistentActor()

    let context = ReceivePersistentActor.Context
    let publish = context.System.EventStream.Publish
    let mutable transactions: Map<string, TransferMoney> = Map.empty
    let mutable accountPassivationQueue: Map<string, int> = Map.empty
    let mutable accounts: Map<string, IActorRef> = Map.empty

    let updateState (event: Event) =
        match event with
        | :? TransactionCreated as e -> transactions <- transactions.Add(e.Command.TransactionId, e.Command)
        | :? TransactionPassivated as e -> transactions <- transactions.Remove e.Id
        | _ -> ()

    let handleEvent (event: Event) action =
        commandHandler.Persist(
            event,
            fun e ->
                updateState e
                action ()
                publish e
        )

    let incrementPassivationQueue accountId =
        accountPassivationQueue <-
            let count = accountPassivationQueue[accountId]

            accountPassivationQueue
            |> Map.remove accountId
            |> Map.add (accountId) (count + 1)

    let createAccount accountId =
        match accountPassivationQueue.TryFind accountId with
        | Some _ ->
            incrementPassivationQueue accountId
            accounts[accountId]
        | None ->
            let actor = context.ActorOf(AccountActor.Props(accountId), "Account_" + accountId)
            accountPassivationQueue <- accountPassivationQueue.Add(accountId, 1)
            accounts <- accounts.Add(accountId, actor)
            actor

    let createTransaction (command: TransferMoney) =
        if not (transactions.ContainsKey(command.TransactionId)) then
            handleEvent (command |> TransactionCreated) (fun () ->
                context.ActorOf(TransactionProcess.Props(command), "TransactionProcess_" + command.TransactionId)
                |> ignore)

    let transferMoney (command: TransferMoney) =
        command.Source |> createAccount |> ignore
        command.Target |> createAccount |> ignore
        command |> createTransaction
        true

    let setBalance (command: SetBalance) =
        (command.AccountId |> createAccount).Tell command
        true

    let passivateAccount (command: PassivateAccount) =
        let id = command.Id
        let count = accountPassivationQueue[id] - 1

        if count = 0 then
            context
                .Sender
                .GracefulStop(TimeSpan.FromSeconds(1.0))
                .Wait()

            accountPassivationQueue <- accountPassivationQueue.Remove id
            accounts <- accounts.Remove(id)
        else
            accountPassivationQueue <-
                accountPassivationQueue
                |> Map.remove id
                |> Map.add id count

    let passivateTransaction (command: PassivateTransaction) =
        handleEvent (TransactionPassivated(command.Id)) (fun () ->
            context
                .Sender
                .GracefulStop(TimeSpan.FromSeconds(1.0))
                .Wait())

    let recover (e: obj) =
        match e with
        | :? RecoveryCompleted ->
            transactions
            |> Map.iter (fun _ v -> transferMoney v |> ignore)
        | :? Event as ev -> updateState ev
        | _ -> ()

    do
        commandHandler.Recover(recover)
        commandHandler.Command(transferMoney)
        commandHandler.Command(setBalance)
        commandHandler.Command(passivateAccount)
        commandHandler.Command(passivateTransaction)

    override __.PersistenceId = "CommandHandler"

    static member Props() =
        Props.Create(fun _ -> new CommandHandler())
