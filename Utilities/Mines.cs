using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Net.NetworkInformation;
using System.Xml.Linq;

namespace SeedFinding
{
    public enum Geode
    {
        Geode = 535,
        FrozenGeode = 536,
        MagmaGeode = 537,
        OmniGeode = 749,
        Trove = 275,
        Coconut = 791,
        MysteryBox,
        GoldenMysteryBox
    }

    public enum FloorType
    {
        Slime,
        Monster,
        Dino,
        Quarry,
        Mushroom,
        None
    }
    public class Mines
    {
        public static List<Geode> allGeodes = new() { Geode.Geode, Geode.FrozenGeode, Geode.MagmaGeode, Geode.OmniGeode, Geode.Trove, Geode.Coconut, Geode.MysteryBox, Geode.GoldenMysteryBox };

        public static List<int> CheckStone1_5(int gameId, int floor, int x, int y, bool ladder=false, bool geologist = false, bool excavator=false)
        {
            return CheckStone1_5(x*1000 + y + floor + gameId / 2, ladder,geologist,excavator,floor);
        }

        public static List<int> CheckStone1_6(int day, int gameId, int floor, int x, int y, bool ladder = false, bool geologist = false, bool excavator = false)
        {

            Random r = Utility.CreateDaySaveRandom(day, gameId, x * 1000, y, floor);
            return CheckStone(r, ladder, geologist, excavator,floor);
        }

        public static List<int> CheckStone1_5(int seed, bool ladder = false, bool geologist = false, bool excavator = false, int floor = 0)
        {

            Random r = new Random(seed);
            return CheckStone(r, ladder, geologist, excavator, floor);
        }

        private static List<int> CheckStone(Random r,bool ladder=false, bool geologist = false, bool excavator=false, int floor=0)
        {
            List<int> results = new List<int>();
            r.Next();
            if (!ladder)
            {
                r.NextDouble();
            }
            if (geologist)
            {
                r.NextDouble();
            }

            if (r.NextDouble() < 0.022 * (excavator ? 2 : 1))
            {
                if (geologist && r.NextDouble() < 0.5)
                {
                    results.Add(535);
                }
                results.Add(535);
            }
            if (r.NextDouble() < 0.005 * (excavator ? 2 : 1))
            {
                if (geologist && r.NextDouble() < 0.5)
                {
                    results.Add(749);
                }
                results.Add(749);
            }

            if (r.NextDouble() < 0.05 * 0.8) 
            {
                r.NextDouble();
                r.NextDouble();
                if (r.NextDouble() < 0.25)
                {
                    results.Add(382);
                }

                results.Add(378);
                // Other ores are possible and relate to the floor number.  Haven't implemented yet.
            }

            return results;
        }

        public static int getRandomIntWithExceptions(Random r, int minValue, int maxValueExclusive, List<int> exceptions)
        {
            int value = r.Next(minValue, maxValueExclusive);
            while (exceptions != null && exceptions.Contains(value))
            {
                value = r.Next(minValue, maxValueExclusive);
            }
            return value;
        }

