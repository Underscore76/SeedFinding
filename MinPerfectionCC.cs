using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedFinding
{
	public class MinPerfectionCC
	{
		public static bool ValidSeed(int gameId, bool report = false)
		{
			SpecialOrders.sewingMachineAccess = true;
			SpecialOrders.islandAccess = false;
			SpecialOrders.resortAccess = false;

			List<string> radioQuests = new() { "QiChallenge9", "QiChallenge10" };
			List<string> neededQuests = new() { "Caroline", "Clint", "Gunther", "Linus", "Robin2", "Willy", "Wizard", "Wizard2" };
			List<string> completed = new();
			bool hasDemetrius = false;
			bool hasRadio = false;
			for (int day = 57; day < 114; day += 7)
			{
				if (day==78) SpecialOrders.islandAccess=true;
				if (day==92) SpecialOrders.resortAccess=true;
				var results = SpecialOrders.GetOrders(gameId, day, completedOrders: completed);
				bool found = false;
				foreach (var result in results)
				{
					if (found && hasRadio)
					{
						break;
					}
					if (!found)
					{
						if (neededQuests.Contains(result.questKey))
						{
							completed.Add(result.questKey);
							neededQuests.Remove(result.questKey);
							found = true;
							continue;
						}

						if (!hasDemetrius && result.questKey.Contains("Demetrius"))
						{
							hasDemetrius = true;
							found = true;
							continue;
						}
					}

					if (!hasRadio && day>91)
					{
						if (radioQuests.Contains(result.questKey))
						{
							hasRadio = true;
							continue;
						}
					}
				}

				if (!found)
				{
					return false;
				}
			}
			if (neededQuests.Count > 0 || !hasDemetrius || !hasRadio)
			{
				return false;
			}
			return true;
		}

		public static double Search(int startId, int endId, int blockSize, out List<int> validSeeds)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			var bag = new ConcurrentBag<int>();
			var partioner = Partitioner.Create(startId, endId, blockSize);
			//for(int seed = startId; seed < endId; seed++) { 
			Parallel.ForEach(partioner, (range, loopState) =>
			{
				for (int seed = range.Item1; seed < range.Item2; seed++)
				{
					if (ValidSeed(seed))
					{
						bag.Add(seed);
						Console.WriteLine(seed);
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
