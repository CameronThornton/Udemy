module Commands

type Command() =
    class
    end

type TransferMoney(source: string, target: string, amount: decimal, transactionId: string) =
    inherit Command()

    member __.Source = source
    member __.Target = target
    member __.Amount = amount
    member __.TransactionId = transactionId
    private new() = TransferMoney("", "", 0M, "")

type ConfirmReservation(transactionId: string) =
    member __.TransactionId = transactionId

type GetBalance() =
    inherit Command()

type SetBalance(accountId: string, amount: decimal) =
    inherit Command()

    member __.Amount = amount
    member __.AccountId = accountId

type PassivateAccount(id: string) =
    inherit Command()

    member __.Id = id

type PassivateTransaction(id: string) =
    inherit Command()

    member __.Id = id
