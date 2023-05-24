// used to search for a specific set of ids in a cart

using System;
using System.Linq;
using SeedFinding.Cart;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net;

namespace SeedFinding
{
	public class VaultCartSeeding
	{
        static HashSet<int> NeededItems = new HashSet<int>()
        {
            188, 768, 769
            // green bean / solar essence / void essence
        };
        static HashSet<int> OneOfTheseItems = new HashSet<int>()
        {
            715, 716, 717, 720, 721, 722
            // lobster / crayfish / crab / shrimp / snail / periwinkle
        };
        static bool ValidCart(int seed)
        {
            var stock = CompressedTravelingCart.GetStock(seed)
                .Select(o => o.Id).ToHashSet();
            return (
                stock.IsSupersetOf(NeededItems) &&
                OneOfTheseItems.Any((o) => stock.Contains(o))
            );
        }

        public static double Search(int numSeeds, int blockSize, out List<int> validSeeds)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var bag = new ConcurrentBag<int>();
            var partioner = Partitioner.Create(0, numSeeds, blockSize);
            Parallel.ForEach(partioner, (range, loopState) =>
            {
                for (int seed = range.Item1; seed < range.Item2; seed++)
                {
                    if (ValidCart(seed))
                    {
                        bag.Add(seed);
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

