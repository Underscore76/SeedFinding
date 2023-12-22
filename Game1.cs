using StardewValley.Hashing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedFinding
{
    internal class Game1
    {
        public static bool UseLegacyRandom = true;
        public static ulong uniqueIDForThisGame = 0;
        public static uint DaysPlayed = 0;
        public static IHashUtility hash = new HashUtility();
    }
}
