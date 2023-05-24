// example file used for defining the search
// includes things like specific day search of the cart
// and specific location forage spawns on a planned run day

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SeedFinding.Cart;
using SeedFinding.Bundles;
using SeedFinding.Locations;

namespace SeedFinding
{
	public class VaultSeeding
	{
        static List<HashSet<int>> Bundles = new List<HashSet<int>>()
        {
            new HashSet<int>(){768,769}, //solar/void essence
            new HashSet<int>(){396,398,402}, //spice berry/grape/sweet pea
            new HashSet<int>(){334,335,336}, // copper/iron/gold bars
            new HashSet<int>(){132,140,148}, // bream/walleye/eel
            new HashSet<int>(){188}, // green bean
        };

        // cart needs to contain items to satisfy two sets of bundles
        static HashSet<CartItem> ValidCart(int seed, int day, int minMatch, out bool valid)
        {
            var stock = TravelingCart.GetStock(seed, day);
            HashSet<int> stockIds = new HashSet<int>(stock.Select(o => o.Id));
            int count = 0;
            foreach (var bundle in Bundles)
            {
                if (bundle.IsProperSubsetOf(stockIds))
                {
                    count++;
                }
                if (count >= minMatch) break;
            }
            valid = count >= minMatch;
            return stock;
        }

        static HashSet<ObjectInfo.ObjectData> ValidBeach(int seed, int day, out bool valid)
        {
            Beach beach = new Beach(seed);
            beach.RunToDay(day);
            Rect box = new Rect(0, 56, 0, 24); // only want the left side beach
            //Rect box = new Rect(30, 43, 0, 16); // much tighter bounds on the search
            var items = beach.RegionBound(box);
            valid = items.Count == 4;
            return items;
        }

        static HashSet<ObjectInfo.ObjectData> ValidBus(int seed, int day, out bool valid)
        {
            BusStop bus = new BusStop(seed);
            bus.RunToDay(day);
            Rect box = new Rect(0, bus.Width, 0, bus.Height); // full map scan
            var items = bus.RegionBound(box);
            valid = items.Count == 3;
            return items;
        }
        static HashSet<ObjectInfo.ObjectData> ValidMountain(int seed, int day, out bool valid)
        {
            Mountain mtn = new Mountain(seed);
            mtn.RunToDay(day);
            Rect box = new Rect(0, mtn.Width, 0, mtn.Height); // full map scan
            var items = mtn.RegionBound(box);
            valid = items.Any((o) => o.Name == "Wild Horseradish");
            return items;
        }

        public static void Run()
        {
            int neededCartMatches = 2;
            int cartDay = 7;
            int runDay = 5;
            string filename = string.Format("Vault_TC{0}.txt", cartDay);
            using (StreamWriter file = new(filename, append: true))
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                List<int> Seeds = new List<int>();

                //int nSeeds = 0x7FFFFFFF;
                int nSeeds = 1 << 24;
                for (int seed = 0; seed < nSeeds; seed++)
                {
                    if (seed > 0 && seed % 1000000 == 0)
                    {
                        Console.WriteLine(string.Format("{0} by {1}", seed, stopwatch.Elapsed.TotalSeconds));
                    }

                    var stock = ValidCart(seed, cartDay, neededCartMatches, out bool valid);
                    if (!valid) continue;

                    //var bus = ValidBus(seed, 5, out valid);
                    //if (!valid) continue;

                    var beach = ValidBeach(seed, runDay, out valid);
                    if (!valid) continue;


                    //var mtn = ValidMountain(seed, 5, out valid);
                    //if (!valid) continue;

                    Console.WriteLine(seed);
                    Console.WriteLine(string.Join("\n", stock));
                    Console.WriteLine(string.Join("\n", beach));
                    //Console.WriteLine(string.Join("\n", bus));
                    //Console.WriteLine(string.Join("\n", mtn));
                    file.WriteLine(string.Format("{0}: {1}", seed, string.Join(",", stock)));

                }
                stopwatch.Stop();
                double seconds = stopwatch.ElapsedMilliseconds / 1000.0;
                double sps = nSeeds / seconds;
                Console.WriteLine(string.Format("Analyzed {0} seeds in {1} seconds ({2} sps)", nSeeds, seconds, sps));
                Console.WriteLine(string.Format("Int32.Max Eval Time: {0} seconds", Int32.MaxValue / sps));
            }
        }
    }
}

