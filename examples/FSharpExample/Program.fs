open System
open System.IO

open System.Threading.Tasks
open Microsoft.Extensions.Configuration

open Serilog
open Serilog.Events
open Serilog.Sinks.SpectreConsole
open Spectre.Console

type User = {
    Name: string
}

[<EntryPoint>]
let main argv =

    Log.Logger <-
        LoggerConfiguration()
            .WriteTo.SpectreConsole("{Timestamp:HH:mm:ss} [{Level:u4}] {Message:lj}{NewLine}{Exception}",  minLevel = LogEventLevel.Verbose)
            .MinimumLevel.Verbose()
            .CreateLogger()

    // or for configuration via appsettings.json

    //let configuration =
    //    ConfigurationBuilder()
    //        .SetBasePath(Directory.GetCurrentDirectory())
    //        .AddJsonFile("appsettings.json")
    //        .Build()

    //Log.Logger <-
    //    LoggerConfiguration()
    //        .ReadFrom.Configuration(configuration)
    //        .CreateLogger()

    // let status = AnsiConsole.Status()
    // status.Start("my status", fun ctx ->
    //     let user = { Name = "test_user" }
    //     Log.Information($"%A{user}")
    //     Log.Information("symbol {")
    //
    //     Log.Verbose("Verbose level example with {0}", "parameter")
    //     Log.Debug("Debug level example with {0}", "parameter")
    //     Log.Information("Information level example with {0}", "parameter")
    //     Log.Warning("Warning level example with {0}", "parameter")
    //
    //     Task.Delay(5000).Wait()
    // )

    let user = { Name = "test_user" }
    Log.Information($"%A{user}")
    Log.Information("symbol {")

    Log.Verbose("Verbose level example with {0}", "parameter")
    Log.Debug("Debug level example with {0}", "parameter")
    Log.Information("Information level example with {0}", "parameter")
    Log.Warning("Warning level example with {0}", "parameter")

    try
        raise (Exception "Message")
    with
    | ex -> Log.Error(ex, "Error level example with {0}", "parameter")

    Log.Fatal("Fatal level example with {0}", "parameter")

    Console.ReadKey() |> ignore

    0
