using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using SeedFinding.Bundles;
using SeedFinding.Cart1_6;
using SeedFinding.Locations;
using SeedFinding.Trash1_6;

namespace SeedFinding
{
    public class BoilerRoomClassic
    {
        // defining the specific bundles needed
        public const ulong DesiredBundles = (
                // crafts
                // boiler
                CompressedFlags.BOILER_BLACKSMITH
                | CompressedFlags.BOILER_ENGINEER
                | CompressedFlags.BOILER_ADVENTURER
                | CompressedFlags.BOILER_ADVENTURER_SOLAR_ESSENCE
                | CompressedFlags.BOILER_ADVENTURER_VOID_ESSENCE
            );


        public static bool ValidSeed(int gameId, bool curate = false)
        {
            int fireQuartz = 0;
            int frozenTear = 0;
            for (int geode = 1; geode < 10; geode++)
            {
                (string, int) item = Mines.GetGeodeContents1_6(gameId, geode, Geode.OmniGeode);
                if (item.Item1 == "(O)82")
                {
                    fireQuartz = geode;
                }
                if (item.Item1 == "(O)84")
                {
                    frozenTear = geode;
                }
                if (fireQuartz != 0 && frozenTear != 0)
                {
                    break;
                }
            }

            if (fireQuartz == 0 || frozenTear == 0) {
                return false;
            }

            int[] days = new int[] { 7, 12, 14, 19};
            bool found = false;
            foreach (var day in days)
            {
                var stock = TravelingCart.GetStock(gameId, day);
                HashSet<string> stockIds = new HashSet<string>(stock.Select(o => o.Id));
                HashSet<string> trashItems = new HashSet<string>(Trash1_6.Trash.getAllTrash(gameId, day, 0.1, hasFurnace: true).Select(x=>x.Id));
                if (RequiredItems.IsSubsetOf(stockIds.Union(trashItems)))
                {
                    found = true;
                }

                if (found)
                {
                    if (curate)
                    {
                        var onlyNeeded = stock.Where(i => RequiredItems.Contains(i.Id)).ToArray();
                        Console.WriteLine($"{gameId}    {day}   {String.Join(",", onlyNeeded)}    {onlyNeeded.Select(i => i.Cost).ToArray().Sum(x =>x)}");
                    }
                    break;
                }
            }

            if (!found)
            {
                return false;
            }

            return true;
        }
        static HashSet<string> RequiredItems = new HashSet<string>()
        {

            "335",//": "Iron Bar

            "336",//": "Gold Bar

            "768",//": "Solar Essence

            "769"//": "Void Essence
        };




