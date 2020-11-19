namespace Normio.Commands.Api

module Compositions =
    let handleOpenExamRequest eventStore req =
        openExamCommander
        |> handleCommand eventStore req
        
    let handleStartExamRequest eventStore req =
        startExamCommander
        |> handleCommand eventStore req

    let handleEndExamRequest eventStore req =
        endExamCommander
        |> handleCommand eventStore req

    let handleCloseExamRequest eventStore req =
        closeExamCommander
        |> handleCommand eventStore req

    let handleAddStudentRequest eventStore req =
        addStudentCommander
        |> handleCommand eventStore req
        
    let handleRemoveStudentRequest eventStore req =
        removeStudentCommander
        |> handleCommand eventStore req
    
    let handleLetStudentInRequest eventStore req =
        letStudentInCommander
        |> handleCommand eventStore req

    let handleLetStudentOutRequest eventStore req =
        letStudentOutCommander
        |> handleCommand eventStore req
 
    let handleAddHostRequest eventStore req =
        addHostCommander
        |> handleCommand eventStore req

    let handleRemoveHostRequest eventStore req =
        removeHostCommander
        |> handleCommand eventStore req
    
    let handleLetHostInRequest eventStore req =
        letHostInCommander
        |> handleCommand eventStore req

    let handleLetHostOutRequest eventStore req =
        letHostOutCommander
        |> handleCommand eventStore req

    let handleCreateSubmissionRequest eventStore req =
        createSubmissionCommander
        |> handleCommand eventStore req

    let handleCreateQuestionRequest eventStore req =
        createQuestionCommander
        |> handleCommand eventStore req

    let handleDeleteQuestionRequest eventStore req =
        deleteQuestionCommander
        |> handleCommand eventStore req
    
    let handleSendMessageRequest eventStore req =
        sendMessageCommander
        |> handleCommand eventStore req

    let handleChangeTitleRequest eventStore req =
        changeTitleCommander
        |> handleCommand eventStore req
