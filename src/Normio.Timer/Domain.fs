namespace Normio.Timer.Domain

open System

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
            DateTime.Compare(this.Time, (other :?> TimerData).Time)
