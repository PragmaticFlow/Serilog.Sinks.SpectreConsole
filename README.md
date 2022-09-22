# Serilog.Sinks.SpectreConsole

[![build](https://github.com/PragmaticFlow/Serilog.Sinks.SpectreConsole/actions/workflows/build.yml/badge.svg)](https://github.com/PragmaticFlow/Serilog.Sinks.SpectreConsole/actions/workflows/build.yml)
[![NuGet](https://img.shields.io/nuget/v/Serilog.Sinks.SpectreConsole.svg)](https://www.nuget.org/packages/Serilog.Sinks.SpectreConsole/)
[![Gitter](https://badges.gitter.im/nbomber/community.svg)](https://gitter.im/nbomber/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

A Serilog sink that writes log events to console using [Spectre.Console](https://github.com/spectresystems/spectre.console). \
Output is plain text. \
The sink is written in F#.

## Getting started
The sink is available as a NuGet package. \
You can install it using the following command:

`Install-Package Serilog.Sinks.SpectreConsole`

To enable the sink, use .SpectreConsole() extension method for C#:

```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.SpectreConsole("{Timestamp:HH:mm:ss} [{Level:u4}] {Message:lj}{NewLine}{Exception}", minLevel: LogEventLevel.Information)
    .MinimumLevel.Verbose()
    .CreateLogger();

Log.Information("Information level example with {0}", "parameter");
```

For F#, use `.spectreConsole` instead:
```
Log.Logger <- 
    LoggerConfiguration() 
        .WriteTo.spectreConsole("{Timestamp:HH:mm:ss} [{Level:u4}] {Message:lj}{NewLine}{Exception}",  minLevel = LogEventLevel.Verbose)
        .MinimumLevel.Verbose()
        .CreateLogger()

Log.Information("Information level example with {0}", "parameter")
```

For more information, take a look at examples.

## Configuration via `appsettings.json`
To configure the sink via 'appsettings.json' configuration, you have to install NuGet packages:

`Install-Package Microsoft.Extensions.Configuration.Json`
`Install-Package Serilog.Settings.Configuration`

Then use `ReadFrom.Configuration()` method:

C# example:
```charp
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration()
    .CreateLogger();
```

F# example:
```fsharp
let configuration = 
    ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build()

Log.Logger <- 
    LoggerConfiguration() 
        .ReadFrom.Configuration()
        .CreateLogger()
```

In `appsettings.json` configuration file, write the following section:

```json
"Serilog": {
    "WriteTo": [
      {
        "Name": "SpectreConsole",
        "Args": {
          "outputTemplate": "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}", 
          "minLevel": "Verbose"
        }
      }
    ]
}
```
