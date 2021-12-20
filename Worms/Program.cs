﻿using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using CommandLine;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Worms.Behaviour;
using Worms.Database;
using Worms.Food;
using Worms.Names;
using Worms.StateObserver;
using Worms.Utility;

[assembly: InternalsVisibleTo("WormsTest")]
[assembly: InternalsVisibleTo("WormsServer")]
[assembly: InternalsVisibleTo("WormsOptimizer")]

namespace Worms {
    internal static class Program {
        internal const int SIMULATION_STEPS = 100;

        private const string DEFAULT_OUTPUT_FILE = "sim.out";
        private const string CONNECTION_STRING_KEY = "WormsDb";

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
                while (e.InnerException is not null) {
                    Console.Error.WriteLine(e.InnerException.Message);
                    e = e.InnerException;
                }
            }
        }

        private static void GenerateBehaviour(Options options) {
            using var db = new MainContext(
                new DbContextOptionsBuilder<MainContext>()
                    .EnableSensitiveDataLogging()
                    .UseSqlServer(ConfigurationManager.ConnectionStrings[CONNECTION_STRING_KEY].ConnectionString)
                    .Options
            );
            var repository = new WorldBehaviorsRepository(db);

            var worldBehaviour = WorldBehaviourGenerator.GenerateNew(
                options.GenerateBehavior!,
                new RandomFoodGenerator(),
                SIMULATION_STEPS
            );

            repository.Add(worldBehaviour);
        }

        private static void SimulateBehaviour(Options options) {
            using var outFile = new FileStream(options.OutputFile, FileMode.Create);
            using var outWriter = new StreamWriter(outFile);

            var hostBuilder = CreateHostBuilder(
                outWriter,
                options.SimulateBehavior!,
                options.ServerUrl
            );
            using var host = hostBuilder.Build();

            host.Run();
        }

        private static IHostBuilder CreateHostBuilder(
            TextWriter outputWriter,
            string worldBehaviorName,
            string serverUrl
        ) =>
            Host
                .CreateDefaultBuilder()
                .ConfigureServices(
                    (
                        _,
                        services
                    ) => {
                        services.AddHostedService<SimulationService>();

                        services.AddDbContext<MainContext>(
                            options => options.UseSqlServer(
                                ConfigurationManager.ConnectionStrings[CONNECTION_STRING_KEY].ConnectionString
                            )
                        );

                        services.AddHttpClient();

                        services.AddSingleton<IWorldBehaviorsRepository, WorldBehaviorsRepository>();

                        services.AddTransient<INameGenerator, NameGenerator>();
                        services.AddTransient<IFoodGenerator, LazyDbWorldBehaviorLoader>(
                            provider => new LazyDbWorldBehaviorLoader(
                                provider.GetRequiredService<IWorldBehaviorsRepository>(),
                                worldBehaviorName
                            )
                        );
                        services.AddTransient<IBehaviour, RemoteBehavior>(
                            provider =>
                                new RemoteBehavior(
                                    provider.GetRequiredService<IHttpClientFactory>(),
                                    serverUrl
                                )
                        );

                        services.AddSingleton<IStateObserver, TextStateWriter>(_ => new TextStateWriter(outputWriter));
                    }
                );

        // ReSharper disable once ClassNeverInstantiated.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public sealed class Options {
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
                "simulate",
                Required = false,
                Default = null,
                HelpText = "Name of the world behaviour to simulate. Cannot be used with \"--generate-new\"."
            )]
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string? SimulateBehavior { get; set; } = null;

            [Option(
                "server-url",
                Required = true,
                HelpText = "URL of a worm behaviour server."
            )]
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string ServerUrl { get; set; } = "";

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
        }
    }
}