namespace Serilog.Sinks.SpectreConsole

open Serilog.Events
open Serilog.Formatting.Display
open Serilog.Parsing

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

    type LogEventRenderer = LogEvent -> unit

    let textRenderer (token: TextToken) (logEvent: LogEvent) =
        SpectreConsole.writeText token.Text

    let propRenderer (token: PropertyToken) (logEvent: LogEvent) =
        if logEvent.Properties.ContainsKey token.PropertyName then
            logEvent.Properties[token.PropertyName]
            |> LogEventPropertyValue.toString token
            |> SpectreConsole.escapeMarkup
            |> SpectreConsole.highlightProp
            |> SpectreConsole.writeMarkup

    let messageRenderer (logEvent: LogEvent) =
        logEvent.MessageTemplate.Tokens
        |> Seq.iter(fun token ->
            if token :? TextToken then
                textRenderer (token :?> TextToken) logEvent
            else
                propRenderer (token :?> PropertyToken) logEvent
        )

    let timestampRenderer (token: PropertyToken) (logEvent: LogEvent)=
        token.Format
        |> logEvent.Timestamp.ToString
        |> SpectreConsole.writeText

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
        |> SpectreConsole.writeMarkup

    let newLineRenderer (logEvent: LogEvent) =
        SpectreConsole.writeNewLine ()

    let exceptionRenderer (logEvent: LogEvent) =
        if not (isNull logEvent.Exception) then
            SpectreConsole.writeException logEvent.Exception

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
        |> SpectreConsole.writeMarkup

    let eventPropRenderer (token: PropertyToken) (logEvent: LogEvent) =
        propRenderer token logEvent

    let private createPropTokenRenderer (outputTemplate: MessageTemplate) (token: PropertyToken) =
        match token.PropertyName with
        | OutputProperties.MessagePropertyName       -> messageRenderer
        | OutputProperties.TimestampPropertyName     -> timestampRenderer token
        | OutputProperties.LevelPropertyName         -> levelRenderer token
        | OutputProperties.NewLinePropertyName       -> newLineRenderer
        | OutputProperties.ExceptionPropertyName     -> exceptionRenderer
        | OutputProperties.PropertiesPropertyName    -> propertiesRenderer token outputTemplate
        | _                                          -> eventPropRenderer token

    let createRenderer (outputTemplate: MessageTemplate) (token: MessageTemplateToken) =
        if token :? TextToken then
            textRenderer(token :?> TextToken)
        else
            createPropTokenRenderer outputTemplate (token :?> PropertyToken)
