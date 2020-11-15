namespace Normio.Commands.Api

open System
open System.Text.Json.Serialization
open Normio.Core.Commands

[<AutoOpen>]
module EndExam = 
    [<CLIMutable>]
    type EndExamRequest =
        {
            [<JsonPropertyName("examId")>]
            ExamId : Guid
        }

    let validateEndExam req = async {
        return req.ExamId |> Ok
    }

    let endExamCommander = {
        Validate = validateEndExam
        ToCommand = EndExam
    }
