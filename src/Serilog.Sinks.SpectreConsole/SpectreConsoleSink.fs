namespace Serilog.Sinks.SpectreConsole

open System.IO
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

open Serilog.Configuration
open Serilog.Core
open Serilog.Events
open Serilog.Formatting
open Serilog.Parsing
open Spectre.Console

module internal LogEvent =

    [<Literal>]
    let DefaultConsoleOutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"

    let toString (textFormatter: ITextFormatter) (logEvent: LogEvent) =
        let writer = new StringWriter()
        textFormatter.Format(logEvent, writer)
        writer.Flush()
        writer.ToString()

type SpectreConsoleSink (outputTemplate: string) =

    let template = MessageTemplateParser().Parse(outputTemplate)
    let _renderers = template.Tokens |> Seq.map(SpectreRenderer.createRenderer template) |> Array.ofSeq

    interface ILogEventSink with

        member _.Emit(logEvent: LogEvent) =
            _renderers
            |> Seq.collect(fun renderer -> renderer logEvent)
            |> Seq.iter AnsiConsole.Write

[<Extension>]
type SpectreConsoleSinkExtensions() =

    [<CompiledName("SpectreConsole")>]
    [<Extension>]
    static member spectreConsole (loggerConfiguration: LoggerSinkConfiguration,
                                  [<Optional;DefaultParameterValue(LogEvent.DefaultConsoleOutputTemplate)>] outputTemplate: string,
                                  [<Optional;DefaultParameterValue(LogEventLevel.Information)>] minLevel: LogEventLevel) =

        let sink = SpectreConsoleSink(outputTemplate)
        loggerConfiguration.Sink(sink, minLevel)
