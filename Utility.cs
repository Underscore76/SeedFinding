using System.Collections.Generic;
using System;
using System.Drawing;

namespace SeedFinding
{
    public enum Season
    {
        Spring,
        Summer,
        Fall,
        Winter
    }
    public class Utility
    {

        public static Season getSeasonFromDay(int day)
        {
            int season = (day - 1) / 28 % 4;
            switch (season)
            {
                case 0:
                    return Season.Spring;
                case 1:
                    return Season.Summer;
                case 2:
                    return Season.Fall;
                case 3:
                    return Season.Winter;

            }
            return Season.Spring;
        }

        public static int GetRandomItemFromSeason(Season season, int randomSeedAddition, bool forQuest, int cookingRecipesKnown, int gameId, int days, bool changeDaily = true, bool hasFurnace = false, bool hasDesert = false, int mines = 0)
        {
            int dayOfMonth = (days - 1) % 28 + 1;
            int year = (days - 1) / (28 * 4) + 1;
            Random r;
            //if (Game1.version.StartsWith("1.6"))
            //{
            //    r = Utility.CreateRandom(gameId, changeDaily ? days : 0, randomSeedAddition);
            //}
            //else
            //{
            r = new Random(gameId + (int)(changeDaily ? days : 0) + randomSeedAddition);
            //}
            List<int> possibleItems = new List<int>
            {
                68, //Topaz
                66, //Amethyst
                78, //Cave Carrot
                80, //Quartz
                86, //Earth Crystal
                152, //Seaweed
                167, //Joja Cola
                153, //Green Algae
                420 //Red Musrhoom
            };
            if ((forQuest && (mines > 40)) || (!forQuest && (mines > 40)))
            {
                possibleItems.AddRange(new int[5]
                {
                    62, //Aquamarine
                    70, //Jade
                    72, //Diamond
                    84, //Frozen Tear
                    422 //Purple Mushroom
                });
            }
            if ((forQuest && (mines > 80)) || (!forQuest && (mines > 80)))
            {
                possibleItems.AddRange(new int[3]
                {
                    64, //Ruby
                    60, //Emerald
                    82 //Fire Quartz
                });
            }
            if (hasDesert)
            {
                possibleItems.AddRange(new int[4]
                {
                    88, //Coconut
                    90, //Cactus Fruit
                    164, //Sandfish
                    165 //Scorpian Carp
                });
            }
            if (hasFurnace)
            {
                possibleItems.AddRange(new int[4]
                {
                    334, //Copper Bar
                    335, //Iron Bar
                    336, //Gold Bar
                    338 //Refined Quartz
                });
            }
            if (season == Season.Spring)
            {
                possibleItems.AddRange(new int[17]
                {
                    16, //Horseradish
                    18, //Daffodil
                    20, //Leek
                    22, //Dandelion
                    129, //Anchovy
                    131, //Sardine
                    132, //Bream
                    136, //Largemouth Bass
                    137, //Smallmouth Bass
                    142, //Carp
                    143, //Catfish
                    145, //Sunfish
                    147, //Herring
                    148, //Eel
                    152, //Seaweed
                    167, //Joja Cola
                    267 //Flounder
                });
            }
            else if (season == Season.Summer)
            {
                possibleItems.AddRange(new int[16]
                {
                    128,
                    130,
                    132,
                    136,
                    138,
                    142,
                    144,
                    145,
                    146,
                    149,
                    150,
                    155,
                    396,
                    398,
                    402,
                    267
                });
            }
            else if (season == Season.Fall)
            {
                possibleItems.AddRange(new int[18]
                {
                    404,
                    406,
                    408,
                    410,
                    129,
                    131,
                    132,
                    136,
                    137,
                    139,
                    140,
                    142,
                    143,
                    148,
                    150,
                    154,
                    155,
                    269
                });
            }
            else if (season == Season.Winter)
            {
                possibleItems.AddRange(new int[17]
                {
                    412,
                    414,
                    416,
                    418,
                    130,
                    131,
                    132,
                    136,
                    140,
                    141,
                    144,
                    146,
                    147,
                    150,
                    151,
                    154,
                    269
                });
            }
            if (forQuest)
            {
                for (int x = 0; x < cookingRecipesKnown; x++)
                {
                    r.NextDouble();
                }
            }
            return possibleItems[r.Next(possibleItems.Count)];
        }
        public static List<int> possibleCropsAtThisTime(Season season, bool firstWeek, int year, bool desert)
        {
            List<int> firstWeekCrops = null;
            List<int> secondWeekCrops = null;
            if (season == Season.Spring)
            {
                firstWeekCrops = new List<int>
                {
                    24,
                    192
                };
                if (year > 1)
                {
                    firstWeekCrops.Add(250);
                }
                if (desert)
                {
                    firstWeekCrops.Add(248);
                }
                secondWeekCrops = new List<int>
                {
                    190,
                    188
                };
                if (desert)
                {
                    secondWeekCrops.Add(252);
                }
                secondWeekCrops.AddRange(firstWeekCrops);
            }
            else if (season == Season.Summer)
            {
                firstWeekCrops = new List<int>
                {
                    264,
                    262,
                    260
                };
                secondWeekCrops = new List<int>
                {
                    254,
                    256
                };
                if (year > 1)
                {
                    firstWeekCrops.Add(266);
                }
                if (desert)
                {
                    secondWeekCrops.AddRange(new int[2]
                    {
                        258,
                        268
                    });
                }
                secondWeekCrops.AddRange(firstWeekCrops);
            }
            else if (season == Season.Fall)
            {
                firstWeekCrops = new List<int>
                {
                    272,
                    278
                };
                secondWeekCrops = new List<int>
                {
                    270,
                    276,
                    280
                };
                if (year > 1)
                {
                    secondWeekCrops.Add(274);
                }
                if (desert)
                {
                    firstWeekCrops.Add(284);
                    secondWeekCrops.Add(282);
                }
                secondWeekCrops.AddRange(firstWeekCrops);
            }
            if (firstWeek)
            {
                return firstWeekCrops;
            }
            return secondWeekCrops;
        }

        public static Season ConvertDaysToSeason(int daysPlayed)
        {
            switch (((daysPlayed - 1) / 28) % 4)
            {
                case 0:
                    return Season.Spring;
                case 1:
                    return Season.Summer;
                case 2:
                    return Season.Fall;
                case 3:
                    return Season.Winter;
            }

            return Season.Spring;
        }

        public static List<Point> getBorderOfThisRectangle(Rectangle r)
        {
            List<Point> border = new List<Point>();
            for (int l = r.X; l < r.Right; l++)
            {
                border.Add(new Point(l, r.Y));
            }
            for (int k = r.Y + 1; k < r.Bottom; k++)
            {
                border.Add(new Point(r.Right - 1, k));
            }
            for (int j = r.Right - 2; j >= r.X; j--)
            {
                border.Add(new Point(j, r.Bottom - 1));
            }
            for (int i = r.Bottom - 2; i >= r.Y + 1; i--)
            {
                border.Add(new Point(r.X, i));
            }
            return border;
        }
    }
}