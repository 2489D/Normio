module Normio.Web.Dev.Configurations

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
open Normio.Web.Dev.Serialization
open Normio.Web.Dev.ErrorHandler

let configureCors (builder : CorsPolicyBuilder) =
    builder.WithOrigins("http://localhost:3000")
           .WithOrigins("http://localhost:8080")
           .AllowAnyMethod()
           .AllowAnyHeader()
           .AllowCredentials()
           |> ignore

let configureApp webApp (app : IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IWebHostEnvironment>()
    (match env.EnvironmentName with
    | "Development" -> app.UseDeveloperExceptionPage()
    | _ -> app.UseGiraffeErrorHandler(errorHandler)
    ).UseHttpsRedirection()
        .UseCors(configureCors)
        .UseCors("ClientPermission")
        .UseStaticFiles()
        .UseRouting()
        .UseEndpoints(fun endpoints ->
            endpoints.MapHub<EventHub>("/signal")
            |> ignore)
        .UseGiraffe(webApp)

let configureServices (services : IServiceCollection) =
    services.AddSignalR()
        .AddJsonProtocol(fun options ->
            options.PayloadSerializerOptions.Converters.Add(JsonFSharpConverter())) |> ignore
    services.AddCors(fun options ->
        options.AddPolicy("ClientPermission", fun policy ->
            policy.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .WithOrigins("http://localhost:3000")
                |> ignore
            )
        ) |> ignore
    services.AddSingleton<NormioEventWorker>() |> ignore
    services.AddGiraffe() |> ignore
    services.AddSingleton<IJsonSerializer>(SystemTextJsonSerializer(fsSerializationOption)) |> ignore

let configureLogging (builder : ILoggingBuilder) =
    builder.AddFilter(fun l -> l.Equals LogLevel.Debug)
           .AddConsole()
           .AddDebug() |> ignore
