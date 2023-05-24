using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedFinding.Locations
{
    public struct Rect
    {
        public int minX;
        public int maxX;
        public int minY;
        public int maxY;
        public Rect(int minx, int maxx, int miny, int maxy)
        {
            minX = minx;
            maxX = maxx;
            minY = miny;
            maxY = maxy;
        }
        public bool Contains(Tile tile)
        {
            return minX <= tile.X && tile.X <= maxX && minY <= tile.Y && tile.Y <= maxY;
        }
    }
    public struct Tile
    {
        public int X;
        public int Y;
        public Tile(int x, int y) { X = x; Y = y; }
        public override string ToString()
        {
            return string.Format("({0:D2},{1:D2})", X, Y);
        }
    }

    public class Layer
    {
        public int Width;
        public int Height;
        public HashSet<Tile> Tiles;
        public static Layer LoadCSV(string filepath)
        {
            int dimY = 0;
            int dimX = 0;
            HashSet<Tile> tiles = new HashSet<Tile>();

            using (var reader = new StreamReader(filepath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    if (dimX == 0)
                    {
                        dimX = values.Length;
                    }
                    for (int col = 0; col < dimX; col++)
                    {
                        if (values[col] != "-1")
                        {
                            tiles.Add(new Tile(col, dimY));
                        }
                    }
                    dimY++;
                }
            }
            Layer layer = new Layer();
            layer.Width = dimX;
            layer.Height = dimY;
            layer.Tiles = tiles;
            return layer;
        }
    }

    public class Map
    {
        public Layer Front;
        public Layer AlwaysFront;
        public Layer Back;
        public Layer Buildings;

        public Map(string baseName)
        {
            Front = Layer.LoadCSV(string.Format("data/{0}_Front.csv", baseName));
            AlwaysFront = Layer.LoadCSV(string.Format("data/{0}_AlwaysFront.csv", baseName));
            Back = Layer.LoadCSV(string.Format("data/{0}_Back.csv", baseName));
            Buildings = Layer.LoadCSV(string.Format("data/{0}_Buildings.csv", baseName));
        }

        public bool InLayer(string layer, Tile tile)
        {
            switch (layer)
            {
                case "Front":
                    return Front.Tiles.Contains(tile);
                case "AlwaysFront":
                    return AlwaysFront.Tiles.Contains(tile);
                case "Back":
                    return Back.Tiles.Contains(tile);
                case "Buildings":
                    return Buildings.Tiles.Contains(tile);
                default:
                    throw new Exception("invalid layer name");
            }
        }
    }

    public abstract class Location
    {
        public readonly int Width;
        public readonly int Height;

        public Dictionary<Tile, ObjectInfo.ObjectData> ForageSpawns;
        public int Seed;
        public int Day;
        public Location(int seed, int width, int height)
        {
            ForageSpawns = new Dictionary<Tile, ObjectInfo.ObjectData>();
            Seed = seed;
            Day = 0;
            Width = width;
            Height = height;
        }
        public static HashSet<Tile> LoadTiles(string filepath)
        {
            var result = new HashSet<Tile>();
            foreach (var tup in JsonConvert.DeserializeObject<List<List<int>>>(File.ReadAllText(filepath)))
            {
                result.Add(new Tile(tup[0], tup[1]));
            }
            return result;
        }
        public abstract void Spawn();

        public void Spawn(HashSet<Tile> SpawnableTiles, HashSet<Tile> BadTiles, Dictionary<int, List<SpawnChance>> SeasonalSpawns)
        {
            if (ForageSpawns.Count >= 6) return;
            int season = (Day == 0) ? 0 : (((Day - 1) / 28) % 4);
            Random rand = new Random((Seed / 2) + Day);
            int numToSpawn = rand.Next(1, Math.Min(5, 7 - ForageSpawns.Count));
            for (int i = 0; i < numToSpawn; i++)
            {
                for (int t = 0; t < 11; t++)
                {
                    Tile check = new Tile(rand.Next(Width), rand.Next(Height));
                    int index = rand.Next(SeasonalSpawns[season].Count);
                    if (!ForageSpawns.ContainsKey(check) && SpawnableTiles.Contains(check))
                    {
                        double result = rand.NextDouble();
                        if (result < SeasonalSpawns[season][index].P)
                        {
                            if (BadTiles.Contains(check)) continue;

                            ForageSpawns[check] = ObjectInfo.Get(SeasonalSpawns[season][index].Id);
                            break;
                        }
                    }
                }
            }
        }
        public void ProcessDay()
        {
            int dayOfMonth = (Day - 1) % 28 + 1;
            int dayOfWeek = dayOfMonth % 7;
            if (dayOfWeek == 0 && Day != 0)
            {
                ForageSpawns.Clear();
            }
            if (dayOfWeek == 0)
            {
                Spawn();
                Spawn();
            }
            Spawn();
            if (dayOfMonth == 1)
            {
                Spawn();
            }
            if (Day < 4)
            {
                Spawn();
            }
            Day++;
        }
        public void RunToDay(int day)
        {
            ForageSpawns.Clear();
            Day = 0;
            Spawn();
            Day++;
            while (Day <= day)
            {
                ProcessDay();
            }
        }
        public HashSet<ObjectInfo.ObjectData> RegionBound(Rect box)
        {
            return new(ForageSpawns.Where(pair => box.Contains(pair.Key)).Select(pair => pair.Value));
        }
    }
}
