using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;

namespace SeedFinding
{
    
    public class Quests
    {
        public enum QuestType
        {
            ResourceCollectionQuest,
            SlayMonsterQuest,
            FishingQuest,
            SocializeQuest,
            ItemDeliveryQuest,
            None

        }
        public struct QuestResults
        {
            public string Person;
            public string Id;
            public QuestType Type;
			public int Amount;
            public QuestResults(string person, string id, QuestType type = QuestType.None, int amount = 1)
            {
                
                Person = person;
                Id = id;
                Type = type;
				Amount = amount;
            }
            public override string ToString()
            {
                return string.Format("{0}: {1}", Item.Get(Id).Name, Person);
            }
        }

		public static QuestResults GetQuest(int gameId, int daysPlayed, bool hasSocialiseQuest = true, int mineLevel = 0, int cookingRecipesKnown=2, List<string> personsKnown = null, bool hasFurnace = false, bool hasDesert = false, int fishingLevel = 0, int miningLevel = 0, int forageLevel = 0 )
		{
			QuestType type = GetQuestType(gameId, daysPlayed, hasSocialiseQuest, mineLevel);
			switch (type)
			{
				case QuestType.ResourceCollectionQuest:
					return GetResourceCollectionQuest(gameId, daysPlayed, mineLevel, miningLevel, forageLevel);
				case QuestType.SocializeQuest:
					return new QuestResults("", "", type);
				case QuestType.SlayMonsterQuest:
					return GetSlayMonsterQuest(gameId, daysPlayed, mineLevel);
				case QuestType.ItemDeliveryQuest:
					return GetItemDeliveryQuest(gameId, daysPlayed, 2, personsKnown, hasFurnace, hasDesert, mineLevel);
				case QuestType.FishingQuest:
					return GetFishingQuest(gameId, daysPlayed, fishingLevel);
				default:
					return new QuestResults("", "", type);

			}
		}

        public static QuestType GetQuestType(int gameId, int daysPlayed, bool hasSocialiseQuest = true, int mineLevel = 0)
        {
            if (daysPlayed <= 1)
            {
                return QuestType.None;
            }
            double d = Utility.CreateDaySaveRandom(daysPlayed, gameId, 100.0, daysPlayed * 777).NextDouble();
            if (d < 0.08)
            {
                return QuestType.ResourceCollectionQuest;
            }
            if (d < 0.2 && mineLevel > 0 && daysPlayed > 5)
            {
                return QuestType.SlayMonsterQuest;
            }
            if (d < 0.5)
            {
                return QuestType.None;
            }
            if (d < 0.6)
            {
                return QuestType.FishingQuest;
            }
            if (d < 0.66 && daysPlayed % 7 == 1)
            {
                if (!hasSocialiseQuest)
                {
                    return QuestType.SocializeQuest;
                }
                return QuestType.ItemDeliveryQuest;
            }
            return QuestType.ItemDeliveryQuest;
        }
        public static QuestResults GetItemDeliveryQuest(int gameId, int daysPlayed, int cookingRecipesKnown, List<string> persons, bool hasFurnace = false, bool hasDesert = false, int mines = 0)
        {
            QuestType questType = GetQuestType(gameId, daysPlayed);
            if (questType != QuestType.ItemDeliveryQuest)
            {
                return new QuestResults("", "");
            }
            Random random = Utility.CreateRandom(gameId, daysPlayed);

            int resource;
            int dayOfMonth = (daysPlayed - 1) % 28 + 1;
            int year = (daysPlayed - 1) / (28 * 4) + 1;

            string target = "";
            string item = "";

            if (persons.Count == 0)
                return new QuestResults("", "");

            target = persons[random.Next(persons.Count)];

            Season season = Utility.ConvertDaysToSeason(daysPlayed);
            if (season != Season.Winter && random.NextDouble() < 0.15)
            {
                List<int> crops = Utility.possibleCropsAtThisTime(season, (dayOfMonth <= 7) ? true : false, year, hasDesert);
                resource = crops.ElementAt(random.Next(crops.Count));
            }
            else
            {
                resource = Utility.getRandomItemFromSeason(season, 1000, true, cookingRecipesKnown, gameId, daysPlayed, true, hasFurnace, hasDesert, mines);
                if (resource == -5)
                {
                    resource = 176;
                }
                if (resource == -6)
                {
                    resource = 184;
                }

            }
            item = resource.ToString();

            return new QuestResults(target, item, questType);
        }
        public static int GetItemDeliveryQuestItem(int gameId, int daysPlayed, int cookingRecipesKnown, bool hasFurnace = false, bool hasDesert = false, int mines = 0)
        {
            Random random = Utility.CreateRandom(gameId, daysPlayed);
            
            int resource;
            int dayOfMonth = (daysPlayed - 1) % 28 + 1;
            int year = (daysPlayed - 1) / (28 * 4) + 1;

            // Choose person
            random.NextDouble();

            Season season = Utility.ConvertDaysToSeason(daysPlayed);
            if (!(season == Season.Winter) && random.NextDouble() < 0.15)
            {
                List<int> crops = Utility.possibleCropsAtThisTime(season, (dayOfMonth <= 7) ? true : false, year, hasDesert);
                resource = crops.ElementAt(random.Next(crops.Count));
            }
            else
            {
               
                resource = Utility.GetRandomItemFromSeason(season, 1000, true, cookingRecipesKnown, gameId, daysPlayed, true, hasFurnace, hasDesert, mines);
                if (resource == -5)
                {
                    resource = 176;
                }
                if (resource == -6)
                {
                    resource = 184;
                }

            }
            return resource;
        }

