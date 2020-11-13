module Normio.Persistence.FileSave

open System
open System.IO

// what to do if the file already exist?
// currently: just overlap
type IFileSaver =
    abstract SaveQuestion: (Guid * Guid) -> callback:(FileStream -> 'a) -> Async<'a>
    abstract SaveSubmission: (Guid * Guid * Guid) -> callback:(FileStream -> 'a) -> Async<'a>
    // both two can throw exception

(*
    3 way to design IFileGetter functions

    1. get callback function in input (current way)
    2. return async<FileStream>
    3. return Task<byte[]> (return value from File.ReadAllBytesAsync)
*)
type IFileGetter =
    abstract GetQuestion: (Guid * Guid) -> callback: (FileStream -> 'a) -> Async<'a>
    abstract GetSubmission: (Guid * Guid * Guid) -> callback: (FileStream -> 'a) -> Async<'a>
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
    let otherDir =
        match other with
        | :? Guid as id -> id.ToString()
        | :? string as s -> s
        | _ -> failwith "concatenating wrong type to directory"
    Path.Combine(directory, otherDir)

let private saveFile directory (filename: string) callback = async {
    if Directory.Exists directory |> not
    then Directory.CreateDirectory directory |> ignore

    use destStream = File.Create (directory +. filename)
    return callback destStream
}

let private getFile filePath callback = async {
    use stream = File.OpenRead filePath
    return callback stream
}

let private saveQuestion root ((examId, questionId): Guid * Guid) callback = async {
    let questionDirectory = root +. examId +. "Questions"
    return! saveFile questionDirectory (questionId.ToString()) callback
}

let private saveSubmission root ((examId, studentId, submissionId): Guid * Guid * Guid) callback = async {
    let studentDirectory = root +. examId +. studentId
    return! saveFile studentDirectory (submissionId.ToString()) callback
}

let private getQuestion root ((examId, questionId): Guid * Guid) callback = async {
    let questionFilePath = root +. examId +. "Questions" +. questionId
    return! getFile questionFilePath callback
}

let private getSubmission root ((examId, studentId, submissionId): Guid * Guid * Guid) callback = async {
    let submissionFilePath = root +. examId +. studentId +. submissionId
    return! getFile submissionFilePath callback
}


// can throw exception
let inMemoryFileSaver root =
    if Directory.Exists root |> not
    then Directory.CreateDirectory root |> ignore
    { new IFileSaver with
        member _.SaveQuestion ctx callback = saveQuestion root ctx callback
        member _.SaveSubmission ctx callback = saveSubmission root ctx callback }

// can throw exception
let inMemoryFileGetter root =
    if Directory.Exists root |> not
    then failwith "Invalid root"
    { new IFileGetter with
        member _.GetQuestion ctx callback
            = getQuestion root ctx callback
        member _.GetSubmission ctx callback
            = getSubmission root ctx callback }
