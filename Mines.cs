using System;
using System.Collections.Generic;

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

        

        public static (int,int) GetGeodeContents(int gameId, int geodesCracked, Geode whichGeode, int deepestMineLevel=0, bool qibeans=false)
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

            if (r.NextDouble() <= 0.1 && qibeans)
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
    }
}