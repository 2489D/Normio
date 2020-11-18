namespace Normio.Timer

open System
open Normio.Core.Commands

[<AutoOpen>]
module Domain =
    [<CustomEquality; CustomComparison>]
    type TimerData = {
        Command: Command
        Time: DateTime
    } with
        override this.Equals(other) =
            match other with
            | :? TimerData as td -> this.Time = td.Time || this.Command = td.Command
            | _ -> false
        override this.GetHashCode() = hash this
        interface IComparable with
            override this.CompareTo(other) =
                this.Time.CompareTo((other :?> TimerData).Time)
