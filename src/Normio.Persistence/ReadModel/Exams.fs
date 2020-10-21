module Normio.Persistence.Exams

open System
open FSharp.Control
open FSharp.CosmosDb

open Normio.Core.Domain
open Normio.Persistence.EventStore
open Normio.Persistence.ReadModels
open Normio.Persistence.Projections
open Normio.Persistence.Queries

let private getConn connString =
    connString
    |> Cosmos.fromConnectionString
    |> Cosmos.database "EventStore"
    |> Cosmos.container "querySide"

let private getExam connString (examId: Guid) =
    getConn connString
    |> Cosmos.query "SELECT * FROM e WHERE e.ExamId = @Id"
    |> Cosmos.parameters [
        "@Id", box examId
    ]
    |> Cosmos.execAsync<ExamReadModel option>
    |> AsyncSeq.firstOrDefault None

let private openExam connString examId title = async {
    let exam =
        { Id = examId
          ExamId = examId
          Status = BeforeExam
          Title = title
          Questions = []
          Submissions = []
          Students = Map.empty
          Hosts = Map.empty }
    
    getConn connString
    |> Cosmos.insert exam
    |> Cosmos.execAsync
    |> ignore
}
        
let private startExam connString examId = async {
    getConn connString
    |> Cosmos.update (string examId) (string examId) (fun exam -> { exam with Status = DuringExam })
    |> Cosmos.execAsync
    |> ignore
}

let private endExam connString examId = async {
    getConn connString
    |> Cosmos.update (string examId) (string examId) (fun exam -> { exam with Status = AfterExam })
    |> Cosmos.execAsync
    |> ignore
}

let private closeExam connString examId = async {
    getConn connString
    |> Cosmos.delete (string examId) (string examId)
    |> Cosmos.execAsync
    |> ignore
}

let private addStudent connString examId (student: Student) = async {
    let key = string examId
    getConn connString
    |> Cosmos.update key key (fun exam -> { exam with Students = exam.Students |> Map.add student.Id student })
    |> Cosmos.execAsync
    |> ignore
}

let private removeStudent connString examId studentId = async {
    let key = string examId
    getConn connString
    |> Cosmos.update key key (fun exam -> { exam with Students = exam.Students |> Map.remove studentId })
    |> Cosmos.execAsync
    |> ignore
}

let private addHost connString examId (host: Host) = async {
    let key = string examId
    getConn connString
    |> Cosmos.update key key (fun exam -> { exam with Hosts = exam.Hosts |> Map.add host.Id host })
    |> Cosmos.execAsync
    |> ignore
}

let private removeHost connString examId hostId = async {
    let key = string examId
    getConn connString
    |> Cosmos.update key key (fun exam -> { exam with Hosts = exam.Hosts |> Map.remove hostId })
    |> Cosmos.execAsync
    |> ignore
}

let private createSubmission connString examId submission = async {
    let key = string examId
    getConn connString
    |> Cosmos.update key key (fun exam -> { exam with Submissions = submission :: exam.Submissions })
    |> Cosmos.execAsync
    |> ignore
}

let private createQuestion connString examId (file: File) = async {
    let key = string examId
    getConn connString
    |> Cosmos.update key key (fun exam -> { exam with Questions = file :: exam.Questions })
    |> Cosmos.execAsync
    |> ignore
}

let private deleteQuestion connString examId fileId = async {
    let key = string examId
    getConn connString
    |> Cosmos.update key key (fun exam -> { exam with Questions = exam.Questions |> List.filter (fun f -> f.Id <> fileId)})
    |> Cosmos.execAsync
    |> ignore
}

let private changeTitle connString examId title = async {
    let key = string examId
    getConn connString
    |> Cosmos.update key key (fun exam -> { exam with Title = title })
    |> Cosmos.execAsync
    |> ignore
}


let examActions connString =
    { new IExamAction with
        member this.OpenExam examId title = openExam connString examId title
        member this.StartExam examId = startExam connString examId
        member this.EndExam examId = endExam connString examId
        member this.CloseExam examId = closeExam connString examId
        member this.AddStudent examId student = addStudent connString examId student
        member this.RemoveStudent examId studentId = removeStudent connString examId studentId
        member this.AddHost examId host = addHost connString examId host
        member this.RemoveHost examId hostId = removeHost connString examId hostId
        member this.CreateSubmission examId submission = createSubmission connString examId submission
        member this.CreateQuestion examId file = createQuestion connString examId file
        member this.DeleteQuestion examId fileId = deleteQuestion connString examId fileId
        member this.ChangeTitle examId title = changeTitle connString examId title }

let examQueries connString = {
    GetExamByExamId = getExam connString
}

let cosmosQueries connString: Queries = {
    Exam = examQueries connString
}

let cosmosActions connString: ProjectionActions = {
    Exam = examActions connString
}
