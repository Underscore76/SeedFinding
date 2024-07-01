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

		public QuestResults GetSlayMonsterQuest(int seed, int daysPlayed, int mineLevel)
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
	}
}