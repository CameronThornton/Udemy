module States

type IState = Akka.Persistence.Fsm.PersistentFSM.IFsmState

type Start() =
    interface IState with
        member __.Identifier = "Start"

type SourceCommitting() =
    interface IState with
        member __.Identifier = "SourceCommitting"

type TargetApproving() =
    interface IState with
        member __.Identifier = "TargetApproving"

type TargetCommitting() =
    interface IState with
        member __.Identifier = "TargetCommitting"

type TransactionComplete() =
    interface IState with
        member __.Identifier = "TransactionComplete"