        public static (string,int) GetGeodeContents1_6(int gameId, int geodesCracked, Geode whichGeode, int deepestMineLevel=0, bool qibeans = false, bool farmingMastery=false, int fishingLevel=0, bool hasCoconutHat = false, bool hasMysteryBook = false)
        {
            Random r = Utility.CreateRandom(geodesCracked, gameId / 2); 
            int prewarm_amount = r.Next(1, 10);
            for (int i = 0; i < prewarm_amount; i++)
            {
                r.NextDouble();
            }
            prewarm_amount = r.Next(1, 10);
            for (int j = 0; j < prewarm_amount; j++)
            {
                r.NextDouble();
            }
            if (whichGeode == Geode.MysteryBox || whichGeode == Geode.GoldenMysteryBox)
            {
                if (geodesCracked > 10 || whichGeode == Geode.GoldenMysteryBox)
                {
                    double rareMod = ((whichGeode == Geode.MysteryBox) ? 1 : 2);
                    if (whichGeode == Geode.GoldenMysteryBox && farmingMastery && r.NextBool(0.005))
                    {
                        return ("(O)GoldenAnimalCracker",1);
                    }
                    if (r.NextBool(0.002 * rareMod))
                    {
                        return ("(O)279",1);
                    }
                    if (r.NextBool(0.004 * rareMod))
                    {
                        return ("(O)74",1);
                    }
                    if (r.NextBool(0.008 * rareMod))
                    {
                        return ("(O)166",1);
                    }
                    if (r.NextBool(0.01 * rareMod + (hasMysteryBook ? 0.0 : (0.0004 * geodesCracked))))
                    {
                        return (r.Choose("(O)PurpleBook", "(O)Book_Mystery"),1);
                    }
                    if (r.NextBool(0.01 * rareMod))
                    {
                        return (r.Choose("(O)797", "(O)373"),1);
                    }
                    if (r.NextBool(0.01 * rareMod))
                    {
                        return ("(H)MysteryHat",1);
                    }
                    if (r.NextBool(0.01 * rareMod))
                    {
                        return ("(S)MysteryShirt",1);
                    }
                    if (r.NextBool(0.01 * rareMod))
                    {
                        return ("(WP)MoreWalls:11",1);
                    }
                    if (r.NextBool(0.1) || whichGeode == Geode.GoldenMysteryBox)
                    {
                        switch (r.Next(15))
                        {
                            case 0:
                                return ("(O)288", 5);
                            case 1:
                                return ("(O)253", 3);
                            case 2:
                                if (fishingLevel >= 6 && r.NextBool())
                                {
                                    return (r.Choose("(O)687", "(O)695"),1);
                                }
                                return ("(O)242", 2);
                            case 3:
                                return ("(O)204", 2);
                            case 4:
                                return ("(O)369", 20);
                            case 5:
                                return ("(O)466", 20);
                            case 6:
                                return ("(O)773", 2);
                            case 7:
                                return ("(O)688", 3);
                            case 8:
                                return ("(O)" + r.Next(628, 634),1);
                            case 9:
                                return ("(O)getRandomLowGradeCropForThisSeason", 20);
                            case 10:
                                if (r.NextBool())
                                {
                                    return ("(W)60",1);
                                }
                                return (r.Choose("(O)533", "(O)534"),1);
                            case 11:
                                return ("(O)621",1);
                            case 12:
                                return ("(O)MysteryBox", r.Next(3, 5));
                            case 13:
                                return ("(O)SkillBook_" + r.Next(5),1);
                            case 14:
                                return ("(O)RaccoonSeed", 8);
                        }
                    }
                }
                switch (r.Next(14))
                {
                    case 0:
                        return ("(O)395", 3);
                    case 1:
                        return ("(O)287", 5);
                    case 2:
                        return ("(O)getRandomLowGradeCropForThisSeason", 8);
                    case 3:
                        return ("(O)" + r.Next(727, 734),1);
                    case 4:
                        return ("(O)" + getRandomIntWithExceptions(r, 194, 240, new List<int> { 217 }),1);
                    case 5:
                        return ("(O)709", 10);
                    case 6:
                        return ("(O)369", 10);
                    case 7:
                        return ("(O)466", 10);
                    case 8:
                        return ("(O)688",1);
                    case 9:
                        return ("(O)689",1);
                    case 10:
                        return ("(O)770", 10);
                    case 11:
                        return ("(O)MixedFlowerSeeds", 10);
                    case 12:
                        if (r.NextBool(0.4))
                        {
                            return r.Next(4) switch
                            {
                                0 => ("(O)525",1),
                                1 => ("(O)529",1),
                                2 => ("(O)888",1),
                                _ => ("(O)" + r.Next(531, 533),1),
                            };
                        }
                        return ("(O)MysteryBox", 2);
                    case 13:
                        return ("(O)690",1);
                    default:
                        return ("(O)382",1);
                }
            }
            if (r.NextBool(0.1) && qibeans)
            {
                return ("(O)890", (!r.NextBool(0.25)) ? 1 : 5);
            }
            if (whichGeode == Geode.Trove || whichGeode == Geode.Coconut || r.NextBool())
            {
                switch (whichGeode)
                {
                    case Geode.Geode:
                        return (r.ChooseFrom(new List<string> {
          "(O)538",
          "(O)542",
          "(O)548",
          "(O)549",
          "(O)552",
          "(O)555",
          "(O)556",
          "(O)557",
          "(O)558",
          "(O)566",
          "(O)568",
          "(O)569",
          "(O)571",
          "(O)574",
          "(O)576",
          "(O)121" }), 1);
                        break;
                    case Geode.FrozenGeode:
                        return (r.ChooseFrom(new List<string> {
          "(O)541",
          "(O)544",
          "(O)545",
          "(O)546",
          "(O)550",
          "(O)551",
          "(O)559",
          "(O)560",
          "(O)561",
          "(O)564",
          "(O)567",
          "(O)572",
          "(O)573",
          "(O)577",
          "(O)123" }), 1);

                        break;
                    case Geode.MagmaGeode:
                        return (r.ChooseFrom(new List<string> {
          "(O)539",
          "(O)540",
          "(O)543",
          "(O)547",
          "(O)553",
          "(O)554",
          "(O)562",
          "(O)563",
          "(O)565",
          "(O)570",
          "(O)575",
          "(O)578",
          "(O)122" }), 1);

                        break;
                    case Geode.OmniGeode:
                        if (r.NextBool(0.008) && geodesCracked >= 16)
                        {
                            return ("(O)74", 1);
                        }
                        return (r.ChooseFrom(new List<string> {
          "(O)538",
          "(O)542",
          "(O)548",
          "(O)549",
          "(O)552",
          "(O)555",
          "(O)556",
          "(O)557",
          "(O)558",
          "(O)566",
          "(O)568",
          "(O)569",
          "(O)571",
          "(O)574",
          "(O)576",
          "(O)541",
          "(O)544",
          "(O)545",
          "(O)546",
          "(O)550",
          "(O)551",
          "(O)559",
          "(O)560",
          "(O)561",
          "(O)564",
          "(O)567",
          "(O)572",
          "(O)573",
          "(O)577",
          "(O)539",
          "(O)540",
          "(O)543",
          "(O)547",
          "(O)553",
          "(O)554",
          "(O)562",
          "(O)563",
          "(O)565",
          "(O)570",
          "(O)575",
          "(O)578",
          "(O)121",
          "(O)122",
          "(O)123" }), 1);

                        break;
                    case Geode.Trove:
                        return (r.ChooseFrom(new List<string> { "(O)100",
          "(O)101",
          "(O)103",
          "(O)104",
          "(O)105",
          "(O)106",
          "(O)108",
          "(O)109",
          "(O)110",
          "(O)111",
          "(O)112",
          "(O)113",
          "(O)114",
          "(O)115",
          "(O)116",
          "(O)117",
          "(O)118",
          "(O)119",
          "(O)120",
          "(O)121",
          "(O)122",
          "(O)123",
          "(O)124",
          "(O)125",
          "(O)166",
          "(O)373",
          "(O)797",
          "(O)Book_Artifact" }), 1);

                        break;
                    case Geode.Coconut:

                        if (r.NextBool(0.05) && !hasCoconutHat)
                        {
                            return ("(H)75", 1);
                        }
                        string item = r.ChooseFrom(new List<string> {
          "(O)69",
          "(O)835",
          "(O)833",
          "(O)831",
          "(O)820",
          "(O)292",
          "(O)386" });
                        if (item == "(O)833" || item == "(O)831" || item == "(O)386")
                        {
                            return (item, 5);
                        }
                        return (item, 1);
                        break;
                }
            }
            /*if (Game1.objectData.TryGetValue(geode.ItemId, out var data))
            {
                List<ObjectGeodeDropData> geodeDrops = data.GeodeDrops;
                if (geodeDrops != null && geodeDrops.Count > 0 && (!data.GeodeDropsDefaultItems || r.NextBool()))
                {
                    foreach (ObjectGeodeDropData drop in data.GeodeDrops.OrderBy((ObjectGeodeDropData p) => p.Precedence))
                    {
                        if (!r.NextBool(drop.Chance) || (drop.Condition != null && !GameStateQuery.CheckConditions(drop.Condition, null, null, null, null, r)))
                        {
                            continue;
                        }
                        Item item = ItemQueryResolver.TryResolveRandomItem(drop, new ItemQueryContext(null, null, r), avoidRepeat: false, null, null, null, delegate (string query, string error)
                        {
                            Game1.log.Error($"Geode item '{geode.QualifiedItemId}' failed parsing item query '{query}' for {"GeodeDrops"} entry '{drop.Id}': {error}");
                        });
                        if (item != null)
                        {
                            if (drop.SetFlagOnPickup != null)
                            {
                                item.SetFlagOnPickup = drop.SetFlagOnPickup;
                            }
                            return item;
                        }
                    }
                }
            }*/
            int amount = r.Next(3) * 2 + 1;
            if (r.NextBool(0.1))
            {
                amount = 10;
            }
            if (r.NextBool(0.01))
            {
                amount = 20;
            }
            if (r.NextBool())
            {
                switch (r.Next(4))
                {
                    case 0:
                    case 1:
                        return ("(O)390", amount);
                    case 2:
                        return ("(O)330",1);
                    default:
                        return whichGeode switch
                        {
                            Geode.OmniGeode => ("(O)" + (82 + r.Next(3) * 2),1),
                            Geode.Geode => ("(O)86",1),
                            Geode.FrozenGeode => ("(O)84",1),
                            _ => ("(O)82",1),
                        };
                }
            }
            if (whichGeode != Geode.Geode)
            {
                if (whichGeode == Geode.FrozenGeode)
                {
                    return r.Next(4) switch
                    {
                        0 => ("(O)378", amount),
                        1 => ("(O)380", amount),
                        2 => ("(O)382", amount),
                        _ => ((deepestMineLevel > 75) ? "(O)384" : "(O)380", amount),
                    };
                }
                return r.Next(5) switch
                {
                    0 => ("(O)378", amount),
                    1 => ("(O)380", amount),
                    2 => ("(O)382", amount),
                    3 => ("(O)384", amount),
                    _ => ("(O)386", amount / 2 + 1),
                };
            }
            return r.Next(3) switch
            {
                0 => ("(O)378", amount),
                1 => ((deepestMineLevel > 25) ? "(O)380" : "(O)378", amount),
                _ => ("(O)382", amount),
            };
        }
        public static void PrintGeodeContents(int gameId, int startingGeode, int count, List<Geode> geodeTypes, string delimiter, bool excludeOres=true, int deepestMineLevel=0, bool qibeans = false, bool printBestGeode=false, int printBestGeodeMinPrice=0, bool before1_5=false)
        {
            List<string> unsellables = new List<string>() { "100", "101", "103", "104", "105", "106", "108", "109", "110", "111", "112", "113", "114", "115", "116", "117", "118", "119", "120", "121", "122", "123", "124", "125", "330", "390" };
            if (excludeOres)
            {
                unsellables = (List<string>)unsellables.Union(new List<string>() { "378", "380", "382", "384", "386" });
            }
            string header = $"Index";
            foreach (Geode geode in geodeTypes)
            {
                header += $"{delimiter}{geode.ToString()}{delimiter}{geode.ToString()} Amount{delimiter}{geode.ToString()} Price";
            }
            if (printBestGeode) { header += $"{delimiter}Best Geode{delimiter}Profit"; }
            Console.WriteLine(header);

            for (int i = startingGeode; i < startingGeode + count; i++)
            {
                string line = $"{i}";
                Geode bestGeode = geodeTypes.First();
                int bestPrice = 0;
                foreach (Geode geode in geodeTypes)
                {
                    var contents = GetGeodeContents1_6(gameId, i, geode, deepestMineLevel, qibeans, before1_5);
                    int price = 0;
                    if (!unsellables.Contains(Item.Get(contents.Item1).id))
                    {
                        price = Item.Get(contents.Item1).Price * contents.Item2;
                    }
                    line += $"{delimiter}{Item.Get(contents.Item1).Name}{delimiter}{contents.Item2}{delimiter}{price}";

                    if (price >= bestPrice)
                    {
                        bestPrice = price;
                        bestGeode = geode;
                    }
                }

                if (printBestGeode && bestPrice > printBestGeodeMinPrice)
                {
                    line += $"{delimiter}{bestGeode}{delimiter}{bestPrice}";
                }
                Console.WriteLine(line);
            }
        }

        

