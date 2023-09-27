using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;

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
    public struct Tile : IEquatable<Tile>
    {
        public int X;
        public int Y;
        public int index;
        public Tile(int x, int y) { X = x; Y = y; index = -1; }
        public override string ToString()
        {
            return string.Format("({0:D2},{1:D2})", X, Y);
        }
        public bool Equals(Tile other)
        {
            return X == other.X && Y == other.Y;
        }
    }

    public struct Bubbles
    {
        public Tile Tile;
        public int StartTime;
        public int EndTime;

        public Bubbles(Tile tile, int startTime, int endTime) {
            Tile = tile;
            StartTime = startTime;
            EndTime = endTime; 
        }
        public Bubbles(int x, int y, int startTime, int endTime) {
            Tile = new Tile(x, y);
            StartTime = startTime;
            EndTime = endTime; 
        }
        public override string ToString()
        {
            return string.Format("({0:D2},{1:D2}) {2:D4}-{3:D4}", Tile.X, Tile.Y, StartTime, EndTime);
        }
    }

    public class Layer
    {
        public int Width;
        public int Height;
        public Dictionary<(int,int),Tile> Tiles;
        public static Layer LoadCSV(string filepath)
        {
            int dimY = 0;
            int dimX = 0;
            Dictionary<(int, int), Tile> tiles = new Dictionary<(int, int), Tile>();

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
                            tiles.Add((col,dimY),new Tile(col, dimY) { index = int.Parse(values[col]) });
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
        public List<int> WaterTiles;

        public Map(string baseName)
        {
            Front = Layer.LoadCSV(string.Format("data/{0}_Front.csv", baseName));
            AlwaysFront = Layer.LoadCSV(string.Format("data/{0}_AlwaysFront.csv", baseName));
            Back = Layer.LoadCSV(string.Format("data/{0}_Back.csv", baseName));
            Buildings = Layer.LoadCSV(string.Format("data/{0}_Buildings.csv", baseName));
            WaterTiles = JsonConvert.DeserializeObject<List<int>>(File.ReadAllText(string.Format("data/{0}_water_tilelist.json", baseName)));

        }

        public bool InLayer(string layer, Tile tile)
        {
            switch (layer)
            {
                case "Front":
                    return Front.Tiles.ContainsKey((tile.X,tile.Y));
                case "AlwaysFront":
                    return AlwaysFront.Tiles.ContainsKey((tile.X, tile.Y));
                case "Back":
                    return Back.Tiles.ContainsKey((tile.X, tile.Y));
                case "Buildings":
                    return Buildings.Tiles.ContainsKey((tile.X, tile.Y));
                default:
                    throw new Exception("invalid layer name");
            }
        }

        public bool IsWaterTile(Tile tile) {
            if (!Back.Tiles.ContainsKey((tile.X, tile.Y)))
            {
                return false;
            }
            Tile mapTile = Back.Tiles[(tile.X, tile.Y)];
            return WaterTiles.Contains(mapTile.index);
        }

        public bool IsOpenWater(Tile tile)
        {
            if (!IsWaterTile(tile))
            {
                return false;
            }

            if (Buildings.Tiles.ContainsKey((tile.X,tile.Y)))
            {
                return false;
            }

            return true;
        }

        public int DistanceToLand(Tile tile)
        {
            int tileX = tile.X;
            int tileY = tile.Y;
            Rectangle r = new Rectangle(tileX - 1, tileY - 1, 3, 3);
            bool foundLand = false;
            int distance = 1;
            while (!foundLand && r.Width <= 11)
            {
                foreach (Point v in Utility.getBorderOfThisRectangle(r))
                {
                    if (InLayer("Back",tile) && IsWaterTile(tile))
                    {
                        foundLand = true;
                        distance = r.Width / 2;
                        break;
                    }
                }
                r.Inflate(1, 1);
            }
            if (r.Width > 11)
            {
                distance = 6;
            }
            return distance - 1;
        }
    }

    public abstract class Location
    {
        public readonly int Width;
        public readonly int Height;

        public Dictionary<Tile, ObjectInfo.ObjectData> ForageSpawns;
        public List<Tile> ArtifactSpots;
        public List<Bubbles> Bubbles;
        public int Seed;
        public int Day;
        public Location(int seed, int width, int height)
        {
            ForageSpawns = new Dictionary<Tile, ObjectInfo.ObjectData>();
            ArtifactSpots = new List<Tile>();
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
            int season = (Day == 0) ? 0 : (((Day - 1) / 28) % 4);
            if (ForageSpawns.Count < 6)
            {
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

            /*if (ArtifactSpots.Count > 1) {
                return;
            }
            double chanceForNewArtifactAttempt = 1.0;
            while (rand.NextDouble() < chanceForNewArtifactAttempt)
            {
                int xCoord = rand.Next(Width);
                int yCoord = rand.Next(Height);
                Tile location = new Tile(xCoord, yCoord);
                if (this.isTileLocationTotallyClearAndPlaceable(location) && this.getTileIndexAt(xCoord, yCoord, "AlwaysFront") == -1 && this.getTileIndexAt(xCoord, yCoord, "Front") == -1 && !this.isBehindBush(location) && (this.doesTileHaveProperty(xCoord, yCoord, "Diggable", "Back") != null || (this.GetSeasonForLocation().Equals("winter") && this.doesTileHaveProperty(xCoord, yCoord, "Type", "Back") != null && this.doesTileHaveProperty(xCoord, yCoord, "Type", "Back").Equals("Grass"))))
                {
                    if (this.name.Equals("Forest") && xCoord >= 93 && yCoord <= 22)
                    {
                        continue;
                    }
                    this.objects.Add(location, new Object(location, 590, 1));
                }
                chanceForNewArtifactAttempt *= 0.75;
                if (season == 3)
                {
                    chanceForNewArtifactAttempt += 0.10000000149011612;
                }
            }*/

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



        public void ProcessBubbles(Map map)
        {
            Bubbles.Clear();
            bool bubblesExist = false;
            int x;
            int y;
            int startTime = 0;
            Tile tile = new Tile(0,0);
            for(int time = 610; time < 2600; time += 10)
            {
                if (time % 100 >= 60)
                {
                    continue;
                }

                Random random = new Random(time + Seed / 2 + Day);

                if (!bubblesExist)
                {
                    if (random.NextDouble() < 0.5)
                    {
                        for (int tries = 0; tries < 2; tries++)
                        {
                            tile = new Tile(random.Next(0, Width), random.Next(0, Height));
                            if (!map.IsOpenWater(tile))
                            {
                                continue;
                            }
                            int toLand = map.DistanceToLand(tile);
                            if (toLand > 1 && toLand < 5)
                            {
                                startTime = time;
                                bubblesExist = true;
                                break;
                            }
                        }
                    }
                }else if (random.NextDouble() < 0.1)
                {
                    bubblesExist = false;
                    Bubbles.Add(new Locations.Bubbles(tile, startTime, time));
                }
            }

            if (bubblesExist)
            {
                Bubbles.Add(new Locations.Bubbles(tile, startTime, 2600));
            }
        }
    }
}
