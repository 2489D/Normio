module Normio.Web.Dev.Configurations

open System
open System.Text.Json
open System.Text.Json.Serialization
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection

open Giraffe
open Giraffe.Serialization

open Normio.Web.Dev.Hub
open Normio.Web.Dev.ErrorHandler

/// reference : https://github.com/Tarmil/FSharp.SystemTextJson/blob/master/docs/Using.md#using-with-giraffe
type SystemTextJsonSerializer(options: JsonSerializerOptions) =
    interface IJsonSerializer with
        member _.Deserialize<'T>(string: string) = JsonSerializer.Deserialize<'T>(string, options)
        member _.Deserialize<'T>(bytes: byte[]) = JsonSerializer.Deserialize<'T>(ReadOnlySpan bytes, options)
        member _.DeserializeAsync<'T>(stream) = JsonSerializer.DeserializeAsync<'T>(stream, options).AsTask()
        member _.SerializeToBytes<'T>(value: 'T) = JsonSerializer.SerializeToUtf8Bytes<'T>(value, options)
        member _.SerializeToStreamAsync<'T>(value: 'T) stream = JsonSerializer.SerializeAsync<'T>(stream, value, options)
        member _.SerializeToString<'T>(value: 'T) = JsonSerializer.Serialize<'T>(value, options)

let configureCors (builder : CorsPolicyBuilder) =
    builder.WithOrigins("http://localhost:8080")
           .AllowAnyMethod()
           .AllowAnyHeader()
           |> ignore

let configureApp webApp (app : IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IWebHostEnvironment>()
    (match env.EnvironmentName with
    | "Development" ->
        app.UseDeveloperExceptionPage()
    | _ -> app.UseGiraffeErrorHandler(errorHandler))
        .UseHttpsRedirection()
        .UseCors(configureCors)
        .UseStaticFiles()
        .UseRouting()
        .UseEndpoints(fun endpoints ->
            endpoints.MapHub<EventHub>("/signal")
            |> ignore)
        .UseGiraffe(webApp)

let configureServices (services : IServiceCollection) =
    services.AddCors()    |> ignore
    services.AddSignalR()
        .AddJsonProtocol(fun options ->
            options.PayloadSerializerOptions.Converters.Add(JsonFSharpConverter()))
    |> ignore
    
    // TODO
    services.AddSingleton<NormioEventWorker>() |> ignore
    services.AddGiraffe() |> ignore
    let jsonOptions = JsonSerializerOptions()
    jsonOptions.Converters.Add(JsonFSharpConverter())
    services.AddSingleton(jsonOptions) |> ignore
    services.AddSingleton<IJsonSerializer, SystemTextJsonSerializer>() |> ignore


let configureLogging (builder : ILoggingBuilder) =
    builder.AddFilter(fun l -> l.Equals LogLevel.Debug)
           .AddConsole()
           .AddDebug() |> ignore
