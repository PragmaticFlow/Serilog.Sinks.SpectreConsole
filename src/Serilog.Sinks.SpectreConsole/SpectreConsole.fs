module internal SpectreConsole

open Spectre.Console

let writeText (text: string) =
    text |> Text |> AnsiConsole.Write

let writeMarkup (text: string) =
    AnsiConsole.Write(Markup text)

let writeException (ex: exn) =
    AnsiConsole.WriteException ex

let writeNewLine () =
    AnsiConsole.WriteLine()

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
