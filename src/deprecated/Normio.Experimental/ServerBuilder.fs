module Normio.ExperimentalServerBuilder

open System

type Server<'T, 'Error> = Result<'T, 'Error>

type ServerBuilder() =
    member __.Bind(m, f) = Result.bind f m
