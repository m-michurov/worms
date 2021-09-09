using System.Collections.Generic;
using Worms.Utility;

namespace Worms {
    internal interface ISimulationState {
        IEnumerable<string> Worms { get; }
        
        bool IsWorm(Vector2Int position);
    }
}