namespace Normio.Timer

open System

[<AutoOpen>]
module Domain =
    [<CustomEquality; CustomComparison>]
    type TimerData = {
        Id: Guid
        Time: DateTime
        Task: Async<unit>
    } with
        override this.Equals(other) =
            match other with
            | :? TimerData as td -> this.Id = td.Id
            | _ -> false
        override this.GetHashCode() = hash this.Id
        interface IComparable with
            override this.CompareTo(other) =
                this.Time.CompareTo((other :?> TimerData).Time)
