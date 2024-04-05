using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Diagnostics;
using System.IO;

using SeedFinding.Cart1_6;
using SeedFinding.Bundles;
using SeedFinding.Locations;
using SeedFinding.Trash1_6;
using Newtonsoft.Json;
using static SeedFinding.ObjectInfo;
using System.Resources;
using StardewValley;

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

            // run a search for specific remux bundle set in 1.6
            bool runRemixSearch1_6 = true;

            // run a search for a specific remix bundle set
            bool runRemixSearch = false;

            // search for a TAS vault seed that looks at both forage and cart bundles
            bool runVaultSearch = false;

            // search for a TAS vault seed that is just cart based
            bool runCartSearch = false;

            //
            bool runBoilerSearch = false;

            // Quick and dirty call to specific searches.  Adjust this as needed for your searches.
            if (true)
            {
                //Console.WriteLine(Game1.hash.GetDeterministicHashCode("skillBook_Traveler"));
                //return;
                Game1.UseLegacyRandom = true;
                //Console.WriteLine(String.Join(",",new List<string>(TravelingCart.GetStock(359003761, 5).Select(o=>Item.Get(o.Id).Name))));
                //Console.WriteLine(String.Join(",", Trash1_6.Trash.getAllTrash(48462440, 12, 0.1)));
                //Console.WriteLine(Trash1_6.Trash.getTrash(48462440, 12, Trash1_6.Trash.Can.George, -0.054).ToString());
                //return;
                /*foreach(var item in TravelingCart.GetStockNew(168, 5))
                {
                    Console.WriteLine(item.ToString());
                }
                return;*/
                // Console.WriteLine(String.Join(",",new List<string>(TravelingCart.GetStock(184400, 5).Select(o=>Item.Get(o.Id).Name))));
                // return;

                //Trash1_6.Trash.getAllTrash(34676331, 14, 0.1, hasFurnace: true);
                BoilerRoomClassic.Curate();
                return;

                //FileStream fs = new FileStream("BoilerRoom.txt", FileMode.Create);
                // First, save the standard output.
                //TextWriter tmp = Console.Out;
                //StreamWriter sw = new StreamWriter(fs);
                //Console.SetOut(sw);
                int numSeeds = Int32.MaxValue;
                double time = BoilerRoomClassic.Search(1891000000 + 1, numSeeds, blockSize, out List<int> validSeeds);
                foreach (var item in validSeeds)
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine($"Total Time: {time} (sps: {numSeeds / time})");
               // Console.SetOut(tmp);
               // sw.Close();
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

            if (runRemixSearch1_6)
            {
                bool curate = true;
                Game1.UseLegacyRandom = true;
                List<int> validSeeds = new List<int>();
                RemixCC.Search(100000, 1 << 16, out validSeeds, curate);
                foreach (int seed in validSeeds)
                {
                    File.AppendAllText("RemixCC1_6.txt", seed.ToString() + Environment.NewLine);
                }
                return;
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

            if (runBoilerSearch)
            {

                Game1.UseLegacyRandom = true;

                //Console.WriteLine(String.Join(",",new List<string>(TravelingCart.GetStock(359003761, 5).Select(o=>Item.Get(o.Id).Name))));
                //Console.WriteLine(String.Join(",", Trash1_6.Trash.getAllTrash(48462440, 12, 0.1)));
                //Console.WriteLine(Trash1_6.Trash.getTrash(48462440, 12, Trash1_6.Trash.Can.George, -0.054).ToString());
                //return;
                /*foreach(var item in TravelingCart.GetStockNew(168, 5))
                {
                    Console.WriteLine(item.ToString());
                }
                return;*/
                // Console.WriteLine(String.Join(",",new List<string>(TravelingCart.GetStock(184400, 5).Select(o=>Item.Get(o.Id).Name))));
                // return;

                //BoilerRoomClassic.ValidSeed(9110854);
                //return;

                //FileStream fs = new FileStream("BoilerRoom.txt", FileMode.Create);
                // First, save the standard output.
                //TextWriter tmp = Console.Out;
                //StreamWriter sw = new StreamWriter(fs);
                //Console.SetOut(sw);
                int numSeeds = Int32.MaxValue;
                double time = BoilerRoomClassic.Search(-1 + 1, numSeeds, blockSize, out List<int> validSeeds);
                foreach (var item in validSeeds)
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine($"Total Time: {time} (sps: {numSeeds / time})");
                // Console.SetOut(tmp);
                // sw.Close();
            }

            return;

        }
    }
}

