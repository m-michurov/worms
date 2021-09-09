using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Worms.Utility;

namespace Worms.StateObserver {
    internal sealed class SnapshotCollector : IStateObserver {
        private const int MIN_X = -10;
        private const int MIN_Y = -10;

        private const int MAX_X = 10;
        private const int MAX_Y = 10;

        private readonly List<string> snapshots = new();

        public void StateChanged(ISimulationState s) {
            var snapshot = new StringBuilder();

            snapshot.Append('+');
            snapshot.Append('-', (MAX_X - MIN_X + 1) * 2);
            snapshot.Append('+');
            snapshot.Append('\n');

            for (var y = MAX_Y; y >= MIN_Y; y -= 1) {
                snapshot.Append('|');
                for (var x = MIN_X; x <= MAX_X; x += 1) {
                    var position = new Vector2Int(x, y);
                    if (s.IsWorm(position)) {
                        snapshot.Append('s');
                    } else {
                        snapshot.Append(' ');
                    }

                    snapshot.Append(' ');
                }

                snapshot.Append('|');
                snapshot.Append('\n');
            }

            snapshot.Append('+');
            snapshot.Append('-', (MAX_X - MIN_X + 1) * 2);
            snapshot.Append('+');

            snapshots.Add(snapshot.ToString());
        }

        internal void Show(TimeSpan stepDelay) {
            Console.Clear();
            
            for (var i = 1; i <= snapshots.Count; i += 1) {
                var snapshot = snapshots[i - 1];
                Console.SetCursorPosition(0, 0);
                Console.WriteLine($"Step #{i}");
                Console.WriteLine(snapshot);
                Thread.Sleep(stepDelay);
            }
        }
    }
}