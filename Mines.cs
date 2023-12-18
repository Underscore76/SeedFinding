using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;

namespace SeedFinding
{
    public enum Geode
    {
        Geode = 535,
        FrozenGeode = 536,
        MagmaGeode = 537,
        OmniGeode = 749,
        Trove = 275,
        Coconut = 791
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
        public static List<int> CheckStone(int gameId, int floor, int x, int y, bool ladder=false, bool geologist = false, bool excavator=false)
        {
            return CheckStone(x*1000 + y + floor + gameId / 2, ladder,geologist,excavator,floor);
        }

        public static List<int> CheckStone(int seed,bool ladder=false, bool geologist = false, bool excavator=false, int floor=0)
        {
            List<int> results = new List<int>();
            Random r = new Random(seed);
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

        public static void PrintGeodeContents(int gameId, int startingGeode, int count, List<Geode> geodeTypes, string delimiter, bool excludeOres=true, int deepestMineLevel=0, bool qibeans = false, bool printBestGeode=false, int printBestGeodeMinPrice=0, bool before1_5=false)
        {
            List<int> unsellables = new List<int>() { 100, 101, 103, 104, 105, 106, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 330, 390 };
            if (excludeOres)
            {
                unsellables = (List<int>)unsellables.Union(new List<int>() { 378, 380, 382, 384, 386 });
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
                    var contents = GetGeodeContents(gameId, i, geode, deepestMineLevel, qibeans, before1_5);
                    int price = 0;
                    if (!unsellables.Contains(ObjectInfo.Get(contents.Item1).Id))
                    {
                        price = ObjectInfo.Get(contents.Item1).Cost * contents.Item2;
                    }
                    line += $"{delimiter}{ObjectInfo.Get(contents.Item1).Name}{delimiter}{contents.Item2}{delimiter}{price}";

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