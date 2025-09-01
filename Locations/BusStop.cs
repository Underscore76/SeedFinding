using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedFinding.Locations
{
    public class BusStop : Location
    {
        public static HashSet<Tile> SpawnableTiles;
        public static HashSet<Tile> BadTiles;
        public static Dictionary<int, List<SpawnChance>> SeasonalSpawns;
        static BusStop()
        {
            SpawnableTiles = Location.LoadTiles(@"data/busstop_spawn_tiles.json");
            BadTiles = Location.LoadTiles(@"data/busstop_bad_tiles.json");
            SeasonalSpawns = new Dictionary<int, List<SpawnChance>>
            {
                [0] = new List<SpawnChance>() { new(18, 0.9), new(20, 0.4), new(22, 0.7) },
                [1] = new List<SpawnChance>() { new(396, 0.4), new(398, 0.4), new(402, 0.7) },
                [2] = new List<SpawnChance>() { new(406, 0.6), new(408, 0.4) },
                [3] = new List<SpawnChance>() { new(414, 0.33), new(418, 0.6), new(283, 0.5) }
            };
        }

        public BusStop(uint seed) : base(seed, 35, 30)
        {
        }
        public override void Spawn()
        {
            Spawn(SpawnableTiles, BadTiles, SeasonalSpawns);
        }
    }
}
