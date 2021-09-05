using Worms.Utility;

namespace Worms {
    internal class Action {
        private Action() { }

        internal sealed class Move : Action {
            internal readonly Direction Direction;
            internal Move(Direction direction) => Direction = direction;
        }

        internal sealed class Reproduce : Action {
            internal readonly Direction Direction;
            internal Reproduce(Direction direction) => Direction = direction;
        }

        internal sealed class Nothing : Action {}
    }
}