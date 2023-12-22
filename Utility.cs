using System.Collections.Generic;
using System;
using System.Drawing;
using SeedFinding.Locations;
using static System.Environment;
using System.Globalization;
using System.Numerics;
using System.Text;
using StardewValley.Hashing;

namespace SeedFinding
{
    public enum Season
    {
        Spring,
        Summer,
        Fall,
        Winter
    }

    public enum Quality
    {
        Basic = 0,
        Silver = 1,
        Gold = 2,
        Iridium = 4
    }

    public enum Fertilizer
    {
        None,
        Basic,
        Quality,
        Deluxe
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

        public static int getYearFromDay(int day)
        {
            return ((day - 1) / (28 *4))+1;
        }

        public static int getDayOfMonthFromDay(int day)
        {
            return (day - 1) % 28 + 1;
        }

        public static string getDayDescription(int day)
        {
            return $"{getSeasonFromDay(day)} {getDayOfMonthFromDay(day)} {getYearFromDay(day)}";
        }

        public static int GetRandomItemFromSeason(Season season, int randomSeedAddition, bool forQuest, int cookingRecipesKnown, int gameId, int days, bool changeDaily = true, bool hasFurnace = false, bool hasDesert = false, int mines = 0)
        {
            Random r = Utility.CreateRandom(gameId, changeDaily ? days : 0, randomSeedAddition);

            return GetRandomItemFromSeason(season, forQuest, r, cookingRecipesKnown, hasFurnace, hasDesert, mines);
        }

