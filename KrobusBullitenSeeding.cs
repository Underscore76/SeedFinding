// used to search for a specific set of ids in a cart

using System;
using System.Linq;
using SeedFinding.Cart;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net;
using SeedFinding.Trash;
using System.IO;
using static System.Net.WebRequestMethods;
using System.Collections;
using System.Threading;
using static System.Formats.Asn1.AsnWriter;
using SeedFinding.Locations;

namespace SeedFinding
{
    public class KrobusBullitenSeeding
    {
        static HashSet<int> NeededItems = new HashSet<int>()
        {
            284, //Beet
            432 //Truffle Oil

        };
        static HashSet<int> FiveItems = new HashSet<int>()
        {
            284, //Beet
            432 //Truffle Oil
        };




        public static int CheckTroves(int seed, int startCount, int count)
        {

            int result = 0;
            for (int geode = startCount; geode < startCount+count; geode++) 
            {


                var magmaContents = Mines.GetGeodeContents(seed, geode, Geode.Trove, 115);
                var item = ObjectInfo.Get(magmaContents.Item1);
                if (item.Cost >= 2500)
                {
                    result += item.Cost;
                }
            }
            return result;
        }

        public static bool CheckSeed15(int seed, bool report = false)
        {

            int day = 15;

            // Billboard
            Random random = new Random(seed + day);
            double randResult = random.NextDouble();
            if (randResult < 0.8148148148148 || randResult > 0.8518518519)
            {
                return false;
            }

            // Resort
            var visitors = Resort.CalculateResortVisitors(seed, day);
            HashSet<string> WantedVistiors = new() { "Penny", "Jas", "Vincent" };
            if (!WantedVistiors.IsSubsetOf(visitors.ToHashSet()))
            {
                return false;
            }

            return true;
        }

        public static bool CheckSeed14(int seed, bool report = false)
        {
            int day = 14;

            // Billboard
            Random random = new Random(seed + day);
            double randResult = random.NextDouble();
            if (randResult < 0.8148148148148 || randResult > 0.8518518519)
            {
                return false;
            }

            
            // Cart
            var stock = CompressedTravelingCart.GetStock(seed, day).Where(o => o.Quantity == 5)
                .Select(o => o.Id).ToHashSet();

            bool fives = FiveItems.IsSubsetOf(stock);

            if (!fives)
            {
                return false;
            }

            // CalicoJack
            int wins = 0;
            int timesPlayed = -1;
            while (wins < 20)
            {
                timesPlayed++;
                if (CalicoJack.CalculateHand(timesPlayed, day, seed) == -1)
                {
                    wins = 0;
                    continue;
                }

                wins += 1;
            }
            if (timesPlayed > 120)
            {
                return false;
            }

            // Resort
            /*var visitors = Resort.CalculateResortVisitors(seed, day);
            HashSet<string> WantedVistiors = new() { "Penny", "Jas", "Vincent" };
            if (!WantedVistiors.IsSubsetOf(visitors.ToHashSet()))
            {
                return false;
            }*/

            Mountain mountain = new Mountain(seed);

            mountain.Day = day;
            mountain.ProcessBubbles(Mountain.map);

            int totalTime = 0;
            foreach (Bubbles bubbles in mountain.Bubbles)
            {
                if (bubbles.StartTime > 1200)
                {
                    break;
                }
                if (bubbles.Distance < 2)
                {
                    continue;
                }

                totalTime += bubbles.TotalMinutes();
            }
            Console.WriteLine($"{seed}  {ObjectInfo.Get(Quests.GetItemDeliveryQuestItem(seed, day, 5, true, true, 121)).Name}  {totalTime}");

            return true;
        }

