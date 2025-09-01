using System;
using System.Collections.Generic;
using System.Net;
using SeedFinding.Locations;

namespace SeedFinding.Utilities
{
    public class Trash_new
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
        private static string getCanOwner(Can can) //for 1.6 trash seeding,desert can is irrelevent enough idc
        {
            return can switch
            {
                Can.Jodi => "JodiAndKent",
                Can.Emily => "EmilyAndHaley",
                Can.Lewis => "Mayor",
                Can.Museum => "Museum",
                Can.Clint => "Blacksmith",
                Can.Gus => "Saloon",
                Can.George => "Evelyn",
                Can.Joja => "JojaMart",
                _ => "",
            };
        }
        private static Tile getCanTile(Can can)
        {
            return can switch //for 1.5 trash seeding
            {
                Can.Jodi => new(13, 86),
                Can.Emily => new(19, 89),
                Can.Lewis => new(56, 85),
                Can.Museum => new(108, 91),
                Can.Clint => new(97, 80),
                Can.Gus => new(47, 70),
                Can.George => new(52, 63),
                Can.Joja => new(110, 56),
                _ => new Tile(),
            };
        }
        public static HashSet<TrashItem> GetAllTrash(uint gameId, int day, double luck = 0, int numCansChecked = 0, bool theatre = false,
         bool hasFurnace = false, bool hasDesert = false, int mines = 0, bool hasBook = false,bool completeCC = false)
         {
            HashSet<TrashItem> results = new();
             foreach (Can can in Enum.GetValues(typeof(Can)))
             {
                 var trash = GetTrash(gameId,day,can,luck,numCansChecked,theatre,hasFurnace,hasDesert,mines,hasBook,completeCC);
                 if (trash.Id != "") results.Add(trash);
             }
             return results;
         }
        public static TrashItem GetTrash(uint gameId, int day, Can can, double luck = 0, int numCansChecked = 0, bool theatre = false,
         bool hasFurnace = false, bool hasDesert = false, int mines = 0, bool hasBook = false,bool completeCC = false)
         {
            //unchecked if 1.4 is different
            Func<uint,int,Can,double,int,bool,bool,bool,int,bool,bool,TrashItem> getTrash = (Game1.Version < Game1.Version1_6)? GetTrash1_5 : GetTrash1_6;
            return getTrash(gameId,day,can,luck,numCansChecked,theatre,hasFurnace,hasDesert,mines,hasBook,completeCC);
         }
        
        public static TrashItem GetTrash1_6(uint gameId, int day, Can can, double luck = 0, int numCansChecked = 0, bool theatre = false,
         bool hasFurnace = false, bool hasDesert = false, int mines = 0, bool hasBook = false,bool completeCC = false)
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

            if ((numCansChecked > 20) && garbageRandom.NextDouble() < 0.002) return new("GarbageHat", can, day, -0.1); //garbage hat
            if ((numCansChecked > 50) && garbageRandom.NextDouble() < 0.002) return new("TrashCatalogue",can,day,-0.1); //trash catalogue
            // Beans
            if (Game1.QiBeansActive && garbageRandom.NextDouble() < 0.25) return new("890",can,day,-0.1); //maybe wrong, wait for confirmation
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
                        return new("223", can, day, Math.Max(minLuck, roll2 - 0.2));
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
                                return new(new List<string> { "809", "270", "270", "270" }[garbageRandom.Next(4)], can, day, minLuck);
                            }
                        }
                        if (!completeCC)
                        {
                            Random syncedRandom = Utility.CreateRandom(Game1.hash.GetDeterministicHashCode("garbage_joja"), gameId, day);
                            if (syncedRandom.NextBool(0.2))
                            {
                                return new("167", can, day, minLuck);
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
                                return new("749", can, day, Math.Max(minLuck, roll2 - 0.2));
                            }
                            return new("535", can, day, Math.Max(minLuck, roll2 - 0.2));
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
                            return new("DishOfTheDay", can, day, Math.Max(minLuck, roll2 - 0.2));
                        }
                    }
                    break;
                // Desert Can (Always drops eggs so removed all implementation)
            }
            string item = "";
            if ((numCansChecked > 20) && garbageRandom.NextDouble() < 0.01)
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
            return new(item, can, day, minLuck);
        }
        public static TrashItem GetTrash1_5(uint gameId, int day, Can can, double luck = 0, int numCansChecked = 0,
         bool theatre = false, bool hasFurnace = false, bool hasDesert = false, int mines = 0,bool hasBook = false,bool completeCC = false)
        {
            Random garbageRandom = new((int)gameId / 2 + day + 777 + (int)can * 77);
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
            bool mega = (numCansChecked > 20) && garbageRandom.NextDouble() < 0.01;
            bool doubleMega = (numCansChecked > 20) && garbageRandom.NextDouble() < 0.002;
            
            double roll = garbageRandom.NextDouble();
            double roll2;
            double minLuck = roll - 0.2;
            if (doubleMega)
            {
                // Garbage Hat
                return new TrashItem("GarbageHat", can, day, -0.1 );
            }
            if (mega || roll < 0.2 + luck)
            {
                string item = "168";
                switch (garbageRandom.Next(10))
                {
                    case 0:
                        item = "168";
                        break;
                    case 1:
                        item = "167";
                        break;
                    case 2:
                        item = "170";
                        break;
                    case 3:
                        item = "171";
                        break;
                    case 4:
                        item = "172";
                        break;
                    case 5:
                        item = "216";
                        break;
                    case 6:
                        Tile tile = getCanTile(can);
                        item = Utility.GetRandomItemFromSeason(Utility.getSeasonFromDay(day), tile.X * 653 + tile.Y * 777, false, 0, gameId, day, true, hasFurnace, hasDesert, mines).ToString();
                        break;
                    case 7:
                        item = "403";
                        break;
                    case 8:
                        item = (309 + garbageRandom.Next(3)).ToString();
                        break;
                    case 9:
                        item = "153";
                        break;
                }
                roll2 = garbageRandom.NextDouble();
                if (!(roll2 < 0.2 + luck)) return new(item,can,day,minLuck);
                switch (can)
                {
                    case Can.Museum:
                        item = garbageRandom.NextDouble() < 0.05 ? "749" : "535";
                        return new(item,can,day,Math.Max(minLuck,roll2-0.2));
                    case Can.Clint:
                        item = (378 + garbageRandom.Next(3) * 2).ToString();
                        garbageRandom.Next(1, 5); //no clue what this line does lol, is it removable?
                        return new(item,can,day,Math.Max(minLuck,roll2-0.2));
                    case Can.Gus:
                        return new("DishOfTheDay",can,day,Math.Max(minLuck,roll2-0.2));
                    case Can.George:
                        return new("223",can,day,Math.Max(minLuck,roll2-0.2));
                    case Can.Joja:
                        item = !theatre ? "167" : !(garbageRandom.NextDouble() < 0.25) ? "270" : "809";
                        return new(item,can,day,Math.Max(minLuck,roll2-0.2));
                    default: //game1.random bean drop
                        return new(item,can,day,minLuck);
                }
            }
            return new("",can,day,minLuck);
        }
    }
}