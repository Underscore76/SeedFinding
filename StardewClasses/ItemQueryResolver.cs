using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeedFinding.Utilities;

namespace SeedFinding.StardewClasses
{
	public class ItemQueryResolver
	{
		public static class DefaultResolvers
		{

			public static Item[] ErrorResult(string key, string arguments, Action<string, string> logError, string message)
			{
				logError?.Invoke((key + " " + arguments).Trim(), message);
				return LegacyShims.EmptyArray<Item>();
			}

			public static IList<Item> TryResolve_LostBookOrItem(string key, string arguments, ItemQueryContext context, bool avoidRepeat, HashSet<string> avoidItemIds, Action<string, string> logError)
			{

				return new Item[1]
				{
				Item.Get("(O)102")
				};
			}

			/// <summary>Resolve the <c>RANDOM_ARTIFACT_FOR_DIG_SPOT</c> item query.</summary>
			/// <inheritdoc cref="T:StardewValley.Delegates.ResolveItemQueryDelegate" />
			public static IList<Item> TryResolve_RandomArtifact(string key, string arguments, ItemQueryContext context, bool avoidRepeat, HashSet<string> avoidItemIds, Action<string, string> logError)
			{
				Random random = context.Random;
				Farmer player = context.Player;
				string locationName = context.Location.Name;
				int chanceMultiplier = (!Game1.archaeologyEnchant ? 1 : 2);
				foreach (KeyValuePair<string, Item> keyValuePair in (IEnumerable<KeyValuePair<string, Item>>)Item.Items)
				{
					if (keyValuePair.Value.Type.Contains("Arch"))
					{

						Dictionary<string, float> dropChances = keyValuePair.Value.ArtifactSpotChances;
						if (dropChances != null && dropChances.TryGetValue(locationName, out var chance) && random.NextBool((float)chanceMultiplier * chance))
						{
							return new Item[1]
							{
							Item.Get("(O)" + keyValuePair.Key)
							};
						}
					}
				}
				return LegacyShims.EmptyArray<Item>();
			}

			/// <summary>Resolve the <c>SECRET_NOTE_OR_ITEM</c> item query.</summary>
			/// <inheritdoc cref="T:StardewValley.Delegates.ResolveItemQueryDelegate" />
			public static IList<Item> TryResolve_SecretNoteOrItem(string key, string arguments, ItemQueryContext context, bool avoidRepeat, HashSet<string> avoidItemIds, Action<string, string> logError)
			{
				GameLocation location = context.Location;
				Farmer player = context.Player;
				if (location != null && Game1.magnifyingGlass)
				{

					return new Item[1]
					{
					Item.Get("(O)79")
					};
				}
				if (string.IsNullOrWhiteSpace(arguments))
				{
					return LegacyShims.EmptyArray<Item>();
				}
				return new Item[1]
					{
					Item.Get(arguments)
					};
			}
		}
		public static Item TryResolveRandomItem(ISpawnItemData data, ItemQueryContext context, bool avoidRepeat = false, HashSet<string> avoidItemIds = null, Func<string, string> formatItemId = null, Item inputItem = null, Action<string, string> logError = null)
		{
			return ItemQueryResolver.TryResolve(data, context, ItemQuerySearchMode.RandomOfTypeItem, avoidRepeat, avoidItemIds, formatItemId, logError, inputItem).FirstOrDefault();
		}

		public static IList<Item> TryResolve(string query, ItemQueryContext context, ItemQuerySearchMode filter = ItemQuerySearchMode.All, bool avoidRepeat = false, HashSet<string> avoidItemIds = null, Action<string, string> logError = null)
		{
			if (string.IsNullOrWhiteSpace(query))
			{
				return DefaultResolvers.ErrorResult(query, "", logError, "must specify an item ID or query");
			}
			string queryKey = query;
			string arguments = null;
			int splitIndex = query.IndexOf(' ');
			if (splitIndex > -1)
			{
				queryKey = query.Substring(0, splitIndex);
				arguments = query.Substring(splitIndex + 1);
			}
			if (context == null)
			{
				context = new ItemQueryContext();
			}
			context.QueryString = query;
			if (context.ParentContext != null)
			{
				List<string> path = new List<string>();
				for (ItemQueryContext cur = context; cur != null; cur = cur.ParentContext)
				{
					bool num = path.Contains(cur.QueryString);
					path.Add(cur.QueryString);
					if (num)
					{
						logError?.Invoke(query, "detected circular reference in item queries: " + string.Join(" -> ", path));
						return LegacyShims.EmptyArray<Item>();
					}
				}
			}
			if (ItemResolvers.TryGetValue(queryKey, out var resolver))
			{
				return ItemQueryResolver.FilterResults(resolver(queryKey, arguments ?? string.Empty, context, avoidRepeat, avoidItemIds, logError ?? new Action<string, string>(LogNothing)), filter, avoidRepeat, avoidItemIds);
			}
			Item instance = Item.Get(query);
			if (instance != null && (avoidItemIds == null || !avoidItemIds.Contains(instance.id)))
			{
				return new Item[1]
				{
				instance
				};
			}
			return LegacyShims.EmptyArray<Item>();
		}