        public static bool CheckSeed13(int seed, Desert desert, Forest forest, bool report = false)
        {
            int day = 13;
            desert.Seed = seed;
            desert.Day = day + 1;
            desert.ForageSpawns.Clear();

            desert.Spawn();

            int coconutCount = desert.ForageSpawns.Where(s => s.Value.Id == 88).Count();

            if (coconutCount < 2)
            {
                return false;
            }

            forest.Seed = seed;
            forest.Day = day;
            forest.ProcessBubbles(Forest.map);

            int totalTime = 0;
            foreach (Bubbles bubbles in forest.Bubbles)
            {
                if (bubbles.StartTime > 1200)
                {
                    break;
                }
                if (bubbles.Distance < 2 ||
                    bubbles.Tile.X < 89 ||
                    bubbles.Tile.X > 104 ||
                    bubbles.Tile.Y > 46)
                {
                    continue;
                }
                int startTime = Math.Max(bubbles.StartTime, 630);
                int endTime = Math.Min(bubbles.EndTime, 1200);

                if (startTime > endTime)
                {
                    continue;
                }

                totalTime += bubbles.TotalMinutes(startTime, endTime);
            }

            if (totalTime < 330)
            {
                return false;
            }

            if (report)
            {
                Console.WriteLine($"{seed}  {coconutCount}  {totalTime}");
            }

            return true;

        }
        public static bool CheckSeed(int seed, bool report = false)
        {
            if (seed % 2 == 1)
            {
                //return false;
            }
            int day = 13;

            /*Random random = new Random(seed + day);
            double randResult = random.NextDouble();
            if (randResult < 0.6 || randResult > 0.75)
            {
                return false;
            }

            int item = Quests.GetItemDeliveryQuestItem(seed, day, 3, true, false, 89);
            if (!new List<int>
            { 68, //Topaz
                66, //Amethyst
                78, //Cave Carrot
                80, //Quartz
                86, //Earth Crystal
                152, //Seaweed
                167, //Joja Cola
                153, //Green Algae
                420, //Red Musrhoom
                334, //Copper Bar
                335, //Iron Bar
                336, //Gold Bar
                    62, //Aquamarine
                    70, //Jade
                    72, //Diamond
                    84, //Frozen Tear
                    422, //Purple Mushroom
                    64, //Ruby
                    60, //Emerald
                    82 //Fire Quartz
            }.Contains(item))
            {
               // return false;
            }*/
            //if (seed % 2 == 1)
            //{
            //    return false;
            //}
            /*if (NightEvents.GetEvent(seed, day + 1) != NightEvents.Event.Fairy)
            {
                return false;
            }*/

            /*if (Trash.Trash.getTrash(seed,day,Trash.Trash.Can.George, 0.008, hasFurnace: true) != 223)
            {
                return false;
            }*/

            /* bool found = false;
             int item = 0;
             //item = Utility.GetArtifactspot(seed, day, x, y, location);
             if (item == 103)
             {
                 found = true;
             }


             if (!found)
             {
                 //return false;
             }*/

            /*List<(int, int)> horseradishspots = new()
            {
                (69,27),
                (66,27),
                (67,26),
                (65,25)
            };
            found = false;
            foreach (var spot in horseradishspots)
            {
                if (Utility.ForageQuality(seed, day, spot.Item1, spot.Item2, 5) == Quality.Gold && Utility.ForageDoubled(seed, day, spot.Item1, spot.Item2, 5))
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                //return false;
            }
            List<(int, int)> horseradishCrops = new()
            {
                (60,24),
                (61,24),
                (62,24),
                (63,24),
                (64,24),
                (60,25),
                (61,25),
                (62,25),
                (63,25),
                (64,25),
                (60,26),
                (61,26),
                (63,26),
                (64,26),
                (60,27),
                (61,27),
                (62,27),
                (63,27),
                (64,27),
                (60,28),
                (61,28),
                (62,28),
                (63,28),
                (64,28)
            };
            found = false;
            foreach (var spot in horseradishCrops)
            {
                if (Utility.CropQuality(seed, day, spot.Item1, spot.Item2, 0,Fertilizer.None) == Quality.Gold)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                return false;
            }*/

            List<List<int>> spots = new List<List<int>>();
            /*spots.Add(new List<int> { 5, 9, 6 });
            spots.Add(new List<int> { 5, 10, 7 });
            spots.Add(new List<int> { 5, 11, 7 });
            spots.Add(new List<int> { 5, 11, 6 });
            spots.Add(new List<int> { 15, 6, 9 });
            spots.Add(new List<int> { 15, 8, 9 });*/
            /*spots.Add(new List<int> { 25, 28, 5 });
            spots.Add(new List<int> { 25, 28, 6 });
            spots.Add(new List<int> { 25, 30, 5 });
            spots.Add(new List<int> { 35, 5, 7 });
            spots.Add(new List<int> { 35, 5, 8 });
            spots.Add(new List<int> { 35, 6, 8 });
            spots.Add(new List<int> { 35, 7, 8 });
            spots.Add(new List<int> { 35, 7, 7 });*/
            /*spots.Add(new List<int> { 45, 9, 6 });
            spots.Add(new List<int> { 45, 10, 7 });
            spots.Add(new List<int> { 45, 11, 7 });
            spots.Add(new List<int> { 45, 11, 6 });
            spots.Add(new List<int> { 55, 6, 9 });
            spots.Add(new List<int> { 55, 8, 9 });
            spots.Add(new List<int> { 65, 28, 5 });
            spots.Add(new List<int> { 65, 28, 6 });
            spots.Add(new List<int> { 65, 30, 5 });
            spots.Add(new List<int> { 75, 5, 7 });
            spots.Add(new List<int> { 75, 5, 8 });
            spots.Add(new List<int> { 75, 6, 8 });
            spots.Add(new List<int> { 75, 7, 8 });
            spots.Add(new List<int> { 75, 7, 7 });
            spots.Add(new List<int> { 85, 9, 6 });
            spots.Add(new List<int> { 85, 10, 7 });
            spots.Add(new List<int> { 85, 11, 7 });
            spots.Add(new List<int> { 85, 11, 6 });
            spots.Add(new List<int> { 95, 6, 9 });
            spots.Add(new List<int> { 95, 8, 9 });
            spots.Add(new List<int> { 105, 28, 5 });
            spots.Add(new List<int> { 105, 28, 6 });
            spots.Add(new List<int> { 105, 30, 5 });
            spots.Add(new List<int> { 115, 5, 7 });
            spots.Add(new List<int> { 115, 5, 8 });
            spots.Add(new List<int> { 115, 6, 8 });
            spots.Add(new List<int> { 115, 7, 8 });
            spots.Add(new List<int> { 115, 7, 7 });*/

            bool found = false;
            /*foreach (List<int> spot in spots)
            {
                List<int> results = Mines.CheckStone(seed, spot[0], spot[1], spot[2], false, true);
                //if (results.Intersect(new List<int> { (int)Geode.Geode, (int)Geode.OmniGeode }).Count() == 2)
                //if (results.Intersect(new List<int> { (int)Geode.OmniGeode }).Count() ==1)
                //if (results.Count(x => x == (int)Geode.Geode) + results.Count(x => x == (int)Geode.OmniGeode) == 4 && results.Count() > 4)
                if (results.Count(x => x == (int)Geode.Geode) + results.Count(x => x == (int)Geode.OmniGeode) == 4)
                //if (results.Count() >= 4)
                //if (results.Count() >= 2)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                //return false;
            }*/

            /*int copperCount = 0;
            int geodeMoney = 0;

            for (int num = 0; num < 20; num++)
            {
                (int, int) content = Mines.GetGeodeContents(seed, num + 1 + 158, Geode.Geode);
                if (content.Item1 == 378)
                {
                    copperCount += content.Item2;
                    continue;
                }

                int price = int.Parse(ObjectInfo.ObjectInformation[content.Item1.ToString()].Split('/')[1]) * content.Item2;
                content = Mines.GetGeodeContents(seed, num + 1 + 158, Geode.OmniGeode);
                int omniPrice = int.Parse(ObjectInfo.ObjectInformation[content.Item1.ToString()].Split('/')[1]) * content.Item2;

                geodeMoney += Math.Max(price, omniPrice);

            }
            if (copperCount < 45 || geodeMoney < 5500 - copperCount * 75)
            {
                return false;
            }*/

            /*int drop = Mines.DropShaftDrop(seed, 121, day);

            if (Mines.GetFloorType(seed, day, 121 + drop) != FloorType.Dino)
            {
                return false;
            }*/


            /*List<(int, int)> musselSpots = new()
            {
(77,82),
(93,84),
(96,83),
(69,78),
(80,80),
(92,83),
(64,82),
(66,78),
(74,80),
(75,79)
            };
            int nuts = 0;
            foreach (var musselSpot in musselSpots)
            {
                if (Utility.MusselNutDrop(seed, day, musselSpot.Item1, musselSpot.Item2))
                {
                    nuts++;
                }
            }

            if (nuts < 5)
            {
                return false;
            }*/

            /*List<int> Tier3 = new()
            {
                89,
                96,
                118,


            };
            List<int> Tier2 = new()
            {
                82,
                84,
                88,
                98,
                106,
                111,
                113,
                116
            };
            List<int> floors = new();
            int score = 0;
            List<int> monsters = new();
            for (int i = 81; i < 120; i++)
            {
                FloorType type = Mines.GetFloorType(seed, day, i);
                switch (type)
                {
                    case FloorType.Mushroom:
                        floors.Add(i);
                        score += Tier3.Contains(i) ? 3 : Tier2.Contains(i) ? 2 : 1;
                        break;
                    case FloorType.Monster:
                    case FloorType.Slime:
                        monsters.Add(i);
                        break;
                }
            }



            if (floors.Count < 4)
            {
                return false;
            }

            if (score < 10)
            {
                return false;
            }*/

            if (!ValidCart(seed, day))
            {
                return false;
            }

            /*List<(int, int)> forageSpots = new()
            {
(15,49),
(64,24),
(64,25),
(103,8),
(93,93),
(10,15)
            };

            int doubleCount = 0;
            foreach (var spot in forageSpots)
            {
                doubleCount += Utility.ForageDoubled(seed, day, spot.Item1, spot.Item2, 10,true) ? 1 : 0;
            }*/

            if (report)
            {
                //Console.WriteLine($"{seed}  {String.Join(",", floors.Select(x => x.ToString()).ToArray())}  {score}  {doubleCount}");
                //Console.WriteLine($"{seed}  {doubleCount}");
            }
             return true;

            int cash = 0;
            int geodeCount = 428;
            int count = 0;
            List<int> unsellables = new List<int>() { 100, 101, 103, 104, 105, 106, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 330, /*378, 380, 382, 384, 386,*/ 390 };
            /*while (cash < 62000)
            {
                count++;
                var magmaContents = Mines.GetGeodeContents(seed, geodeCount+count, Geode.Geode, 120);
                var omniContents = Mines.GetGeodeContents(seed, geodeCount+count, Geode.OmniGeode, 120);

                int value = Math.Max(unsellables.Contains(magmaContents.Item1) ? 0 : ObjectInfo.Get(magmaContents.Item1).Cost * magmaContents.Item2, unsellables.Contains(omniContents.Item1) ? 0: ObjectInfo.Get(omniContents.Item1).Cost * omniContents.Item2);

                cash += value;
            }

            if (count > 400)
            {
                return false;
            }*/


            //Trash.Trash.getAllTrash(seed,day, 0.010);
            int badCount = 0;
            int count21 = 0;
            List<int> idealLayouts = new() {12,20, 21,23,24, 26, 29,30,31, 32, 33,34};
            var levels = Volcano.GetLevels(seed, day);
            foreach (var level in levels)
            {
                if (!idealLayouts.Contains(level))
                {
                    badCount++;
                }
                if (level == 21)
                    count21++;
            }

            if (badCount > 1)
            {
                return false;
            }
            if(count21 < 2)
            {
                return false;
            }

            if (new Random(seed).NextDouble() > 0.2)
            {
                return false;
            }

            //Console.WriteLine($"{seed}");
            //Console.WriteLine($"{seed} {copperCount} {geodeMoney}");
            //Console.WriteLine($"{seed} {ObjectInfo.Get(item).Name}");
            if (report)
            {
                //Console.WriteLine($"{seed}  {String.Join(",", floors.Select(x => x.ToString()).ToArray())}  {score}  {String.Join(",", Trash.Trash.getAllTrash(seed, day, -0.062, hasDesert: true, hasFurnace: true, mines: 120).Select(x => ObjectInfo.Get(x).Name))}   {String.Join(",", levels.Select(x => x.ToString()).ToArray())} {count21}");
            }
            return true;
        }

