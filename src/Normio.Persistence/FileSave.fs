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
    // both two can throw exception


type private InMemoryFileSaver(path) =
    do Directory.CreateDirectory path |> ignore // can throw exception

    interface IFileSaver with
        member _.SaveQuestion eid qid q = async {
            let qpath = path + """\""" + eid.ToString() + """\Questions"""

            if Directory.Exists qpath |> not
            then Directory.CreateDirectory qpath |> ignore

            use stream = File.Create (qpath + """\""" + qid.ToString())
            return! q.CopyToAsync stream |> Async.AwaitTask
        }

        member _.SaveSubmission eid sid s = async {
            let spath = path + """\""" + eid.ToString() + """\Submissions"""

            if Directory.Exists spath |> not
            then Directory.CreateDirectory spath |> ignore

            use stream = File.Create (spath + """\""" + sid.ToString())
            return! s.CopyToAsync stream |> Async.AwaitTask
        }

let createInMemoryFileSaver path = (InMemoryFileSaver path) :> IFileSaver
