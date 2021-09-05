using System.Collections.Generic;

namespace Worms.Utility {
    internal static class ListExtensions {
        internal static void UnorderedRemove<T>(
            this List<T> list,
            int i
        ) {
            list[i] = list[^1];
            list.RemoveAt(list.Count - 1);
        }
    }
}