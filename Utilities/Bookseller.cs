using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedFinding.Utilities
{
	public class Bookseller
	{
		public static bool isBooksellerDay(int seed, int day)
		{
			int year = Utility.getYearFromDay(day);
			Season season = Utility.getSeasonFromDay(day);
			return getBooksellerDays(seed, year, season).Contains(Utility.getDayOfMonthFromDay(day));
		}

		public static List<int> getBooksellerDays(int seed, int year, Season season)
		{
			int seasonIndex = (int)season;
			Random r = Utility.CreateRandom(year * 11, seed, seasonIndex);
			int[] possible_days = null;
			List<int> days = new List<int>();
			switch (Game1.season)
			{
				case Season.Spring:
					possible_days = new int[5] { 11, 12, 21, 22, 25 };
					break;
				case Season.Summer:
					possible_days = new int[5] { 9, 12, 18, 25, 27 };
					break;
				case Season.Fall:
					possible_days = new int[8] { 4, 7, 8, 9, 12, 19, 22, 25 };
					break;
				case Season.Winter:
					possible_days = new int[6] { 5, 11, 12, 19, 22, 24 };
					break;
			}
			int index1 = r.Next(possible_days.Length);
			days.Add(possible_days[index1]);
			days.Add(possible_days[(index1 + possible_days.Length / 2) % possible_days.Length]);

			return days;
		}
		public static List<Item> getBooksellerStock(int seed, int day)
		{
			if (!isBooksellerDay(seed, day))
			{
				return null;
			}
			List<Item> stock = new List<Item>();
			Random rng = Utility.CreateDaySaveRandom(day, seed);
			string book;
			List<string> available = new List<string>() { "(O)SkillBook_0",
				  "(O)SkillBook_1",
				  "(O)SkillBook_2",
				  "(O)SkillBook_3",
				  "(O)SkillBook_4"};
			if (Utility.CreateRandom(Game1.hash.GetDeterministicHashCode("purplebookSale"), seed, day).NextDouble() < 0.25)
			{
				stock.Add(Item.Get("PurpleBook"));
			}
			if (Utility.CreateRandom(Game1.hash.GetDeterministicHashCode("thirdBookSale"), seed, day).NextDouble() < 0.6)
			{
				book = available[rng.Next(available.Count)];
				stock.Add(Item.Get(book));
				available.Remove(book);
			}
			if (Utility.CreateRandom(Game1.hash.GetDeterministicHashCode("secondBookSale"), seed, day).NextDouble() < 0.8)
			{
				book = available[rng.Next(available.Count)];
				stock.Add(Item.Get(book));
				available.Remove(book);
			}


			book = available[rng.Next(available.Count)];
			stock.Add(Item.Get(book));
			available.Remove(book);

			if (Utility.getYearFromDay(day) > 3)
			{
				stock.Add(Item.Get(new List<string>()
				{"(O)Book_Trash",
		  "(O)Book_Crabbing",
		  "(O)Book_Bombs",
		  "(O)Book_Roe",
		  "(O)Book_WildSeeds",
		  "(O)Book_Woodcutting",
		  "(O)Book_Defense",
		  "(O)Book_Friendship",
		  "(O)Book_Void",
		  "(O)Book_Marlon",
		  "(O)Book_Artifact" }[rng.Next(11)]));
			}

			return stock;
		}
	}
}
