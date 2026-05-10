using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SeedFinding.Volcano;

namespace SeedFinding
{
	public class VolcanoSearch
	{

		public static List<(int, int, int, int, int)> BasicEval(int level, int seed, int lastLayout = 0)
		{
			List<(int, int, int, int, int)> Results = new();
			int layout = Volcano.Volcano.GetSingleLevelSeed(
				seed,
				level,
				lastLayout,
				dailyLuck: Volcano.Volcano.dailyLuck,
				specialExists: false,
				luckLevel: Volcano.Volcano.luckLevel,
				shortcutUnlocked: false
			);
			Volcano.VolcanoFloor floor = new Volcano.VolcanoFloor(level, layout, seed);

			int total = 0;
			int count = 0;
			foreach (var kvp in floor.monsters)
			{
				if (kvp.Value == "(O)Magma Sprite" || kvp.Value == "(O)Magma Sparker")
				{
					count++;
				}
				Console.WriteLine($"Monster {kvp.Value} at {kvp.Key}");
				total++;
			}
			foreach (var loc in floor.barrelLocations)
			{
				Console.WriteLine($"Barrel at {loc}");
			}
			var tup = (level, seed, layout, total, count);
			Results.Add(tup);
			return Results;
		}

		public static List<(int, int, int)> floor1Search(int seed)
		{
			List<(int, int, int)> Results = new();
			int level = 2;
			int layout = Volcano.Volcano.GetSingleLevelSeed(seed, level, 0, specialExists: false, luckLevel: 6, shortcutUnlocked: true);

			if (layout != 37)
			{
				return Results;
			}
			else
			{
				Volcano.VolcanoFloor floor = new Volcano.VolcanoFloor(level, layout, seed);
				int count = 0;
				if (floor.monsterCounts.ContainsKey("(O)Magma Sprite"))
				{
					count += floor.monsterCounts["(O)Magma Sprite"];
				}
				if (floor.monsterCounts.ContainsKey("(O)Magma Sparker"))
				{
					count += floor.monsterCounts["(O)Magma Sparker"];
				}
				if (count >= 25)
				{
					var tup = (level, seed, count);
					Results.Add(tup);
				}
				return Results;
			}
		}

		public static List<(int, int, int, int)> floorSearch(int seed)
		{
			List<(int, int, int, int)> Results = new();
			string result = "";
			int modResult;
			// Evaluate floor 9
			// Suspect we won't need floor 9 - will do this search later.

			// Evaluate floor 1 - 8
			for (int level = 1; level <= 8; level++)
			{
				if (level == 5)
				{
					continue;
				}
				List<int> layouts = new() {
					Volcano.Volcano.GetSingleLevelSeed(seed, level, 0, specialExists:true,luckLevel:6,shortcutUnlocked:true),
					Volcano.Volcano.GetSingleLevelSeed(seed, level, 0, specialExists:false,luckLevel:6,shortcutUnlocked:true),
					Volcano.Volcano.GetSingleLevelSeed(seed, level, 0, specialExists:false,luckLevel:6,shortcutUnlocked:false),
					// Don't care about this scenario - can't get junimo layout or monster layout with it Volcano.Volcano.GetSingleLevelSeed(seed, level, 0, specialExists:true,luckLevel:6,shortcutUnlocked:false)
				};
				foreach (int layout in layouts)
				{
					//layout = Volcano.Volcano.GetSingleLevelSeed(seed, level, 0, specialExists:true,luckLevel:6,shortcutUnlocked:true);
					if (layout == 46 || layout == 37)
					{
						Volcano.VolcanoFloor floor = new Volcano.VolcanoFloor(level, layout, seed);
						int count = 0;
						if (floor.monsterCounts.ContainsKey("(O)Magma Sprite"))
						{
							count += floor.monsterCounts["(O)Magma Sprite"];
						}
						if (floor.monsterCounts.ContainsKey("(O)Magma Sparker"))
						{
							count += floor.monsterCounts["(O)Magma Sparker"];
						}
						if (layout == 46 && count >= 25 || layout == 37 && count >= 20)
						{
							var tup = (level, seed, layout, count);

							Results.Add(tup);
							//Console.WriteLine(tup);
						}
					}
				}
			}
			return Results;
		}


		public static double Search(int start, int end)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			var bag = new ConcurrentBag<int>();
			//(int, int) range = (start, end);
			var options = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
			Parallel.For(start, end, options, (seed) =>
			{
				if (seed % 2 == 0) { return; }

				//Console.WriteLine(seed);
				List<(int, int, int, int)> Results = floorSearch(seed);

				if (Results.Count > 0)
				{
					string result = string.Join("\n",
						Results.Select(tup => $"{{\"level\":{tup.Item1},\"seed\":{tup.Item2},\"layout\":{tup.Item3},\"count\":{tup.Item4}}}")
					);
					Console.WriteLine(result);
					File.AppendAllText("VolcanoSearch.txt", result + "\n");
				}
			});
			double seconds = stopwatch.Elapsed.TotalSeconds;
			return seconds;
		}
	}
}
