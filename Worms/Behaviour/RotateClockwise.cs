using Worms.Utility;

namespace Worms.Behaviour {
    internal sealed class RotateClockwise : IBehaviour {
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

        public Action NextAction(
            ISimulationState simulation,
            Worm worm
        ) {
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