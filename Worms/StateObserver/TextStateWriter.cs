using System.IO;

namespace Worms.StateObserver {
    internal sealed class TextStateWriter : IStateObserver {
        private readonly TextWriter textWriter;

        public TextStateWriter(TextWriter textWriter_) => textWriter = textWriter_;

        public void StateChanged(ISimulationState s) => textWriter.WriteLine(
            $"Worms: [{string.Join(", ", s.Worms)}] Food: [{string.Join(", ", s.Foods)}]"
        );
    }
}