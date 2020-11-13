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

let (+.) directory (other: obj) =
    match other with
    | :? Guid as id -> id.ToString()
    | :? string as s -> s
    | _ -> failwith "concatenating wrong type to directory"
    |> (+) (directory + """\""")

let private saveFile directory (filename: string) (sourceStream: FileStream) = async {
    if Directory.Exists directory |> not
    then Directory.CreateDirectory directory |> ignore

    use destStream = File.Create (directory +. filename)
    return! sourceStream.CopyToAsync destStream |> Async.AwaitTask
}

let private getFile filePath callback = async {
    use stream = File.OpenRead filePath
    return callback stream
}

let private saveQuestion root (examId: Guid) (questionId: Guid) (questionStream: FileStream) = async {
    let questionDirectory = root +. examId +. "Questions"
    return! saveFile questionDirectory (questionId.ToString()) questionStream
}

let private saveSubmission root (examId: Guid) (studentId: Guid) (submissionId: Guid) (submissionStream: FileStream) = async {
    let studentDirectory = root +. examId +. studentId
    return! saveFile studentDirectory (submissionId.ToString()) submissionStream
}

let private getQuestion root (examId: Guid) (questionId: Guid) callback = async {
    let questionFilePath = root +. examId +. "Questions" +. questionId
    return! getFile questionFilePath callback
}

let private getSubmission root (examId: Guid) (studentId: Guid) (submissionId: Guid) callback = async {
    let submissionFilePath = root +. examId +. studentId +. submissionId
    return! getFile submissionFilePath callback
}


// can throw exception
let inMemoryFileSaver root =
    if Directory.Exists root |> not
    then Directory.CreateDirectory root |> ignore
    { new IFileSaver with
        member _.SaveQuestion examId questionId questionStream
            = saveQuestion root examId questionId questionStream
        member _.SaveSubmission examId studentId submissionId submissionStream
            = saveSubmission root examId studentId submissionId submissionStream }

// can throw exception
let inMemoryFileGetter root =
    if Directory.Exists root |> not
    then failwith "Invalid root"
    { new IFileGetter with
        member _.GetQuestion examId questionId callback
            = getQuestion root examId questionId callback
        member _.GetSubmission examId studentId submissionId callback
            = getSubmission root examId studentId submissionId callback }
