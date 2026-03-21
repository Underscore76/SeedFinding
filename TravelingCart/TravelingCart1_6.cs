using System;
using System.Collections.Generic;

namespace SeedFinding.Cart1_6
{
    public struct CartItem
    {
        public string Id;
        public int Cost;
        public int Quantity;

        public CartItem(string id, int cost, int qty)
        {
            Id = id;
            Cost = cost;
            Quantity = qty;
        }
        public override string ToString()
        {
            return string.Format("{0}: C:{1:D4} Q:{2}", Item.Get(Id).Name, Cost, Quantity);
        }
    }
    public class TravelingCart
    {
        public static HashSet<string> possibleItemsMaster;
        public static HashSet<string> possibleItemsMaster163;
        public static List<float> randomAmounts;
        public static List<float> randomMultiply;
        public static (int,int)[] itemAndAdvances163;
        public static (int,int)[] itemAndAdvances;
        public static int totalCallsAfter; //Same between 1.6.3+ and 1.6.0
        public static Dictionary<string,int> idToCall;
        public static bool version163;

        static TravelingCart()
        {
            possibleItemsMaster = new()
            {
                "16",
                "18",
                "20",
                "22",
                "24",
                "78",
                "88",
                "90",
                "92",
                "128",
                "129",
                "130",
                "131",
                "132",
                "136",
                "137",
                "138",
                "139",
                "140",
                "141",
                "142",
                "143",
                "144",
                "145",
                "146",
                "147",
                "148",
                "149",
                "150",
                "151",
                "154",
                "155",
                "156",
                "164",
                "165",
                "167",
                "176",
                "174",
                "180",
                "182",
                "184",
                "186",
                "188",
                "190",
                "192",
                "194",
                "195",
                "196",
                "197",
                "198",
                "199",
                "200",
                "201",
                "202",
                "203",
                "204",
                "205",
                "206",
                "207",
                "208",
                "209",
                "210",
                "211",
                "212",
                "213",
                "214",
                "215",
                "216",
                "218",
                "219",
                "220",
                "221",
                "222",
                "223",
                "224",
                "225",
                "226",
                "227",
                "228",
                "229",
                "230",
                "231",
                "232",
                "233",
                "234",
                "235",
                "236",
                "237",
                "238",
                "239",
                "240",
                "241",
                "242",
                "243",
                "244",
                "248",
                "250",
                "251",
                "252",
                "253",
                "254",
                "256",
                "257",
                "258",
                "259",
                "260",
                "262",
                "264",
                "266",
                "268",
                "270",
                "271",
                "272",
                "273",
                "274",
                "276",
                "278",
                "280",
                "281",
                "282",
                "283",
                "284",
                "286",
                "287",
                "288",
                "293",
                "296",
                "298",
                "299",
                "300",
                "301",
                "302",
                "303",
                "304",
                "306",
                "307",
                "309",
                "310",
                "311",
                "322",
                "323",
                "324",
                "325",
                "328",
                "329",
                "330",
                "331",
                "333",
                "334",
                "335",
                "336",
                "337",
                "338",
                "340",
                "342",
                "344",
                "346",
                "347",
                "348",
                "350",
                "368",
                "369",
                "370",
                "371",
                "372",
                "378",
                "380",
                "382",
                "384",
                "386",
                "388",
                "390",
                "392",
                "393",
                "394",
                "396",
                "397",
                "398",
                "399",
                "400",
                "401",
                "402",
                "404",
                "405",
                "406",
                "407",
                "408",
                "409",
                "410",
                "411",
                "412",
                "414",
                "415",
                "416",
                "418",
                "420",
                "421",
                "422",
                "424",
                "426",
                "428",
                "430",
                "432",
                "433",
                "436",
                "438",
                "440",
                "442",
                "444",
                "446",
                "456",
                "457",
                "459",
                "465",
                "466",
                "472",
                "473",
                "474",
                "475",
                "476",
                "477",
                "478",
                "479",
                "480",
                "481",
                "482",
                "483",
                "484",
                "485",
                "486",
                "487",
                "488",
                "489",
                "490",
                "491",
                "492",
                "493",
                "494",
                "495",
                "496",
                "497",
                "498",
                "499",
                "427",
                "429",
                "453",
                "455",
                "431",
                "425",
                "591",
                "593",
                "595",
                "597",
                "599",
                "376",
                "604",
                "605",
                "606",
                "607",
                "608",
                "609",
                "610",
                "611",
                "612",
                "613",
                "614",
                "618",
                "621",
                "648",
                "649",
                "651",
                "628",
                "629",
                "630",
                "631",
                "632",
                "633",
                "634",
                "635",
                "636",
                "637",
                "638",
                "684",
                "685",
                "686",
                "687",
                "691",
                "692",
                "693",
                "694",
                "695",
                "698",
                "699",
                "700",
                "701",
                "702",
                "703",
                "704",
                "705",
                "706",
                "707",
                "708",
                "709",
                "715",
                "716",
                "717",
                "718",
                "719",
                "720",
                "721",
                "722",
                "723",
                "724",
                "725",
                "726",
                "727",
                "730",
                "728",
                "729",
                "731",
                "732",
                "733",
                "734",
                "766",
                "767",
                "768",
                "769",
                "771",
                "772",
                "773",
                "787",
                "445",
                "267",
                "265",
                "269"
            };
            possibleItemsMaster163 = new()
            {
                "16",
                "18",
                "20",
                "22",
                "24",
                "78",
                "88",
                "90",
                "92",
                "128",
                "129",
                "130",
                "131",
                "132",
                "136",
                "137",
                "138",
                "139",
                "140",
                "141",
                "142",
                "143",
                "144",
                "145",
                "146",
                "147",
                "148",
                "149",
                "150",
                "151",
                "154",
                "155",
                "156",
                "164",
                "165",
                "167",
                "176",
                "174",
                "180",
                "182",
                "184",
                "186",
                "188",
                "190",
                "192",
                "194",
                "195",
                "196",
                "197",
                "198",
                "199",
                "200",
                "201",
                "202",
                "203",
                "204",
                "205",
                "206",
                "207",
                "208",
                "209",
                "210",
                "211",
                "212",
                "213",
                "214",
                "215",
                "216",
                "218",
                "219",
                "220",
                "221",
                "222",
                "223",
                "224",
                "225",
                "226",
                "227",
                "228",
                "229",
                "230",
                "231",
                "232",
                "233",
                "234",
                "235",
                "236",
                "237",
                "238",
                "239",
                "240",
                "241",
                "242",
                "243",
                "244",
                "248",
                "250",
                "251",
                "252",
                "253",
                "254",
                "256",
                "257",
                "258",
                "259",
                "260",
                "262",
                "264",
                "266",
                "268",
                "270",
                "271",
                "272",
                "273",
                "274",
                "276",
                "278",
                "280",
                "281",
                "282",
                "283",
                "284",
                "286",
                "287",
                "288",
                "293",
                "296",
                "298",
                "299",
                "300",
                "301",
                "302",
                "303",
                "304",
                "306",
                "307",
                "309",
                "310",
                "311",
                "322",
                "323",
                "324",
                "325",
                "328",
                "329",
                "330",
                "331",
                "333",
                "334",
                "335",
                "336",
                "337",
                "338",
                "340",
                "342",
                "344",
                "346",
                "347",
                "348",
                "350",
                "368",
                "369",
                "370",
                "371",
                "372",
                "378",
                "380",
                "382",
                "384",
                "386",
                "388",
                "390",
                "392",
                "393",
                "394",
                "396",
                "397",
                "398",
                "399",
                "400",
                "401",
                "402",
                "404",
                "405",
                "406",
                "407",
                "408",
                "409",
                "410",
                "411",
                "412",
                "414",
                "415",
                "416",
                "418",
                "420",
                "421",
                "422",
                "424",
                "426",
                "428",
                "430",
                "432",
                "433",
                "436",
                "438",
                "440",
                "442",
                "444",
                "446",
                "456",
                "457",
                "459",
                "465",
                "466",
                "472",
                "473",
                "474",
                "475",
                "476",
                "477",
                "478",
                "479",
                "480",
                "481",
                "482",
                "483",
                "484",
                "485",
                "486",
                "487",
                "488",
                "489",
                "490",
                "491",
                "492",
                "493",
                "494",
                "495",
                "496",
                "497",
                "498",
//                "499",
                "427",
                "429",
                "453",
                "455",
                "431",
                "425",
                "591",
                "593",
                "595",
                "597",
                "599",
                "376",
                "604",
                "605",
                "606",
                "607",
                "608",
                "609",
                "610",
                "611",
                "612",
                "613",
                "614",
                "618",
                "621",
                "648",
                "649",
                "651",
                "628",
                "629",
                "630",
                "631",
                "632",
                "633",
                "634",
                "635",
                "636",
                "637",
                "638",
                "684",
                "685",
                "686",
                "687",
                "691",
                "692",
                "693",
                "694",
                "695",
                "698",
                "699",
                "700",
                "701",
                "702",
                "703",
                "704",
                "705",
                "706",
                "707",
                "708",
                "709",
                "715",
                "716",
                "717",
                "718",
                "719",
                "720",
                "721",
                "722",
                "723",
                "724",
                "725",
                "726",
                "727",
                "730",
                "728",
                "729",
                "731",
                "732",
                "733",
                "734",
                "766",
                "767",
                "768",
                "769",
                "771",
                "772",
                "773",
                "787",
                "445",
                "267",
                "265",
                "269"
            };

            randomAmounts = new() { 100.0f,
              200.0f,
              300.0f,
              400.0f,
              500.0f,
              600.0f,
              700.0f,
              800.0f,
              900.0f,
              1000.0f};
            randomMultiply = new() {3.0f,
              4.0f,
              5.0f };
            idToCall = new();
            itemAndAdvances163 = new (int,int)[possibleItemsMaster163.Count];
            int amountToAdvance163 = 0;
            int spot163 = 0;
            itemAndAdvances = new (int,int)[possibleItemsMaster.Count];
            int amountToAdvance = 0;
            int spot = 0;
            int callsDeep = 0;
            foreach (var item in Item.Items)
            {
                if (possibleItemsMaster163.Contains(item.Key))
                {
                    idToCall.Add(item.Key,callsDeep);
                    itemAndAdvances163[spot163] = (int.Parse(item.Key),amountToAdvance163);
                    amountToAdvance163 = 0;
                    spot163++;
                }
                else
                {
                    amountToAdvance163++;
                }
                if (possibleItemsMaster.Contains(item.Key))
                {
                    idToCall.TryAdd(item.Key,callsDeep);
                    itemAndAdvances[spot] = (int.Parse(item.Key),amountToAdvance);
                    amountToAdvance = 0;
                    spot++;
                }
                else
                {
                    amountToAdvance++;
                } 
                callsDeep++;
            }
            totalCallsAfter = amountToAdvance;
        }
        
