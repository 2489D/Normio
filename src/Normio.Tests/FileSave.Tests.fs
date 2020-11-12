module Normio.Tests.FileSave_Tests

open System
open Xunit
open Xunit.Sdk
open FsUnit
open FsUnit.Xunit

open Normio.Persistence.FileSave

/// File Saver Functionality
/// 1. path -> directory validation
/// 2. exam id, question id, question -> save question -> saves question
/// 3. exam id, submission id, submission -> save submission -> saves submission

[<Fact>]
[<Trait("Category", "FileSave")>]
let ``file saver should be created``() =
    try
        createInMemoryFileSaver """\aejofnwl"""
    with
    | _ -> failwith "This should not fail"
