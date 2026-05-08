using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SeedFinding
{
	public class VolcanoSearch
	{

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

		public static List<(int, int, int)> floorSearch(int seed)
		{
			List<(int, int, int)> Results = new();
			string result = "";
			int layout;
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
				layout = Volcano.Volcano.GetSingleLevelSeed(seed, level, 0, specialExists:true,luckLevel:6,shortcutUnlocked:true);
				if (layout == 46)
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
						//Console.WriteLine(tup);
					}
				}
			}
			return Results;
		}
		public static double Search(int start, int end, int blockSize)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			var bag = new ConcurrentBag<int>();
			var partioner = Partitioner.Create(start, end, blockSize);
			//(int, int) range = (start, end);
			Parallel.ForEach(partioner, (range, loopState) =>
			{
				//Location woods = null;
				//for (int seed = start; seed < end; seed++)
				for (int seed = (int)range.Item1; seed < range.Item2; seed++)
				{
					if (seed % 2 == 0) { continue; }

					//Console.WriteLine(seed);
					List<(int, int, int)> Results = floor1Search(seed);

					foreach (var tup in Results)
					{
						File.AppendAllText("Volcano1Search.txt", $"{tup},");
						Console.WriteLine(tup);
					}
				}
			}
			);
			double seconds = stopwatch.Elapsed.TotalSeconds;
			return seconds;
		}
	}
}
