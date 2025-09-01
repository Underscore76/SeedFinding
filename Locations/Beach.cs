using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedFinding.Locations
{
    public class Beach : Location
    {
        public static Map map;
        public static HashSet<Tile> SpawnableTiles;
        public static HashSet<Tile> BadTiles;
        public static Dictionary<int, List<SpawnChance>> SeasonalSpawns;
        static Beach()
        {
            map = new Map("Beach");
            SpawnableTiles = Location.LoadTiles(@"data/beach_spawn_tiles.json");
            BadTiles = new HashSet<Tile>();
            BadTiles.UnionWith(new HashSet<Tile>(map.AlwaysFront.Tiles.Values));
            BadTiles.UnionWith(new HashSet<Tile>(map.Front.Tiles.Values));
            BadTiles.UnionWith(new HashSet<Tile>(map.Buildings.Tiles.Values));
            SeasonalSpawns = new Dictionary<int, List<SpawnChance>>
            {
                [0] = new List<SpawnChance>() { new(372, 0.9), new(718, 0.1), new(719, 0.3), new(723, 0.3) },
                [1] = new List<SpawnChance>() { new(372, 0.9), new(394, 0.5), new(718, 0.1), new(719, 0.3), new(723, 0.3) },
                [2] = new List<SpawnChance>() { new(372, 0.9), new(718, 0.1), new(719, 0.3), new(723, 0.3) },
                [3] = new List<SpawnChance>() { new(372, 0.4), new(392, 0.8), new(718, 0.05), new(719, 0.2), new(723, 0.2) }
            };
        }

        public Beach(uint seed) : base(seed, 104, 50)
        {
        }
        public override void Spawn()
        {

            Spawn(SpawnableTiles, BadTiles, SeasonalSpawns);
        }
    }
}