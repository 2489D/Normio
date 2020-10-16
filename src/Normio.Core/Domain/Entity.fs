namespace Normio.Core.Domain

open System

type Student = {
    Id: Guid
    Name: UserName40
}

type Host = {
    Id: Guid
    Name: UserName40
}

type File = {
    Id: Guid
    Name: FileString200
}

type Submission = {
    Id: Guid
    ExamId: Guid
    Student: Student
    File: File
}

type Exam = {
    Id: Guid
    Title: ExamTitle40
    Questions: File list
    Submissions: Submission list
    Students: Map<Guid, Student>
    Hosts: Map<Guid, Host>
}