		public static QuestResults GetSlayMonsterQuest(int seed, int daysPlayed, int mineLevel)
		{
			Random random = Utility.CreateRandom(seed, daysPlayed);
			string target;
			string item;
			for (int i = 0; i < random.Next(1, 100); i++)
			{
				random.Next();
			}

			List<string> possibleMonsters = new List<string>();
			if (mineLevel < 39)
			{
				possibleMonsters.Add("Green Slime");
				if (mineLevel > 10)
				{
					possibleMonsters.Add("Rock Crab");
				}
				if (mineLevel > 30)
				{
					possibleMonsters.Add("Duggy");
				}
			}
			else if (mineLevel < 79)
			{
				possibleMonsters.Add("Frost Jelly");
				if (mineLevel > 70)
				{
					possibleMonsters.Add("Skeleton");
				}
				possibleMonsters.Add("Dust Spirit");
			}
			else
			{
				possibleMonsters.Add("Sludge");
				possibleMonsters.Add("Ghost");
				possibleMonsters.Add("Lava Crab");
				possibleMonsters.Add("Squid Kid");
			}

			item = possibleMonsters.ElementAt(random.Next(possibleMonsters.Count));
			int numberToKill = 0;
			switch (item)
			{
				case "Green Slime":
					numberToKill = random.Next(4, 11);
					numberToKill = (int)numberToKill - (int)numberToKill % 2;
					break;
				case "Rock Crab":
					numberToKill = random.Next(2, 6);
					break;
				case "Duggy":
					target = "Clint";
					numberToKill = random.Next(2, 4);
					break;
				case "Frost Jelly":
					numberToKill = random.Next(4, 11);
					numberToKill = (int)numberToKill - (int)numberToKill % 2;
					break;
				case "Ghost":
					numberToKill = random.Next(2, 4);
					break;
				case "Sludge":
					numberToKill = random.Next(4, 11);
					numberToKill = (int)numberToKill - (int)numberToKill % 2;
					break;
				case "Lava Crab":
					numberToKill = random.Next(2, 6);
					break;
				case "Squid Kid":
					numberToKill = random.Next(1, 3);
					break;
				case "Skeleton":
					numberToKill = random.Next(6, 12);
					break;
				case "Dust Spirit":
					numberToKill = random.Next(10, 21);
					break;
				default:
					numberToKill = random.Next(3, 7);
					break;
			}

			if (item.Equals("Green Slime") || item.Equals("Frost Jelly") || item.Equals("Sludge"))
			{
				target = "Lewis";
			}
			else if (item.Equals("Rock Crab") || item.Equals("Lava Crab"))
			{
				target = "Demetrius";
			}
			else
			{
				target = "Wizard";
			}

			return new QuestResults(target, item, QuestType.ItemDeliveryQuest, numberToKill);
		}

