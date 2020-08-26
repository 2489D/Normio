# Normio.Clock Test Client

## Usage

``` bash
dotnet run --duration {seconds}
```

This command will make the client to send request of
``` F#
{
    RoomId = // Automatically generated Guid
    ExamStart = Now
    ExamEnd = Now + 5 Seconds
}
```

## Example

``` bash
dotnet run --duration 5
```
