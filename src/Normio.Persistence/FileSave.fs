module Normio.Persistence.FileSave

open System
open System.IO

(*
    "path"
     └─(Exam id) folders
        ├─"Questions" folder
        │  └─(Question id) files
        └─"Submissions" folder
           └─(Submission id) files
*)

type IFileSaver =
    // what to do if the file already exist?
    // currently: just overlap
    abstract SaveQuestion: examId: Guid -> qId: Guid -> question: FileStream -> Async<unit>
    abstract SaveSubmission: examId: Guid -> sId: Guid -> submission: FileStream -> Async<unit>


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
            let qpath = path.Value + """\""" + eid.ToString() + """\Questions"""

            if Directory.Exists qpath |> not
            then Directory.CreateDirectory qpath |> ignore

            try
                use stream = File.Create (qpath + """\""" + qid.ToString())
                return! q.CopyToAsync stream |> Async.AwaitTask
            with
            | _ -> () // TODO
            // docs.microsoft.com/ko-kr/dotnet/api/system.io.file.create
            // docs.microsoft.com/ko-kr/dotnet/api/system.io.stream.copytoasync
        }

        member _.SaveSubmission eid sid s = async {
            let spath = path.Value + """\""" + eid.ToString() + """\Submissions"""

            if Directory.Exists spath |> not
            then Directory.CreateDirectory spath |> ignore

            try
                use stream = File.Create (spath + """\""" + sid.ToString())
                return! s.CopyToAsync stream |> Async.AwaitTask
            with
            | _ -> () // TODO
            // docs.microsoft.com/ko-kr/dotnet/api/system.io.file.create
            // docs.microsoft.com/ko-kr/dotnet/api/system.io.stream.copytoasync
        }

let createInMemoryFileSaver path = (InMemoryFileSaver path) :> IFileSaver
