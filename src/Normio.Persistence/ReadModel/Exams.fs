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
    async {
        try
            let! exam = getConn connString
                        |> Cosmos.query "SELECT * FROM e WHERE e.ExamId = @Id"
                        |> Cosmos.parameters [
                            "@Id", box examId
                        ]
                        |> Cosmos.execAsync<ExamReadModel>
                        |> AsyncSeq.toArrayAsync
            
            return exam |> Array.head |> Some
        with
        | _ -> return None
    }

let private openExam connString examId title = async {
    let exam =
        { Id = examId
          ExamId = examId
          Status = BeforeExam
          Title = title
          Questions = Array.empty
          Submissions = Array.empty
          Students = Array.empty
          Hosts = Array.empty }
    
    try
        do! getConn connString
            |> Cosmos.insert exam
            |> Cosmos.execAsync
            |> AsyncSeq.iter ignore
    with
    | e -> printfn "%A" e
}
        
let private startExam connString examId = async {
   do! getConn connString
       |> Cosmos.update (string examId) (string examId) (fun exam -> { exam with Status = DuringExam })
       |> Cosmos.execAsync
       |> AsyncSeq.iter ignore
}

let private endExam connString examId = async {
    do! getConn connString
        |> Cosmos.update (string examId) (string examId) (fun exam -> { exam with Status = AfterExam })
        |> Cosmos.execAsync
        |> AsyncSeq.iter ignore
}

let private closeExam connString examId = async {
    do! getConn connString
        |> Cosmos.delete<ExamReadModel> (string examId) (string examId)
        |> Cosmos.execAsync
        |> AsyncSeq.iter ignore
}

let private addStudent connString examId (student: Student) = async {
    let key = string examId
    do! getConn connString
        |> Cosmos.update key key (fun (exam: ExamReadModel) -> { exam with Students = exam.Students |> Array.append [| student |] })
        |> Cosmos.execAsync
        |> AsyncSeq.iter ignore
}

let private removeStudent connString examId studentId = async {
    let key = string examId
    do! getConn connString
        |> Cosmos.update key key (fun (exam: ExamReadModel) -> { exam with Students = exam.Students |> Array.filter (fun s -> s.Id <> studentId) })
        |> Cosmos.execAsync
        |> AsyncSeq.iter ignore
}

let private addHost connString examId (host: Host) = async {
    let key = string examId
    do! getConn connString
        |> Cosmos.update key key (fun (exam: ExamReadModel) -> { exam with Hosts = exam.Hosts |> Array.append [| host |] })
        |> Cosmos.execAsync
        |> AsyncSeq.iter ignore
}

let private removeHost connString examId hostId = async {
    let key = string examId
    do! getConn connString
        |> Cosmos.update key key (fun (exam: ExamReadModel) -> { exam with Hosts = exam.Hosts |> Array.filter (fun h -> h.Id <> hostId) })
        |> Cosmos.execAsync
        |> AsyncSeq.iter ignore
}

let private createSubmission connString examId submission = async {
    let key = string examId
    do! getConn connString
        |> Cosmos.update<ExamReadModel> key key (fun exam -> { exam with Submissions = exam.Submissions |> Array.append [| submission |] })
        |> Cosmos.execAsync
        |> AsyncSeq.iter ignore
}

let private createQuestion connString examId questionId = async {
    let key = string examId
    do! getConn connString
        |> Cosmos.update<ExamReadModel> key key (fun exam -> { exam with Questions = exam.Questions |> Array.append [| questionId |] })
        |> Cosmos.execAsync
        |> AsyncSeq.iter ignore
}

let private deleteQuestion connString examId questionId = async {
    let key = string examId
    do! getConn connString
        |> Cosmos.update key key (fun (exam: ExamReadModel) -> { exam with Questions = exam.Questions |> Array.filter (fun questionId' -> questionId' <> questionId)})
        |> Cosmos.execAsync
        |> AsyncSeq.iter ignore
}

let private changeTitle connString examId title = async {
    let key = string examId
    do! getConn connString
        |> Cosmos.update key key (fun (exam: ExamReadModel) -> { exam with Title = title })
        |> Cosmos.execAsync
        |> AsyncSeq.iter ignore
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
