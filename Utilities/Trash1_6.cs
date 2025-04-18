using System;
using System.Collections.Generic;
using System.Net;
using SeedFinding.Extensions;
using SeedFinding.Locations;

namespace SeedFinding.Trash1_6
{
    public class Trash
    {
        public enum Can
        {
            Jodi = 0,
            Emily = 1,
            Lewis = 2,
            Museum = 3,
            Clint = 4,
            Gus = 5,
            George = 6,
            Joja = 7,
            Desert = 8
        }

        public struct TrashItem
        {
            public string Id;
            public Can Can;
            public double MinLuck;
            public int Day;
            public TrashItem(string id, Can can, int day = -1, double luck=0.1)
            {
                Id = id;
                Can = can;
                MinLuck = luck;
                Day = day;
            }
            public override string ToString()
            {
                return string.Format("Can: {0}, Name: {1}, Day:{2}, MinLuck:{3}", getCanOwner(Can), Item.Get(Id).Name, Day.ToString(), MinLuck.ToString());
            }
        }

        private static Tile getCanTile(Can can)
        {
            switch (can)
            {
                case Can.Jodi:
                    return new Tile(13, 86);
                    break;
                case Can.Emily:
                    return new Tile(19, 89);
                    break;
                case Can.Lewis:
                    return new Tile(56, 85);
                    break;
                case Can.Museum:
                    return new Tile(108, 91);
                    break;
                case Can.Clint:
                    return new Tile(97, 80);
                    break;
                case Can.Gus:
                    return new Tile(47, 70);
                    break;
                case Can.George:
                    return new Tile(52, 63);
                    break;
                case Can.Joja:
                    return new Tile(110, 56);
                    break;

            }
            return new Tile();
        }

        private static string getCanOwner(Can can)
        {
            switch (can)
            {
                case Can.Jodi:
                    return "JodiAndKent";
                    break;
                case Can.Emily:
                    return "EmilyAndHaley";
                    break;
                case Can.Lewis:
                    return "Mayor";
                    break;
                case Can.Museum:
                    return "Museum";
                    break;
                case Can.Clint:
                    return "Blacksmith";
                    break;
                case Can.Gus:
                    return "Saloon";
                    break;
                case Can.George:
                    return "Evelyn";
                    break;
                case Can.Joja:
                    return "JojaMart";
                    break;
                case Can.Desert:
                    return "DesertFestival";
                    break;

            }
            return "";
        }
        public static HashSet<TrashItem> getAllTrash(int gameId, int day, double luck = 0, bool twentyOneChecked = false, bool fiftyChecked = false, bool desertFestival = false, bool theatre = false, bool hasFurnace = false, bool hasDesert = false, int mines = 0, bool hasBook = false, bool completeCC = false)

        {
            HashSet<TrashItem> results = new HashSet<TrashItem>();

            foreach (Can can in Can.GetValues(typeof(Can))) 
            {
                if (!desertFestival && can == Can.Desert)
                {
                    continue;
                }
                var trash = getTrash(gameId, day, can, luck, twentyOneChecked, fiftyChecked, theatre, hasFurnace, hasDesert, mines, hasBook, completeCC);
                if (trash.Id != "")
                {
                    results.Add(trash);
                }
            }
            return results;
        }
        public static TrashItem getTrash(long gameId, int day, Can can, double luck = 0, bool twentyOneChecked = false, bool fiftyChecked = false, bool theatre = false, bool hasFurnace = false, bool hasDesert = false, int mines = 0, bool hasBook = false,bool completeCC = false)

