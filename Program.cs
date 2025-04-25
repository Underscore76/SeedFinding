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
using SeedFinding;
using System.Xml.XPath;

namespace SeedFinding
{
	class Program
	{
		static void Main(string[] args)
		{
			//nicer navigation for finding output written files
			string currentPath = Directory.GetCurrentDirectory(); //this kinda fucks up if this isnt in a larger folder, im lazy
			string parentPath = Directory.GetParent(currentPath).FullName;
									
			Directory.CreateDirectory(Path.Join(parentPath,"Output"));
			string writeDirectory = Path.Combine(parentPath,"Output");
			
			// do some speed testing before a long search
			bool runSpeedTest = false;
			// the optimal size for this will vary depending on your system
			int blockSize = 1 << 13;

			// run a search for specific remux bundle set in 1.6
			bool runRemixSearch1_6 = false;

			// run a search for a specific remix bundle set
			bool runRemixSearch = false;

			// search for a TAS vault seed that looks at both forage and cart bundles
			bool runVaultSearch = false;

			// search for a TAS vault seed that is just cart based
			bool runCartSearch = false;
			// generate and store cart seeds with "enough" useful items
			bool skullCavernDepth = false;
			
			bool cartSearchRange = true;
			//
			// Quick and dirty call to specific searches.  Adjust this as needed for your searches.
			
			
			if (skullCavernDepth)
			{
				Game1.UseLegacyRandom = false;
			    int numSeeds = 1 << 10;
			    int day = 14;
			    int endFloor = 77257;
			    List<Tuple<int,List<int>,int>> bestPaths = new();
			    double time = SCDepthB.SearchParallel(numSeeds,numSeeds >> 3,out bestPaths,endFloor,day);
			    Console.WriteLine("{0} seconds taken for {1} seeds",time,numSeeds);
			    Console.WriteLine("{0} seeds per second",numSeeds/time);
			    foreach (Tuple<int,List<int>,int> path in bestPaths)
			    {
			        string floors = string.Join(",",path.Item2);
			        File.AppendAllText(Path.Join(writeDirectory,$"SCDepth{endFloor}-{day}.txt"),path.Item3.ToString() + ":"+ path.Item1.ToString() + ":" + floors + Environment.NewLine);
			    }
			}
			
			if (runRemixSearch1_6)
			{
				bool curate = false;
				Game1.UseLegacyRandom = false;
				List<int> validSeeds = new List<int>();
				RemixCC.Search(int.MaxValue, 1 << 28, out validSeeds, curate);
				foreach (int seed in validSeeds)
				{
					File.AppendAllText(Path.Join(writeDirectory,"RemixCC1_6.txt"), seed.ToString() + Environment.NewLine);
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

