﻿module Normio.Commands.Api.CommandHandlers

open Normio.Core.Commands
open Normio.Core.Errors
open Normio.Core.CommandHandlers
open Normio.Storage.EventStore

let getExamIdFromCommand = function
| OpenExam (examId, _) -> examId
| StartExam examId -> examId
| EndExam examId -> examId
| CloseExam examId -> examId
| AddStudent (examId, _) -> examId
| RemoveStudent (examId, _) -> examId
| AddHost (examId, _) -> examId
| RemoveHost (examId, _) -> examId
| CreateSubmission (examId, _) -> examId
| CreateQuestion (examId, _) -> examId
| DeleteQuestion (examId, _) -> examId
| ChangeTitle (examId, _) -> examId

type Commander<'a, 'b> = {
    Validate: 'a -> Async<Choice<'b, string>>
    ToCommand: 'b -> Command
}

/// 1. Validate the input data
/// 2. Get the corresponding state from the input data
/// 3. Evolve with the command from the state
/// 4. Get the result
let handleCommand (eventStore : IEventStore) commandData commander = async {
    let! validatedData = commander.Validate commandData
    match validatedData with
    | Choice1Of2 validatedCommandData ->
        let command = commander.ToCommand validatedCommandData
        let! state = eventStore.GetState (getExamIdFromCommand command)
        match evolve state command with
        | Ok (newState, events) ->
            return (newState, events) |> Ok
        | Error err -> return err.ToString() |> Error
    | Choice2Of2 msg ->
        return msg |> Error
}
