# Normio

Online Exam/Quiz Management API provider


# Architecture

```
User Client
(ClientReq/ClientRes)
Persistence Layer(PL)
((Aggregate, Command), Event list))
Function layer
```

## User


1. Host: Online exam managers
    - Do we need hierachy for hosts? (e.g. master host and other hosts)
2. Students: students enrolled for the online exam


## Persistence Layer


1. Provides ReadModels for each user client
    - A read model defines what kinds of data should be visible to the user
2. Persistence Aggregate


## Function Layer (oldState -> newState)

1. Progress


# Server Architecture

## Initial Setup
Server initially creates `POOL_SIZE=100` rooms wrapped by `MailboxProcessor` built in `F#`.

## RoomPool
The server maintains `RoomPool` which is an instance passing messages down to `Room` instances.
`RoomPool` maintains a list of `RoomId`s to identify `Room`s.

## Message Passing

