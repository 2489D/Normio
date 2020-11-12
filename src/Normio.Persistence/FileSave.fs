module Normio.Persistence.FileSave

open System
open System.IO

type IFileSaver =
    // what to do if the file already exist?
    // currently: just overlap
    abstract SaveQuestion: qId: Guid -> question: FileStream -> Async<unit>
    abstract SaveSubmission: sId: Guid -> submission: FileStream -> Async<unit>
    // what should return in get?


type ValidDirectory = private ValidDirectory of string
    with
        member this.Value = this |> fun (ValidDirectory path) -> path

module ValidDirectory =
    let create path =
        try
            Directory.CreateDirectory (path + "\Questions") |> ignore
            Directory.CreateDirectory (path + "\Submissions") |> ignore
            path |> Some
        with
        | _ -> None // TODO
        // docs.microsoft.com/en-us/dotnet/api/system.io.directory.createdirectory


type private InMemoryFileSaver(path: ValidDirectory) =
    let qpath = path.Value + "\Questions"
    let spath = path.Value + "\Submissions"

    interface IFileSaver with
        member _.SaveQuestion id q = async {
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
