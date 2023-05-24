using System;
using System.Collections;
using System.Collections.Generic;

namespace SeedFinding.Cart
{
    public class CompressedCartItem
    {
        public UInt16 State;
        public CompressedCartItem(int itemID, int costMultipler, int hundredsMultiplier, bool qty)
        {
            int itemIndex = CompressedTravelingCart.ObjectOrdering.IndexOf(itemID);
            State = (ushort)(itemIndex & 0x1FF);                    // 0b0000000XXXXXXXXX
            State += (ushort)((costMultipler & 0x3) << 9);          // 0b00000XX000000000
            State += (ushort)((hundredsMultiplier & 0xF) << 11);    // 0b0XXXX00000000000
            State += (ushort)(qty ? 0x8000 : 0);                    // 0bX000000000000000
        }
        public CompressedCartItem(UInt16 state) { State = state; }

        public int Id
        {
            get
            {
                return CompressedTravelingCart.ObjectOrdering[State & 0x1FF];
            }
        }
        public int Quantity
        {
            get
            {
                return (State & 0x8000) == 0 ? 1 : 5;
            }
        }
        public int Cost
        {
            get
            {
                return Math.Max(
                    100 * (1 + ((State >> 11) & 0xF)),
                    ObjectInfo.Get(Id).Cost * (3 + ((State >> 9) & 0x3))
                    );
            }
        }
        public override string ToString()
        {
            return string.Format(
                "{0} x{1} ({2}) 0b{3}",
                ObjectInfo.Get(Id).Name, Quantity, Cost,
                Convert.ToString(State, 2).PadLeft(16, '0')
            );
        }
    }

    public class CompressedTravelingCart
	{
        public static HashSet<int> ValidObjects;
        public static List<int> ObjectOrdering;
        static CompressedTravelingCart()
        {
            ObjectOrdering = new()
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
                174,
                176,
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
                265,
                266,
                267,
                268,
                269,
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
                376,
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
                425,
                426,
                427,
                428,
                429,
                430,
                431,
                432,
                433,
                436,
                438,
                440,
                442,
                444,
                445,
                446,
                453,
                455,
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
                591,
                593,
                595,
                597,
                599,
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
                648,
                649,
                651,
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
                728,
                729,
                730,
                731,
                732,
                733,
                734,
                766,
                767,
                768,
                769,
                771,
                772,
                773,
                787
            };
            ValidObjects = new HashSet<int>(ObjectOrdering);
        }

        public static HashSet<CompressedCartItem> GetStock(int gameSeed, int day)
        {
            return GetStock(gameSeed + day);
        }

        public static HashSet<CompressedCartItem> GetStock(int seed)
        {
            Dictionary<int, CompressedCartItem> stock = new Dictionary<int, CompressedCartItem>();
            Random random = new Random(seed);
            for (int i = 0; i < 10; i++)
            {
                int costMultiplier, hundredsMultiplier;
                bool qty;
                int num = random.Next(2, 790);
                while (true)
                {
                    num = (num + 1) % 790;
                    if (!ValidObjects.Contains(num))
                        continue;
                    hundredsMultiplier = random.Next(1, 11) - 1;
                    costMultiplier = random.Next(3, 6) - 3;
                    qty = random.NextDouble() < 0.1;
                    if (stock.ContainsKey(num))
                        continue;
                    stock[num] = new CompressedCartItem(num, costMultiplier, hundredsMultiplier, qty);
                    break;
                }
            }
            return new(stock.Values);
        }
    }
}
