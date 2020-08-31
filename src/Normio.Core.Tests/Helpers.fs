module Normio.Core.TestHelpers

open NUnit.Framework
open FsUnit
open Normio.Domain
open Normio.States
open Normio.Errors
open Normio.Events
open Normio.Commands
open Normio.CommandHandlers

let Given (state : State) = state
let When command state = (command, state)
let ThenStateShouldBe expectedState (command, state) =
  match evolve state command with
  | Ok (actualState, events) ->
      actualState |> should equal expectedState
      events |> Some
  | Error err ->
      sprintf "Expected : %A, But Actual : %A" expectedState err
      |> Assert.Fail
      None

let WithEvents expectedEvents actualEvents =
  match actualEvents with
  | Some (actualEvents) ->
    actualEvents |> should equal expectedEvents
  | None -> None |> should equal expectedEvents

let ShouldFailWith (expectedError : Error) (command, state) =
  match evolve state command with
  | Error err -> err |> should equal expectedError
  | Ok r ->
      sprintf "Expected : %A, But Actual : %A" expectedError r
      |> Assert.Fail

