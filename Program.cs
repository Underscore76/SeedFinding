using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Diagnostics;
using System.IO;

using SeedFinding.Cart;
using SeedFinding.Bundles;
using SeedFinding.Locations;
using SeedFinding.Trash;

namespace SeedFinding
{
    class Program
    {
        static void Main(string[] args)
        {
            // do some speed testing before a long search
            bool runSpeedTest = false;
            // the optimal size for this will vary depending on your system
            int blockSize = 1 << 16;

            // run a search for a specific remix bundle set
            bool runRemixSearch = false;

            // search for a TAS vault seed that looks at both forage and cart bundles
            bool runVaultSearch = false;

            // search for a TAS vault seed that is just cart based
            bool runCartSearch = false;

            // Quick and dirty call to specific searches.  Adjust this as needed for your searches.
            if (true)
            {
                //DynamicCCRemixSeeding.Curate();
                //return;
                FileStream fs = new FileStream("Output.txt", FileMode.Create);
                // First, save the standard output.
                TextWriter tmp = Console.Out;
                StreamWriter sw = new StreamWriter(fs);
                Console.SetOut(sw);
                int numSeeds = Int32.MaxValue;
                double time = DynamicCCRemixSeeding.Search(numSeeds, blockSize, out List<int> validSeeds);
                foreach (var item in validSeeds)
                {
                    Console.WriteLine(item);
                }
                Console.Read();
            }
            return;


            Console.WriteLine($"Start: {DateTime.Now}");
            if (runSpeedTest)
            {
                int numSeeds = 1 << 24;
                Console.WriteLine($"Estimating Remix Bundle Speed using {numSeeds} seeds");
                double time = RemixSeeding.Search(numSeeds, blockSize, out var _);
                Console.WriteLine($"Total Time: {time} (sps: {numSeeds / time})");
                Console.WriteLine($"Estimated +int32 Scan Time: {(Int32.MaxValue / (float)numSeeds) * time}");

                Console.WriteLine($"Estimating Cart Compute Speed using {numSeeds} seeds");
                time = VaultCartSeeding.Search(numSeeds, blockSize, out var _);
                Console.WriteLine($"Total Time: {time} (sps: {numSeeds / time})");
                Console.WriteLine($"Estimated +int32 Scan Time: {(Int32.MaxValue / (float)numSeeds) * time}");
            }

            if (runRemixSearch)
            {
                FileStream fs = new FileStream("Remix.txt", FileMode.Create);
                // First, save the standard output.
                TextWriter tmp = Console.Out;
                StreamWriter sw = new StreamWriter(fs);
                Console.SetOut(sw);
                Console.WriteLine("RemixSearch");
                int numSeeds = Int32.MaxValue;
                double time = DynamicCCRemixSeeding.Search(numSeeds, blockSize, out List<int> validSeeds);
                foreach (var item in validSeeds)
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine($"Total Time: {time} (sps: {numSeeds / time})");
                Console.SetOut(tmp);
                sw.Close();
            }

            if (runVaultSearch)
            {
                Console.WriteLine("TAS Vault Seed Searching");
                VaultSeeding.Run();
            }

            if (runCartSearch)
            {
                Console.WriteLine("Full cart seed search");
                int numSeeds = Int32.MaxValue;
                double time = VaultCartSeeding.Search(numSeeds, blockSize, out List<int> validSeeds);
                Console.WriteLine($"Total Time: {time} (sps: {numSeeds / time})");
            }

        }
    }
}

