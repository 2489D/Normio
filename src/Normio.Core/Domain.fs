module Normio.Core.Domain

open System

type Student = {
    Id: Guid
    Name: string
}

type Host = {
    Id: Guid
    Name: string
}

type File = {
    Id: Guid
    Name: string
}

type Submission = {
    Id: Guid
    Exam: Guid
    Student: Student
    File: File
}

type Exam = {
    Id: Guid
    Title: string
    Questions: File list
    Students: Map<Guid, Student>
    Hosts: Map<Guid, Host>
}
