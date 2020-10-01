module Normio.Storage.Exams

open System
open System.Collections.Generic

open Normio.Domain
open Normio.Projections
open Normio.ReadModels

(*
Don't Panic!
The codes below executed after all the validations in the Domain Layer!
So, don't bother yourself about the error handling here
*)

let private exams =
    let dict = Dictionary<Guid, ExamReadModel>()
    dict

let getExam examId =
    exams.[examId]
    |> async.Return

let private openExam examId title =
    async {
        let exam = {
            Id = examId
            Status = BeforeExam
            Title = title
            Students = Map.empty
            Hosts = Map.empty
            Questions = []
        }
        exams.TryAdd(examId, exam) |> ignore
    }

let private startExam examId =
    async {
        let exam = exams.[examId]
        exams.[examId] <- { exam with Status = DuringExam }
    }

let private endExam examId =
    async {
        let exam = exams.[examId]
        exams.[examId] <- { exam with Status = AfterExam }
    }

let private closeExam examId =
    async {
        exams.Remove(examId) |> ignore
    }

let private addStudent examId (student: Student) =
    async {
        let exam = exams.[examId]
        exams.[examId] <- { exam with Students = exam.Students |> Map.add student.Id student }
    }

let private removeStudent examId (student: Student) =
    async {
        let exam = exams.[examId]
        exams.[examId] <- { exam with Students = exam.Students |> Map.remove student.Id }
    }

let private addHost examId (host: Host) =
    async {
        let exam = exams.[examId]
        exams.[examId] <- { exam with Hosts = exam.Hosts |> Map.add host.Id host }
    }

let private removeHost examId (host: Host) =
    async {
        let exam = exams.[examId]
        exams.[examId] <- { exam with Hosts = exam.Hosts |> Map.remove host.Id }
    }

let private createQuestion examId (file: File) =
    async {
        let exam = exams.[examId]
        exams.[examId] <- { exam with Questions = file :: exam.Questions }
    }

let private deleteQuestion examId (file: File) =
    async {
        let exam = exams.[examId]
        exams.[examId] <- { exam with Questions = exam.Questions |> List.filter (fun f -> f.Id <> file.Id)}
    }

let private changeTitle examId title =
    async {
        let exam = exams.[examId]
        exams.[examId] <- { exam with Title = title }
    }

let examActions = {
    OpenExam = openExam
    StartExam = startExam
    EndExam = endExam
    CloseExam = closeExam
    AddStudent = addStudent
    RemoveStudent = removeStudent
    AddHost = addHost
    RemoveHost = removeHost
    CreateQuestion = createQuestion
    DeleteQuestion = deleteQuestion
    ChangeTitle = changeTitle
}