        public static (int,int) GetGeodeContents(int gameId, int geodesCracked, Geode whichGeode, int deepestMineLevel=0, bool qibeans=false, bool before1_5=false)
        {
            Random r = new Random(geodesCracked + gameId / 2);
            int prewarm_amount;
            prewarm_amount = r.Next(1, 10);
            for (int j = 0; j < prewarm_amount; j++)
            {
                r.NextDouble();
            }
            prewarm_amount = r.Next(1, 10);
            for (int i = 0; i < prewarm_amount; i++)
            {
                r.NextDouble();
            }

            if (!before1_5 && r.NextDouble() <= 0.1 && qibeans)
            {
                bool five = r.NextDouble() < 0.25;
                return (890, (!five) ? 1 : 5);
            }
            // TODO, add golden coconut

            if (whichGeode == Geode.Trove || !(r.NextDouble() < 0.5))
            {
                string[] treasures = ObjectInfo.ObjectInformation[((int)(whichGeode)).ToString()].Split('/')[6].Split(' ');
                int index = Convert.ToInt32(treasures[r.Next(treasures.Length)]);
                if (whichGeode == Geode.OmniGeode && r.NextDouble() < 0.008 && geodesCracked > 15)
                {
                    return (74, 1);
                }
                return (index, 1);
            }
            int amount = r.Next(3) * 2 + 1;
            if (r.NextDouble() < 0.1)
            {
                amount = 10;
            }
            if (r.NextDouble() < 0.01)
            {
                amount = 20;
            }
            if (r.NextDouble() < 0.5)
            {
                switch (r.Next(4))
                {
                    case 0:
                    case 1:
                        return (390, amount);
                    case 2:
                        return (330, 1);
                    case 3:
                        {
                            int parentSheetIndex;
                            switch (whichGeode)
                            {
                                case Geode.OmniGeode:
                                    return (82 + r.Next(3) * 2, 1);
                                default:
                                    parentSheetIndex = 82;
                                    break;
                                case Geode.FrozenGeode:
                                    parentSheetIndex = 84;
                                    break;
                                case Geode.Geode:
                                    parentSheetIndex = 86;
                                    break;
                            }
                            return (parentSheetIndex, 1);
                        }
                }
            }
            else
            {
                switch (whichGeode)
                {
                    case Geode.Geode:
                        switch (r.Next(3))
                        {
                            case 0:
                                return (378, amount);
                            case 1:
                                return ((deepestMineLevel > 25) ? 380 : 378, amount);
                            case 2:
                                return (382, amount);
                        }
                        break;
                    case Geode.FrozenGeode:
                        switch (r.Next(4))
                        {
                            case 0:
                                return (378, amount);
                            case 1:
                                return (380, amount);
                            case 2:
                                return (382, amount);
                            case 3:
                                return ((deepestMineLevel > 75) ? 384 : 380, amount);
                        }
                        break;
                    default:
                        switch (r.Next(5))
                        {
                            case 0:
                                return (378, amount);
                            case 1:
                                return (380, amount);
                            case 2:
                                return (382, amount);
                            case 3:
                                return (384, amount);
                            case 4:
                                return (386, amount / 2 + 1);
                        }
                        break;
                }
            }
            return (390, 1);
        }
        
