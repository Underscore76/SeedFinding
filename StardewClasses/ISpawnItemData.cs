using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedFinding.StardewClasses
{
	//
	// Summary:
	//     A data entry which specifies item data to create.
	public interface ISpawnItemData
	{
		//
		// Summary:
		//     The item(s) to create.
		//
		// Remarks:
		//     This can be...
		//     • A qualified item ID.
		//     • ALL_ITEMS [type], for either every item in the game or every item of the given
		//     type.
		//     • DISH_OF_THE_DAY, for the dish of the day sold at the Saloon (if any).
		//     • FLAVORED_WINE <ingredient item ID>, for a wine using the given item as ingredient.
		//     • ITEMS_LOST_ON_DEATH, for items lost when the player collapsed in the mines
		//     which can be recovered from Marlon's shop.
		//     • ITEMS_SOLD_BY_PLAYER <shop location id>, for items the player has recently
		//     sold to that shop.
		//     • LOCATION_FISH <location name> <bobber x> <bobber y> <water depth>, for fish
		//     that can be caught in a location based on its Data/Locations entry.
		//     • LOST_BOOK_OR_ITEM [alternate item query], for a Lost Book (if they haven't
		//     all been found), else the given item query (if provided), else nothing.
		//     • MONSTER_SLAYER_REWARDS, for rewards that can currently be collected from Gil
		//     in the Adventurer's Guild.
		//     • RANDOM_ARTIFACT_FOR_DIG_SPOT, for the first artifact in Data/Objects which
		//     lists the current location as a spawn location and whose chance matches.
		//     • RANDOM_BASE_SEASON_ITEM, for a random seasonal vanilla item which can be found
		//     by searching garbage cans, breaking containers in the mines, etc.
		//     • RANDOM_ITEMS <item data definition ID> [options], for random items based on
		//     a custom query. The optional options can be any combination of...
		//     • @[!]has_category [ids] to require one of the given categories, or any valid
		//     category if none specified;
		//     • @[!]has_id <ids> to require one of the given item IDs;
		//     • @[!]has_id_in_base_range <min id> <max id> to require a numeric item ID within
		//     the given range;
		//     • @[!]has_id_prefix <prefixes> to require that the item ID start with one of
		//     the given prefixes;
		//     • @[!]has_object_type [types] to require one of the given object types (like
		//     Arch for artifacts), or any valid type if none specified;
		//     • @[!]allow_missing_price to include items which have a base price of zero or
		//     less, which are otherwise excluded by default;
		//     • @count <count> to return the specified number of items (default 1).
		//     The [!] is an optional negation. For example, @has_category -14 requires category
		//     -14 and @!has_category -14 excludes it.
		//     • SECRET_NOTE_OR_ITEM [alternate item query], for a Secret Note (if the player
		//     unlocked them and hasn't found them all), else the given item query (if provided),
		//     else nothing.
		//     • SHOP_TOWN_KEY, for the special Town Key shop item (may be ignored outside shops).
		//     • TOOL_UPGRADES [tool ID], for tool upgrades listed in Data/Shops for the given
		//     tool ID (or all tool upgrades if [tool ID] is omitted).
		string ItemId { get; set; }

		//
		// Summary:
		//     A list of random item IDs to choose from, using the same format as StardewValley.GameData.ISpawnItemData.ItemId.
		//     If set, StardewValley.GameData.ISpawnItemData.ItemId is ignored.
		List<string> RandomItemId { get; set; }

		//
		// Summary:
		//     The minimum stack size for the item to create, or -1 to keep the default value.
		//
		// Remarks:
		//     A value in the StardewValley.GameData.ISpawnItemData.MinStack to StardewValley.GameData.ISpawnItemData.MaxStack
		//     range is chosen randomly. If the maximum is lower than the minimum, the stack
		//     is set to StardewValley.GameData.ISpawnItemData.MinStack.
		int MinStack { get; set; }

		//
		// Summary:
		//     The maximum stack size for the item to create, or -1 to match StardewValley.GameData.ISpawnItemData.MinStack.
		int MaxStack { get; set; }

		//
		// Summary:
		//     The quality of the item to create. One of 0 (normal), 1 (silver), 2 (gold), 4
		//     (iridium), or -1 (keep the quality as-is).
		int Quality { get; set; }

		//
		// Summary:
		//     For objects only, the internal name to set (or null for the item's name in data).
		//     This should usually be null.
		string ObjectInternalName { get; set; }

		//
		// Summary:
		//     For objects only, a tokenizable string for the display name to show (or null
		//     for the item's default display name). See remarks on Object.displayNameFormat.
		string ObjectDisplayName { get; set; }

		//
		// Summary:
		//     For tool items only, the initial upgrade level, or -1 to keep the default value.
		int ToolUpgradeLevel { get; set; }

		//
		// Summary:
		//     Whether to add the crafting/cooking recipe for the item, instead of the item
		//     itself.
		bool IsRecipe { get; set; }

		//
		// Summary:
		//     Changes to apply to the result of StardewValley.GameData.ISpawnItemData.MinStack
		//     and StardewValley.GameData.ISpawnItemData.MaxStack.
		List<QuantityModifier> StackModifiers { get; set; }

		//
		// Summary:
		//     How multiple StardewValley.GameData.ISpawnItemData.StackModifiers should be combined.
		QuantityModifier.QuantityModifierMode StackModifierMode { get; set; }

		//
		// Summary:
		//     Changes to apply to the StardewValley.GameData.ISpawnItemData.Quality.
		//
		// Remarks:
		//     These operate on the numeric quality values (i.e. 0 = normal, 1 = silver, 2 =
		//     gold, and 4 = iridium). For example, silver × 2 is gold.
		List<QuantityModifier> QualityModifiers { get; set; }

		//
		// Summary:
		//     How multiple StardewValley.GameData.ISpawnItemData.QualityModifiers should be
		//     combined.
		QuantityModifier.QuantityModifierMode QualityModifierMode { get; set; }
	}
}
