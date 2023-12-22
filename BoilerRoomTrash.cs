using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using SeedFinding.Bundles;
using SeedFinding.Cart;
using SeedFinding.Locations;
using SeedFinding.Trash;

namespace SeedFinding
{
    public class BoilerRoomTrash
    {


        static bool ValidSeed(int gameId, bool curate = false)
        {
            int fireQuartz = 0;
            int frozenTear = 0;
            int earthCrystal = 0;
            for (int geode = 1; geode < 10; geode++)
            {
                (int, int) item = Mines.GetGeodeContents(gameId, geode, Geode.OmniGeode);
                if (fireQuartz == 0 && item.Item1 == 82)
                {
                    fireQuartz = geode;
                    continue;
                }
                if (frozenTear == 0 && item.Item1 == 84)
                {
                    frozenTear = geode;
                    continue;
                }
                item = Mines.GetGeodeContents(gameId, geode, Geode.Geode);

                if (item.Item1 == 86)
                {
                    earthCrystal = geode;
                    break;
                }
            }

            if (fireQuartz == 0 || frozenTear == 0) {
                return false;
            }

            int[] days = new int[] { 5, 7, 12, 14, 19, 21, 26, 28 };
            bool found = false;
            int cartDay = 0;
            int crabItems = 0;

            HashSet<string> remainingItems = new HashSet<string>(RequiredItems);
            if (earthCrystal != 0)
            {
                remainingItems.Remove("86");
            }

            //for (int day = 5; day < 28; day++)
            foreach (var day in days)
            {
                var stock = TravelingCart.GetStock(gameId, day);
                HashSet<string> stockIds = new HashSet<string>(stock.Select(o => o.Id.ToString()));
                HashSet<string> trashItems = Trash.Trash.getAllTrash(gameId, day, 0.1, hasFurnace: true);
                if (!trashItems.Contains("749"))
                {
                    continue;
                }
                if (CartItems.IsSubsetOf(stockIds))
                {
                    found = true;
                    crabItems = CrabItems.Intersect(stockIds).Count();
                    cartDay = day;

                    foreach (var item in stockIds) {
                        remainingItems.Remove(item);
                    }
                }

                if (found)
                {
                    if (curate)
                    {
                        var onlyNeeded = stock.Where(i => CartItems.Contains(i.Id.ToString())).ToArray();
                        Console.WriteLine($"{gameId}    {day}   {String.Join(",", onlyNeeded)}    {onlyNeeded.Select(i => i.Cost).ToArray().Sum(x => x)}");
                    }
                    break;
                }
            }

            if (!found)
            {
                return false;
            }

            // Now second trash day
            found = false;
            int moreCrabItems = 0;
            for (int day = 5; day < 28; day++)
            {
                if (day == cartDay)
                {
                    continue;
                }

                HashSet<string> trashItems = Trash.Trash.getAllTrash(gameId, day, 0.1, hasFurnace: true);
                if (!trashItems.Contains("749"))
                {
                    continue;
                }

                if (days.Contains(day))
                {
                    var stock = TravelingCart.GetStock(gameId, day);
                    HashSet<string> stockIds = new HashSet<string>(stock.Select(o => o.Id.ToString()));
                    moreCrabItems = CrabItems.Intersect(stockIds).Count();

                    trashItems.UnionWith(stockIds);

                }

                if (crabItems + moreCrabItems > 0 && remainingItems.IsSubsetOf(trashItems))
                {
                    found = true;
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
            "86", // Earth Crystal
            "80", // Quartz
            "334", // Copper Bar
            "335",//": "Iron Bar

            "336",//": "Gold Bar

            "768",//": "Solar Essence

            "769" //": "Void Essence
        };
        static HashSet<string> CartItems = new HashSet<string>()
        {

            //335,//": "Iron Bar

            //336,//": "Gold Bar

            "768",//": "Solar Essence

            "769"//": "Void Essence
        };
        static HashSet<string> CrabItems = new HashSet<string>()
        {
            "372", // Clam
            "715", // Lobster
            "716", // Crayfish
            "717", // Crab
            "718", // Cockle
            "719", // Mussel
            "720", // Shrimp
            "721", // Snail
            "722", // Periwinkle
            "723" // Oyster

        };




        // single search
        public static void Curate()
        {
            List<int> seeds = new List<int> {-2117246539,-2117246537,-2100532792,-2089575220,-2062036062,-2055208307,-2055208300,-2055208298,-2055208293,-2055208291,-2032324152,-2024319400,-2001941680,-1997358182,-1994616543,-1949694619,-1949186503,-1942731028,-1942731026,-1929060407,-1801774861,-1783513817,-1778385526,-1734056280,-1723626652,-1710333222,-1636595764,-1636595762,-1628067445,-1627129891,-1589661186,-1570377054,-1554087629,-1542385809,-1527634134,-1511074586,-1511074584,-1491460302,-1491257459,-1486956085,-1484147961,-1460953306,-1427012979,-1413221003,-1383963082,-1350314930,-1345886556,-1339410757,-1331175871,-1295165995,-1295165979,-1287917595,-1254477661,-1242426124,-1242426122,-1242426117,-1242426115,-1236149475,-1236149473,-1196411049,-1149690395,-1138864923,-1103267471,-1099276768,-1099241581,-1099241567,-1099241565,-1093028937,-1084148878,-1078899974,-1073494860,-1068048580,-1062370888,-1018716584,-1018716582,-1018106709,-1018106707,-971366161,-970461672,-961128392,-957933261,-949800207,-944858000,-943604238,-942001822,-926638605,-926638603,-926638598,-926638596,-924934585,-882241496,-877774431,-868420110,-846934191,-846868768,-804234975,-793659799,-791022518,-734974060,-687062162,-669960034,-654309823,-633842107,-633532908,-624479695,-613939414,-610390370,-607490732,-588472610,-566465310,-564492265,-557097056,-543850138,-512964578,-493529342,-480809993,-480809991,-469173472,-469173470,-469173465,-469173463,-418518576,-408357262,-394689046,-378941375,-372949573,-367546238,-356449328,-327632251,-327632249,-327632244,-327632242,-283475204,-262291623,-231728897,-225910868,-225910866,-214586470,-214586468,-214586463,-200563225,-154714616,-146351208,-125722313,-109008620,-67816912,-65371058,-40392396,-23430440,-23430438,-2697145,14271228,57862249,57862251,57862256,57862258,67816891,69647489,150696415,214586442,214586444,214586449,214586451,217691174,225910847,225910849,242462337,244498174,248517172,327632223,327632225,353492986,362384395,369618548,382953868,394689013,394689018,394689020,394689027,425393381,469173444,469173446,469173451,469173453,480809967,480809972,480809974,540661461,545149335,545149337,564469229,638397904,750351899,751267986,850773104,885391073,909829769,926638572,926638577,926638579,926638584,926638586,1010701553,1018106681,1018106683,1018106688,1018106690,1018716542,1018716544,1018716563,1018716565,1048093117,1049974459,1098663402,1099241546,1099523100,1153264523,1242426096,1242426098,1263684038,1320148325,1332352260,1332352262,1332352267,1339410724,1339410726,1339410731,1339410733,1344099161,1424261229,1424619296,1470975392,1499686643,1511074565,1511074567,1517247620,1517247622,1517247627,1517247629,1529844821,1636595738,1889459446,2001078727,2055208272,2055208274,2100901510,2117246513,2117246518,2117246520
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
                for (int seed = range.Item1; seed < range.Item2; seed++)
                {
                    if (ValidSeed(seed))
                    {
                        bag.Add(seed);

                        Console.WriteLine(seed);
                    }
                    if (ValidSeed(-seed))
                    {
                        bag.Add(-seed);

                        Console.WriteLine(-seed);
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


