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
                Students = Seq.empty
                Hosts = Seq.empty
                Questions = Seq.empty
                Submissions = Seq.empty
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
        member this.CreateQuestion examId question = createQuestion examId question
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