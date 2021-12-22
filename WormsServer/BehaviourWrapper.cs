using System.Collections.Generic;
using System.Linq;
using Worms.Behaviour;
using WormsOptimizer;

// 89 16 60 -> 25.76

namespace WormsServer {
    public sealed class BehaviourWrapper {
        private const int MAX_BEHAVIOURS = 50;

        private readonly object accessLock = new();

        private long ticks = long.MinValue;
        private readonly Dictionary<int, (IBehaviour, long)> behaviours = new();

        public IBehaviour this[int run] {
            get {
                lock (accessLock) {
                    ticks += 1;

                    var (behaviour, _) = behaviours.GetOrAdd(
                        run,
                        _ => (
                            new BigBrainBehaviour {
                                FirstReproductionStepThreshold = 40,
                                FirstReproductionEnergyThreshold = 20,
                                SecondReproductionEnergyThreshold = 50,
                                MaxWormsCount = 4
                            },
                            ticks
                        )
                    );

                    if (behaviours.Count > MAX_BEHAVIOURS) {
                        RemoveLeastRecentlyUsed();
                    }

                    return behaviour;
                }
            }
        }

        private void RemoveLeastRecentlyUsed() {
            var oldest = behaviours.OrderBy(it => it.Value.Item2).First().Key;
            behaviours.Remove(oldest);
        }
    }
}