		private static void LogNothing(string query, string error)
		{
		}

		public static IList<Item> TryResolve(ISpawnItemData data, ItemQueryContext context, ItemQuerySearchMode filter = ItemQuerySearchMode.All, bool avoidRepeat = false, HashSet<string> avoidItemIds = null, Func<string, string> formatItemId = null, Action<string, string> logError = null, Item inputItem = null)
		{
			//Random random = context?.Random ?? Game1.random;
			Random random = context?.Random;
			string itemId = data.ItemId;
			List<string> randomItemId = data.RandomItemId;
			if (randomItemId != null && randomItemId.Any())
			{
				if (avoidItemIds != null)
				{
					if (!Utility.TryGetRandomExcept(data.RandomItemId, avoidItemIds, random, out itemId))
					{
						return LegacyShims.EmptyArray<Item>();
					}
				}
				else
				{
					itemId = random.ChooseFrom(data.RandomItemId);
				}
			}
			if (formatItemId != null)
			{
				itemId = formatItemId(itemId);
			}
			IList<Item> results = ItemQueryResolver.TryResolve(itemId, context, filter, avoidRepeat, avoidItemIds, logError);
			foreach (Item item in results)
			{
				ItemQueryResolver.ApplyItemFields(item, data, context, inputItem);
			}
			return results;
		}

		public static Item[] ErrorResult(string key, string arguments, Action<string, string> logError, string message)
		{
			logError?.Invoke((key + " " + arguments).Trim(), message);
			return LegacyShims.EmptyArray<Item>();
		}

		public static Dictionary<string, ResolveItemQueryDelegate> ItemResolvers { get; } = new Dictionary<string, ResolveItemQueryDelegate>(StringComparer.OrdinalIgnoreCase)
		{
			//["ALL_ITEMS"] = DefaultResolvers.TryResolve_AllItems,
			//["DISH_OF_THE_DAY"] = DefaultResolvers.TryResolve_DishOfTheDay,
			//["FLAVORED_ITEM"] = DefaultResolvers.TryResolve_FlavoredItem,
			//["ITEMS_LOST_ON_DEATH"] = DefaultResolvers.TryResolve_ItemsLostOnDeath,
			//["ITEMS_SOLD_BY_PLAYER"] = DefaultResolvers.TryResolve_ItemsSoldByPlayer,
			//["LOCATION_FISH"] = DefaultResolvers.TryResolve_LocationFish,
			["LOST_BOOK_OR_ITEM"] = DefaultResolvers.TryResolve_LostBookOrItem,
			//["MONSTER_SLAYER_REWARDS"] = DefaultResolvers.TryResolve_MonsterSlayerRewards,
			["RANDOM_ARTIFACT_FOR_DIG_SPOT"] = DefaultResolvers.TryResolve_RandomArtifact,
			//["RANDOM_BASE_SEASON_ITEM"] = DefaultResolvers.TryResolve_RandomBaseSeasonItem,
			//["RANDOM_ITEMS"] = DefaultResolvers.TryResolve_RandomItems,
			["SECRET_NOTE_OR_ITEM"] = DefaultResolvers.TryResolve_SecretNoteOrItem
			//["SHOP_TOWN_KEY"] = DefaultResolvers.TryResolve_ShopTownKey,
			//["TOOL_UPGRADES"] = DefaultResolvers.TryResolve_ToolUpgrades
		};
		private static IList<Item> FilterResults(IList<Item> results, ItemQuerySearchMode filter, bool avoidRepeat, HashSet<string> avoidItemIds)
		{
			if (filter == ItemQuerySearchMode.All && !avoidRepeat && (avoidItemIds == null || !avoidItemIds.Any()))
			{
				return results;
			}
			bool onlyItems = filter != ItemQuerySearchMode.All;
			bool returnFirstMatch = filter == ItemQuerySearchMode.FirstOfTypeItem;
			HashSet<string> duplicates = (avoidRepeat ? new HashSet<string>() : null);
			for (int i = 0; i < results.Count; i++)
			{
				Item item = results[i];
				if ((onlyItems && !(item is Item)) || !((!(avoidItemIds?.Contains(item.id))) ?? true) || !(duplicates?.Add(item.id) ?? true))
				{
					results.RemoveAt(i);
					i--;
				}
				else if (returnFirstMatch)
				{
					break;
				}
			}
			if (results.Count > 1)
			{
				switch (filter)
				{
					case ItemQuerySearchMode.FirstOfTypeItem:
						return new Item[1] { results[0] };
					case ItemQuerySearchMode.RandomOfTypeItem:
						//return new Item[1] { Game1.random.ChooseFrom(results) };
						return null;
				}
			}
			return results;
		}

