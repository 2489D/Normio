module Normio.Persistence.FileSave

open System
open System.IO

// what to do if the file already exist?
// currently: just overlap
type IFileSaver =
    abstract SaveQuestion: examId: Guid -> questionId: Guid -> questionStream: FileStream -> Async<unit>
    abstract SaveSubmission: examId: Guid -> studentId: Guid -> submissionId: Guid -> submissionStream: FileStream -> Async<unit>
    // both two can throw exception

(*
    3 way to design IFileGetter functions

    1. get callback function in input (current way)
    2. return async<FileStream>
    3. return Task<byte[]> (return value from File.ReadAllBytesAsync)
*)
type IFileGetter =
    abstract GetQuestion: examId: Guid -> questionId: Guid -> callback: (FileStream -> 'a) -> Async<'a>
    abstract GetSubmission: examId: Guid -> studentId: Guid -> submissionId: Guid -> callback: (FileStream -> 'a) -> Async<'a>
    // both two can throw exception


// InMemory directory design
(*
    "root"
     └─(Exam id) folders
       ├─"Questions" folder
       │  └─(Question id) files
       └─(Student id) folders
          └─(Submission id) files
*)

let private saveFile directory filename (sourceStream: FileStream) = async {
    if Directory.Exists directory |> not
    then Directory.CreateDirectory directory |> ignore

    use destStream = File.Create (directory + """\""" + filename)
    return! sourceStream.CopyToAsync destStream |> Async.AwaitTask
}

let private getFile filePath callback = async {
    use stream = File.OpenRead filePath
    return callback stream
}

let private saveQuestion root (examId: Guid) (questionId: Guid) (questionStream: FileStream) = async {
    let questionDirectory = root + """\""" + examId.ToString()
                                 + """\Questions"""
    return! saveFile questionDirectory (questionId.ToString()) questionStream
}

let private saveSubmission root (examId: Guid) (studentId: Guid) (submissionId: Guid) (submissionStream: FileStream) = async {
    let studentDirectory = root + """\""" + examId.ToString()
                                + """\""" + studentId.ToString()
    return! saveFile studentDirectory (submissionId.ToString()) submissionStream
}

let private getQuestion root (examId: Guid) (questionId: Guid) callback = async {
    let questionFilePath = root + """\""" + examId.ToString()
                                + """\Questions"""
                                + """\""" + questionId.ToString()
    return! getFile questionFilePath callback
}

let private getSubmission root (examId: Guid) (studentId: Guid) (submissionId: Guid) callback = async {
    let submissionFilePath = root + """\""" + examId.ToString()
                                  + """\""" + studentId.ToString()
                                  + """\""" + submissionId.ToString()
    return! getFile submissionFilePath callback
}


let inMemoryFileSaver root =
    { new IFileSaver with
        member _.SaveQuestion examId questionId questionStream
            = saveQuestion root examId questionId questionStream
        member _.SaveSubmission examId studentId submissionId submissionStream
            = saveSubmission root examId studentId submissionId submissionStream }

let inMemoryFileGetter root =
    { new IFileGetter with
        member _.GetQuestion examId questionId callback
            = getQuestion root examId questionId callback
        member _.GetSubmission examId studentId submissionId callback
            = getSubmission root examId studentId submissionId callback }
