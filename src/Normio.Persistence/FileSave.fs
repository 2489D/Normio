module Normio.Persistence.FileSave

open System
open System.IO

(*
    "path"
     └─(Exam id) folders
       ├─"Questions" folder
       │  └─(Question id) files
       └─(Student id) folders
          └─(Submission id) files
*)

// what to do if the file already exist?
// currently: just overlap
type IFileSaver =
    abstract SaveQuestion:
        examId: Guid
        -> questionId: Guid
        -> questionStream: FileStream
        -> Async<unit>

    abstract SaveSubmission:
        examId: Guid
        -> studentId: Guid
        -> submissionId: Guid
        -> submissionStream: FileStream
        -> Async<unit>

    // both two can throw exception

type IFileGetter =
    abstract GetQuestion: examId: Guid -> questionId: Guid -> Async<FileStream>
    abstract GetSubmission: examId: Guid -> studentId: Guid -> submissionId: Guid -> Async<FileStream>


// FIXME : indentation (readability)
let private saveQuestion path
    (examId: Guid)
    (questionId: Guid)
    (questionStream: FileStream) = async {
    let questionPath = path + """\""" + examId.ToString() + """\Questions"""

    if Directory.Exists questionPath |> not
    then Directory.CreateDirectory questionPath |> ignore

    use stream = File.Create (questionPath + """\""" + questionId.ToString())
    return! questionStream.CopyToAsync stream |> Async.AwaitTask
}

let private saveSubmission path
    (examId: Guid)
    (studentId: Guid)
    (submissionId: Guid)
    (submissionStream: FileStream) = async {
    let studentPath = path + """\""" + examId.ToString() + """\""" + studentId.ToString()

    if Directory.Exists studentPath |> not
    then Directory.CreateDirectory studentPath |> ignore

    use stream = File.Create (studentPath + """\""" + submissionId.ToString())
    return! submissionStream.CopyToAsync stream |> Async.AwaitTask
}


let inMemoryFileSaver path =
    { new IFileSaver with
        member _.SaveQuestion examId questionId questionStream
            = saveQuestion path examId questionId questionStream
        member _.SaveSubmission examId studentId submissionId submissionStream
            = saveSubmission path examId studentId submissionId submissionStream }