        public static void Curate()
        {

            List<int> seeds = new List<int>() { 82613437, 83969985, 100742660, 214766436, 297380551, 544416555, 760421488, 1014481273, 1041939395, 1371048853, 1498807821, 1597249295, 1726676423, 1748531390, 1761218614, 1798681902, 1826210124, 1833921766, 1839306595, 1951743482, 1958193759, 1959236666, 2072166632, 2100230364 };
            foreach (var seed in seeds) { 
                CheckSeed14(seed, true);
            }
        }

        public static double Search(int start, int end, int blockSize, out List<int> validSeeds)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var bag = new ConcurrentBag<int>();
            var partioner = Partitioner.Create(start, end, blockSize);
            Parallel.ForEach(partioner, (range, loopState) =>
            {
                for (int seed = range.Item1; seed < range.Item2; seed++)
                {
                    if (!CheckSeed15(seed))
                    {
                        continue;
                    }

                    bag.Add(seed);
                    FileStream fs = null;
                    int i = 0;
                    while (fs == null)
                    {
                        try
                        {
                            fs = new FileStream($"KrobusDaybil15{i}.txt", FileMode.Append);
                        }
                        catch
                        {
                            i++;
                        }
                    }
                    StreamWriter stream = new StreamWriter(fs);
                    stream.Write($"{seed},");
                    Console.WriteLine(seed);
                    stream.Close();
                }
            });
            double seconds = stopwatch.Elapsed.TotalSeconds;
            Console.WriteLine($"Found: {bag.Count} sols in {seconds.ToString("F2")} s");
            validSeeds = bag.ToList();
            validSeeds.Sort();
            return seconds;
        }

