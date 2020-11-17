module Normio.Persistence.ReadModels.InMemory

open System
open System.Collections.Generic

open FsHttp
open FsHttp.DslCE

open Normio.Core.Commands
open Normio.Persistence.Projections
open Normio.Persistence.Queries
open Normio.Persistence.ReadModels

// FIXME: let entry point configure this
let timerServiceUri = "https://localhost:8083"

type InMemoryTimersReadModel internal (timerUri: string) =
    let mutable timers: TimerReadModel list = List.empty
    
    let registerCommand (command: Command) (time: DateTime) =
        async {
            let timer =
                { Command = command
                  Time = time }
            if timers |> Seq.map (fun t -> t.Command) |> Seq.contains command |> not
            then
                // TODO: this is forget and fire
                try
                    http {
                        POST (timerUri + "/api/create")
                        CacheControl "no-cache"
                        body
                        json (sprintf """{ "command" : %s, "time" : %s }""" (string command) (string time))
                    } |> ignore
                with
                | _ -> () // TODO -> timer creation failed
                do timers <- timer :: timers
        }
    
    let removeCommand (command: Command) =
        async {
            do timers <- timers |> List.filter (fun timer -> timer.Command <> command)
            try
                http {
                    POST (timerUri + "/api/delete")
                    CacheControl "no-cache"
                    body
                    json (sprintf """{ "command" : %s }""" (string command))
                } |> ignore
            with
            | _ -> ()
        }
    
    member _.GetCommandsById (id: Guid) =
        async {
            return timers |> List.filter (fun timer -> timer.Command.ExamId = id)
        }

    interface ITimerAction with
        member _.CreateTimer command time = registerCommand command time
        member _.RemoveTimer command = removeCommand command

type InMemoryExamsReadModel internal () =
    let exams = Dictionary<Guid, ExamReadModel>()

    let openExam examId title startTime (duration: TimeSpan) =
        async {
            let minutes = duration.TotalMinutes
            let exam = ExamReadModel.Initial examId title startTime minutes
            exams.Add(examId, exam)
        }

    let startExam examId =
        async {
            let exam = exams.[examId]
            exams.[examId] <- { exam with Status = DuringExam }
        }

    let endExam examId =
        async {
            let exam = exams.[examId]
            exams.[examId] <- { exam with Status = AfterExam }
        }

    let closeExam examId =
        async {
            exams.Remove(examId) |> ignore
        }

    let addStudent examId student =
        async {
            let exam = exams.[examId]
            let students =
                seq {
                    yield student
                    yield! exam.Students
                }
            exams.[examId] <- { exam with Students = students }
        }

    let removeStudent examId studentId =
        async {
            let exam = exams.[examId]
            exams.[examId] <- { exam with Students = exam.Students |> Seq.filter (fun s -> s.Id <> studentId) }
        }

    let addHost examId host =
        async {
            let exam = exams.[examId]
            let hosts =
                seq {
                    yield host
                    yield! exam.Hosts
                }
            exams.[examId] <- { exam with Hosts = hosts }
        }

    let removeHost examId hostId =
        async {
            let exam = exams.[examId]
            exams.[examId] <- { exam with Hosts = exam.Hosts |> Seq.filter (fun h -> h.Id <> hostId) }
        }

    let createQuestion examId question =
        async {
            let exam = exams.[examId]
            let questions =
                seq {
                    yield question
                    yield! exam.Questions
                }
            exams.[examId] <- { exam with Questions = questions }
        }

    let createSubmission examId submission =
        async {
            let exam = exams.[examId]
            let submissions =
                seq {
                    yield submission
                    yield! exam.Submissions
                }
            exams.[examId] <- { exam with Submissions = submissions }
        }

    let deleteQuestion examId questionId =
        async {
            let exam = exams.[examId]
            exams.[examId] <- { exam with Questions = exam.Questions |> Seq.filter (fun question -> question.Id <> questionId)}
        }

    let sendMessage examId message =
        async {
            let exam = exams.[examId]
            exams.[examId] <- { exam with Messages = seq { yield message; yield! exam.Messages }}
        }

    let changeTitle examId title =
        async {
            let exam = exams.[examId]
            exams.[examId] <- { exam with Title = title }
        }
    
    member this.GetExam examId =
        async {
            match exams.TryGetValue examId with
            | true, exam -> return Some exam
            | _ -> return None
        }
    
    interface IExamAction with
        member this.OpenExam examId title startTime duration = openExam examId title startTime duration
        member this.StartExam examId = startExam examId
        member this.EndExam examId = endExam examId
        member this.CloseExam examId = closeExam examId
        member this.AddStudent examId student = addStudent examId student
        member this.RemoveStudent examId studentId = removeStudent examId studentId
        member this.AddHost examId host = addHost examId host
        member this.RemoveHost examId hostId = removeHost examId hostId
        member this.CreateSubmission examId submission = createSubmission examId submission
        member this.CreateQuestion examId question = createQuestion examId question
        member this.DeleteQuestion examId questionId = deleteQuestion examId questionId
        member this.SendMessage examId message = sendMessage examId message
        member this.ChangeTitle examId title = changeTitle examId title

let private inMemoryExamsReadModelInstance = InMemoryExamsReadModel()
let private inMemoryTimersReadModelInstance = InMemoryTimersReadModel(timerServiceUri)
 
let examActions = inMemoryExamsReadModelInstance :> IExamAction
let timerActions = inMemoryTimersReadModelInstance :> ITimerAction

let inMemoryTimerQueries = {
    GetCommandsById = inMemoryTimersReadModelInstance.GetCommandsById
}

let inMemoryExamQueries = {
    GetExamByExamId = inMemoryExamsReadModelInstance.GetExam
}

let inMemoryQueries = {
    Exam = inMemoryExamQueries
    Timer = inMemoryTimerQueries
}

let inMemoryActions: ProjectionActions = {
    Exam = examActions
    Timer = timerActions
}