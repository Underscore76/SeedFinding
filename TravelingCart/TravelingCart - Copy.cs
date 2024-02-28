using SeedFinding.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
        public static List<int> possibleItemsMaster;
        public static HashSet<int> zeroSellPriceItems;
        public static List<float> randomAmounts;
        public static List<float> randomMultiply;

        static TravelingCart()
        {
            zeroSellPriceItems = new()
            {
                168,
                169,
                170,
                171,
                172,
                745,
                746,
                747,
                748,
                770
            };
            possibleItemsMaster = new()
            {
                16,
                18,
                20,
                22,
                24,
                78,
                88,
                90,
                92,
                128,
                129,
                130,
                131,
                132,
                136,
                137,
                138,
                139,
                140,
                141,
                142,
                143,
                144,
                145,
                146,
                147,
                148,
                149,
                150,
                151,
                154,
                155,
                156,
                164,
                165,
                167,
                168,
                169,
                170,
                171,
                172,
                176,
                174,
                180,
                182,
                184,
                186,
                188,
                190,
                192,
                194,
                195,
                196,
                197,
                198,
                199,
                200,
                201,
                202,
                203,
                204,
                205,
                206,
                207,
                208,
                209,
                210,
                211,
                212,
                213,
                214,
                215,
                216,
                218,
                219,
                220,
                221,
                222,
                223,
                224,
                225,
                226,
                227,
                228,
                229,
                230,
                231,
                232,
                233,
                234,
                235,
                236,
                237,
                238,
                239,
                240,
                241,
                242,
                243,
                244,
                248,
                250,
                251,
                252,
                253,
                254,
                256,
                257,
                258,
                259,
                260,
                262,
                264,
                266,
                268,
                270,
                271,
                272,
                273,
                274,
                276,
                278,
                280,
                281,
                282,
                283,
                284,
                286,
                287,
                288,
                293,
                296,
                298,
                299,
                300,
                301,
                302,
                303,
                304,
                306,
                307,
                309,
                310,
                311,
                322,
                323,
                324,
                325,
                328,
                329,
                330,
                331,
                333,
                334,
                335,
                336,
                337,
                338,
                340,
                342,
                344,
                346,
                347,
                348,
                350,
                368,
                369,
                370,
                371,
                372,
                378,
                380,
                382,
                384,
                386,
                388,
                390,
                392,
                393,
                394,
                396,
                397,
                398,
                399,
                400,
                401,
                402,
                404,
                405,
                406,
                407,
                408,
                409,
                410,
                411,
                412,
                414,
                415,
                416,
                418,
                420,
                421,
                422,
                424,
                426,
                428,
                430,
                432,
                433,
                436,
                438,
                440,
                442,
                444,
                446,
                456,
                457,
                459,
                465,
                466,
                472,
                473,
                474,
                475,
                476,
                477,
                478,
                479,
                480,
                481,
                482,
                483,
                484,
                485,
                486,
                487,
                488,
                489,
                490,
                491,
                492,
                493,
                494,
                495,
                496,
                497,
                498,
                499,
                427,
                429,
                453,
                455,
                431,
                425,
                591,
                593,
                595,
                597,
                599,
                376,
                604,
                605,
                606,
                607,
                608,
                609,
                610,
                611,
                612,
                613,
                614,
                618,
                621,
                648,
                649,
                651,
                628,
                629,
                630,
                631,
                632,
                633,
                634,
                635,
                636,
                637,
                638,
                684,
                685,
                686,
                687,
                691,
                692,
                693,
                694,
                695,
                698,
                699,
                700,
                701,
                702,
                703,
                704,
                705,
                706,
                707,
                708,
                709,
                715,
                716,
                717,
                718,
                719,
                720,
                721,
                722,
                723,
                724,
                725,
                726,
                727,
                730,
                728,
                729,
                731,
                732,
                733,
                734,
                745,
                746,
                747,
                748,
                766,
                767,
                768,
                769,
                770,
                771,
                772,
                773,
                787,
                445,
                267,
                265,
                269
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
        }

        public static List<CartItem> GetStockNew(int gameSeed, int day)
        {
            List<CartItem> stock = new List<CartItem>();
            SortedDictionary<int, Item> randomOrder = new();
            Random random = Utility.CreateRandom(day, gameSeed / 2);
            foreach (var item in Item.Items)
                {
                int i = random.Next();
                if (!Int32.TryParse(item.Key, out var index)){
                    continue;
                }
                //Check sale price
                if (item.Value.Price == 0)
                {
                    continue;
                }
                if (isObjectOffLimitsForSale(item.Key))
                {
                    continue;
                }
                if (index >= 2 && index <= 789)
                {
                    randomOrder.Add(i, item.Value);
                }
            }

            List<String> list = new List<String> { "Quest", "Minerals", "Arch" };
            foreach (var item in randomOrder)
            {
                if (!(item.Value.Category < 0))
                {
                    continue;
                }

                if (item.Value.Category == -999)
                {
                    continue;
                }
                if (list.Contains(item.Value.Type)){
                    continue;
                }
                int cost = (int)Math.Max(random.ChooseFrom(randomAmounts), random.ChooseFrom(randomMultiply) * item.Value.Price);
                int qty = random.NextBool(0.1f) ? 5 : 1;
                stock.Add(new CartItem(item.Value.id, cost, qty));
            }
            return stock;
        }

        public static bool isObjectOffLimitsForSale(string id)
        {
            switch (id)
            {
                case "79":
                case "163":
                case "162":
                case "161":
                case "160":
                case "159":
                case "158":
                case "305":
                case "308":
                case "326":
                case "341":
                case "413":
                case "437":
                case "439":
                case "454":
                case "460":
                case "682":
                case "681":
                case "680":
                case "645":
                case "688":
                case "690":
                case "689":
                case "774":
                case "775":
                case "797":
                case "798":
                case "799":
                case "800":
                case "801":
                case "802":
                case "803":
                case "417":
                case "807":
                case "261":
                case "279":
                case "277":
                case "447":
                case "812":
                case "292":
                case "69":
                case "91":
                case "73":
                case "289":
                case "935":
                    return true;
                default:
                    return false;
            }
        }

        public static List<CartItem> GetStock(int gameSeed, int day)
        {
            List<CartItem> stock = new List<CartItem>();
            HashSet<string> addedIds = new();
            Random random = Utility.CreateRandom(day, gameSeed / 2);
            int count = 10;
            List<Item> items = new();

            List<int> possibleItems = new( possibleItemsMaster );
            while (count > 0 && possibleItems.Count > 0)
            {
                int index = random.Next(possibleItems.Count);
                int metadata2 = possibleItems[index];
                Item instance = Item.Get(metadata2.ToString());
                instance.id = metadata2.ToString();
                if (instance.Price == 0)
                {
                    possibleItems.RemoveAt(index);
                    continue;
                }
                items.Add(instance);
                count--;
                possibleItems.RemoveAt(index);
            }

            foreach (Item item in items)
            {
                int cost = (int)Math.Max(random.ChooseFrom(randomAmounts),random.ChooseFrom(randomMultiply) * item.Price);
                int qty = random.NextBool(0.1f) ? 5 : 1;
                if (addedIds.Add(item.id))
                {
                    stock.Add(new CartItem(item.id, cost, qty));
                }

            }

            // TODO: Red cabbage week

            // TODO: Furniture
            random.NextBool();
            random.NextBool();

            // Rare seed
            int rsqty = random.NextBool(0.1f) ? 5 : 1;
            if (addedIds.Add("347"))
            {
                stock.Add(new CartItem("347", 1000, rsqty));
            }

            return stock;
        }
    }
}
