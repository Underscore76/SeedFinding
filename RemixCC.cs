using System;
using System.IO;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using SeedFinding.Bundles1_6;
using SeedFinding.Cart;
using SeedFinding.Locations;
using SeedFinding.Trash;
using System.Numerics;

namespace SeedFinding
{
    public class RemixCC
    {
        public static BigInteger requiredBundles = (
                CompressedFlags.CRAFTS_FOREST |
                CompressedFlags.CRAFTS_WILD_MEDICINE |
                CompressedFlags.PANTRY_GARDEN |
                CompressedFlags.PANTRY_RARE |
                CompressedFlags.PANTRY_BREWER |
                CompressedFlags.BULLETIN_FORAGER
            );

        public static BigInteger KillerBundles = (
                CompressedFlags.BULLETIN_CHEF |
                CompressedFlags.BULLETIN_ENCHANTER |
                CompressedFlags.BULLETIN_HOME_COOK |
                CompressedFlags.BULLETIN_DYE_DUCK_FEATHER
            );

        static bool ValidSeed(int gameId, bool curate)
        {

            var bundles = RemixedBundles.Generate(gameId);
            if (bundles.Contains(KillerBundles))
            {
                return false;
            }
            if(!bundles.Satisfies(requiredBundles))
            {
                return false;
            }
            if (curate)
            {
                var bundle = string.Join(", \n", bundles.Curate().ToArray());
                Console.WriteLine($"Seed: {gameId}{Environment.NewLine}Bundle Flags:{Environment.NewLine}{bundle}");
            }
            return true;
        }

        // parallel search
        public static double Search(int numSeeds, int blockSize, out List<int> validSeeds, bool curate)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var bag = new ConcurrentBag<int>();
            var partioner = Partitioner.Create(0, numSeeds, blockSize);
            Parallel.ForEach(partioner, (range, loopState) =>
            {
                for (int seed = range.Item1; seed < range.Item2; seed++)
                {
                    if (ValidSeed(seed, curate))
                    {
                        bag.Add(seed);
                        if (!curate)
                        {
                            Console.WriteLine(seed);
                        }
                        
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


