open System
open System.Collections.Generic

open Normio.Domain
open Normio.Projections
open Normio.ReadModels

let private exams =
    let dict = new Dictionary<Guid, ExamReadModel>()
    dict

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

let private removeStudent examId (studnent: Student) =
    async {
        let exam = exams.[examId]
        exams.[examId] <- { exam with Students = exam.Students |> Map.remove studnent.Id }
    }

let examActions = {
    OpenExam = openExam
    StartExam = startExam
    EndExam = endExam
    CloseExam = closeExam
    AddStudent = addStudnet
    RemoveStudent = removeStudent
    AddHost = 
    RemoveHost = 
    CreateQuestion = 
    DeleteQuestion = 
    ChangeTitle = 
}
