using System;
using System.IO;
using System.Runtime.CompilerServices;
using CommandLine;
using Microsoft.EntityFrameworkCore;
using Worms.Behaviour;
using Worms.Database;
using Worms.Food;
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
                if (options.GenerateBehavior is not null && options.SimulateBehavior is null) {
                    GenerateBehaviour(options);
                    return;
                }

                if (options.SimulateBehavior is not null && options.GenerateBehavior is null) {
                    SimulateBehaviour(options);
                    return;
                }

                Console.Error.WriteLine(
                    "Please provide either a behaviour name to simulate or to generate. " +
                    "See \"--help\" for details."
                );
            } catch (IOException e) {
                Console.Error.WriteLine(e.Message);
            } catch (Exception e) {
                Console.Error.WriteLine($"Unhandled exception: {e.Message}");
                if (e.InnerException is not null) {
                    Console.Error.WriteLine(e.InnerException.Message);
                }
            }
        }

        private static WorldBehaviorContext CreateDbContext(Options options) =>
            new(
                new DbContextOptionsBuilder<WorldBehaviorContext>()
                    .EnableSensitiveDataLogging()
                    .UseSqlServer(options.ConnectionString)
                    .Options
            );

        private static void GenerateBehaviour(Options options) {
            using var db = CreateDbContext(options);
            var repository = new WorldBehaviorsRepository(db);

            var worldBehaviour = WorldBehaviourGenerator.GenerateNew(
                options.GenerateBehavior!,
                new RandomFoodGenerator(),
                STEPS
            );

            repository.Add(worldBehaviour);
        }

        private static void SimulateBehaviour(Options options) {
            using var db = CreateDbContext(options);
            var repository = new WorldBehaviorsRepository(db);

            var worldBehaviour = repository.GetByName(options.SimulateBehavior!);
            if (worldBehaviour is null) {
                Console.Error.WriteLine(
                    $"World behavior with name \"{options.SimulateBehavior}\" does not exist"
                );
                return;
            }

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
                new ListFoodGenerator(worldBehaviour.ToFoodPositions()),
                new HedonisticBehaviour(),
                observer
            ).Chain(it => it.TrySpawnWorm(Vector2Int.Zero));
            s.Run(STEPS);

            snapshotCollector?.Show(TimeSpan.FromMilliseconds(DELAY_MS));
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private sealed class Options {
            [Value(
                0,
                MetaName = nameof(OutputFile),
                Default = DEFAULT_OUTPUT_FILE,
                HelpText = "File to write simulation results to.",
                Required = false
            )]
            // ReSharper disable once PropertyCanBeMadeInitOnly.Local
            public string OutputFile { get; set; } = DEFAULT_OUTPUT_FILE;

            [Option(
                Default = false,
                HelpText = "Visualize simulation in CLI."
            )]
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public bool Visualize { get; set; }

            [Option(
                "simulate",
                Required = false,
                Default = null,
                HelpText = "Name of the world behaviour to simulate. Cannot be used with \"--generate-new\"."
            )]
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string? SimulateBehavior { get; set; } = null;

            [Option(
                "generate-new",
                Required = false,
                Default = null,
                HelpText = "Generate new world behaviour with given name. " +
                           "Simulation does not run with this option. " +
                           "Cannot be used with \"--simulate\"."
            )]
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string? GenerateBehavior { get; set; } = null;

            [Option(
                "connection-string",
                Required = true,
                HelpText = "Connection string used to connect to the database."
            )]
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string ConnectionString { get; set; } = "";
        }
    }
}