module internal SpectreConsole

open Spectre.Console
open Spectre.Console.Rendering

let text (text): IRenderable =
    Text text

let markup (text): IRenderable =
    Markup text

let error (ex: exn): IRenderable =
    ex.GetRenderable()

let newLine: IRenderable =
    Text.NewLine

let escapeMarkup (text) =
    Markup.Escape text

let highlightProp (text) =
    $"[lime]{text}[/]"

let highlightMuted (text) =
    $"[grey]{text}[/]"

let highlightVerbose (text) =
    highlightMuted(text)

let highlightDebug (text) =
    $"[silver]{text}[/]"

let highlightInfo (text) =
    $"[deepskyblue1]{text}[/]"

let highlightWarning (text) =
    $"[yellow]{text}[/]"

let highlightError (text) =
    $"[red]{text}[/]"

let highlightFatal (text) =
    $"[maroon]{text}[/]"
