module Normio.Core.Errors

type CommandError =
    | CannotOpenExam of context:string
    | CannotStartExam of context:string
    | CannotEndExam of context:string
    | CannotCloseExam of context:string
    | CannotAddStudent of context:string
    | CannotRemoveStudent of context:string
    | CannotAddHost of context:string
    | CannotRemoveHost of context:string
    | CannotCreateQuestion of context:string
    | CannotDeleteQuestion of context:string
    | CannotChangeTitle of context:string

module CommandError =
    let toString = function
        | CannotOpenExam ctx -> sprintf "Cannot open exam: %A" ctx
        | CannotStartExam ctx -> sprintf "Cannot start exam: %A" ctx
        | CannotEndExam ctx -> sprintf "Cannot end exam: %A" ctx
        | CannotCloseExam ctx -> sprintf "Cannot close exam: %A" ctx
        | CannotAddStudent ctx -> sprintf "Cannot add student: %A" ctx
        | CannotRemoveStudent ctx -> sprintf "Cannot remove student: %A" ctx
        | CannotAddHost ctx -> sprintf "Cannot add host: %A" ctx
        | CannotRemoveHost ctx -> sprintf "Cannot remove host: %A" ctx
        | CannotCreateQuestion ctx -> sprintf "Cannot create question: %A" ctx
        | CannotDeleteQuestion ctx -> sprintf "Cannot delete question: %A" ctx
        | CannotChangeTitle ctx -> sprintf "Cannot change title: %A" ctx
