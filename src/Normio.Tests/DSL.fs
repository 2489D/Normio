module Normio.Tests.DSL

open FsUnit
open Normio.Core
open Xunit
open Normio.Core.CommandHandlers
open Normio.Core.States
open Xunit.Sdk

let given (state : State) = state

let applied command state = evolve state command

let thenStateShouldBe expectedState (command, state) =
    match evolve state command with
    | Ok (actualState, events) ->
        actualState |> should equal expectedState
        events |> Some
    | Error err ->
        printf "Expected : %A, But Equal : %A" expectedState err
        Assert.True(false)
        None

let thenShouldBeOk = function
    | Ok _ -> ()
    | Error _ -> XunitException("this should be ok") |> raise

let thenShouldBeError = function
    | Ok _ -> XunitException("this should be an error") |> raise
    | Error _ -> ()
