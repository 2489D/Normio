module Normio.Persistence.ReadModels.InMemory

open System
open System.Collections.Generic
open Normio.Persistence.Projections
open Normio.Persistence.Queries
open Normio.Persistence.ReadModels

type private InMemoryExamsReadModel() =
    let exams = Dictionary<Guid, ExamReadModel>()

    let openExam examId title =
        async {
            let exam = {
                Id = examId
                ExamId = examId
                Status = BeforeExam
                Title = title
                Students = Array.empty
                Hosts = Array.empty
                Questions = Array.empty
                Submissions = Array.empty
            }
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
            exams.[examId] <- { exam with Students = exam.Students |> Array.append [| student |] }
        }

    let removeStudent examId studentId =
        async {
            let exam = exams.[examId]
            exams.[examId] <- { exam with Students = exam.Students |> Array.filter (fun s -> s.Id <> studentId) }
        }

    let addHost examId host =
        async {
            let exam = exams.[examId]
            exams.[examId] <- { exam with Hosts = exam.Hosts |> Array.append [| host |] }
        }

    let removeHost examId hostId =
        async {
            let exam = exams.[examId]
            exams.[examId] <- { exam with Hosts = exam.Hosts |> Array.filter (fun h -> h.Id <> hostId) }
        }

    let createQuestion examId questionId =
        async {
            let exam = exams.[examId]
            exams.[examId] <- { exam with Questions = exam.Questions |> Array.append [| questionId |] }
        }

    let createSubmission examId submission =
        async {
            let exam = exams.[examId]
            exams.[examId] <- { exam with Submissions = exam.Submissions |> Array.append [| submission |] }
        }

    let deleteQuestion examId questionId =
        async {
            let exam = exams.[examId]
            exams.[examId] <- { exam with Questions = exam.Questions |> Array.filter (fun qId -> qId <> questionId)}
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
        member this.OpenExam examId title = openExam examId title
        member this.StartExam examId = startExam examId
        member this.EndExam examId = endExam examId
        member this.CloseExam examId = closeExam examId
        member this.AddStudent examId student = addStudent examId student
        member this.RemoveStudent examId studentId = removeStudent examId studentId
        member this.AddHost examId host = addHost examId host
        member this.RemoveHost examId hostId = removeHost examId hostId
        member this.CreateSubmission examId submission = createSubmission examId submission
        member this.CreateQuestion examId questionId = createQuestion examId questionId
        member this.DeleteQuestion examId questionId = deleteQuestion examId questionId
        member this.ChangeTitle examId title = changeTitle examId title

let private inMemoryExamsReadModelInstance = InMemoryExamsReadModel()
 
let examActions = inMemoryExamsReadModelInstance :> IExamAction

let inMemoryExamQueries = {
    GetExamByExamId = inMemoryExamsReadModelInstance.GetExam
}

let inMemoryQueries = {
    Exam = inMemoryExamQueries
}

let inMemoryActions: ProjectionActions = {
    Exam = examActions
}