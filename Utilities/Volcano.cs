using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SeedFinding
{
    public class Volcano
    {

        public static List<int> GetLevels(int gameId, int day, double dailyLuck = 0.0, int luckLevel = 0)
        {
            List<int> levels = new();
            bool foundSpecialLevel = false;
            float luckMultiplier = 1f + (float)luckLevel * 0.035f + (float)dailyLuck / 2f;
            int lastLayout = 0;
            for (int level = 1; level < 10; level++) {
                if (level == 5)
                {
                    lastLayout = 31;
                    levels.Add(lastLayout);
                    continue;
                }
                if (level == 9)
                {
                    lastLayout = 30;
                    levels.Add(lastLayout);
                    continue;
                }
                List<int> valid_layouts = new();
                for (int i = 1; i < 30; i++)
                {
                    valid_layouts.Add(i);
                }


                Random layout_random = new Random(day * level + level * 5152 + gameId / 2);
                if (level > 1 && layout_random.NextDouble() < 0.5 * luckMultiplier) {
                    if (!foundSpecialLevel)
                    {
                        for (int k = 32; k < 38; k++)
                        {
                            valid_layouts.Add(k);
                        }
                    }
                }
                if (lastLayout != 0)
                {
                    valid_layouts.Remove(lastLayout);
                }

                lastLayout = valid_layouts[layout_random.Next(valid_layouts.Count)];

                if (lastLayout >= 32)
                {
                    foundSpecialLevel = true;
                }

                levels.Add(lastLayout);
            }
            return levels;
        }
    
    }
}
