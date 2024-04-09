using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeedFinding.Trash1_6;
using static SeedFinding.StepPredictions;

namespace SeedFinding
{
    internal class MarriageSpeedrun
    {
        static List<string> goodItems = new() {"16","18","20","22","24", "396", "398", "402", "254", "256", "264", "262", "260" };

        // single search
        public static void Curate()
        {
            List<int> seeds = new List<int> { 1318530,1753067,4159179,4991265,7455183,12031326,15864292,32766840,38181722,42156802,45824175,46081743,48635425,64894713,71946228,85260661,90708533,96073928,106422842,107075105,114194160,121268083,121603651,130184389,133846306,137000619,151232918,152035653,169902932,178622061,199030443,201055427,202307524,218013517,218964882,220702882,221774901,222242599,224612887,235684949,238053271,240157720,242450091,243363097,245283869,276655553,280339790,285899168,305528536,306889591,309855572,310335232,311484972,312864718,316145730,319132279,321099972,326561214,343487064,366587227,367163955,367237007,375240793,379342921,402498640,411818797,415669369,419213932,421210540,427001338,434037298,437277052,443966268,444391748,451467299,452434653,456167735,456861843,458613528,461046425,469970074,484352977,508094941,519078214,525552032,535012314,536830061,537332167,553378468,556146088,559437575,563330049,569759348,580426008,581523511,585710318,594771534,597964682,601475739,602731872,606698835,618247386,618525583,620531063,621074578,622033323,626102547,626983591,626896034,641030046,647140257,650601507,657129702,660536949,672559454,691575612,696243233,698622396,702032019,706142482,707396179,709405054,713220731,715118050,726019094,727608310,750942855,752667875,758407592,760717575,775220083,787916117,804244744,827158529,833453617,834660367,835334645,842950063,856241493,862131041,862267628,863210386,871299690,876012487,877782661,879956390,888558105,896387235,903120736,914301785,922591126,928822117,934222503,940082109,942729242,947389462,952077533,953103721,958545785,970117414,973880760,985117051,985572335,1000595443,1003848449,1008014597,1010728652,1020426906,1027444870,1034504901,1040493112,1044014859,1049549794,1050593396,1054528899,1058804192,1072493772,1082971672,1083555828,1085326184,1091638215,1096713354,1100577759,1100702430,1102996457,1103746297,1108495781,1111064850,1111846076,1112236729,1117190797,1126306656,1126609553,1141837715,1142162831,1142380692,1152826328,1154314144,1158948839,1161977642,1176694048,1178267006,1181704375,1182622322,1192989549,1211329895,1218125682,1222790055,1224041219,1236962710,1238518232,1238978272,1245028628,1265270309,1293441747,1298303462,1298948392,1307600131,1318689951,1329709841,1330401313,1340953364,1348653193,1349165450,1353283337,1360219725,1361032661,1364934437,1388533353,1390278669,1395207513,1396671932,1400344318,1407730710,1416045234,1416416873,1437353209,1439136173,1439227261,1439536640,1443474536,1444948682,1458226925,1464024050,1466718532,1467510962,1469819793,1479941908,1481666409,1485603614,1504724054,1506147542,1510048574,1515739376,1529791228,1535347946,1536746417,1547247898,1559317093,1569076525,1569045701,1581033915,1581764650,1582324446,1596709562,1598857767,1605923676,1606496702,1610411776,1610731450,1612847900,1615592550,1619753794,1623533884,1624123505,1625544680,1627546695,1631381913,1631803422,1642788497,1646900025,1649528964,1663022965,1669817344,1671146508,1683302612,1701043153,1704496622,1708263550,1720866859,1726572102,1730376176,1730661565,1735746794,1736635207,1744163209,1749772927,1758215973,1758245650,1768101172,1780937375,1782305862,1783384769,1789104738,1793853878,1804222443,1805609948,1814432743,1817348543,1820227914,1820784677,1822484843,1830034222,1837554415,1852035772,1859864509,1862112568,1875924515,1877280139,1888608712,1890507075,1893715327,1900584511,1906993266,1910308183,1918404725,1926805793,1938388145,1938957004,1975518569,1979016740,1982716239,1983585776,1985981237,1988849540,1991116435,1994668195,2000914263,2001336349,2016353379,2030794206,2050442135,2051008669,2054513067,2058059140,2066611516,2071018228,2075217056,2076962901,2095474099,2103751199,2104017973,2117137043,2122033623,2124530162,2128492694,2129787482,2133362166,2136778745,1627669577
            };

            foreach (var seed in seeds)
            {
                if (ValidSeed(seed, true))
                {
                    //      Console.WriteLine($"{seed}");
                }
            }
        }
        public static bool ValidSeed(int gameId,bool report = false)
        {
            string reportString = "";
            // Day 20 needs to rain
            if (Weather.getWeather(20,gameId) != Weather.WeatherType.Rain)
            {
                return false;
            }

            // Get dish of the day
            //var trash = Trash1_6.Trash.getTrash(gameId, 20, Trash1_6.Trash.Can.Gus, 0.1);
            //if (trash.Id != "DishOfTheDay")
            //{
            //    return false;
            //}

            // Need a quest on the 20th
            var quest20 = Quests.GetItemDeliveryQuest(gameId, 20, 1, new() { "Robin", "Lewis", "Shane" }, false, false, 0);
            if (quest20.Type == Quests.QuestType.None)
            {
                return false;
            }

            if (quest20.Person != "Shane")
            {
                return false;
            }

            //reportString = $"Day20 Trash Luck: {trash.MinLuck}";

            // Make sure there is enough rain to grow snips before 20th
            //
            int springRainCount = 1;
            int day;
            bool hasQuest = false;
            string quests = "";

            // Once snips are ready, look for quests
            //day++;
            HashSet<string> trashItems = new ();
            HashSet<Trash1_6.Trash.TrashItem> trashItemsFull = new();
            int questCount = 1;
            int questsForWeek = 0;
            int summerRainCount = 0;
            bool firstSummerQuest = false;
            int availableRecipes = 1;
            int totalRecipes = 1;

            for (day = 2; day < 54; day++)
            {
                if (day == 21)
                {
                    totalRecipes++;
                }
                if (day == 39 || day == 24 || day == 13)
                {
                    continue;
                }
                HashSet<Trash1_6.Trash.TrashItem> dayTrashFull;
                HashSet<string> dayTrash;
                Quests.QuestResults quest;
                double luck;
                if (!hasQuest)
                {
                    // Day of and day before festivals can't have a quest
                    if (day == 38 || day == 39 || day == 23 || day == 24 || day == 12 || day == 13)
                    {
                        continue;
                    }
                    quest = Quests.GetItemDeliveryQuest(gameId, day, totalRecipes, new() { "Robin", "Lewis", "Shane" }, false, false, 0);
                    if (quest.Type == Quests.QuestType.None)
                    {
                        continue;
                    }
                    if (quest.Person != "Shane")
                    {
                        continue;
                    }
                    // First quest needs to also have trash so we can step manip for it
                    var trash = Trash1_6.Trash.getTrash(gameId, day, Trash1_6.Trash.Can.Gus, 0.1);
                    if (trash.Id != "DishOfTheDay")
                    {
                        continue;
                    }
                    luck = 0.1;
                    // Get list of trash for the day
                    dayTrashFull = new()
                    {
                        Trash1_6.Trash.getTrash(gameId, day, Trash1_6.Trash.Can.Jodi, luck),
                        Trash1_6.Trash.getTrash(gameId, day, Trash1_6.Trash.Can.George, luck),
                        Trash1_6.Trash.getTrash(gameId, day, Trash1_6.Trash.Can.Lewis, luck),
                        Trash1_6.Trash.getTrash(gameId, day, Trash1_6.Trash.Can.Emily, luck)
                    };

                    dayTrashFull.RemoveWhere(x => x.Id == "");



                    //Trash1_6.Trash.getAllTrash(gameId, day, 0.1);
                    dayTrash = new HashSet<string>(dayTrashFull.Select(x => x.Id));

                    // See if we have access to the quest item
                    if (!goodItems.Contains(quest.Id) && !dayTrash.Contains(quest.Id))
                    {
                        continue;
                    }

                    // No parsnip at this time
                    if (quest.Id == "24")
                    {
                        continue;
                    }
                    int neededSteps = -1;
                    for (int steps = 0; steps < 1000; steps++)
                    {
                        StepResult result = StepPredictions.Predict(gameId, day - 1, steps, new List<string>() { "Lewis", "Robin", "Shane" });
                        if (result.Dish == 215 && result.DailyLuck >= trash.MinLuck)
                        {
                            neededSteps = steps;
                            break;
                        }
                    }
                    quests += $"\t{day}\t{Item.Get(quest.Id).Name}";
                    reportString = $"First Trash Luck: {trash.MinLuck}\tSteps: {neededSteps}";
                    hasQuest = true;
                    continue;
                }
                if (day > 28 && firstSummerQuest || day <= 28)
                {
                    Weather.WeatherType type = Weather.getWeather(day, gameId);
                    bool isRain = new HashSet<Weather.WeatherType> { Weather.WeatherType.Rain, Weather.WeatherType.Storm, Weather.WeatherType.GreenRain }.Contains(type);
                    if (day > 28)
                    {
                        if (isRain)
                        {
                            summerRainCount++;
                        }

                        if (summerRainCount < 5)
                        {
                           // continue;
                        }
                    }else if (isRain)
                    {
                        springRainCount++;
                    }

                    //Schedules change on green rain
                    if (type == Weather.WeatherType.GreenRain)
                    {
                        continue;
                    }
                }

                if (day % 7 == 0)
                {
                    questsForWeek = 0;
                }
                if (questsForWeek == 2 && !(day ==20))
                {
                    continue;
                }
                // Day of and day before festivals can't have a quest
                if (day == 38 || day == 39 || day == 23 || day == 24 || day == 12 || day == 13)
                {
                    continue;
                }
                quest = Quests.GetItemDeliveryQuest(gameId, day, totalRecipes, new() { "Robin", "Lewis", "Shane" },false,false,0);
                if (quest.Type == Quests.QuestType.None)
                {
                    if (day == 20)
                    {
                        return false;
                    }
                    continue;
                }

                if (quest.Person != "Shane")
                {
                    continue;
                }

                luck = -0.1;
                // Get list of trash for the day
                dayTrashFull = new()
                {
                    Trash1_6.Trash.getTrash(gameId, day, Trash1_6.Trash.Can.Jodi, luck),
                    Trash1_6.Trash.getTrash(gameId, day, Trash1_6.Trash.Can.George, luck),
                    Trash1_6.Trash.getTrash(gameId, day, Trash1_6.Trash.Can.Lewis, luck),
                    Trash1_6.Trash.getTrash(gameId, day, Trash1_6.Trash.Can.Emily, luck),
                    Trash1_6.Trash.getTrash(gameId, day, Trash1_6.Trash.Can.Gus, luck)
                };

                dayTrashFull.RemoveWhere(x => x.Id == "");


                
                //Trash1_6.Trash.getAllTrash(gameId, day, 0.1);
                dayTrash = new HashSet<string>(dayTrashFull.Select(x => x.Id));

                // See if we have access to the quest item
                if (goodItems.Contains(quest.Id) || trashItems.Contains(quest.Id) || dayTrash.Contains(quest.Id)){

                    // Too early for some items
                    if (quest.Id == "260" && day < 34 || quest.Id == "254" && day < 41 || quest.Id == "256" && day < 40 || quest.Id == "262" && day < 33 || quest.Id == "264" && day < 35)
                    {
                        continue;
                    }
                    // If weekend, needs to rain
                    if (day % 7 == 0 || day % 7 == 6)
                    {
                        if (!(new HashSet<Weather.WeatherType> { Weather.WeatherType.Rain, Weather.WeatherType.Storm, Weather.WeatherType.GreenRain }.Contains(Weather.getWeather(day, gameId))))
                        { 
                            continue; 
                        }
                    }

                    if (!firstSummerQuest && day > 28)
                    {
                        firstSummerQuest = true;
                    }


                    // Account for 3rd quest on week that ends Spring 20
                    if (questsForWeek < 2)
                    {
                        questsForWeek++;
                        questCount++;
                        if (questCount == 6)
                        {
                            totalRecipes++;
                        }
                    }
                    quests += $"\t{day}\t{Item.Get(quest.Id).Name}";
                    if (questCount == 8)
                    {
                        break;
                    }

                    trashItems.UnionWith(dayTrash);
                    trashItemsFull.UnionWith(dayTrashFull);

                    if (trashItems.Contains(quest.Id))
                    {
                        trashItems.Remove(quest.Id);
                    }
                }else if(day == 20)
                {
                    return false;
                }
            }

            // Check for enough quests
            if (questCount < 8)
            {
                return false;
            }
            if (report)
            {
                FileStream fs = new FileStream($"MarriageSeededCurate.txt", FileMode.Append);
                StreamWriter stream = new StreamWriter(fs);
                string line = $"{gameId}\t{quests}\t{day}\t{reportString}\tSpring Rain: {springRainCount}\tSummer Rain: {summerRainCount}\t{String.Join(", ", trashItemsFull)}";
                stream.WriteLine(line);
                stream.Close();
                Console.WriteLine(line);
            }

            return true;
        }
        public static double Search(int startId, int endId, int blockSize, out List<int> validSeeds)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var bag = new ConcurrentBag<int>();
            var partioner = Partitioner.Create(startId, endId, blockSize);
            //for(int seed = startId; seed < endId; seed++) { 
            Parallel.ForEach(partioner, (range, loopState) =>
            {
                for (int seed = range.Item1; seed < range.Item2; seed++)
                {
                    if (ValidSeed(seed))
                    {
                        bag.Add(seed);
                        FileStream fs = null;
                        int i = 0;
                        while (fs == null)
                        {
                            try
                            {
                                fs = new FileStream($"MarriageSeededV5_{i}.txt", FileMode.Append);
                            }
                            catch
                            {
                                i++;
                            }
                        }
                        StreamWriter stream = new StreamWriter(fs);
                        stream.Write($"{seed},");
                        Console.WriteLine(seed);
                        stream.Close();
                    }
                }
            });
            double seconds = stopwatch.Elapsed.TotalSeconds;
            Console.WriteLine($"Found: {bag.Count} sols in {seconds.ToString("F2")} s");
            validSeeds = bag.ToList();
            validSeeds.Sort();
            return seconds;
        }
    }
}
