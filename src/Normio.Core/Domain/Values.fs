namespace Normio.Core.Domain

open System
open System.Net.Mail
open System.Text.Json.Serialization

[<AutoOpen>]
module Values =
    [<AutoOpen>]
    module Strings =
        type ExamTitle40 = private ExamTitle40 of string
            with
                member this.Value = this |> fun (ExamTitle40 title) -> title

                static member Create title =
                    let len = title |> String.length
                    if len > 40 then
                        StringTooLong "Exam Title should be less than or equal to 40"
                        |> Error
                    else if len = 0 || isNull title then
                        EmptyString "Exam Title should not be empty"
                        |> Error
                    else
                        ExamTitle40 title |> Ok

        type UserName40 = private UserName40 of string
            
            with
                member this.Value = this |> fun (UserName40 name) -> name 
                static member Create name =
                    let len = name |> String.length
                    if len > 40 then
                        StringTooLong "User Name should be less than or equal to 40"
                        |> Error
                    else if len <= 0 || isNull name then
                        EmptyString "User Name should not be empty"
                        |> Error
                    else
                        UserName40 name |> Ok

        /// Reference: https://docs.microsoft.com/ko-kr/dotnet/api/system.net.mail.mailaddress.-ctor?view=net-5.0
        type UserEmail = private UserEmail of MailAddress

            with
                member this.Value = this |> fun (UserEmail email) -> email
                static member Create email =
                    try
                        if isNull email || email |> String.length = 0
                        then EmptyString "Email should not be empty" |> Error
                        else System.Net.Mail.MailAddress(email) |> Ok
                    with
                    | :? FormatException as exp -> WrongFormat "The string is not in a right email format" |> Error

        type UserPhoneNumber = private UserPhoneNumber of string
        
            with
                member this.Value = this |> fun (UserPhoneNumber email) -> email
                
                // TODO: how to validate a phone number(Maybe regex?)
                // Domain Knowledge problem:
                // 1. Korea phone number is different from phone number of other countries 
                static member Create number =
                    UserPhoneNumber number |> Ok
                    
                

        type MessageContent = private MessageContent of string

            with
                static member Create content =
                    let len = content |> String.length
                    if len = 0 || isNull content then
                        EmptyString "Message Content should not be empty"
                        |> Error
                    else
                        MessageContent content |> Ok
