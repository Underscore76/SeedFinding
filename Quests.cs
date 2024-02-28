using System.Collections.Generic;
using System;
using System.Linq;

namespace SeedFinding
{
    
    public class Quests
    {
        public static int GetItemDeliveryQuestItem(int gameId, int daysPlayed, int cookingRecipesKnown, bool hasFurnace = false, bool hasDesert = false, int mines = 0)
        {
            Random random;
            //if (Game1.version.StartsWith("1.6"))
            //{
            //    random = Utility.CreateRandom(seed, daysPlayed);
            //}
            //else
            //{
                random = new Random(gameId + daysPlayed);
            //}
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

        
    }
}