        public static FloorType GetFloorType(int gameID, int day, int floor, bool visitedQuarry=false)
        {
            Random r = new Random(day + floor * 100 + gameID / 2);
            if ( r.NextDouble() < 0.044 && (floor >= 121 || floor % 5 != 0 && floor % 40 > 5 && floor % 40 < 30 && floor % 40 != 19))
            {
                FloorType type;
                if (r.NextDouble() < 0.5)
                {
                    type = FloorType.Monster;
                }
                else
                {
                    type = FloorType.Slime;
                }
                if (floor > 126 && r.NextDouble() < 0.5)
                {
                    type = FloorType.Dino;
                }

                return type;
            }
            else if (floor < 121 && r.NextDouble() < 0.044 && visitedQuarry && floor % 40 > 1 && floor % 5 != 0)
            {
                return FloorType.Quarry;
            }

           r = new Random(day * floor + 4 * floor + gameID / 2);

            if (r.NextDouble() < 0.3 && floor > 2)
            {
                //this.isLightingDark.Value = true;
                //this.lighting = new Color(120, 120, 40);
                if (r.NextDouble() < 0.3)
                {
                    //this.lighting = new Color(150, 150, 60);
                }
            }
            if (r.NextDouble() < 0.15 && floor > 5 && floor != 120)
            {
                //this.isLightingDark.Value = true;
                /*switch (this.getMineArea())
                {
                    case 0:
                    case 10:
                        this.lighting = new Color(110, 110, 70);
                        break;
                    case 40:
                        this.lighting = Color.Black;
                        if (this.GetAdditionalDifficulty() > 0)
                        {
                            this.lighting = new Color(237, 212, 185);
                        }
                        break;
                    case 80:
                        this.lighting = new Color(90, 130, 70);
                        break;
                }*/
            }
            if (r.NextDouble() < 0.035 && floor >= 80 && floor < 120 && floor % 5 != 0 )
            {
                return FloorType.Mushroom;
            }

            return FloorType.None;
        }

        public static int DropShaftDrop(int gameid, int floor, int day)
        {
            Random random = new Random(floor + gameid + day-1);
            int levelsDown = random.Next(3, 9);
            if (random.NextDouble() < 0.1)
            {
                levelsDown = levelsDown * 2 - 1;
            }
            if (floor < 220 && floor + levelsDown > 220)
            {
                levelsDown = 220 - floor;
            }

            return levelsDown;
        }
    }
}