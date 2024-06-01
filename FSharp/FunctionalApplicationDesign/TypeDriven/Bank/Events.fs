module Events

open System.Text.Json
open Commands

//[<JsonObject(MemberSerialization.Fields)>]
type Event() =
    class
    end

type AmountReserved(amount: decimal, transactionId: string) =
    inherit Event()

    member __.TransactionId = transactionId
    member __.Amount = amount
    private new() = AmountReserved(0M, "")

type ReservationConfirmed(transactionId: string) =
    inherit Event()

    member __.TransactionId = transactionId
    private new() = ReservationConfirmed("")

type TransferRejected(transactionId: string, reason: string) =
    inherit Event()

    member __.TransactionId = transactionId
    member __.Reason = reason
    private new() = TransferRejected("", "")

type BalanceSet(amount: decimal) =
    inherit Event()

    member __.Amount = amount
    private new() = BalanceSet(0M)

type AccountCreated(accountId: string) =
    inherit Event()

    member __.AcountId = accountId
    private new() = AccountCreated("")

type TransactionCreated(command: TransferMoney) =
    inherit Event()

    member __.Command = command

    private new() = TransactionCreated(Unchecked.defaultof<TransferMoney>)

type AccountPassivated(id: string) =
    inherit Event()

    member __.Id = id
    private new() = AccountPassivated("")

type TransactionPassivated(id: string) =
    inherit Event()

    member __.Id = id
    private new() = TransactionPassivated("")