        {

            float baseChance = 0.2f;

            baseChance += (float)luck;
            if (hasBook)
            {
                baseChance += 0.2f;
            }

            Random garbageRandom = Utility.CreateDaySaveRandom(day, gameId, 777 + Game1.hash.GetDeterministicHashCode(getCanOwner(can)));

            int prewarm = garbageRandom.Next(0, 100);
            for (int k = 0; k < prewarm; k++)
            {
                garbageRandom.NextDouble();
            }
            prewarm = garbageRandom.Next(0, 100);
            for (int j = 0; j < prewarm; j++)
            {
                garbageRandom.NextDouble();
            }

            double roll = garbageRandom.NextDouble();
            double minLuck = roll - 0.2 - (hasBook ? 0.2 : 0);
            bool baseChancePassed = roll < (double)baseChance;

			if (fiftyChecked && garbageRandom.NextDouble() < 0.002)
			{
				// Trash Catalogue
				return new TrashItem("TrashCatalogue", can, day, -0.1);
			}

            if (twentyOneChecked && garbageRandom.NextDouble() < 0.002)
            {
                // Garbage Hat
                return new TrashItem("GarbageHat", can, day, -0.1 );
            }

            // Beans

            double roll2;
            // Specific cans
            switch (can)
            {
                case Can.Clint:
                    roll2 = garbageRandom.NextDouble();
                    if (baseChancePassed && roll2 < (0.2 + luck))
                    {
                        return new TrashItem(new List<string> { "378", "380", "382" }[garbageRandom.Next(3)],can,day, Math.Max(minLuck, roll2 - 0.2));
                    } 
                    break;
                case Can.Emily:
                    break;
                case Can.George:
                    roll2 = garbageRandom.NextDouble();
                    if (baseChancePassed && roll2 < (0.2 + luck))
                    {
                        return new TrashItem("223", can, day, Math.Max(minLuck, roll2 - 0.2));
                    }
                    break;
                case Can.Jodi:
                    break;
                case Can.Joja:
                    if (baseChancePassed)
                    {
                        if (theatre)
                        {
                            Random syncedRandom = Utility.CreateRandom(Game1.hash.GetDeterministicHashCode("garbage_joja"), gameId, day);
                            if (syncedRandom.NextBool(0.2))
                            {
                                return new TrashItem(new List<string> { "809", "270", "270", "270" }[garbageRandom.Next(4)], can, day, minLuck);

                            }
                        }
                        if (!completeCC)
                        {
                            Random syncedRandom = Utility.CreateRandom(Game1.hash.GetDeterministicHashCode("garbage_joja"), gameId, day);
                            if (syncedRandom.NextBool(0.2))
                            {
                                return new TrashItem("167", can, day, minLuck);

                            }
                        }
                    }
                    break;
                case Can.Lewis:
                    break;
                case Can.Museum:
                    if (baseChancePassed)
                    {
                        Random syncedRandom = Utility.CreateRandom(Game1.hash.GetDeterministicHashCode("garbage_museum_535"), gameId, day);
                        roll2 = syncedRandom.NextDouble();
                        if (roll2 < (0.2 + luck))
                        {
                            syncedRandom = Utility.CreateRandom(Game1.hash.GetDeterministicHashCode("garbage_museum_749"), gameId, day);
                            if (syncedRandom.NextBool(0.05))
                            {
                                return new TrashItem("749", can, day, Math.Max(minLuck, roll2 - 0.2));
                            }
                            return new TrashItem("535", can, day, Math.Max(minLuck, roll2 - 0.2));

                        }

                    }
                    break;
                case Can.Gus:
                    if (baseChancePassed)
                    {
                        Random syncedRandom = Utility.CreateRandom(Game1.hash.GetDeterministicHashCode("garbage_saloon_dish"), gameId, day);
                        roll2 = syncedRandom.NextDouble();
                        if (roll2 < (0.2 + luck))
                        {
                            return new TrashItem("DishOfTheDay", can, day, Math.Max(minLuck, roll2 - 0.2));

                        }

                    }
                    break;

                // Desert Cans (Always drops eggs?)

            }
            string item = "";
            if (twentyOneChecked && garbageRandom.NextDouble() < 0.01)
            {
                item = new List<string> { "153", "216", "403", "309", "310", "311", "RANDOM_BASE_SEASON_ITEM" }[garbageRandom.Next(7)];
                if (item == "RANDOM_BASE_SEASON_ITEM")
                {
                    item = Utility.GetRandomItemFromSeason(Utility.getSeasonFromDay(day), false, garbageRandom, 1, hasFurnace, hasDesert, mines).ToString();
                }
            }

            if (baseChancePassed)
            {
                item = new List<string> { "153", "216", "403", "309", "310", "311", "RANDOM_BASE_SEASON_ITEM", "168", "167", "170", "171", "172" }[garbageRandom.Next(12)];
                if (item == "RANDOM_BASE_SEASON_ITEM")
                {
                    item = Utility.GetRandomItemFromSeason(Utility.getSeasonFromDay(day), false, garbageRandom, 1, hasFurnace, hasDesert, mines).ToString();
                }
            }
            return new TrashItem(item, can, day, minLuck);
           
        }
    }
}