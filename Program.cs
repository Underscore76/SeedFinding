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
using SeedFinding.Locations1_6;

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
            bool runRemixSearch1_6 = false;

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

				Game1.UseLegacyRandom = true;
				uint numSeeds = UInt32.MaxValue;
				uint start = 0;
				//KrobusSeeding.CheckSeed(666262426, true);
				//return;
				//for (int seed2 = 0; seed2 < 100000; seed2++)
				//{
				//KrobusSeeding.CheckSeed(2093299582, true);
				//}
				//return;
				//KrobusSeeding.Curate();
				//return;		
				//KrobusSeeding.ExploreSeed(3848814016, 39, 58);
				//KrobusSeeding.CheckSeed(3848814004, true);
				//KrobusSeeding.ExploreSeed(3848814004, 45, 64);
				//Locations1_6.Location.digUpArtifactSpot(5, 3848814004, "Town", 70, 75);
				//Console.WriteLine(KrobusSeeding.GeodeOpening(3848814004, true));
				return;
				KrobusSeeding.Search(start, numSeeds, blockSize, out List<long> validSeeds);
				return;
				uint seed = 379647118;

				Game1.UseLegacyRandom = true;
				int startingValue = 655568;
				for (int i = startingValue; i <= int.MaxValue; i++)
				{
					if (MAXPERFECTION.ValidSeed(i))
					{
						Console.WriteLine(i);
						return;
					}
				}
				//Console.WriteLine(Game1.hash.GetDeterministicHashCode("location_weather"));
				//Console.WriteLine(Utility.CreateRandom(15120335, 18).NextDouble());
				return;
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
                //Console.WriteLine(StepPredictions.Predict(371897450, 12, 24, new List<string>(){"Lewis","Robin"}));
                //Console.WriteLine(Weather.getWeather(48, 483662850));
                //Console.WriteLine(Weather.getWeather(49, 483662850));
                //Console.WriteLine(StepPredictions.Predict(483662850, 3, 13, new List<string>() { "One"}));
                //Console.WriteLine(NightEvents1_6.NightEvent.GetEvent(189726570, 17));
                MarriageSpeedrun.Curate();
                //MarriageSpeedrun.ValidSeedv2(1598879837, true);
                //Console.WriteLine(Trash1_6.Trash.getTrash(833453617, 2, Trash1_6.Trash.Can.Gus, 0.1));

                //SpecialOrders.islandAccess = true;
                //Console.WriteLine(String.Join(",", SpecialOrders.GetOrders(1, 1, "1.6", null, new List<string>() { "Pierre" }).Select(o=>o.name)));
                //Locations1_6.Map map = JsonConvert.DeserializeObject<Locations1_6.Map>(File.ReadAllText(@"Locations1_6/Beach.json"));
                //Console.WriteLine(String.Join(", ", Bubbles.Predict(map, 1, 1)));
                return;

                //FileStream fs = new FileStream("BoilerRoom.txt", FileMode.Create);
                // First, save the standard output.
                //TextWriter tmp = Console.Out;
                //StreamWriter sw = new StreamWriter(fs);
                //Console.SetOut(sw);
                //int numSeeds = Int32.MaxValue;
                //double time = MarriageSpeedrun.Search(-1 + 1, numSeeds, blockSize, out List<int> validSeeds);
                foreach (var item in validSeeds)
                {
                    Console.WriteLine(item);
                }
                //Console.WriteLine($"Total Time: {time} (sps: {numSeeds / time})");
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
                uint numSeeds = Int32.MaxValue;
                double time = DynamicCCRemixSeeding.Search(numSeeds, blockSize, out List<uint> validSeeds);
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

