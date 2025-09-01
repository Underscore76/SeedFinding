using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedFinding.Locations
{
    public class Mountain : Location
    {
        public static Map map;
        public static HashSet<Tile> SpawnableTiles;
        public static HashSet<Tile> BadTiles;
        public static Dictionary<int, List<SpawnChance>> SeasonalSpawns;
        static Mountain()
        {
            map = new Map("Mountain");
            SpawnableTiles = Location.LoadTiles(@"data/mountain_spawn_tiles.json");
            BadTiles = Location.LoadTiles(@"data/mountain_bad_tiles.json");
            SeasonalSpawns = new Dictionary<int, List<SpawnChance>>
            {
                [0] = new List<SpawnChance>() { new(20, 0.7), new(16, 0.5) },
                [1] = new List<SpawnChance>() { new(396, 0.5), new(398, 0.8) },
                [2] = new List<SpawnChance>() { new(404, 0.4), new(406, 0.4), new(408, 0.9) },
                [3] = new List<SpawnChance>() { new(414, 0.85), new(418, 0.9), new(283, 0.5) }
            };
        }

        public Mountain(uint seed) : base(seed, 135, 41)
        {
        }
        public override void Spawn()
        {
            Spawn(SpawnableTiles, BadTiles, SeasonalSpawns);
        }
    }
}
