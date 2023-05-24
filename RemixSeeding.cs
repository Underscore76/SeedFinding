using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SeedFinding.Bundles;

namespace SeedFinding
{
	public class RemixSeeding
	{
        // defining the specific bundles needed
        public const ulong DesiredBundles = (
                // crafts
                CompressedFlags.CRAFTS_SPRING_FORAGE_WILD_HORSERADISH
                | CompressedFlags.CRAFTS_SPRING_FORAGE_DAFFODIL
                | CompressedFlags.CRAFTS_SPRING_FORAGE_LEEK
                | CompressedFlags.CRAFTS_SPRING_FORAGE_DANDELION
                | CompressedFlags.CRAFTS_WINTER_FORAGE_SNOW_YAM
                | CompressedFlags.CRAFTS_WINTER_FORAGE_WINTER_ROOT
                | CompressedFlags.CRAFTS_WINTER_FORAGE_CROCUS
                | CompressedFlags.CRAFTS_CONSTRUCTION
                | CompressedFlags.CRAFTS_EXOTIC
                // pantry
                | CompressedFlags.PANTRY_RARE
                | CompressedFlags.PANTRY_ARTISAN
                | CompressedFlags.PANTRY_GARDEN
                // fish
                | CompressedFlags.FISH_QUALITY
                // boiler
                | CompressedFlags.BOILER_BLACKSMITH
                | CompressedFlags.BOILER_GEOLOGIST
                | CompressedFlags.BOILER_ADVENTURER
                | CompressedFlags.BOILER_ADVENTURER_SOLAR_ESSENCE
                | CompressedFlags.BOILER_ADVENTURER_VOID_ESSENCE
                // bulletin
                | CompressedFlags.BULLETIN_FODDER
                | CompressedFlags.BULLETIN_CHILDREN
                | CompressedFlags.BULLETIN_FIELD_RESEARCH
                | CompressedFlags.BULLETIN_ENCHANTER
                | CompressedFlags.BULLETIN_DYE
                | CompressedFlags.BULLETIN_DYE_RED_MUSHROOM
                | CompressedFlags.BULLETIN_DYE_AMARANTH
                | CompressedFlags.BULLETIN_DYE_SUNFLOWER
                | CompressedFlags.BULLETIN_DYE_CACTUS_FRUIT
                | CompressedFlags.BULLETIN_DYE_BLUEBERRY
                | CompressedFlags.BULLETIN_DYE_IRIDIUM_BAR
            );

        static bool ValidSeed(int seed)
        {
            return RemixedBundles.Generate(seed).Satisfies(DesiredBundles);
        }

        // parallel search
        public static double Search(int numSeeds, int blockSize, out List<int> validSeeds)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var bag = new ConcurrentBag<int>();
            var partioner = Partitioner.Create(0, numSeeds, blockSize);
            Parallel.ForEach(partioner, (range, loopState) =>
            {
                for (int seed = range.Item1; seed < range.Item2; seed++)
                {
                    if (ValidSeed(seed))
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

