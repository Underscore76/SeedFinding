using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using SeedFinding.Bundles;
using SeedFinding.Cart1_6;
using SeedFinding.Locations;
using SeedFinding.Trash1_6;

namespace SeedFinding
{
    public class BoilerRoomClassic
    {
        // defining the specific bundles needed
        public const ulong DesiredBundles = (
                // crafts
                // boiler
                CompressedFlags.BOILER_BLACKSMITH
                | CompressedFlags.BOILER_ENGINEER
                | CompressedFlags.BOILER_ADVENTURER
                | CompressedFlags.BOILER_ADVENTURER_SOLAR_ESSENCE
                | CompressedFlags.BOILER_ADVENTURER_VOID_ESSENCE
            );


        public static bool ValidSeed(int gameId, bool curate = false)
        {
            int fireQuartz = 0;
            int frozenTear = 0;
            for (int geode = 1; geode < 10; geode++)
            {
                (string, int) item = Mines.GetGeodeContents1_6(gameId, geode, Geode.OmniGeode);
                if (item.Item1 == "(O)82")
                {
                    fireQuartz = geode;
                }
                if (item.Item1 == "(O)84")
                {
                    frozenTear = geode;
                }
                if (fireQuartz != 0 && frozenTear != 0)
                {
                    break;
                }
            }

            if (fireQuartz == 0 || frozenTear == 0) {
                return false;
            }

            int[] days = new int[] { 5, 7, 12, 14, 19};
            bool found = false;
            foreach (var day in days)
            {
                var stock = TravelingCart.GetStock(gameId, day);
                HashSet<string> stockIds = new HashSet<string>(stock.Select(o => o.Id));
                HashSet<string> trashItems = new HashSet<string>(Trash1_6.Trash.getAllTrash(gameId, day, 0.1, hasFurnace: true).Select(x=>x.Id));
                if (RequiredItems.IsSubsetOf(stockIds.Union(trashItems)))
                {
                    found = true;
                }

                if (found)
                {
                    if (curate)
                    {
                        var onlyNeeded = stock.Where(i => RequiredItems.Contains(i.Id)).ToArray();
                        Console.WriteLine($"{gameId}    {day}   {String.Join(",", onlyNeeded)}    {onlyNeeded.Select(i => i.Cost).ToArray().Sum(x =>x)}");
                    }
                    break;
                }
            }

            if (!found)
            {
                return false;
            }

            return true;
        }
        static HashSet<string> RequiredItems = new HashSet<string>()
        {

            "335",//": "Iron Bar

            "336",//": "Gold Bar

            "768",//": "Solar Essence

            "769"//": "Void Essence
        };




        // single search
        public static void Curate()
        {
            List<int> seeds = new List<int> { 34676331 
            };

            foreach (var seed in seeds)
            {
                ValidSeed(seed, true);
            }
        }

        // parallel search
        public static double Search(int startId, int endId, int blockSize, out List<int> validSeeds)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var bag = new ConcurrentBag<int>();
            var partioner = Partitioner.Create(startId, endId, blockSize);
            Parallel.ForEach(partioner, (range, loopState) =>
            {
                //FileStream fs = new FileStream($"BoilerRoom{Task.CurrentId}.txt", FileMode.OpenOrCreate);
                //StreamWriter stream = new StreamWriter(fs);
                for (int seed = range.Item1; seed < range.Item2; seed++)
                {
                    if (ValidSeed(seed))
                    {
                        bag.Add(seed);
                        FileStream fs = null;
                        int i = 0;
                        while (fs == null)
                        {
                            try
                            {
                                fs = new FileStream($"BoilerRoomLegacy{i}.txt", FileMode.Append);
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
                    if (ValidSeed(-seed))
                    {
                        bag.Add(-seed);
                        FileStream fs = null;
                        int i = 0;
                        while (fs == null)
                        {
                            try
                            {
                                fs = new FileStream($"BoilerRoomLegacy{i}.txt", FileMode.Append);
                            }
                            catch
                            {
                                i++;
                            }
                        }
                        StreamWriter stream = new StreamWriter(fs);
                        stream.Write($"{-seed},");
                        Console.WriteLine(-seed);
                        stream.Close();
                    }
                }
            });
            double seconds = stopwatch.Elapsed.TotalSeconds;
            Console.WriteLine($"Found: {bag.Count} sols in {seconds.ToString("F2")} s");
            validSeeds = bag.ToList();
            validSeeds.Sort();
            return seconds;
        }
    }
}


