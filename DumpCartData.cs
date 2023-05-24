using System;
using SeedFinding.Cart;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SeedFinding
{
	public class DumpCartData
	{
        // dumps seeds to file in blockSize chunks
        static double Dump(uint numSeeds, uint blockSize, bool verbose = false)
        {
            uint seedStart = 0;
            double totalTime = 0;
            while (seedStart < numSeeds)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                uint seedEnd = Math.Min(seedStart + blockSize, Int32.MaxValue);
                AllCartGenerator.DumpSeeds((int)seedStart, (int)seedEnd);
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

        // dumps seeds to file using parallel threads to dump blocks
        static double ParallelDump(int numSeeds, int blockSize, bool verbose = false)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var partioner = Partitioner.Create(0, numSeeds, blockSize);
            Parallel.ForEach(partioner, (range, loopState) =>
            {
                AllCartGenerator.DumpSeeds(range.Item1, range.Item2);
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

