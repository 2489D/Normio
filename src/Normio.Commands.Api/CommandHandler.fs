module Normio.CommandHandlers

open Normio.CommandHandlers
open Normio.Commands
open Normio.Storage.EventStore

type Commander<'a, 'b> = {
    Validate: 'a -> Async<Choice<'b, string>>
    ToCommand: 'b -> Command
}

type ErrorResponse = {
    Message: string
}

let err msg = { Message = msg }

let getExamIdFromCommand = function
| OpenExam (examId, _) -> examId
| StartExam examId -> examId
| EndExam examId -> examId
| CloseExam examId -> examId
| AddStudent (examId, _) -> examId
| RemoveStudent (examId, _) -> examId
| AddHost (examId, _) -> examId
| RemoveHost (examId, _) -> examId
| CreateQuestion (examId, _) -> examId
| DeleteQuestion (examId, _) -> examId
| ChangeTitle (examId, _) -> examId

let handleCommand eventStore commandData commander = async {
    let! validatedData = commander.Validate commandData
    match validatedData with
    | Choice1Of2 validatedCommandData ->
        let command = commander.ToCommand validatedCommandData
        let! state = eventStore.GetState (getExamIdFromCommand command)
        match state with
        | Ok validState ->
            match evolve validState command with
            | Ok (newState, events) ->
                return (newState, events) |> Ok
            | Error msg -> return msg |> Error
        | Error msg -> return msg |> EventStoreError.toString |> Error
    | Choice2Of2 msg ->
        return msg |> Error
}