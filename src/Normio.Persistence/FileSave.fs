module Normio.Persistence.FileSave

open System
open System.IO

// if two or more user try to access the same file, can throw exception

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
    abstract GetQuestion: (Guid * Guid) -> callback: (FileStream -> 'a) -> ifQuestionNotFound: (Guid * Guid -> 'a) -> Async<'a>
    abstract GetSubmission: (Guid * Guid * Guid) -> callback: (FileStream -> 'a) -> ifSubmissionNotFound: (Guid * Guid * Guid -> 'a) -> Async<'a>
    // both two can throw exception

type IFileDeleter =
    abstract DeleteQuestion: (Guid * Guid) -> Async<bool>
    // can throw exception

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
let (+.) directory subpath = Path.Combine(directory, subpath.ToString())

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

let private getFileNameByWithoutExtension directory filenameWithoutExtension =
    if Directory.Exists directory
    then Directory.GetFiles(directory, filenameWithoutExtension.ToString() + ".*")
    else [||]

let private saveQuestion root ((examId, questionId): Guid * Guid) extension callback = async {
    let questionDirectory = root +. examId +. "Questions"
    return! saveFile questionDirectory (questionId.ToString() + extension) callback
}

let private saveSubmission root ((examId, studentId, submissionId): Guid * Guid * Guid) extension callback = async {
    let studentDirectory = root +. examId +. studentId
    return! saveFile studentDirectory (submissionId.ToString() + extension) callback
}

let private getQuestion root ((examId, questionId): Guid * Guid) callback ifNotFound = async {
    let questionDirectory = root +. examId +. "Questions"
    let questionFileNameArray = getFileNameByWithoutExtension questionDirectory questionId
    if questionFileNameArray.Length >= 2
    then failwithf "duplicated question at %s" (questionDirectory +. questionId) // this must not happen
    match questionFileNameArray with
    | [|questionFileName|] -> return! getFile (questionDirectory +. questionFileName) callback
    | _ -> return ifNotFound (examId, questionId)
}

let private getSubmission root ((examId, studentId, submissionId): Guid * Guid * Guid) callback ifNotFound = async {
    let submissionDirectory = root +. examId +. studentId
    let submissionFileNameArray = getFileNameByWithoutExtension submissionDirectory submissionId
    if submissionFileNameArray.Length >= 2
    then failwithf "duplicated submission at %s" (submissionDirectory +. submissionId) // this must not happen
    match submissionFileNameArray with
    | [|submissionFileName|] -> return! getFile (submissionDirectory +. submissionFileName) callback
    | _ -> return ifNotFound (examId, studentId, submissionId)
}

let private deleteQuestion root ((examId, questionId): Guid * Guid) = async {
    let questionDirectory = root +. examId +. "Questions"
    let questionFileNameArray = getFileNameByWithoutExtension questionDirectory questionId
    if questionFileNameArray.Length >= 2
    then failwithf "duplicated question at %s" (questionDirectory +. questionId) // this must not happen
    match questionFileNameArray with
    | [|questionFileName|] ->
        File.Delete (questionDirectory +. questionFileName)
        return true
    | _ -> return false
}


// can throw exception
let inMemoryFileSaver root =
    if Directory.Exists root |> not
    then Directory.CreateDirectory root |> ignore
    { new IFileSaver with
        member _.SaveQuestion ids extension callback = saveQuestion root ids extension callback
        member _.SaveSubmission ids extension callback = saveSubmission root ids extension callback }

// inMemoryFileSaver should be called earlier than below two

// can throw exception
let inMemoryFileGetter root =
    if Directory.Exists root |> not
    then failwithf "Root: %s does not exists" root
    { new IFileGetter with
        member _.GetQuestion ids callback ifQuestionNotFound = getQuestion root ids callback ifQuestionNotFound
        member _.GetSubmission ids callback ifSubmissionNotFound = getSubmission root ids callback ifSubmissionNotFound }

let inMemoryFileDeleter root =
    if Directory.Exists root |> not
    then failwithf "Root: %s does not exists" root
    { new IFileDeleter with
        member _.DeleteQuestion ids = deleteQuestion root ids }
