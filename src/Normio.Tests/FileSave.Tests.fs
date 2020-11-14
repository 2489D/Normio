﻿module Normio.Tests.FileSave_Tests

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
let root = """/Users/bonjune/2489D/Normio/src"""
let sourceFilePath = root + """/sample.txt"""
let extension = """.txt"""
(*
let root = """D:\root"""
let sourceFilePath = """D:\sample.txt"""
let extension = """.txt"""
*)

// if this flag is false, should delete created directories & files manually
// if this flag is true, root should differ with application's current working directory
let autoClean = true

[<Fact>]
let ``question should be saved``() =
    let fileSaver = inMemoryFileSaver root

    let examId = Guid.NewGuid()
    let questionId = Guid.NewGuid()

    use questionStream = File.OpenRead sourceFilePath

    let questionCtx = (examId, questionId)
    fileSaver.SaveQuestion questionCtx extension questionStream.CopyTo
    |> Async.RunSynchronously

    File.Exists (root +. examId +. "Questions" +. questionId + extension)
    |> should equal true

    if autoClean then Directory.Delete(root, true)

[<Fact>]
let ``submission should be saved``() =
    let fileSaver = inMemoryFileSaver root

    let examId = Guid.NewGuid()
    let studentId = Guid.NewGuid()
    let submissionId = Guid.NewGuid()

    use submissionStream = File.OpenRead sourceFilePath

    let submissionCtx = (examId, studentId, submissionId)
    fileSaver.SaveSubmission submissionCtx extension submissionStream.CopyTo
    |> Async.RunSynchronously

    File.Exists (root +. examId +. studentId +. submissionId + extension)
    |> should equal true

    if autoClean then Directory.Delete(root, true)

[<Fact>]
let ``length of question should equal with source``() =
    let streamLength (stream: FileStream) = stream.Length
    let sourceStreamLength =
        use sourceStream = File.OpenRead sourceFilePath
        streamLength sourceStream

    let fileSaver = inMemoryFileSaver root

    let examId = Guid.NewGuid()
    let questionId = Guid.NewGuid()

    use questionStream = File.OpenRead sourceFilePath

    let questionCtx = (examId, questionId)
    fileSaver.SaveQuestion questionCtx extension questionStream.CopyTo
    |> Async.RunSynchronously

    let fileGetter = inMemoryFileGetter root

    fileGetter.GetQuestion questionCtx extension streamLength
    |> Async.RunSynchronously
    |> should equal sourceStreamLength

    if autoClean then Directory.Delete(root, true)

[<Fact>]
let ``length of submission should equal with source``() =
    let streamLength (stream: FileStream) = stream.Length
    let sourceStreamLength =
        use sourceStream = File.OpenRead sourceFilePath
        streamLength sourceStream

    let fileSaver = inMemoryFileSaver root

    let examId = Guid.NewGuid()
    let studentId = Guid.NewGuid()
    let submissionId = Guid.NewGuid()

    use submissionStream = File.OpenRead sourceFilePath
    
    let submissionCtx = (examId, studentId, submissionId)
    fileSaver.SaveSubmission submissionCtx extension submissionStream.CopyTo
    |> Async.RunSynchronously

    let fileGetter = inMemoryFileGetter root

    fileGetter.GetSubmission submissionCtx extension streamLength
    |> Async.RunSynchronously
    |> should equal sourceStreamLength

    if autoClean then Directory.Delete(root, true)
