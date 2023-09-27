using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SeedFinding.Bundles;
using SeedFinding.Trash;

namespace SeedFinding
{
	public class DynamicCCRemixSeeding
	{
        // defining the specific bundles needed
        public const ulong DesiredBundles = (
                // crafts
                CompressedFlags.CRAFTS_WINTER_FORAGE_SNOW_YAM
                | CompressedFlags.CRAFTS_WINTER_FORAGE_WINTER_ROOT
                | CompressedFlags.CRAFTS_WINTER_FORAGE_CROCUS
                | CompressedFlags.CRAFTS_WINTER_FORAGE_CRYSTAL_FRUIT
                | CompressedFlags.CRAFTS_CONSTRUCTION
                | CompressedFlags.CRAFTS_EXOTIC
                // pantry
                | CompressedFlags.PANTRY_RARE
                | CompressedFlags.PANTRY_ARTISAN
                | CompressedFlags.PANTRY_FISH_FARMER
                // fish
                | CompressedFlags.FISH_SPECIALITY
                // boiler
                //| CompressedFlags.BOILER_BLACKSMITH
                //| CompressedFlags.BOILER_GEOLOGIST
                //| CompressedFlags.BOILER_ADVENTURER
                //| CompressedFlags.BOILER_ADVENTURER_SOLAR_ESSENCE
                //| CompressedFlags.BOILER_ADVENTURER_VOID_ESSENCE
                // bulletin
                | CompressedFlags.BULLETIN_DYE
                | CompressedFlags.BULLETIN_DYE_RED_MUSHROOM
                | CompressedFlags.BULLETIN_DYE_SEA_URCHIN
                | CompressedFlags.BULLETIN_DYE_SUNFLOWER
                | CompressedFlags.BULLETIN_DYE_CACTUS_FRUIT
                | CompressedFlags.BULLETIN_DYE_AQUAMARINE
                | CompressedFlags.BULLETIN_DYE_IRIDIUM_BAR
                | CompressedFlags.BULLETIN_FIELD_RESEARCH
                | CompressedFlags.BULLETIN_ENCHANTER
                | CompressedFlags.BULLETIN_CHILDREN
                | CompressedFlags.BULLETIN_HOME_COOK
            );

        static bool ValidSeed(int seed)
        {
            //return RemixedBundles.Generate(seed).Satisfies(DesiredBundles);
            if (Trash.Trash.getTrash(seed, 1, Trash.Trash.Can.Gus) != 194)
                return false;

            int geode = Trash.Trash.getTrash(seed, 1, Trash.Trash.Can.Museum);

            if (geode != 535 && geode != 749)
                return false;

            (int, int) content = Mines.GetGeodeContents(seed, 1, geode == 535 ? Geode.Geode : Geode.OmniGeode);
            if (content.Item1 != 378 || content.Item2 != 20)
                return false;

            return true;
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