        public static int GetRandomItemFromSeason(Season season, bool forQuest, Random r, int cookingRecipesKnown, bool hasFurnace = false, bool hasDesert = false, int mines = 0) { 
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
    
        static Dictionary<string, List<(int, double)>> archDict = new Dictionary<string, List<(int, double)>>
            {
                {"Town", new List<(int, double)> { (100, .04), (103, .01), (105, .01), (106, .008), (110, .05), (119, .005), (123, .005), (126, .001), (127, .001), (579, .01) } },
                {"Mountain", new List<(int, double)> { (101, .02), (103, .04), (105, .02), (107, .008), (109, .008), (112, .05), (114, .01), (115, .03), (119, .01), (120, .05), (126, .001), (127, .001), (581, .01), (587, .01), (589, .03) } },
                {"Forest", new List<(int, double)> { (101, .02), (103, .03), (104, .01), (105, .02), (106, .01), (109, .01), (114, .01), (115, .03), (119, .01), (120, .05), (123, .01), (126, .001), (127, .001), (580, .01), (587, .01), (588, .01), (589, .03) } },
                {"BusStop", new List<(int, double)> { (101, .02), (103, .03), (115, .04), (120, .05), (123, .01), (126, .001), (127, .001), (584, .01) } },
                {"Beach", new List<(int, double)> { (106, .02), (116, .1), (117, .05), (118, .1), (126, .001), (127, .001), (582, .01), (586, .03), (588, .01), (589, .03) } },
                {"Mine", new List<(int, double)> { (107, .01) } },
                {"UndergroundMine", new List<(int, double)> { (108, .01), (119, .02), (121, .01), (122, .001), (123, .02), (126, .001), (127, .001), (585, .01) } },
                {"Farm", new List<(int, double)> { (111, .1), (113, .1), (126, .001), (127, .001), (583, .01) } },
                {"Desert", new List<(int, double)> { (124, .04), (125, .08), (588, .1) } },
                {"Backwoods", new List<(int, double)> { }
            }
        };
        static Dictionary<string, List<(int, double)>> locationDict = new Dictionary<string, List<(int, double)>>
            {
                {"Farm", new List<(int,double)>{(382, .05), (770, .1), (390, .25), (330, 1)}},
                {"UndergroundMine", new List<(int,double)>{(107, .01)}},
                {"Desert", new List<(int,double)>{(390, .25), (330, 1)}},
                {"BusStop", new List<(int,double)>{(584, .08), (378, .15), (102, .15), (390, .25), (330, 1)}},
                {"Forest", new List<(int,double)>{(378, .08), (579, .1), (588, .1), (102, .15), (390, .25), (330, 1)}},
                {"Town", new List<(int,double)>{(378, .2), (110, .2), (583, .1), (102, .2), (390, .25), (330, 1)}},
                {"Mountain", new List<(int,double)>{(382, .06), (581, .1), (378, .1), (102, .15), (390, .25), (330, 1)}},
                {"Backwoods", new List<(int,double)>{(382, .06), (582, .1), (378, .1), (102, .15), (390, .25), (330, 1)}},
                {"Railroad", new List<(int,double)>{(580, .1), (378, .15), (102, .19), (390, .25), (330, 1)}},
                {"Beach", new List<(int,double)>{(384, .08), (589, .09), (102, .15), (390, .25), (330, 1)}},
                {"Woods", new List<(int,double)>{(390, .25), (330, 1)}},
                {"IslandNorth", new List<(int,double)>{(791, .05), (292, .05), (774, .1), (749, 1)}},
                {"IslandSouth", new List<(int,double)>{(791, .05), (292, .05), (774, .1), (749, 1)}},
                {"IslandWest", new List<(int,double)>{(791, .05), (292, .05), (774, .1), (749, 1)}},
                {"IslandSouthEast", new List<(int,double)>{(791, .05), (292, .05), (774, .1), (749, 1)}},
                {"IslandSouthEastCave", new List<(int,double)>{(791, .05), (292, .05), (774, .1), (749, 1)}},
                {"IslandSecret", new List<(int,double)>{(791, .05), (292, .05), (774, .1), (749, 1)}},
                {"IslandNorthCave1", new List<(int,double)>{(107, .01)} }
        };

        public static (int,int) GetArtifactspot(int gameId, int day, int x, int y,string location)
        {
            Random r = new Random(x * 2000 + y + gameId / 2 + day);
            int toDigUp = -1;
            //bool archaeologyEnchant = who != null && who.CurrentTool != null && who.CurrentTool is Hoe && who.CurrentTool.hasEnchantmentOfType<ArchaeologistEnchantment>();
            foreach (var v in archDict[location])
            {
                if ( r.NextDouble() < v.Item2) // Arch enchant doubles here
                {
                    toDigUp = v.Item1;
                    break;
                }
                if (toDigUp != -1)
                {
                    break;
                }
            }
            if (r.NextDouble() < 0.2 && !(location == "Farm"))
            {
                toDigUp = 102;
            }
            if (toDigUp != -1)
            {
                return (toDigUp,1);
            }
            //bool generousEnchant = who != null && who.CurrentTool != null && who.CurrentTool is Hoe && who.CurrentTool.hasEnchantmentOfType<GenerousEnchantment>();
            //float generousChance = 0.5f;
            if (ConvertDaysToSeason(day) == Season.Winter && r.NextDouble() < 0.5 && !(location == "Desert"))
            {
                if (r.NextDouble() < 0.4)
                {
                    return (416, 1);
                    //if (generousEnchant && r.NextDouble() < (double)generousChance)
                    //{
                    //    Game1.createObjectDebris(416, xLocation, yLocation, who.UniqueMultiplayerID);
                    //}
                }
                else
                {
                    return (412, 1);
                    //if (generousEnchant && r.NextDouble() < (double)generousChance)
                    //{
                    //    Game1.createObjectDebris(412, xLocation, yLocation, who.UniqueMultiplayerID);
                    //}
                }
            }
            //if (Game1.random.NextDouble() <= 0.25 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
            //{
            //    Game1.createMultipleObjectDebris(890, xLocation, yLocation, r.Next(2, 6), who.UniqueMultiplayerID, this);
            //}
            if (ConvertDaysToSeason(day) == Season.Spring && r.NextDouble() < 0.0625 && !(location == "Desert") && !(location == "Beach"))
            {
                return (273, 1);
                //if (generousEnchant && r.NextDouble() < (double)generousChance)
                //{
                //    Game1.createMultipleObjectDebris(273, xLocation, yLocation, r.Next(2, 6), who.UniqueMultiplayerID, this);
                //}
            }
            //if (Game1.random.NextDouble() <= 0.2 && (Game1.MasterPlayer.mailReceived.Contains("guntherBones") || (Game1.player.team.specialOrders.Where((SpecialOrder x) => (string)x.questKey == "Gunther") != null && Game1.player.team.specialOrders.Where((SpecialOrder x) => (string)x.questKey == "Gunther").Count() > 0)))
            //{
            //    Game1.createMultipleObjectDebris(881, xLocation, yLocation, r.Next(2, 6), who.UniqueMultiplayerID, this);
            //}
            if (!locationDict.ContainsKey(location))
            {
                return (-1,-1);
            }
            List<(int,double)> rawData = locationDict[location];
            if (rawData.Count == 0 || rawData[0].Equals("-1"))
            {
                return (-1, -1);
            }
            foreach (var v in locationDict[location]) {
                if (!(r.NextDouble() <= v.Item2))
                {
                    continue;
                }
                toDigUp = Convert.ToInt32(v.Item1);
                if (ObjectInfo.ObjectInformation.ContainsKey(toDigUp.ToString()) && (ObjectInfo.ObjectInformation[toDigUp.ToString()].Split('/')[3].Contains("Arch") || toDigUp == 102))
                {
                    return (toDigUp, 1);
                }
                //if (toDigUp == 330 && this.HasUnlockedAreaSecretNotes(who) && Game1.random.NextDouble() < 0.11)
                //{
                //    Object o = this.tryToCreateUnseenSecretNote(who);
                //    if (o != null)
                //    {
                //        Game1.createItemDebris(o, new Vector2((float)xLocation + 0.5f, (float)yLocation + 0.5f) * 64f, -1, this);
                //        break;
                //    }
                //}
                //else if (toDigUp == 330 && Game1.stats.DaysPlayed > 28 && Game1.random.NextDouble() < 0.1)
                //{
                //    Game1.createMultipleObjectDebris(688 + Game1.random.Next(3), xLocation, yLocation, 1, who.UniqueMultiplayerID);
                //}
                return (toDigUp, r.Next(1, 4));
                //if (generousEnchant && r.NextDouble() < (double)generousChance)
                //{
                //    Game1.createMultipleObjectDebris(toDigUp, xLocation, yLocation, r.Next(1, 4), who.UniqueMultiplayerID);
                //}
                //break;
            }
            return (-1, -1);
        }

        public static Quality ForageQuality(int gameId, int day, int x, int y, int level)
        {
            Random r = new Random(gameId / 2 + day + x + y * 777);
            if (r.NextDouble() < level / 30.0)
            {
                return Quality.Gold;
            }else if (r.NextDouble() < level / 15.0)
            {
                return Quality.Silver;
            }

            return Quality.Basic;
        }

        public static bool ForageDoubled(int gameId, int day, int x, int y, int level,bool botanist = false)
        {
            Random r = new Random(gameId / 2 + day + x + y * 777);
            if (!botanist)
            {
                if (r.NextDouble() < level / 30.0)
                {
                    //return Quality.Gold;
                }
                else if (r.NextDouble() < level / 15.0)
                {
                    //return Quality.Silver;
                }
            }

            //return Quality.Basic;

            return r.NextDouble() < 0.2;
        }
        public static Quality CropQuality(int gameId, int day, int x, int y, int level, Fertilizer fert)
        {
            int fertilizerQualityLevel = 0;
            Random r = new Random(x * 7 + y * 11 + day + gameId);
            switch (fert)
            {
                case Fertilizer.Basic:
                    fertilizerQualityLevel = 1;
                    break;
                case Fertilizer.Quality:
                    fertilizerQualityLevel = 2;
                    break;
                case Fertilizer.Deluxe:
                    fertilizerQualityLevel = 3;
                    break;
            }
            double chanceForGoldQuality = 0.2 * (level / 10.0) + 0.2 * (double)fertilizerQualityLevel * ((level + 2.0) / 12.0) + 0.01;
            double chanceForSilverQuality = Math.Min(0.75, chanceForGoldQuality * 2.0);
            if (fertilizerQualityLevel >= 3 && r.NextDouble() < chanceForGoldQuality / 2.0)
            {
                return Quality.Iridium;
            }
            else if (r.NextDouble() < chanceForGoldQuality)
            {
                return Quality.Gold;
            }
            else if (r.NextDouble() < chanceForSilverQuality || fertilizerQualityLevel >= 3)
            {
                return Quality.Silver;
            }

            return Quality.Basic;
        }

        public static bool MusselNutDrop(int gameId, int day, int x, int y)
        {
            Random r = new Random(day + gameId / 2 + x * 4000 + y);

            r.Next();
            return r.NextDouble() < 0.1;
        }
        public static Random CreateDaySaveRandom(int days, int gameId, double seedA = 0.0, double seedB = 0.0, double seedC = 0.0)
        {
            return Utility.CreateRandom(days, gameId / 2, seedA, seedB, seedC);
        }

        public static Random CreateRandom(double seedA, double seedB = 0.0, double seedC = 0.0, double seedD = 0.0, double seedE = 0.0)
        {
            return new Random(Utility.CreateRandomSeed(seedA, seedB, seedC, seedD, seedE));
        }

        public static int CreateRandomSeed(double seedA, double seedB, double seedC = 0.0, double seedD = 0.0, double seedE = 0.0)
        {
            if (Game1.UseLegacyRandom)
            {
                return (int)((seedA % 2147483647.0 + seedB % 2147483647.0 + seedC % 2147483647.0 + seedD % 2147483647.0 + seedE % 2147483647.0) % 2147483647.0);
            }
            return Game1.hash.GetDeterministicHashCode((int)(seedA % 2147483647.0), (int)(seedB % 2147483647.0), (int)(seedC % 2147483647.0), (int)(seedD % 2147483647.0), (int)(seedE % 2147483647.0));
        }

        public static T GetRandom<T>(List<T> list, Random random = null)
        {
            if (list == null || list.Count == 0 || random == null)
                return default(T);
            return list[random.Next(list.Count)];
        }

        public static bool isGreenRainDay(int day, int gameId)
        {
            Season season = getSeasonFromDay(day);
            if (season == Season.Summer)
            {
                Random r = Utility.CreateRandom(getYearFromDay(day) * 777, gameId);
                int[] possible_days = new int[8] { 5, 6, 7, 14, 15, 16, 18, 23 };
                return getDayOfMonthFromDay(day) == r.ChooseFrom(possible_days);
            }
            return false;
        }

    }
}