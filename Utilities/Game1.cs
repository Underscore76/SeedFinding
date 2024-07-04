using StardewValley.Hashing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeedFinding.Locations1_6;
using Newtonsoft.Json;
using System.Resources;
using System.Xml.Linq;
using System.IO;

namespace SeedFinding
{
    internal class Game1
    {
        public static bool UseLegacyRandom = true;
        public static ulong uniqueIDForThisGame = 0;
        public static uint DaysPlayed = 0;
        public static IHashUtility hash = new HashUtility();
		public static float DailyLuck = 0.0f;
		public static Season season = Season.Spring;
		public static bool archaeologyEnchant = false;
		public static bool magnifyingGlass = false;
		public static bool QiBeansActive = false;
		public static string location = "";

		public static Dictionary<string, LocationData> locations1_6 = JsonConvert.DeserializeObject<Dictionary<string, LocationData>>(File.ReadAllText($@"Locations1_6/Locations.json"));
	}
}
