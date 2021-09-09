using System;
using Worms.Utility;

namespace Worms.Behaviour {
    internal sealed class RotateClockwise : IBehaviour {
        private const double NO_OP_PROBABILITY = 0.33;

        private static readonly Direction[] directions = {
            Direction.Down,
            Direction.Left,
            Direction.Left,
            Direction.Up,
            Direction.Up,
            Direction.Right,
            Direction.Right,
            Direction.Down
        };

        private bool onTrack;
        private int directionIndex;

        private readonly Random random = new();

        public Action NextAction() {
            if (random.NextBool(NO_OP_PROBABILITY)) {
                return new Action.Nothing();
            }
            
            if (false == onTrack) {
                onTrack = true;
                return new Action.Move(Direction.Right);
            }

            var direction = directions[directionIndex];
            directionIndex += 1;
            directionIndex %= directions.Length;

            return new Action.Move(direction);
        }
    }
}