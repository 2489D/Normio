module Normio.Persistence.FileSave

open System
open System.IO

(*
    "path"
     └─(Exam id) folders
        ├─"Submission" folder
        │  └─(Submission id) files
        └─"Question" folder
           └─(Question id) files
*)

type IFileSaver =
    // what to do if the file already exist?
    // currently: just overlap
    abstract SaveQuestion: examId: Guid -> qId: Guid -> question: FileStream -> Async<unit>
    abstract SaveSubmission: examId: Guid -> sId: Guid -> submission: FileStream -> Async<unit>
    // what should return in get?


type ValidDirectory = private ValidDirectory of string
    with
        member this.Value = this |> fun (ValidDirectory path) -> path

module ValidDirectory =
    let create path =
        try
            Directory.CreateDirectory path |> ignore
            path |> Some
        with
        | _ -> None // TODO
        // docs.microsoft.com/en-us/dotnet/api/system.io.directory.createdirectory


type private InMemoryFileSaver(path: ValidDirectory) =
    interface IFileSaver with
        member _.SaveQuestion eid qid q = async {
            try
                use stream = File.Create (qpath + id.ToString())
                return! q.CopyToAsync stream |> Async.AwaitTask
            with
            | _ -> () // TODO
            // docs.microsoft.com/ko-kr/dotnet/api/system.io.file.create
            // docs.microsoft.com/ko-kr/dotnet/api/system.io.stream.copytoasync
        }

        member _.SaveSubmission id s = async {
            try
                use stream = File.Create (spath + id.ToString())
                return! s.CopyToAsync stream |> Async.AwaitTask
            with
            | _ -> () // TODO
        }

let createInMemoryFileSaver path = (InMemoryFileSaver path) :> IFileSaver
