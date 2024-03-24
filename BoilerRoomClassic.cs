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
                if (item.Item1 == "82")
                {
                    fireQuartz = geode;
                }
                if (item.Item1 == "84")
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

            int[] days = new int[] { 5, 7, 12, 14, 19};
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
            List<int> seeds = new List<int> {4194914,4194915,-8647703,-12871938,39310548,39310549,-40319470,-40319471,47354042,47354043,48462440,48462441,-68014316,-68014317,-98204237,-106204638,-106204639,-110945306,120949636,120949637,128303770,-131840446,-131840447,-134896692,-134896693,153793042,160762886,160762887,162597145,-162670693,164671692,164671693,174599658,174599659,-193932048,-194182533,-195559127,207141399,211090280,248740514,248740515,-248884542,-248884543,263756854,263756855,-278124195,-282471819,-299033136,-299033137,306393200,306393201,314552916,314552917,-315809154,-315809155,319224020,319224021,319409806,319409807,-322417910,-322417911,-333853869,-349415796,-349415797,355792680,355792681,-358950798,-358950799,-358779476,-358779477,-370936837,-381972264,383747619,-385642171,390620456,395370500,395370501,-408041594,-408041595,-414151046,-414151047,418313488,418313489,430713517,-440755816,443016567,-451599878,-451599879,452812539,456478116,-499709362,-499709363,-502661466,-502661467,-513601208,515337479,543213014,547179374,547179375,-548863828,-548863829,554585910,561745304,-563923618,-563923619,-574492716,-580034263,-580931406,-580931407,580759730,582804692,-586272964,586586638,586586639,587424680,587424681,-596716388,-596716389,-607468138,-607468139,612233198,612233199,-618710680,-618710681,629197940,640245670,640245671,660623000,665773602,665773603,669856643,675753580,675753581,-700524379,-706407636,-706407637,717737971,-720451722,-720451723,721883396,721883397,727600654,737944576,737944577,-743484860,-743484861,-756217745,-758069890,-758069891,787391322,787391323,-789749175,798828246,798828247,815409594,815409595,-815974573,-818584730,-823111070,-829433452,838601814,838601815,839468235,-847215936,-847215937,860838171,-861194346,-861194347,874385656,874385657,879948534,879948535,907222319,909935304,909935305,934638262,934638263,935928291,936672598,948854810,948814974,948814975,-949211956,-949211957,971399939,-977181913,-977133424,-977133425,986976190,986976191,1002439345,-1004352068,-1004352069,1007974336,1015154418,1015154419,1023259713,1038350876,1038350877,1050537693,1058630642,1058630643,-1094702330,1104672952,1104672953,-1125170066,-1125170067,1133672568,1133672569,-1159401612,-1162262028,1188790240,1188790241,1189371798,1189371799,-1201585720,1206602329,1221641442,1221641443,-1228787406,-1228787407,-1230065601,1242184662,1242184663,1242548577,1248360374,1248360375,-1257719242,-1277553238,-1277553239,-1297996314,-1297996315,-1300002680,-1300002681,-1314440590,-1314440591,-1320483436,-1320483437,-1341899552,-1341899553,1345477828,1345477829,-1353505722,-1353505723,1358649045,1385373552,1385373553,1388237043,-1409957456,-1409957457,-1410538545,1411030727,1415610324,1415803391,-1445757566,-1445757567,1472717158,1472717159,-1489256362,1491513553,-1500510674,-1500510675,1518416068,1541850580,1541850581,1542786586,-1559643906,-1559643907,1568018910,1568018911,-1568734718,-1568734719,1570331219,-1574017566,1619359995,-1624011920,-1624011921,-1626453478,-1626453479,-1629682204,-1629682205,-1631513976,1639710139,-1648868127,1660409426,1660409427,1661583128,1661583129,-1674043656,-1700489938,-1700489939,-1700671424,-1700671425,-1704203234,-1704203235,1708831857,1729752276,-1734161942,-1734161943,-1737729662,-1737729663,1755715361,1774508674,1774508675,-1783448904,-1783448905,1789921212,1789921213,-1800370925,1818283704,-1819958728,-1819958729,-1839575102,-1839575103,-1866808780,1871875472,1871875473,-1886210758,-1886210759,1887201670,1887201671,1891727213,1901504754,1901504755,-1901928814,1907272108,1907272109,-1918942524,-1918942525,-1930940298,-1930940299,1938305138,1938305139,1940885404,1968535264,1968535265,-1971169013,-1976415679,1992628404,1992628405,1995631470,1995631471,2006187480,2006187481,2007416212,-2013267970,-2033726382,-2042415766,-2042415767,-2049463191,2050716529,2054997958,2054997959,-2063986688,-2063986689,2065102960,2065102961,-2076717738,-2076717739,2077688488,2080027994,2080027995,2082753972,2100548928,2100548929,-2102155694,2107672860,-2109457043,2119450271,2128302924,2128302925,-2137143910,-2137143911,2138513536,2138513537,2143997833
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
            double seconds = stopwatch.Elapsed.TotalSeconds;
            Console.WriteLine($"Found: {bag.Count} sols in {seconds.ToString("F2")} s");
            validSeeds = bag.ToList();
            validSeeds.Sort();
            return seconds;
        }
    }
}


