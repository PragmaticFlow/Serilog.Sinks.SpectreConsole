#!/bin/bash

dotnet restore
dotnet dotnet pack src/Serilog.Sinks.SpectreConsole/Serilog.Sinks.SpectreConsole.fsproj --configuration Release -p:PackageVersion=0.1.4-pre -o ./nuget
dotnet nuget push nuget/*.nupkg --source "github"