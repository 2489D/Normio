module Normio.Web.Dev.Serialization

open System
open System.Text.Json
open System.Text.Json.Serialization
open Giraffe.Serialization

type SystemTextJsonSerializer(options: JsonSerializerOptions) =
    interface IJsonSerializer with
        member _.Deserialize<'T>(string: string) = JsonSerializer.Deserialize<'T>(string, options)
        member _.Deserialize<'T>(bytes: byte[]) = JsonSerializer.Deserialize<'T>(ReadOnlySpan bytes, options)
        member _.DeserializeAsync<'T>(stream) = JsonSerializer.DeserializeAsync<'T>(stream, options).AsTask()
        member _.SerializeToBytes<'T>(value: 'T) = JsonSerializer.SerializeToUtf8Bytes<'T>(value, options)
        member _.SerializeToStreamAsync<'T>(value: 'T) stream = JsonSerializer.SerializeAsync<'T>(stream, value, options)
        member _.SerializeToString<'T>(value: 'T) = JsonSerializer.Serialize<'T>(value, options)

let fsSerializationOption =
    let option = JsonSerializerOptions()
    let unionEncoding =
        JsonUnionEncoding.AdjacentTag
        ||| JsonUnionEncoding.NamedFields
        ||| JsonUnionEncoding.UnwrapOption
        ||| JsonUnionEncoding.UnwrapSingleCaseUnions
        ||| JsonUnionEncoding.UnwrapRecordCases
    let converter = JsonFSharpConverter(unionEncoding = unionEncoding)
    option.Converters.Add(converter)
    option

let serialize what = JsonSerializer.Serialize(what, fsSerializationOption)
