using System;
using System.Collections.Generic;
using System.Net;
using SeedFinding.Locations;

namespace SeedFinding.Trash
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
            Joja = 7
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
        public static HashSet<string> getAllTrash(int gameId, int day, double luck = 0, bool twentyOneChecked = false, bool theatre = false, bool hasFurnace = false, bool hasDesert = false, int mines = 0)
        {
            HashSet<string> results = new HashSet<string>();

            foreach (Can can in Can.GetValues(typeof(Can))) 
            {
                results.Add(getTrash(gameId, day, can, luck, twentyOneChecked, theatre, hasFurnace, hasDesert, mines));
            }
            results.Remove("0");
            return results;
        }
        public static string getTrash(int gameId, int day, Can can, double luck = 0, bool twentyOneChecked = false, bool theatre = false, bool hasFurnace = false, bool hasDesert = false, int mines = 0)
        {
            Random garbageRandom = new Random((int)gameId / 2 + (int)day + 777 + (int)can * 77);
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

            bool mega = twentyOneChecked && garbageRandom.NextDouble() < 0.01;
            bool doubleMega = twentyOneChecked && garbageRandom.NextDouble() < 0.002;

            if (doubleMega)
            {
                // Garbage Hat
                return "GarbageHat";
            }
            else if (mega || garbageRandom.NextDouble() < 0.2 + luck)
            {
                int item = 168;
                switch (garbageRandom.Next(10))
                {
                    case 0:
                        item = 168;
                        break;
                    case 1:
                        item = 167;
                        break;
                    case 2:
                        item = 170;
                        break;
                    case 3:
                        item = 171;
                        break;
                    case 4:
                        item = 172;
                        break;
                    case 5:
                        item = 216;
                        break;
                    case 6:
                        Tile tile = getCanTile(can);
                        item = Utility.GetRandomItemFromSeason(Utility.getSeasonFromDay(day), tile.X * 653 + tile.Y * 777, false, 0, gameId, day, true, hasFurnace, hasDesert, mines);
                        break;
                    case 7:
                        item = 403;
                        break;
                    case 8:
                        item = 309 + garbageRandom.Next(3);
                        break;
                    case 9:
                        item = 153;
                        break;
                }
                if (can == Can.Museum && garbageRandom.NextDouble() < 0.2 + luck)
                {
                    item = 535;
                    if (garbageRandom.NextDouble() < 0.05)
                    {
                        item = 749;
                    }
                }
                if (can == Can.Clint && garbageRandom.NextDouble() < 0.2 + luck)
                {
                    item = 378 + garbageRandom.Next(3) * 2;
                    garbageRandom.Next(1, 5);
                }
                if (can == Can.Gus && garbageRandom.NextDouble() < 0.2 + luck)
                {
                    // DishOfTheDay
                    item = 194;
                }
                if (can == Can.George && garbageRandom.NextDouble() < 0.2 + luck)
                {
                    item = 223;
                }
                if (can == Can.Joja && garbageRandom.NextDouble() < 0.2)
                {
                    item = 167;
                    if (theatre)
                    {
                        item = ((!(garbageRandom.NextDouble() < 0.25)) ? 270 : 809);
                    }
                }
                return item.ToString();
            }
            return "0";
        }
    }
}