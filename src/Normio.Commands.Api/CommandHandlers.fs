﻿module Normio.Commands.Api.CommandHandlers

open Normio.Core.Commands
open Normio.Core.CommandHandlers
open Normio.Persistence.EventStore

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
    Validate: 'a -> Async<Result<'b, string>>
    ToCommand: 'b -> Command
}

/// 1. Validate the input data
/// 2. Get the corresponding state from the input data
/// 3. Evolve with the command from the state
/// 4. Get the result
let handleCommand (eventStore : IEventStore) commandData commander = async {
    let! validatedData = commander.Validate commandData
    match validatedData with
    | Ok validatedCommandData ->
        let command = commander.ToCommand validatedCommandData
        let! state = eventStore.GetState (getExamIdFromCommand command)
        match evolve state command with
        | Ok (newState, events) ->
            return (newState, events) |> Ok
        | Error err -> return err.ToString() |> Error
    | Error msg ->
        return msg |> Error
}
