using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedFinding.StardewClasses
{
	//
	// Summary:
	//     The data for an item to create, used in data assets like StardewValley.GameData.Machines.MachineData
	//     or StardewValley.GameData.Shops.ShopData.
	public class GenericSpawnItemData : ISpawnItemData
	{
		//
		// Summary:
		//     The backing field for StardewValley.GameData.GenericSpawnItemData.Id.
		private string IdImpl;

		//
		// Summary:
		//     An ID for this entry within the current list (not the item itself, which is StardewValley.GameData.GenericSpawnItemData.ItemId).
		//     This only needs to be unique within the current list. For a custom entry, you
		//     should use a globally unique ID which includes your mod ID like ExampleMod.Id_ItemName.
		//[ContentSerializer(Optional = true)]
		public string Id
		{
			get
			{
				if (IdImpl != null)
				{
					return IdImpl;
				}

				if (ItemId != null)
				{
					if (!IsRecipe)
					{
						return ItemId;
					}

					return ItemId + " (Recipe)";
				}

				List<string> randomItemId = RandomItemId;
				if (randomItemId != null && randomItemId.Count > 0)
				{
					if (!IsRecipe)
					{
						return string.Join("|", RandomItemId);
					}

					return string.Join("|", RandomItemId) + " (Recipe)";
				}

				return "???";
			}
			set
			{
				IdImpl = value;
			}
		}

		//[ContentSerializer(Optional = true)]
		public string ItemId { get; set; }

		//[ContentSerializer(Optional = true)]
		public List<string> RandomItemId { get; set; }

		//[ContentSerializer(Optional = true)]
		public int MinStack { get; set; } = -1;


		//[ContentSerializer(Optional = true)]
		public int MaxStack { get; set; } = -1;


		//[ContentSerializer(Optional = true)]
		public int Quality { get; set; } = -1;


		//[ContentSerializer(Optional = true)]
		public string ObjectInternalName { get; set; }

		//[ContentSerializer(Optional = true)]
		public string ObjectDisplayName { get; set; }

		//[ContentSerializer(Optional = true)]
		public int ToolUpgradeLevel { get; set; } = -1;


		//[ContentSerializer(Optional = true)]
		public bool IsRecipe { get; set; }

		//[ContentSerializer(Optional = true)]
		public List<QuantityModifier> StackModifiers { get; set; }

		//[ContentSerializer(Optional = true)]
		public QuantityModifier.QuantityModifierMode StackModifierMode { get; set; }

		//[ContentSerializer(Optional = true)]
		public List<QuantityModifier> QualityModifiers { get; set; }

		//[ContentSerializer(Optional = true)]
		public QuantityModifier.QuantityModifierMode QualityModifierMode { get; set; }
	}
}
