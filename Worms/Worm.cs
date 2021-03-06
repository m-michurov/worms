using Worms.Utility;

namespace Worms {
    internal sealed class Worm {
        internal const int INITIAL_ENERGY = 10;
        internal const int ENERGY_LOSS_PER_STEP = 1;
        internal const int ENERGY_PER_FOOD = 10;
        internal const int REPRODUCTION_ENERGY_COST = 10;

        private readonly string name;
        internal Vector2Int Position { get; private set; }
        internal int Energy { get; private set; } = INITIAL_ENERGY;
        internal bool IsDead => Energy <= 0;

        internal Worm(
            string name,
            Vector2Int position
        ) => (this.name, Position) = (name, position);

        internal void Move(Direction direction) => Position += direction;

        internal void DecreaseEnergy() => Energy -= ENERGY_LOSS_PER_STEP;

        internal void Eat() => Energy += ENERGY_PER_FOOD;

        internal Worm Reproduce(
            string name_,
            Direction direction
        ) {
            Energy -= REPRODUCTION_ENERGY_COST;
            return new Worm(name_, Position + direction);
        }

        public override string ToString() => $"{name}-{Energy} {Position}";

        public override bool Equals(object? obj) => 
            obj is Worm other && other.name == name;

        public override int GetHashCode() => name.GetHashCode();
    }
}