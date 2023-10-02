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
            SeasonalSpawns = new Dictionary<int, List<SpawnChance>>();
            SeasonalSpawns[0] = new List<SpawnChance>() { new SpawnChance(20, 0.7), new SpawnChance(16, 0.5) };
            SeasonalSpawns[1] = new List<SpawnChance>() { new SpawnChance(396, 0.5), new SpawnChance(398, 0.8) };
            SeasonalSpawns[2] = new List<SpawnChance>() { new SpawnChance(404, 0.4), new SpawnChance(406, 0.4), new SpawnChance(408, 0.9) };
            SeasonalSpawns[3] = new List<SpawnChance>() { new SpawnChance(414, 0.85), new SpawnChance(418, 0.9), new SpawnChance(283, 0.5) };
        }

        public Mountain(int seed) : base(seed, 135, 41)
        {
        }
        public override void Spawn()
        {
            Spawn(SpawnableTiles, BadTiles, SeasonalSpawns);
        }
    }
}