        // Takes a list of string ids, the game seed, and the day, and returns a smaller list containing all items with a chance of showing up that day.
        // Catches 99% of hits at 1/17, 99.9999% at 1/10, 99.99999998 at 1/8 confidence. Defaults to 1/8 for maximal inclusiveness.
        public static List<string> getPlausibles(string[] stringIds, int seed, int day, double confidence = 0.125) 
        {
            int lowerThreshold = (int) Math.Round((double)0x7FFFFFFF * confidence);
            int upperThreshold = 0x7FFFFFFF - 100000;
            int rngSeed = Utility.CreateRandomSeed(day, seed / 2, 0, 0, 0);
            List<string> plausibles = new List<string>();
            foreach (string id in stringIds)
            {
                int guess = FastRandom.fastForward(rngSeed,idToCall.GetValueOrDefault(id));
                if (guess < lowerThreshold || guess > upperThreshold)
                {
                    plausibles.Add(id);
                }
            }
            return plausibles;
        }

        // Takes a string id, the game seed, and the day and returns if there is a chance at all for that item to show up that day.
        // Catches 99% of hits at 1/17, 99.9999% at 1/10, 99.99999998 at 1/8 confidence. Defaults to 1/8 for maximal inclusiveness.
        public static bool isPlausible(string stringId, int seed, int day, double confidence = 0.125)
        {
            int lowerThreshold = (int) Math.Round((double)0x7FFFFFFF * confidence);
            int upperThreshold = 0x7FFFFFFF - 100000;
            int rngSeed = Utility.CreateRandomSeed(day, seed / 2, 0, 0, 0);
            int guess = FastRandom.fastForward(rngSeed,idToCall.GetValueOrDefault(stringId));
            return guess < lowerThreshold || guess > upperThreshold;
        }

