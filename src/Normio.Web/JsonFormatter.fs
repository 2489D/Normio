module Normio.Web.JsonFormatter

open System
open Normio.Core.Domain

open Newtonsoft.Json.Linq

let (.=) key (value: obj) = JProperty(key, value)

let jobj jProperties =
    let jObject = JObject()
    jProperties |> List.iter jObject.Add
    jObject

let jArray jObjects =
    let jArray = JArray()
    jObjects |> List.iter jArray.Add
    jArray

let studentJObj (student: Student) =
    jobj [
        "id" .= student.Id
        "name" .= student.Name
    ]

let mapJArray toJObj (map: Map<Guid, _>) =
    map
    |> Map.toList
    |> List.map (fun (_, object) -> object |> toJObj)
    |> jArray


let hostJObj (host: Host) =
    jobj [
        "id" .= host.Id
        "name" .= host.Name
    ]

let examJObj exam =
    jobj [
        "title" .= exam.Title
        "examId" .= exam.Id
        "students" .= (exam.Students |> mapJArray studentJObj)
        "hosts" .= (exam.Hosts |> mapJArray hostJObj)
        "questions" .= (exam.Questions)
    ]