        public static bool ValidGeodes(int seed)
        {
            Console.WriteLine(Mines.GetGeodeContents(seed, 1, Geode.Geode));
            return true;
        }

        public static bool ValidMines(int seed)
        {
            Console.WriteLine(string.Join( ",",Mines.CheckStone1_5(548284736, 25, 28, 6)));
            return true;
        }

        public static bool ValidCart(int seed, int day)
        {
            //if (NightEvents.GetEvent(seed,6) != NightEvents.Event.Fairy)
            //{
            //    continue;
            //}
            var stock = CompressedTravelingCart.GetStock(seed, day)
                .Select(o => o.Id).ToHashSet();

            bool neededCount = NeededItems.IsSubsetOf(stock);
            if (!neededCount)
            {
                return false;
            }


            stock = CompressedTravelingCart.GetStock(seed, day).Where(o => o.Quantity == 5)
                .Select(o => o.Id).ToHashSet();

            //int neededCount = stock.Intersect(NeededItems).Count();
            bool fives = FiveItems.IsSubsetOf(stock);

            //int bonusCount = stock.Intersect(BonusItems).Count();

            //if (neededCount < 4 && bonusCount < 4)

            //Console.WriteLine($"{seed} {neededCount} {fives}");
            return fives;
        }

        public static string Report(int seed)
        {
            string output = "";
            foreach (var item in CompressedTravelingCart.GetStock(seed))
            {
                output += '\t' + ObjectInfo.Get(item.Id).Name;
            }
            return $"{seed} {output}";
        }
    }
}