        // Returns if it is a cart day, including desert festival and night market.
        public static bool isCartDay(int day)
        {
            return day % 7 == 0 || day % 7 == 5 || (day % 28 >= 15 && day%28 <= 17 && (day/28%4 == 0 || day/28%4 == 3));
        }


       public static List<CartItem> GetStock(long gameSeed, int day, string version = "1.6.3")
        {
            bool version163 = new Version(version) >= new Version("1.6.3");
            int seed = Utility.CreateRandomSeed(day, gameSeed / 2);
            seed = (seed == int.MinValue) ? int.MaxValue : Math.Abs(seed);
            if (seed > (1 << 27)) //Loses guaranteed accuracy somewhere around 1.5 * 2^27 but don't want to try to be overly precise [still is like 99.9% but]
            {
                return GetStockOldRNG(seed,version163);
            }
            int counter = 0;
            List<CartItem> stock = new List<CartItem>();
            SortedList<long, int> tenBest = new();
            FastRandom random = FastRandom.createFR(seed);
            foreach (var item in version163 ? itemAndAdvances163 : itemAndAdvances)
            {
                random.counter+=item.Item2;
                counter++;
                long i = random.Next();
                tenBest.Add(counter + (i << 12), item.Item1);
                if (tenBest.Count > 10)
                {
                    tenBest.RemoveAt(10);
                }
            }
            random.counter+=totalCallsAfter;
            foreach (var item in tenBest)
            {
                Item actualItem = Item.Items[item.Value.ToString()];
                int cost = (int)Math.Max(random.ChooseFrom(randomAmounts), random.ChooseFrom(randomMultiply) * actualItem.Price);
                int qty = random.NextBool(0.1f) ? 5 : 1;
                stock.Add(new CartItem(actualItem.id, cost, qty));
            }
            return stock;
        }

