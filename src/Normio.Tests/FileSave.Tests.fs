module Normio.Tests.FileSave_Tests

open System
open System.IO
open Xunit
open FsUnit.Xunit

open Normio.Persistence.FileSave

/// File Saver Functionality
/// 1. root -> inMemoryFileSaver -> create IFileSaver object
/// 2. exam id, question id, question file stream -> save question -> saves question
/// 3. exam id, student id, submission id, submission file stream -> save submission -> saves submission

/// File Getter Functionality
/// 1. root -> inMemoryFileGetter -> create IFileGetter object
/// 2. exam id, question id, callback function -> get question -> gets question and calls callback fucntion
/// 3. exam id, student id, submission id, callback function -> get submission -> gets question and calls callback fucntion

// these should be changed in other computers
let root = """D:"""
let sourceFilePath = """D:\sample.txt"""

// should delete created directories & files manually

[<Fact>]
let ``question should be saved``() =
    let fileSaver = inMemoryFileSaver root

    let examId = Guid.NewGuid()
    let questionId = Guid.NewGuid()

    use questionStream = File.OpenRead sourceFilePath

    fileSaver.SaveQuestion examId questionId questionStream
    |> Async.RunSynchronously

    File.Exists (root + """\""" + examId.ToString() + """\Questions\""" + questionId.ToString())
    |> should equal true

[<Fact>]
let ``submission should be saved``() =
    let fileSaver = inMemoryFileSaver root

    let examId = Guid.NewGuid()
    let studentId = Guid.NewGuid()
    let submissionId = Guid.NewGuid()

    use submissionStream = File.OpenRead sourceFilePath

    fileSaver.SaveSubmission examId studentId submissionId submissionStream
    |> Async.RunSynchronously

    File.Exists (root + """\""" + examId.ToString() + """\""" + studentId.ToString() + """\""" + submissionId.ToString())
    |> should equal true

[<Fact>]
let ``length of question should equal with source``() =
    let callback (stream: FileStream) = stream.Length
    let sourceCallbackResult =
        use sourceStream = File.OpenRead sourceFilePath
        callback sourceStream

    let fileSaver = inMemoryFileSaver root

    let examId = Guid.NewGuid()
    let questionId = Guid.NewGuid()

    use questionStream = File.OpenRead sourceFilePath

    fileSaver.SaveQuestion examId questionId questionStream
    |> Async.RunSynchronously

    let fileGetter = inMemoryFileGetter root

    fileGetter.GetQuestion examId questionId callback
    |> Async.RunSynchronously
    |> should equal sourceCallbackResult

[<Fact>]
let ``length of submission should equal with source``() =
    let callback (stream: FileStream) = stream.Length
    let sourceCallbackResult =
        use sourceStream = File.OpenRead sourceFilePath
        callback sourceStream

    let fileSaver = inMemoryFileSaver root

    let examId = Guid.NewGuid()
    let studentId = Guid.NewGuid()
    let submissionId = Guid.NewGuid()

    use submissionStream = File.OpenRead sourceFilePath

    fileSaver.SaveSubmission examId studentId submissionId submissionStream
    |> Async.RunSynchronously

    let fileGetter = inMemoryFileGetter root

    fileGetter.GetSubmission examId studentId submissionId callback
    |> Async.RunSynchronously
    |> should equal sourceCallbackResult
