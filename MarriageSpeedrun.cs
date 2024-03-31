using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeedFinding.Trash1_6;

namespace SeedFinding
{
    internal class MarriageSpeedrun
    {
        static List<string> goodItems = new() {"16","18","20","22","24", "396", "398", "402", "254", "256", "264", "262", "260" };

        // single search
        public static void Curate()
        {
            List<int> seeds = new List<int> { 132629294,152462939,161192517,179340571,205655929,270613299,274903984,280998229,303905435,355624712,370842600,382080116,533719145,572376298,598955443,616744916,618962709,643027057,648335503,664134455,700423764,753533856,772315020,965065359,1049670546,1069933837,1080911289,1139577241,1248031248,1267361245,1295292444,1494873865,1510232356,1569516771,1586183306,1688549662,1756526268,1827264060,1861741390,1876773875,1887262745,1887161877,1947061953,1965686190,1969751863,2082757404,2111735896,46026823,229930733,342680748,433988280,662537304,854469148,910002864,919687773,992285660,1070812171,1079684673,1266703256,1274243732,1291094998,1300776555,1522148185,1532262320,1537161038,1700474542,1766578733,1804810055,1901844442,2004163059,2021332453
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
            var trash = Trash1_6.Trash.getTrash(gameId, 20, Trash1_6.Trash.Can.Gus, -0.1);
            if (trash.Id != "DishOfTheDay")
            {
                return false;
            }

            // Need a quest on the 20th
            var quest20 = Quests.GetItemDeliveryQuest(gameId, 20, 1, new() { "Robin", "Lewis", "Shane" }, false, false, 0);
            if (quest20.Type == Quests.QuestType.None)
            {
                return false;
            }

            // May be able to get away with no inventory
            if (quest20.Person != "Shane")
            {
                //continue;
            }

            reportString = $"Day20 Trash Luck: {trash.MinLuck}";

            // Make sure there is enough rain to grow snips before 20th
            int rainCount = 1;
            int day;
            for (day = 6; day < 20; day++)
            {
                // Festival can't rain
                if (day == 13)
                    continue;
                if (Weather.getWeather(day,gameId) == Weather.WeatherType.Rain)
                {
                    rainCount++;
                    if (rainCount == 4)
                    {
                        break;
                    }
                }
            }
            if (rainCount < 4)
            {
                return false;
            }

            // Once snips are ready, look for quests
            day++;
            HashSet<string> trashItems = new ();
            HashSet<Trash1_6.Trash.TrashItem> trashItemsFull = new();
            int questCount = 0;
            int questsForWeek = 0;
            string quests = "";
            rainCount = 0;
            for (; day < 54; day++)
            {
                if (day == 39 || day == 24)
                {
                    continue;
                }
                if (day > 28)
                {
                    if (new HashSet<Weather.WeatherType> { Weather.WeatherType.Rain,Weather.WeatherType.Storm,Weather.WeatherType.GreenRain }.Contains(Weather.getWeather(day, gameId)))
                    {
                        rainCount++;
                    }
                    if (rainCount < 5)
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
                if (day == 38 || day == 39 || day == 23 || day == 24)
                {
                    continue;
                }
                var quest = Quests.GetItemDeliveryQuest(gameId, day, 1, new() { "Robin", "Lewis", "Shane" },false,false,0);
                if (quest.Type == Quests.QuestType.None)
                {
                    if (day == 20)
                    {
                        return false;
                    }
                    continue;
                }

                // May be able to get away with no inventory
                if (quest.Person != "Shane")
                {
                    //continue;
                }

                double luck = -0.1;
                // Get list of trash for the day
                HashSet<Trash1_6.Trash.TrashItem> dayTrashFull = new()
                {
                    Trash1_6.Trash.getTrash(gameId, day, Trash1_6.Trash.Can.Jodi, luck),
                    Trash1_6.Trash.getTrash(gameId, day, Trash1_6.Trash.Can.George, luck),
                    Trash1_6.Trash.getTrash(gameId, day, Trash1_6.Trash.Can.Lewis, luck),
                    Trash1_6.Trash.getTrash(gameId, day, Trash1_6.Trash.Can.Emily, luck),
                    Trash1_6.Trash.getTrash(gameId, day, Trash1_6.Trash.Can.Gus, luck)
                };

                dayTrashFull.RemoveWhere(x => x.Id == "");

                
                //Trash1_6.Trash.getAllTrash(gameId, day, 0.1);
                HashSet<string> dayTrash = new HashSet<string>(dayTrashFull.Select(x => x.Id));

                // See if we have access to the quest item
                if (goodItems.Contains(quest.Id) || trashItems.Contains(quest.Id) || dayTrash.Contains(quest.Id)){

                    // Too early for some items
                    if (quest.Id == "260" && day < 34 || quest.Id == "254" && day < 41 || quest.Id == "256" && day < 40 || quest.Id == "262" && day < 33)
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

                    // Account for 3rd quest on week that ends Spring 20
                    if (questsForWeek < 2)
                    {
                        questsForWeek++;
                        questCount++;
                    }
                    quests += $"    {day}   {Item.Get(quest.Id).Name}";
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
                }
            }

            // Check for enough quests
            if (questCount < 8)
            {
                return false;
            }
            if (report)
            {
                Console.WriteLine($"{gameId}    {quests}    {day}   {reportString}  {String.Join(", ", trashItemsFull)}");
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
                                fs = new FileStream($"MarriageSeeded{i}.txt", FileMode.Append);
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
