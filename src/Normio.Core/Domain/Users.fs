namespace Normio.Core.Domain

open System

[<AutoOpen>]
module Users =
    [<CustomEquality; NoComparison>]
    type User =
        { Id: Guid
          Name: UserName40 }
        with
            member this.UpdateName name =
                { this with Name = name }

            override this.Equals(other) =
                match other with
                | :? User as user -> this.Id = user.Id
                | _ -> false
            
            override this.GetHashCode () = hash this.Id

    /// Wow! This type also check equality by the custom equality defined in the `User` type
    type AuthorisedUser =
        | SysAdmin of User
        | ExamAdmin of User
        | ExamManager of User
        | ExamParticipant of User
        
        with
            member this.Id =
                match this with
                | SysAdmin user -> user.Id
                | ExamAdmin user -> user.Id
                | ExamManager user -> user.Id
                | ExamParticipant user -> user.Id

            member this.Name =
                match this with
                | SysAdmin user -> user.Name
                | ExamAdmin user -> user.Name
                | ExamManager user -> user.Name
                | ExamParticipant user -> user.Name
