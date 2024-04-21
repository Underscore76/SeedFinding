using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SeedFinding.Locations1_6;

namespace SeedFinding
{
    public class Result
    {
        public Point Tile;
        public int StartTime;
        public int EndTime;
        public int Distance;
        public string Type;

        public Result(Point tile, int startTime, int endTime, int distance, string type)
        {
            Tile = tile;
            StartTime = startTime;
            EndTime = endTime;
            Distance = distance;
            Type = type;
        }
        public Result(int x, int y, int startTime, int endTime, int distance, string type)
        {
            Tile = new Point(x, y);
            StartTime = startTime;
            EndTime = endTime;
            Distance = distance;
            Type = type;
        }

        public Result() { }
        public override string ToString()
        {
            return string.Format("{4} ({0:D2},{1:D2}) {2:D4}-{3:D4}", Tile.X, Tile.Y, StartTime, EndTime, Type);
        }

        public int TotalMinutes()
        {
            return TotalMinutes(StartTime, EndTime);
        }

        public static int TotalMinutes(int startTime, int endTime)
        {
            // Same hour
            if (startTime / 100 == endTime / 100)
            {
                return endTime - startTime;
            }

            // Minutes until next hour
            int minutes = 60 - (startTime % 100);

            // Treat StartTime as being at next hour
            int time = startTime + minutes + 40;

            int hours = endTime / 100 - time / 100;

            return hours * 60 + minutes + endTime % 100;
        }
    }
    public class Bubbles
    {
        public static Version version164 = new Version("1.6.4");

        public static List<Result> Predict(Map map, int gameId, int day, string version = "1.6.4", int timesFished = 0, int panningLevel = 0, string panningEnchant = "")
        {
            List<Result> results = new List<Result>();
            Point p;

            int mapWidth = map.Width;
            int mapHeight = map.Height;

            int panningWidth = mapWidth;
            int panningHeight = mapHeight;
            int panningWidthStart = 0;
            int panningHeightStart = 0;

            if (map.Name == "IslandNorth")
            {
                panningWidth = 15;
                panningHeight = 70;
                panningWidthStart = 4;
                panningHeightStart = 45;
            }

            Result bubbles = null;
            Result panning = null;

            bool bubblesExist = false;
            bool panningExist = false;

            Version gameVersion = new Version(version);

            if (gameVersion >= version164)
            {
                bubblesExist = false;
                panningExist = false;
            }

            int bubblesStart = 0;

            Layer backLayer = map.FindLayer("Back");
            for (int timeOfDay = 610; timeOfDay < 2600; timeOfDay += 10)
            {
                if (timeOfDay % 100 >= 60)
                    continue;
                Random random;
                if (gameVersion >= version164)
                {
                    random = Utility.CreateDaySaveRandom(day, gameId, timeOfDay, mapWidth);
                }
                else
                {
                    random = Utility.CreateDaySaveRandom(day, gameId, timeOfDay);
                }

                if (!bubblesExist)
                {
                    if (random.NextDouble() < 0.5)
                    {
                        for (int index = 0; index < 2; ++index)
                        {
                            p = new Point(random.Next(0, mapWidth), random.Next(0, mapHeight));
                            if (!map.isOpenWater(p.X, p.Y) || map.doesTileHaveProperty(p.X, p.Y, "NoFishing", "Back") != false)
                            {
                                continue;
                            }

                            int land = map.distanceToLand(p.X, p.Y);
                            if (land > 1 && land <= 4)
                            {
                                bubblesExist = true;
                                bubblesStart = timeOfDay;
                                string type = "Bubbles";
                                if (gameVersion >= version164)
                                {
                                    if (random.NextDouble() < 0.01 && day - 1 > 3 && (map.Name == "Town" || map.Name == "Mountain" || map.Name == "Forest" || map.Name == "Beach") && timeOfDay < 2300 && (timesFished > 2 || day - 1 > 14) && !Utility.isFestivalDay(day))
                                    {
                                        random.NextDouble();
                                        type = "Frenzy";
                                    }
                                }

                                bubbles = new Result()
                                {
                                    Tile = p,
                                    StartTime = timeOfDay,
                                    Type = type,
                                    Distance = land
                                };

                                results.Add(bubbles);

                                break;

                            }
                        }
                    }
                }
                else
                {
                    int splashPointDurationSoFar = Result.TotalMinutes(bubblesStart, timeOfDay);
                    bool check;
                    if (gameVersion >= version164)
                    {
                        check = random.NextDouble() < 0.1 + (double)((float)splashPointDurationSoFar / 1800f) && splashPointDurationSoFar > (bubbles.Type == "Frenzy" ? 90 : 60);
                    }
                    else
                    {
                        check = random.NextDouble() < 0.1;
                    }
                    if (check)
                    {
                        bubblesExist = false;
                        bubbles.EndTime = timeOfDay;
                    }
                }

                Layer buildingsLayer = map.FindLayer("Buildings");
                if (panningLevel > 0 && !(map.Name == "Beach"))
                {
                    if (!panningExist)
                    {
                        if (random.NextDouble() < 0.5)
                        {
                            for (int tries = 0; tries < 8; tries++)
                            {
                                p = new Point(random.Next(panningWidthStart, panningWidth), random.Next(panningHeightStart, panningHeight));
                                int land = map.distanceToLand(p.X, p.Y, landMustBeAdjacentToWalkableTile: true);
                                if (map.isOpenWater(p.X, p.Y) && land <= 1 && (buildingsLayer == null || buildingsLayer.GetTileIndex(p.X, p.Y) == 0))
                                {
                                    panningExist = true;


                                    panning = new Result()
                                    {
                                        Tile = p,
                                        StartTime = timeOfDay,
                                        Type = "Panning",
                                        Distance = land
                                    };

                                    results.Add(panning);

                                    break;
                                }
                            }
                        }
                    }
                    else if (random.NextDouble() < 0.1)
                    {
                        panningExist = false;
                        panning.EndTime = timeOfDay;
                    }
                }
            }

            if (bubblesExist)
            {
                bubbles.EndTime = 2600;
            }
            if (panningExist)
            {
                panning.EndTime = 2600;
            }
            return results;
        }
    }
}