        private static List<CartItem> GetStockOldRNG(int seed, bool version163)
        {
            Random random = new Random(seed);
            List<CartItem> stock = new List<CartItem>();
            SortedList<long, int> tenBest = new();
            int counter = 0;
            foreach (var item in version163 ? itemAndAdvances163 : itemAndAdvances)
            {
                for (int x = 0; x < item.Item2; x++){
                    random.Next();
                }
                counter++;
                long i = random.Next();
                tenBest.Add(counter + (i << 12), item.Item1);
                if (tenBest.Count > 10)
                {
                    tenBest.RemoveAt(10);
                }
            }
            for (int x = 0; x < totalCallsAfter; x++){
                random.Next();
            }
            foreach (var item in tenBest)
            {
                Item actualItem = Item.Items[item.Value.ToString()];
                int cost = (int)Math.Max(random.ChooseFrom(randomAmounts), random.ChooseFrom(randomMultiply) * actualItem.Price);
                int qty = random.NextBool(0.1f) ? 5 : 1;
                stock.Add(new CartItem(actualItem.id, cost, qty));
            }
            return stock;
        }

        // Left alive here just to allow comparing / validating accuracy.
        public static List<CartItem> GetStockPreviousImplementation(long gameSeed, int day, string version = "1.6.3")
        {
            bool version163 = new Version(version) >= new Version("1.6.3");
            List<CartItem> stock = new List<CartItem>();
            SortedDictionary<int, Item> randomOrder = new();
            Random random = Utility.CreateSlowRandom(day, gameSeed / 2);
            foreach (var item in Item.Items)
                {
                int i = random.Next();

                if (!version163 && !possibleItemsMaster.Contains(item.Key) || version163 && !possibleItemsMaster163.Contains(item.Key))
                    continue;
                
                while (randomOrder.ContainsKey(i))
                {
                    i++;
                }
                randomOrder.Add(i, item.Value);
            }

            int count = 0;
            foreach (var item in randomOrder)
            {
                int cost = (int)Math.Max(random.ChooseFrom(randomAmounts), random.ChooseFrom(randomMultiply) * item.Value.Price);
                int qty = random.NextBool(0.1f) ? 5 : 1;
                stock.Add(new CartItem(item.Value.id, cost, qty));
                count++;
                if (count >= 10)
                {
                    break;
                }
            }
            return stock;
        }

        // Says if a skill book will show up in the travelling cart that day (does not say which one)
        public static bool hasBook(long gameSeed, int day)
        {
            ///SYNCED_RANDOM day travelerSkillBook .05
            return Utility.CreateRandom(-451051273, gameSeed, day).NextSingle() < 0.05;
        }

        // Says what a skill book would be in the traveling cart that day, regardless of if one would show up. Slow implementation.
        public static int whatBook(long gameSeed, int day)
        {
            bool bonusCall = false;
            if ((day - 1) % 112 < 56)
            {
                bool hasRare = false;
                foreach (CartItem x in GetStock(gameSeed, day))
                {
                    if (x.Id == "(O)347")
                    {
                        hasRare = true;
                    }
                }
                bonusCall = !hasRare;
            }
            Random shopRandom = Utility.CreateDaySaveRandom(day, gameSeed);
            for (int i = 0; i < (bonusCall ? 1484 : 1483); i++)
            {
                shopRandom.Next();
            }
            return shopRandom.Next(5);
        }

        
    }
}
