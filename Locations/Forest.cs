using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedFinding.Locations
{
    public class Forest : Location
    {
        public static Map map;
        public static HashSet<Tile> SpawnableTiles;
        public static HashSet<Tile> BadTiles;
        public static Dictionary<int, List<SpawnChance>> SeasonalSpawns;
        static Forest()
        {
            map = new Map("Forest");
            //SpawnableTiles = Location.LoadTiles(@"data/beach_spawn_tiles.json");
            BadTiles = new HashSet<Tile>();
            BadTiles.UnionWith(new HashSet<Tile>(map.AlwaysFront.Tiles.Values));
            BadTiles.UnionWith(new HashSet<Tile>(map.Front.Tiles.Values));
            BadTiles.UnionWith(new HashSet<Tile>(map.Buildings.Tiles.Values));
            SeasonalSpawns = new Dictionary<int, List<SpawnChance>>();
            SeasonalSpawns[0] = new List<SpawnChance>() { new SpawnChance(16, .9), new SpawnChance(22, .9) };
            SeasonalSpawns[1] = new List<SpawnChance>() { new SpawnChance(396, .6), new SpawnChance(402, .9) };
            SeasonalSpawns[2] = new List<SpawnChance>() { new SpawnChance(404, .9), new SpawnChance(410, .9) };
            SeasonalSpawns[3] = new List<SpawnChance>() { new SpawnChance(418, .9), new SpawnChance(414, .9), new SpawnChance(283, .5) };
        }

        public Forest(int seed) : base(seed, 120, 120)
        {
        }
        public override void Spawn()
        {

            Spawn(SpawnableTiles, BadTiles, SeasonalSpawns);
        }
    }
}