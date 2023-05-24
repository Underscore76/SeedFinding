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
            SeasonalSpawns = new Dictionary<int, List<SpawnChance>>();
            SeasonalSpawns[0] = new List<SpawnChance>() { new SpawnChance(18, 0.9), new SpawnChance(20, 0.4), new SpawnChance(22, 0.7) };
            SeasonalSpawns[1] = new List<SpawnChance>() { new SpawnChance(396, 0.4), new SpawnChance(398, 0.4), new SpawnChance(402, 0.7) };
            SeasonalSpawns[2] = new List<SpawnChance>() { new SpawnChance(406, 0.6), new SpawnChance(408, 0.4) };
            SeasonalSpawns[3] = new List<SpawnChance>() { new SpawnChance(414, 0.33), new SpawnChance(418, 0.6), new SpawnChance(283, 0.5) };
        }

        public BusStop(int seed) : base(seed, 35, 30)
        {
        }
        public override void Spawn()
        {
            Spawn(SpawnableTiles, BadTiles, SeasonalSpawns);
        }
    }
}
