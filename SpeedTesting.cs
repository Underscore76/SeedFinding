using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using SeedFinding.Bundles;
using SeedFinding.Cart;

namespace SeedFinding
{
	public class SpeedTesting
	{
        // used to validate remix bundle generation
		public static void ValidateRemixBundles(int numTrials)
		{
            Random r = new Random();
            for (int i = 0; i < numTrials; i++)
            {
                var seed = r.Next(1 << 30);
                var b = new BundleGenerator().Generate(seed);
                var c = RemixedBundles.Generate(seed).ToDict();
                foreach (var p in b)
                {
                    bool eq = p.Value.SequenceEqual(c[p.Key]);
                    if (!p.Value.SequenceEqual(c[p.Key]))
                    {
                        Console.WriteLine($"{seed} {p.Key}: {string.Join(",", p.Value)} != {string.Join(",", c[p.Key])}");
                        throw new Exception("not consistent rng");
                    }
                }
            }
        }

        // used to test your parallel cart compute performance
        public static double ParallelCartTiming(int numSeeds, int blockSize, bool verbose=false)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var partioner = Partitioner.Create(0, numSeeds, blockSize);
            Parallel.ForEach(partioner, (range, loopState) =>
            {
                for (int seed = range.Item1; seed < range.Item2; seed++)
                {
                    CompressedTravelingCart.GetStock((int)seed);
                }
            });
            double seconds = stopwatch.Elapsed.TotalSeconds;
            if (verbose)
            {
                double seedsPerSecond = numSeeds / seconds;
                Console.WriteLine(
                    string.Format("{0}  [i:{1}]  [sps:{2}]  [Int32.Max:{3}]",
                        numSeeds.ToString("D10"),
                        seconds.ToString("F2"),
                        seedsPerSecond.ToString("F2"),
                        (Int32.MaxValue / seedsPerSecond).ToString("F2")
                    )
                );
            }
            return seconds;
        }

        // used to test your single threaded cart compute performance
        public static double SerialCartTiming(int numSeeds, int blockSize, bool verbose=false)
        {
            int seedStart = 0;
            double totalTime = 0;
            while (seedStart < numSeeds)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                int seedEnd = seedStart + blockSize;
                if (seedEnd < seedStart) seedEnd = Int32.MaxValue;
                for (int seed = seedStart; seed < seedEnd; seed++)
                {
                    CompressedTravelingCart.GetStock((int)seed);
                }
                seedStart = seedEnd;
                double seconds = stopwatch.Elapsed.TotalSeconds;
                totalTime += seconds;
                if (verbose)
                {
                    double seedsPerSecond = blockSize / seconds;
                    Console.WriteLine(
                        string.Format("{0}  [i:{1}  t:{2}  r:{3}]  [sps:{4}]  [Int32.Max:{5}]",
                            seedEnd.ToString("D10"),
                            seconds.ToString("F2"),
                            totalTime.ToString("F2"),
                            ((numSeeds - seedEnd) / seedsPerSecond).ToString("F2"),
                            seedsPerSecond.ToString("F2"),
                            (Int32.MaxValue / seedsPerSecond).ToString("F2")
                        )
                    );
                }
            }
            return totalTime;
        }

        // used to test sequential read from dump files
        // NOTE: expects DumpCartData.Dump(numSeeds, ...) has previously been run
        // to generate the binary files to read from
        public static double SerialCartRead(uint numSeeds, uint blockSize, bool verbose = false)
        {
            uint seedStart = 0;
            double totalTime = 0;
            while (seedStart < numSeeds)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                uint seedEnd = Math.Min(seedStart + blockSize, Int32.MaxValue);
                AllCartGenerator.GetSeeds((int)seedStart, (int)seedEnd);
                seedStart = seedEnd;
                double seconds = stopwatch.Elapsed.TotalSeconds;
                totalTime += seconds;
                if (verbose)
                {
                    double seedsPerSecond = blockSize / seconds;
                    Console.WriteLine(
                        string.Format("{0}  [i:{1}  t:{2}  r:{3}]  [sps:{4}]  [Int32.Max:{5}]",
                            seedEnd.ToString("D10"),
                            seconds.ToString("F2"),
                            totalTime.ToString("F2"),
                            ((numSeeds - seedEnd) / seedsPerSecond).ToString("F2"),
                            seedsPerSecond.ToString("F2"),
                            (Int32.MaxValue / seedsPerSecond).ToString("F2")
                        )
                    );
                }
            }
            return totalTime;
        }

        // used to test parallel read from dump files
        // TODO: check on thrasing IO when loading dump files
        // NOTE: expects DumpCartData.Dump(numSeeds, ...) has previously been run
        // to generate the binary files to read from
        public static double ParallelCartRead(int numSeeds, int blockSize, bool verbose = false)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var partioner = Partitioner.Create(0, numSeeds, blockSize);
            Parallel.ForEach(partioner, (range, loopState) =>
            {
                var s = AllCartGenerator.GetSeeds(range.Item1, range.Item2);
            });
            double seconds = stopwatch.Elapsed.TotalSeconds;
            if (verbose)
            {
                double seedsPerSecond = numSeeds / seconds;
                Console.WriteLine(
                    string.Format("{0}  [i:{1}]  [sps:{2}]  [Int32.Max:{3}]",
                        numSeeds.ToString("D10"),
                        seconds.ToString("F2"),
                        seedsPerSecond.ToString("F2"),
                        (Int32.MaxValue / seedsPerSecond).ToString("F2")
                    )
                );
            }
            return seconds;
        }
    }
}

