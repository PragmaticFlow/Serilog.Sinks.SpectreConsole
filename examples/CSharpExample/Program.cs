using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SpectreConsole;

namespace CSharpExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.SpectreConsole("{Timestamp:HH:mm:ss} [{Level:u4}] {Message:lj}{NewLine}{Exception}", minLevel: LogEventLevel.Verbose)
                .MinimumLevel.Verbose()
                .CreateLogger();

            // or for configuration via appsettings.json

            //var configuration = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json")
            //    .Build();

            //Log.Logger = new LoggerConfiguration()
            //    .ReadFrom.Configuration(configuration)
            //    .CreateLogger();

            Log.Information("symbol {");

            Log.Verbose("Verbose level example with {0}", "parameter");
            Log.Debug("Debug level example with {0}", "parameter");
            Log.Information("Information level example with {0}", "parameter");
            Log.Warning("Warning level example with {0}", "parameter");

            try
            {
                throw new Exception("Message");
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Error level example with {0}", "parameter");
            }

            Log.Fatal("Fatal level example with {0}", "parameter");

            Console.ReadKey();
        }
    }
}
