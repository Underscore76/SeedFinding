using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedFinding.Locations
{
    public class Desert : Location
    {
        public static Map map;
        public static HashSet<int> SpawnableIndexes = new() { 66, 68 };
        public static HashSet<Tile> BadTiles;
        public static Dictionary<int, List<SpawnChance>> SeasonalSpawns;
        static Desert()
        {
            map = new Map("Desert");
            //SpawnableTiles = Location.LoadTiles(@"data/beach_spawn_tiles.json");
            BadTiles = new HashSet<Tile>();
            BadTiles.UnionWith(new HashSet<Tile>(map.AlwaysFront.Tiles.Values));
            BadTiles.UnionWith(new HashSet<Tile>(map.Front.Tiles.Values));
            BadTiles.UnionWith(new HashSet<Tile>(map.Buildings.Tiles.Values));
            SeasonalSpawns = new Dictionary<int, List<SpawnChance>>
            {
                [0] = new List<SpawnChance>() { new(88, .5), new(90, .5) },
                [1] = new List<SpawnChance>() { new(88, .5), new(90, .5) },
                [2] = new List<SpawnChance>() { new(88, .5), new(90, .5) },
                [3] = new List<SpawnChance>() { new(88, .5), new(90, .5) }
            };

        }

        public Desert(uint seed) : base(seed, 120, 120)
        {
        }
        public override void Spawn()
        {

            Spawn(null, BadTiles, SeasonalSpawns,map,SpawnableIndexes);
        }
    }
}