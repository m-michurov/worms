using System;
using System.IO;
using System.Runtime.CompilerServices;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Worms.Behaviour;
using Worms.Food;
using Worms.Names;
using Worms.StateObserver;

[assembly: InternalsVisibleTo("WormsTest")]

namespace Worms {
    internal static class Program {
        private const string DEFAULT_OUTPUT_FILE = "sim.out";

        private static void Main(string[] args) =>
            Parser.Default
                .ParseArguments<Options>(args)
                .WithParsed(Run);

        private static void Run(Options options) {
            try {
                using var outFile = new FileStream(options.OutputFile, FileMode.Create);
                using var outWriter = new StreamWriter(outFile);

                using var host = CreateHostBuilder(outWriter).Build();
                host.Run();
            } catch (IOException e) {
                Console.Error.WriteLine(e.Message);
            }
        }

        private static IHostBuilder CreateHostBuilder(TextWriter outputWriter) =>
            Host
                .CreateDefaultBuilder()
                .ConfigureServices(
                    (
                        _,
                        services
                    ) => {
                        services.AddHostedService<HostedSimulation>();
                        
                        services.AddTransient<INameGenerator, NameGenerator>();
                        services.AddTransient<IFoodGenerator, FoodGenerator>();
                        services.AddTransient<IBehaviour, HedonisticBehaviour>();
                        services.AddTransient<IStateObserver, TextStateWriter>(_ => new TextStateWriter(outputWriter));
                    }
                );

        // ReSharper disable once ClassNeverInstantiated.Local
        private sealed class Options {
            [Value(
                0,
                MetaName = nameof(OutputFile),
                Default = DEFAULT_OUTPUT_FILE,
                HelpText = "File to write simulation results to",
                Required = false
            )]
            // ReSharper disable once PropertyCanBeMadeInitOnly.Local
            public string OutputFile { get; set; } = DEFAULT_OUTPUT_FILE;
        }
    }
}