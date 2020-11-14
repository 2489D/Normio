module Normio.Persistence.FileSave

open System
open System.IO

// if the file already exist, overlap it
// file extension should include '.'
type IFileSaver =
    abstract SaveQuestion: (Guid * Guid) -> extension: string -> callback:(FileStream -> 'a) -> Async<'a>
    abstract SaveSubmission: (Guid * Guid * Guid) -> extension: string -> callback:(FileStream -> 'a) -> Async<'a>
    // both two can throw exception

(*
    3 way to design IFileGetter functions

    1. get callback function in input (current way)
    2. return async<FileStream>
    3. return Task<byte[]> (return value from File.ReadAllBytesAsync)
*)
type IFileGetter =
    abstract GetQuestion: (Guid * Guid) -> extension: string -> callback: (FileStream -> 'a) -> Async<'a>
    abstract GetSubmission: (Guid * Guid * Guid) -> extension: string -> callback: (FileStream -> 'a) -> Async<'a>
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

// path combine operator
let (+.) directory (other: obj) =
    let otherDir =
        match other with
        | :? Guid as id -> id.ToString()
        | :? string as s -> s
        | _ -> failwith "combining wrong type to directory"
    Path.Combine(directory, otherDir)

let private saveFile directory filename callback = async {
    if Directory.Exists directory |> not
    then Directory.CreateDirectory directory |> ignore

    use destStream = File.Create (directory +. filename)
    return callback destStream
}

let private getFile filePath callback = async {
    use stream = File.OpenRead filePath
    return callback stream
}

let private saveQuestion root ((examId, questionId): Guid * Guid) extension callback = async {
    let questionDirectory = root +. examId +. "Questions"
    return! saveFile questionDirectory (questionId.ToString() + extension) callback
}

let private saveSubmission root ((examId, studentId, submissionId): Guid * Guid * Guid) extension callback = async {
    let studentDirectory = root +. examId +. studentId
    return! saveFile studentDirectory (submissionId.ToString() + extension) callback
}

let private getQuestion root ((examId, questionId): Guid * Guid) extension callback = async {
    let questionFilePath = root +. examId +. "Questions" +. questionId + extension
    return! getFile questionFilePath callback
}

let private getSubmission root ((examId, studentId, submissionId): Guid * Guid * Guid) extension callback = async {
    let submissionFilePath = root +. examId +. studentId +. submissionId + extension
    return! getFile submissionFilePath callback
}


// can throw exception
let inMemoryFileSaver root =
    if Directory.Exists root |> not
    then Directory.CreateDirectory root |> ignore
    { new IFileSaver with
        member _.SaveQuestion ctx extension callback = saveQuestion root ctx extension callback
        member _.SaveSubmission ctx extension callback = saveSubmission root ctx extension callback }

// can throw exception
let inMemoryFileGetter root =
    if Directory.Exists root |> not
    then failwith "Invalid root"
    { new IFileGetter with
        member _.GetQuestion ctx extension callback = getQuestion root ctx extension callback
        member _.GetSubmission ctx extension callback = getSubmission root ctx extension callback }
