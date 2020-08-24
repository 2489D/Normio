// Learn more about F# at http://fsharp.org

open System

type Notification = {
    StartExam: Guid -> Async<unit>
    EndExam: Guid -> Async<unit>
}

type NotiTime = System.DateTimeOffset

type NotiAction = NotiTime * Notification

/// Maintains a minimum heap structure w.r.t `NotiAction.NotiTime`
type NotiHeap =
    | Node of NotiHeap * NotiAction * NotiHeap
    | Empty

module NotiHeap =
    let rec add action heap =
        let (time, noti) = action
        match heap with
        | Node (left, act, right) ->
            if time < (fst act)
            then Node (add act heap, action, right)
            else Node (add action left , act, right)
        | Empty -> Node(Empty, action, Empty)


[<EntryPoint>]
let main argv =
    printfn "Running Normio Clock"
    0 // return an integer exit code
