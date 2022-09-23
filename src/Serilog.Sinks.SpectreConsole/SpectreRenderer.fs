namespace Serilog.Sinks.SpectreConsole

open Serilog.Events
open Serilog.Formatting.Display
open Serilog.Parsing
open Spectre.Console.Rendering

module internal LogEventPropertyValue =

    let toString (token: PropertyToken) (value: LogEventPropertyValue) =
        value.ToString(token.Format, null)

module internal StructureValue =

    let toString (token: PropertyToken) (value: StructureValue) =
        value.ToString(token.Format, null)

module internal MessageTemplate =

    let containsPropName (propName: string) (template: MessageTemplate) =
        template.Tokens
        |> Seq.exists(fun token ->
            (token :? PropertyToken) && (token :?> PropertyToken).PropertyName = propName
        )

module internal SpectreRenderer =

    type RenderableCollection(items: IRenderable seq) =

        interface IRenderable with
            member this.Measure(context, maxWidth) =
                Measurement() // not used

            member this.Render(context, maxWidth) =
                items
                |> Seq.collect(fun x -> x.Render(context, maxWidth))

    let textRenderer (token: TextToken) (logEvent: LogEvent) =
        SpectreConsole.text token.Text

    let propRenderer (token: PropertyToken) (logEvent: LogEvent) =
        if logEvent.Properties.ContainsKey(token.PropertyName) then
            logEvent.Properties[token.PropertyName]
            |> LogEventPropertyValue.toString token
            |> SpectreConsole.escapeMarkup
            |> SpectreConsole.highlightProp
            |> SpectreConsole.markup
        else
            SpectreConsole.empty

    let messageRenderer (logEvent: LogEvent) =
        logEvent.MessageTemplate.Tokens
        |> Seq.map(fun token ->
            if token :? TextToken then
                textRenderer (token :?> TextToken) logEvent
            else
                propRenderer (token :?> PropertyToken) logEvent
        )
        |> Seq.toList

    let timestampRenderer (token: PropertyToken) (logEvent: LogEvent)=
        token.Format
        |> logEvent.Timestamp.ToString
        |> SpectreConsole.text

    let levelRenderer (token: PropertyToken) (logEvent: LogEvent) =

        let levelMoniker = LevelOutputFormat.getLevelMoniker token.Format logEvent.Level

        match logEvent.Level with
        | LogEventLevel.Verbose     -> levelMoniker |> SpectreConsole.highlightVerbose
        | LogEventLevel.Debug       -> levelMoniker |> SpectreConsole.highlightDebug
        | LogEventLevel.Information -> levelMoniker |> SpectreConsole.highlightInfo
        | LogEventLevel.Warning     -> levelMoniker |> SpectreConsole.highlightWarning
        | LogEventLevel.Error       -> levelMoniker |> SpectreConsole.highlightError
        | LogEventLevel.Fatal       -> levelMoniker |> SpectreConsole.highlightFatal
        | _                         -> levelMoniker
        |> SpectreConsole.markup

    let newLineRenderer (logEvent: LogEvent) =
        SpectreConsole.newLine

    let exceptionRenderer (logEvent: LogEvent) =
        if isNull logEvent.Exception
            then SpectreConsole.empty
        else
            SpectreConsole.error logEvent.Exception

    let propertiesRenderer (token: PropertyToken) (outputTemplate: MessageTemplate) (logEvent: LogEvent) =

        let shouldBeRendered (propName: string) (logEvent: LogEvent) (outputTemplate: MessageTemplate) =
            not (MessageTemplate.containsPropName propName logEvent.MessageTemplate)
            && not (MessageTemplate.containsPropName propName outputTemplate)

        logEvent.Properties
        |> Seq.filter(fun x -> shouldBeRendered x.Key logEvent outputTemplate)
        |> Seq.map(fun x -> LogEventProperty(x.Key, x.Value))
        |> StructureValue
        |> StructureValue.toString token
        |> SpectreConsole.escapeMarkup
        |> SpectreConsole.highlightMuted
        |> SpectreConsole.markup

    let eventPropRenderer (token: PropertyToken) (logEvent: LogEvent) =
        propRenderer token logEvent

    let createRenderer (outputTemplate: MessageTemplate) (token: MessageTemplateToken) =
        match token with
        | :? TextToken as t -> textRenderer t >> List.singleton

        | :? PropertyToken as t ->
            match t.PropertyName with
            | OutputProperties.MessagePropertyName    -> messageRenderer
            | OutputProperties.TimestampPropertyName  -> timestampRenderer t >> List.singleton
            | OutputProperties.LevelPropertyName      -> levelRenderer t >> List.singleton
            | OutputProperties.NewLinePropertyName    -> newLineRenderer >> List.singleton
            | OutputProperties.ExceptionPropertyName  -> exceptionRenderer >> List.singleton
            | OutputProperties.PropertiesPropertyName -> propertiesRenderer t outputTemplate >> List.singleton
            | _                                       -> eventPropRenderer t >> List.singleton

        | _ -> failwith "unsupported token"
