using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using SeedFinding.Cart1_6;
namespace SeedFinding.CartEncoding
{
	public class BMCPP //generate filters and bitmask mappings
	{
		public static void getFilter(string filterVal,out Dictionary<string,uint> itemBitMap,out Dictionary<string,uint> multipleItemBitMap)
		{
			if (!filterItems.TryGetValue(filterVal,out Tuple<List<string>,HashSet<string>> rawFilters)){throw new Exception("filterVal not in filterItems");}
			itemBitMap = new();
			multipleItemBitMap = new();
			
			uint currentMapVal = 1;
			foreach (string itemId in rawFilters.Item1)
			{
				itemBitMap.Add(itemId,currentMapVal);
				currentMapVal <<= 1;
				
				if (!rawFilters.Item2.Contains(itemId)) continue;
				multipleItemBitMap.Add(itemId,currentMapVal);
				currentMapVal <<= 1;
			}
		}
		private static List<string> filterItemsEarlySpring = new() //possible required cart items for early spring cc, non checked "possible" bundles:home cook,animals,brewers*,fodder*
		{
			"398", //forage
			"402",
			"408",
			"416",
			"418",
			"283",
			"254", //Su crops
			"256",
			"258",
			"260",
			"270", //Fa crops
			"272",
			"276",
			"593", //Garden
			"595",
			"421",
			"698", //fish(krobusable)
			"699",
			"701",
			"130", //fish(not krobus)
			"140",
			"150",
			"128",
			"376", //chef
			"430",
			"268", //dye
			"444",
			"266",
			"284",
			"300",
		};
		private static HashSet<string> filterItemsEarlySpringMultiple = new() //holly,sunflower,blueberry
		{
			"283",
			"421",
			"258"
		};
		private static Dictionary<string,Tuple<List<string>,HashSet<string>>> filterItems = new() //dict of encoded options 
		{
			{"EarlySpring",new(filterItemsEarlySpring,filterItemsEarlySpringMultiple)}
		};	
	}
	public class BitMaskCartEncode
	{
		public Dictionary<string,uint> filterItems;
		public Dictionary<string,uint> filterItemsMultiple;
		
		public BitMaskCartEncode(Dictionary<string,uint> encodedItems, Dictionary<string,uint> duplicateItems)
		{
			filterItems = encodedItems;
			filterItemsMultiple = duplicateItems;
		}
		public uint ReducedStock(int seed) //encode into binary
		{
			List<CartItem> originalStock = TravelingCart.GetStock(seed,0);
			int usefulItems = 0;
			uint encodeValue = 0;
			uint value = new();
			foreach (CartItem item in originalStock)
			{
				if(!filterItems.TryGetValue(item.Id,out value)) continue;
				encodeValue += value;
				usefulItems += 1;
				
				if(!filterItemsMultiple.TryGetValue(item.Id,out value)) continue;
				encodeValue += value;
			}
			if (usefulItems < 5) return 0; //only keep the data if has "enough" useful items(mitigate effect by only checking for "most" conditions met when using)
			return encodeValue;
		}
	}
}