using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using Worms.Behaviour;
using Worms.Food;
using Worms.Names;
using Worms.StateObserver;
using Worms.Utility;

[assembly: InternalsVisibleTo("WormsTest")]

namespace Worms {
    internal static class Program {
        private const int STEPS = 100;
        private static void Main() {
            var snapshotCollector = new SnapshotCollector();
            using var outFile = new FileStream("sim.out", FileMode.Create);
            using var outWriter = new StreamWriter(outFile);
            
            var s = new Simulation(
                new NameGenerator(),
                new FoodGenerator(),
                new HedonisticBehaviour(),
                new MultiObserver(
                    snapshotCollector,
                    new TextStateWriter(outWriter))
            );
            _ = s.TrySpawnWorm(Vector2Int.Zero);
            s.Run(STEPS);

            foreach (var snapshot in snapshotCollector.Snapshots) {
                Console.Clear();
                Console.WriteLine(snapshot);
                Thread.Sleep(TimeSpan.FromMilliseconds(150));
            }
        }
    }
}