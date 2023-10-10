// used to search for a specific set of ids in a cart

using System;
using System.Linq;
using SeedFinding.Cart;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net;

namespace SeedFinding
{
    public class KrobusBullitenSeeding
    {
        static HashSet<int> NeededItems = new HashSet<int>()
        {
            //258, //Blueberry
		    //376, //Poppy
			//421, //Sunflower
		    430, //Truffle
			724, //Maple Syrup
			725, //Oak Resin
			//300, //Amaranth
			348 //Wine
        };
        static HashSet<int> BonusItems = new HashSet<int>()
        {
            392, //Nautilus Shell
			446, //Rabbits Foot
			//637  //Pomengranate
        };

        public static bool CheckSeed(int seed)
        {
            Random random = new Random(seed + 3);
            double randResult = random.NextDouble();
            if (randResult < 0.75)
            {
                return false;
            }

            int item = Quests.GetItemDeliveryQuestItem(seed, 3, 1, true, false, 25);
            if (!new List<int>
            { 68, //Topaz
                66, //Amethyst
                78, //Cave Carrot
                80, //Quartz
                86, //Earth Crystal
                152, //Seaweed
                167, //Joja Cola
                153, //Green Algae
                420, //Red Musrhoom
                334, //Copper Bar
                335, //Iron Bar
                336 //Gold Bar
            }.Contains(item))
            {
                return false;
            }

            List<List<int>> spots = new List<List<int>>();
            spots.Add(new List<int> { 25, 28, 5 });
            spots.Add(new List<int> { 25, 28, 6 });
            spots.Add(new List<int> { 25, 30, 5 });
            //if (seed % 2 == 1)
            //{
            //    return false;
            //}
            if (NightEvents.GetEvent(seed, 4) != NightEvents.Event.Fairy)
            {
                return false;
            }

            bool found = false;
            foreach (List<int> spot in spots)
            {
                List<int> results = Mines.CheckStone(seed, spot[0], spot[1], spot[2], false, false);
                //if (results.Intersect(new List<int> { (int)Geode.Geode, (int)Geode.OmniGeode }).Count() == 2)
                //if (results.Count(x => x == (int)Geode.Geode) + results.Count(x => x == (int)Geode.OmniGeode) == 4 && results.Count() > 4)
                //if (results.Count(x => x == (int)Geode.Geode) + results.Count(x => x == (int)Geode.OmniGeode) == 4)
                if (results.Count() >= 4)
                    {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                return false;
            }

            /*int copperCount = 0;
            int geodeMoney = 0;

            for (int num = 0; num < 20; num++)
            {
                (int, int) content = Mines.GetGeodeContents(seed, num + 1 + 158, Geode.Geode);
                if (content.Item1 == 378)
                {
                    copperCount += content.Item2;
                    continue;
                }

                int price = int.Parse(ObjectInfo.ObjectInformation[content.Item1.ToString()].Split('/')[1]) * content.Item2;
                content = Mines.GetGeodeContents(seed, num + 1 + 158, Geode.OmniGeode);
                int omniPrice = int.Parse(ObjectInfo.ObjectInformation[content.Item1.ToString()].Split('/')[1]) * content.Item2;

                geodeMoney += Math.Max(price, omniPrice);

            }
            if (copperCount < 45 || geodeMoney < 5500 - copperCount * 75)
            {
                return false;
            }*/

            //Console.WriteLine($"{seed} {copperCount} {geodeMoney}");
            Console.WriteLine($"{seed} {ObjectInfo.Get(item).Name}");
            return true;
        }

        public static double Search(int numSeeds, int blockSize, out List<string> validSeeds)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var bag = new ConcurrentBag<string>();
            var partioner = Partitioner.Create(0, numSeeds, blockSize);
            Parallel.ForEach(partioner, (range, loopState) =>
            {
                for (int seed = range.Item1; seed < range.Item2; seed++)
                {
                    if (!CheckSeed(seed))
                    {
                        continue;
                    }

                    bag.Add(seed.ToString());
                }
            });
            double seconds = stopwatch.Elapsed.TotalSeconds;
            Console.WriteLine($"Found: {bag.Count} sols in {seconds.ToString("F2")} s");
            validSeeds = bag.ToList();
            validSeeds.Sort();
            return seconds;
        }

        public static bool ValidGeodes(int seed)
        {
            Console.WriteLine(Mines.GetGeodeContents(seed, 1, Geode.Geode));
            return true;
        }

        public static bool ValidMines(int seed)
        {
            Console.WriteLine(string.Join( ",",Mines.CheckStone(548284736, 25, 28, 6)));
            return true;
        }

        public static bool ValidCart(int seed)
        {
            //if (NightEvents.GetEvent(seed,6) != NightEvents.Event.Fairy)
            //{
            //    continue;
            //}
            var stock = CompressedTravelingCart.GetStock(seed, 5)
                .Select(o => o.Id).ToHashSet();

            //int neededCount = stock.Intersect(NeededItems).Count();
            int bonusCount = stock.Intersect(BonusItems).Count();

            bool neededCount = NeededItems.IsSubsetOf(stock);
            //int bonusCount = stock.Intersect(BonusItems).Count();

            //if (neededCount < 4 && bonusCount < 4)
            if (!neededCount)
            {
                return false;
            }

            Console.WriteLine($"{seed} {neededCount} {bonusCount}");
            return true;
        }

        public static string Report(int seed)
        {
            string output = "";
            foreach (var item in CompressedTravelingCart.GetStock(seed))
            {
                output += '\t' + ObjectInfo.Get(item.Id).Name;
            }
            return $"{seed} {output}";
        }
    }
}