        // single search
        public static void Curate()
        {
            List<int> seeds = new List<int> { -3336020,-3336021,7555432,8434848,8434849,-97467872,-97467873,109839182,109839183,133725718,133725719,-137959888,-137959889,176366630,176366631,-193668830,-193668831,204600962,204600963,-220255004,-220255005,224039654,224039655,-226705386,227024212,227024213,-250281370,-250281371,-279465022,-279465023,280963052,280963053,-292139576,-292139577,292657798,292657799,-299102254,-299102255,303682442,303682443,-318558060,-318558061,341639642,341639643,351526856,351526857,-361579608,-361579609,391862786,391862787,430086576,430086577,448463694,448463695,-452709426,-452709427,-495309712,-495309713,536447288,536447289,541958632,541958633,-548818528,-548818529,552236272,552236273,556501076,556501077,592659800,-604935304,-604935305,611524908,611524909,-627064160,-627064161,637051310,637051311,652807912,652807913,-656769658,663618102,663618103,-668223166,-668223167,-671730310,-671730311,-675140646,-675140647,-692995356,-692995357,696620004,696620005,-711945512,-711945513,-717056720,-717056721,-734256698,-734256699,-751110042,-751110043,759865532,759865533,770998586,770998587,-781313484,-781313485,-789182696,-789182697,835265224,835265225,840404550,840404551,842884342,842884343,-859246008,-859246009,-877911158,-877911159,878926662,878926663,900924468,900924469,901240136,911274002,911274003,-914455018,-915542136,-915542137,933734376,933734377,946635956,946635957,967929078,967929079,991654274,991654275,-993294134,-993294135,-994537078,-994537079,-1003581020,-1003581021,-1012476396,-1012476397,-1019313530,-1019313531,-1020068922,-1022685886,-1022685887,1038452072,-1091829852,-1091829853,1093880300,1093880301,-1125422150,-1125422151,1157115326,1157115327,1157594540,1157594541,-1158565876,-1158565877,1173976170,1173976171,1193709930,1193709931,-1198164702,-1198164703,-1213564398,-1213564399,-1217943196,-1217943197,1253068996,1253068997,1260026784,1260026785,-1260530576,-1260530577,1303725786,1303725787,1311286980,1311286981,-1338875868,-1338875869,-1358080849,-1361983514,-1361983515,1417258994,1417258995,1450369748,1450369749,-1502907626,-1502907627,-1529616610,-1529616611,-1540330918,-1540330919,-1548355858,-1548355859,1549235374,1549235375,-1560629004,-1560629005,-1562140334,-1562140335,1577055824,-1579442490,-1579442491,1607711882,1607711883,-1674918332,-1674918333,1678749550,1678749551,1714721098,1714721099,-1721231426,-1721231427,-1771239778,-1771239779,-1771239778,-1771239779,1788895712,1788895713,1794898764,1794898765,-1822165082,-1822165083,1825281964,1825281965,1826156492,1826156493,1843212652,1843212653,1877436214,1877436215,-1892602474,-1892602475,1923844286,1923844287,-1944230724,-1944230725,-1958689170,-1958689171,-1963411488,-1963411489,1981399830,1981399831,1984896242,1984896243,-1993676988,-1993676989,-1999002436,-1999002437,-2038447590,-2038447591,-2051260592,-2051260593,-2057382336,-2057382337,-2081145238,-2081145239,2106952511,2121149380,2121149381,-2123221344,-2123221345,-2134495824,-2134495825,2137113460,2137113461,2140551204,2140551205
            };

            foreach (var seed in seeds)
            {
                ValidSeed(seed, true);
            }
        }

        // parallel search
        public static double Search(int startId, int endId, int blockSize, out List<int> validSeeds)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var bag = new ConcurrentBag<int>();
            var partioner = Partitioner.Create(startId, endId, blockSize);
            Parallel.ForEach(partioner, (range, loopState) =>
            {
                //FileStream fs = new FileStream($"BoilerRoom{Task.CurrentId}.txt", FileMode.OpenOrCreate);
                //StreamWriter stream = new StreamWriter(fs);
                for (int seed = range.Item1; seed < range.Item2; seed++)
                //for (int seed = startId; seed <= endId; seed++)
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
                                fs = new FileStream($"BoilerRoomLegacy{i}.txt", FileMode.Append);
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
                    if (ValidSeed(-seed))
                    {
                        bag.Add(-seed);
                        FileStream fs = null;
                        int i = 0;
                        while (fs == null)
                        {
                            try
                            {
                                fs = new FileStream($"BoilerRoomLegacy{i}.txt", FileMode.Append);
                            }
                            catch
                            {
                                i++;
                            }
                        }
                        StreamWriter stream = new StreamWriter(fs);
                        stream.Write($"{-seed},");
                        Console.WriteLine(-seed);
                        stream.Close();
                    }
                }
                });
            //}
            double seconds = stopwatch.Elapsed.TotalSeconds;
            Console.WriteLine($"Found: {bag.Count} sols in {seconds.ToString("F2")} s");
            validSeeds = bag.ToList();
            validSeeds.Sort();
            return seconds;
        }
    }
}


