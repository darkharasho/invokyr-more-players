using System.Collections.Generic;

namespace InvokyrMorePlayers
{
    /// <summary>Slot arithmetic, kept apart from the game so it can be tested.</summary>
    internal static class Slots
    {
        /// <summary>The game's own player limit.</summary>
        public const int VanillaMax = 6;

        /// <summary>Indices 0..cap-1 missing from <paramref name="present"/>, in order.</summary>
        public static List<int> MissingIndices(ICollection<int> present, int cap)
        {
            var missing = new List<int>();
            for (var i = 0; i < cap; i++)
            {
                if (!present.Contains(i))
                    missing.Add(i);
            }
            return missing;
        }

        /// <summary>An index the game has spawn points/characters for: higher ones wrap around.</summary>
        public static int Wrap(int index, int count) => count <= 0 || index < 0 ? index : index % count;

        /// <summary>The least-used of <paramref name="characterCount"/> characters (lowest number on ties).</summary>
        public static int LeastUsed(IEnumerable<int> inUse, int characterCount)
        {
            var counts = new int[characterCount];
            foreach (var c in inUse)
            {
                if (c >= 0 && c < characterCount)
                    counts[c]++;
            }
            var best = 0;
            for (var i = 1; i < characterCount; i++)
            {
                if (counts[i] < counts[best])
                    best = i;
            }
            return best;
        }

        /// <summary>The lobby-list filter value to use instead of <paramref name="value"/> for a player-count key.</summary>
        public static int FilterValue(string key, int value, int cap) =>
            key != null && key.ToLowerInvariant().Contains("player") && value == VanillaMax ? cap : value;
    }
}
