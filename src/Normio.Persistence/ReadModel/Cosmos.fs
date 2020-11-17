module Normio.Persistence.ReadModels.Cosmos

open System
open FSharp.Control
open FSharp.CosmosDb

open Normio.Core
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

let private openExam connString examId title startTime duration = async {
    let exam = ExamReadModel.Initial examId title startTime duration
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
        |> Cosmos.update key key (fun (exam: ExamReadModel) -> { exam with Students = seq { yield student; yield! exam.Students }})
        |> Cosmos.execAsync
        |> AsyncSeq.iter ignore
}

let private removeStudent connString examId studentId = async {
    let key = string examId
    do! getConn connString
        |> Cosmos.update key key (fun (exam: ExamReadModel) -> { exam with Students = exam.Students |> Seq.filter (fun student -> student.Id <> studentId) })
        |> Cosmos.execAsync
        |> AsyncSeq.iter ignore
}

let private addHost connString examId (host: Host) = async {
    let key = string examId
    do! getConn connString
        |> Cosmos.update key key (fun (exam: ExamReadModel) -> { exam with Hosts = seq { yield host; yield! exam.Hosts }})
        |> Cosmos.execAsync
        |> AsyncSeq.iter ignore
}

let private removeHost connString examId hostId = async {
    let key = string examId
    do! getConn connString
        |> Cosmos.update key key (fun (exam: ExamReadModel) -> { exam with Hosts = exam.Hosts |> Seq.filter (fun h -> h.Id <> hostId) })
        |> Cosmos.execAsync
        |> AsyncSeq.iter ignore
}

let private createSubmission connString examId submission = async {
    let key = string examId
    do! getConn connString
        |> Cosmos.update<ExamReadModel> key key (fun exam -> { exam with Submissions = seq { yield submission; yield! exam.Submissions }})
        |> Cosmos.execAsync
        |> AsyncSeq.iter ignore
}

let private createQuestion connString examId question = async {
    let key = string examId
    do! getConn connString
        |> Cosmos.update<ExamReadModel> key key (fun exam -> { exam with Questions = seq { yield question; yield! exam.Questions }})
        |> Cosmos.execAsync
        |> AsyncSeq.iter ignore
}

let private deleteQuestion connString examId questionId = async {
    let key = string examId
    do! getConn connString
        |> Cosmos.update key key (fun (exam: ExamReadModel) -> { exam with Questions = exam.Questions |> Seq.filter (fun question -> question.Id <> questionId)})
        |> Cosmos.execAsync
        |> AsyncSeq.iter ignore
}

let private sendMessage connString examId message =
    async {
        let key = string examId
        do! getConn connString
            |> Cosmos.update key key (fun (exam: ExamReadModel) -> { exam with Messages = seq { yield message; yield! exam.Messages }})
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
        member this.OpenExam examId title startTime duration = openExam connString examId title startTime duration
        member this.StartExam examId = startExam connString examId
        member this.EndExam examId = endExam connString examId
        member this.CloseExam examId = closeExam connString examId
        member this.AddStudent examId student = addStudent connString examId student
        member this.RemoveStudent examId studentId = removeStudent connString examId studentId
        member this.AddHost examId host = addHost connString examId host
        member this.RemoveHost examId hostId = removeHost connString examId hostId
        member this.CreateSubmission examId submission = createSubmission connString examId submission
        member this.CreateQuestion examId questionId = createQuestion connString examId questionId
        member this.DeleteQuestion examId questionId = deleteQuestion connString examId questionId
        member this.SendMessage examId message = sendMessage connString examId message
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