		private static QuestResults GetFishingQuest(int seed, int daysPlayed, int fishingLevel = 0)
		{
			Random random;
			//if (Game1.version >= Game1.version1_6)
			//{
				random = Utility.CreateRandom(seed, daysPlayed);
			//}
			//else
			//{
			//	random = new Random(seed + daysPlayed);
			//}
			int whichFish = 0;
			int numberToFish = 0;
			string target = "";

			Season season = Utility.ConvertDaysToSeason(daysPlayed);
			if (random.NextDouble() < 0.5)
			{
				switch (season)
				{
					case Season.Spring:
						{
							int[] possiblefish2 = new int[8]
							{
						129,
						131,
						136,
						137,
						142,
						143,
						145,
						147
							};
							whichFish = possiblefish2[random.Next(possiblefish2.Length)];
							break;
						}
					case Season.Summer:
						{
							int[] possiblefish2 = new int[10]
							{
						130,
						131,
						136,
						138,
						142,
						144,
						145,
						146,
						149,
						150
							};
							whichFish = possiblefish2[random.Next(possiblefish2.Length)];
							break;
						}
					case Season.Fall:
						{
							int[] possiblefish2 = new int[8]
							{
						129,
						131,
						136,
						137,
						139,
						142,
						143,
						150
							};
							whichFish = possiblefish2[random.Next(possiblefish2.Length)];
							break;
						}
					case Season.Winter:
						{
							int[] possiblefish2 = new int[9]
							{
						130,
						131,
						136,
						141,
						144,
						146,
						147,
						150,
						151
							};
							whichFish = possiblefish2[random.Next(possiblefish2.Length)];
							break;
						}
				}
				//if (Game1.version >= Game1.version1_6)
				//{

					numberToFish = (int)Math.Ceiling(90.0 / (double)Math.Max(1, Item.Get(whichFish.ToString()).Price)) + fishingLevel / 5;
				//}
				//else
				//{
				//	int price = int.Parse(Game1.fish[whichFish.ToString()].Split('/')[1]);
				//	numberToFish = (int)Math.Ceiling(90.0 / (double)Math.Max(1, price)) + (int)nbrFishing.Value / 5;
				//}
				target = "Demetrius";
			}
			else
			{
				switch (season)
				{
					case Season.Spring:
						{
							int[] possiblefish = new int[9]
							{
						129,
						131,
						136,
						137,
						142,
						143,
						145,
						147,
						702
							};
							whichFish = possiblefish[random.Next(possiblefish.Length)];
							break;
						}
					case Season.Summer:
						{
							int[] possiblefish = new int[12]
							{
						128,
						130,
						131,
						136,
						138,
						142,
						144,
						145,
						146,
						149,
						150,
						702
							};
							whichFish = possiblefish[random.Next(possiblefish.Length)];
							break;
						}
					case Season.Fall:
						{
							int[] possiblefish = new int[11]
							{
						129,
						131,
						136,
						137,
						139,
						142,
						143,
						150,
						699,
						702,
						705
							};
							whichFish = possiblefish[random.Next(possiblefish.Length)];
							break;
						}
					case Season.Winter:
						{
							int[] possiblefish = new int[13]
							{
						130,
						131,
						136,
						141,
						143,
						144,
						146,
						147,
						150,
						151,
						699,
						702,
						705
							};
							whichFish = possiblefish[random.Next(possiblefish.Length)];
							break;
						}
				}
				target = "Willy";
				int price = Item.Get(whichFish.ToString()).Price;
				numberToFish = (int)Math.Ceiling(90.0 / (double)Math.Max(1, price)) + fishingLevel / 5;
			}

			return new QuestResults(target, Item.Get(whichFish.ToString()).Name, QuestType.FishingQuest, numberToFish);
		}

		private static QuestResults GetResourceCollectionQuest(int seed, int daysPlayed, int mineLevel = 0, int miningLevel = 0, int forageLevel = 0)
		{
			Random random;
			//if (Game1.version >= Game1.version1_6)
			//{
				random = Utility.CreateRandom(seed, daysPlayed);
			//}
			//else
			//{
			//	random = new Random(seed + daysPlayed);
			//}

			int resource;
			int number = 0;
			string target = "";
			string item = "";

			resource = random.Next(6) * 2;
			for (int i = 0; i < random.Next(1, 100); i++)
			{
				random.Next();
			}
			int highest_mining_level = miningLevel;
			int highest_foraging_level = forageLevel;
			switch (resource)
			{
				case 0:
					resource = 378;
					number = 20 + highest_mining_level * 2 + random.Next(-2, 4) * 2;
					number = (int)number - (int)number % 5;
					target = "Clint";
					break;
				case 2:
					resource = 380;
					number = 15 + highest_mining_level + random.Next(-1, 3) * 2;
					number = (int)((float)(int)number * 0.75f);
					number = (int)number - (int)number % 5;
					target = "Clint";
					break;
				case 4:
					resource = 382;
					number = 10 + highest_mining_level + random.Next(-1, 3) * 2;
					number = (int)((float)(int)number * 0.75f);
					number = (int)number - (int)number % 5;
					target = "Clint";
					break;
				case 6:
					resource = ((mineLevel > 40) ? 384 : 378);
					number = 8 + highest_mining_level / 2 + random.Next(-1, 1) * 2;
					number = (int)((float)(int)number * 0.75f);
					number = (int)number - (int)number % 2;
					target = "Clint";
					break;
				case 8:
					resource = 388;
					number = 25 + highest_foraging_level + random.Next(-3, 3) * 2;
					number = (int)number - (int)number % 5;
					target = "Robin";
					break;
				case 10:
					resource = 390;
					number = 25 + highest_mining_level + random.Next(-3, 3) * 2;
					number = (int)number - (int)number % 5;
					target = "Robin";
					break;
			}
			if (target == null)
			{
				return new QuestResults("", "");
			}
			return new QuestResults(target, Item.Get(resource.ToString()).Name, QuestType.ResourceCollectionQuest, number);
		}
	}
}