using System;
using System.IO;
using System.Runtime.CompilerServices;
using CommandLine;
using Worms.Behaviour;
using Worms.Names;
using Worms.StateObserver;
using Worms.Utility;

[assembly: InternalsVisibleTo("WormsTest")]

namespace Worms {
    internal static class Program {
        private const int STEPS = 100;
        private const int DELAY_MS = 150;
        private const string DEFAULT_OUTPUT_FILE = "sim.out";

        private static void Main(string[] args) =>
            Parser.Default
                .ParseArguments<Options>(args)
                .WithParsed(Run);
        
        private static void Run(Options options) {
            try {
                using var outFile = new FileStream(options.OutputFile, FileMode.Create);
                using var outWriter = new StreamWriter(outFile);

                var snapshotCollector = null as SnapshotCollector;
                var observer = new TextStateWriter(outWriter) as IStateObserver;

                if (options.Visualize) {
                    snapshotCollector = new SnapshotCollector();
                    observer = new MultiObserver(observer, snapshotCollector);
                }

                var s = new Simulation(
                    new NameGenerator(),
                    new RotateClockwise(),
                    observer
                );
                _ = s.TrySpawnWorm(Vector2Int.Zero);
                s.Run(STEPS);

                snapshotCollector?.Show(TimeSpan.FromMilliseconds(DELAY_MS));
            } catch (IOException e) {
                Console.Error.WriteLine(e.Message);
            }
        }

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

            [Option(Default = false, HelpText = "Visualize simulation in CLI")]
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public bool Visualize { get; set; }
        }
    }
}