		public static void ApplyItemFields(Item item, ISpawnItemData data, ItemQueryContext context, Item inputItem = null)
		{
			ItemQueryResolver.ApplyItemFields(item, data.MinStack, data.MaxStack, data.ToolUpgradeLevel, data.ObjectInternalName, data.ObjectDisplayName, data.Quality, data.IsRecipe, data.StackModifiers, data.StackModifierMode, data.QualityModifiers, data.QualityModifierMode, context, inputItem);
		}

		/// <summary>Apply data fields to an item instance.</summary>
		/// <param name="item">The item to modify.</param>
		/// <param name="minStackSize">The minimum stack size for the item to create, or <c>-1</c> to keep it as-is.</param>
		/// <param name="maxStackSize">The maximum stack size for the item to create, or <c>-1</c> to match <paramref name="minStackSize" />.</param>
		/// <param name="toolUpgradeLevel">For tools only, the tool upgrade level to set, or <c>-1</c> to keep it as-is.</param>
		/// <param name="objectInternalName">For objects only, the internal name to use (or <c>null</c> for the item's name in data). This should usually be null.</param>
		/// <param name="objectDisplayName">For objects only, a tokenizable string for the display name to use (or <c>null</c> for the item's default display name). See remarks on <see cref="F:StardewValley.Object.displayNameFormat" />.</param>
		/// <param name="quality">The object quality to set, or <c>-1</c> to keep it as-is.</param>
		/// <param name="isRecipe">Whether to mark the item as a recipe that can be learned by the player, instead of an instance that can be picked up.</param>
		/// <param name="stackSizeModifiers">The modifiers to apply to the item's stack size.</param>
		/// <param name="stackSizeModifierMode">How multiple <paramref name="stackSizeModifiers" /> should be combined.</param>
		/// <param name="qualityModifiers">The modifiers to apply to the item's quality.</param>
		/// <param name="qualityModifierMode">How multiple <paramref name="qualityModifiers" /> should be combined.</param>
		/// <param name="context">The contextual info for item queries, or <c>null</c> to use the global context.</param>
		/// <param name="inputItem">The input item (e.g. machine input) for which to check queries, or <c>null</c> if not applicable.</param>
		/// <remarks>This is applied automatically by methods which take an <see cref="T:StardewValley.GameData.ISpawnItemData" />, so it only needs to be called directly when creating an item from an item query string directly.</remarks>
		public static void ApplyItemFields(Item item, int minStackSize, int maxStackSize, int toolUpgradeLevel, string objectInternalName, string objectDisplayName, int quality, bool isRecipe, List<QuantityModifier> stackSizeModifiers, QuantityModifier.QuantityModifierMode stackSizeModifierMode, List<QuantityModifier> qualityModifiers, QuantityModifier.QuantityModifierMode qualityModifierMode, ItemQueryContext context, Item inputItem = null)
		{
			if (item == null)
			{
				return;
			}
			int stackSize = 1;
			if (!isRecipe)
			{
				if (minStackSize == -1 && maxStackSize == -1)
				{
					stackSize = item.Stack;
				}
				else if (maxStackSize > 1)
				{
					minStackSize = Math.Max(minStackSize, 1);
					maxStackSize = Math.Max(maxStackSize, minStackSize);
					stackSize = (context?.Random).Next(minStackSize, maxStackSize + 1);
				}
				else if (minStackSize > 1)
				{
					stackSize = minStackSize;
				}
				stackSize = (int)Utility.ApplyQuantityModifiers(stackSize, stackSizeModifiers, stackSizeModifierMode, context?.Location, context?.Player, item as Item, inputItem, context?.Random);
			}
			//quality = ((quality >= 0) ? quality : item.Quality);
			//quality = (int)Utility.ApplyQuantityModifiers(quality, qualityModifiers, qualityModifierMode, context?.Location, context?.Player, item as Item, inputItem, context?.Random);
			if (isRecipe)
			{
				//item.IsRecipe = true;
			}
			if (stackSize > -1 && stackSize != item.Stack)
			{
				item.Stack = stackSize;
				//item.FixStackSize();
			}
			//if (quality >= 0 && quality != item.Quality)
			//{
			//	item.Quality = quality;
			//	item.FixQuality();
			//}
			if (!(item is object obj))
			{
				//if (item is Tool tool && toolUpgradeLevel > -1 && toolUpgradeLevel != tool.UpgradeLevel)
				//{
				//    tool.UpgradeLevel = toolUpgradeLevel;
				//}
				return;
			}
			if (objectDisplayName != null)
			{
				//obj.Name = objectInternalName;
			}
			if (objectDisplayName != null)
			{
				// obj.displayNameFormat = objectDisplayName;
			}
		}
	}
}
