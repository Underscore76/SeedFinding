using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedFinding.StardewClasses
{
	//
	// Summary:
	//     As part of an another entry like StardewValley.GameData.Machines.MachineData
	//     or StardewValley.GameData.Shops.ShopData, a change to apply to a numeric quantity.
	public class QuantityModifier
	{
		//
		// Summary:
		//     The type of change to apply for a StardewValley.GameData.QuantityModifier.
		public enum ModificationType
		{
			//
			// Summary:
			//     Add a number to the current value.
			Add,
			//
			// Summary:
			//     Subtract a number from the current value.
			Subtract,
			//
			// Summary:
			//     Multiply the current value by a number.
			Multiply,
			//
			// Summary:
			//     Divide the current value by a number.
			Divide,
			//
			// Summary:
			//     Overwrite the current value with a number.
			Set
		}

		//
		// Summary:
		//     Indicates how multiple quantity modifiers are combined.
		public enum QuantityModifierMode
		{
			//
			// Summary:
			//     Apply each modifier to the result of the previous one. For example, two modifiers
			//     which double a value will quadruple it.
			Stack,
			//
			// Summary:
			//     Apply the modifier which results in the lowest value.
			Minimum,
			//
			// Summary:
			//     Apply the modifier which results in the highest value.
			Maximum
		}

		//
		// Summary:
		//     An ID for this modifier. This only needs to be unique within the current modifier
		//     list. For a custom entry, you should use a globally unique ID which includes
		//     your mod ID like ExampleMod.Id_ModifierName.
		public string Id;

		//
		// Summary:
		//     A game state query which indicates whether this change should be applied. Item-only
		//     tokens are valid for this check, and will check the input (not output) item.
		//     Defaults to always true.
		//[ContentSerializer(Optional = true)]
		public string Condition;

		//
		// Summary:
		//     The type of change to apply.
		public ModificationType Modification;

		//
		// Summary:
		//     The operand to apply to the target value (e.g. the multiplier if StardewValley.GameData.QuantityModifier.Modification
		//     is set to StardewValley.GameData.QuantityModifier.ModificationType.Multiply).
		//[ContentSerializer(Optional = true)]
		public float Amount;

		//
		// Summary:
		//     A list of random amounts to choose from, using the same format as StardewValley.GameData.QuantityModifier.Amount.
		//     If set, StardewValley.GameData.QuantityModifier.Amount is ignored.
		//[ContentSerializer(Optional = true)]
		public List<float> RandomAmount;

		//
		// Summary:
		//     Apply the change to a target value.
		//
		// Parameters:
		//   value:
		//     The current target value.
		//
		//   modification:
		//     The type of change to apply.
		//
		//   amount:
		//     The operand to apply to the target value (e.g. the multiplier if modification
		//     is set to StardewValley.GameData.QuantityModifier.ModificationType.Multiply).
		public static float Apply(float value, ModificationType modification, float amount)
		{
			return modification switch
			{
				ModificationType.Add => value + amount,
				ModificationType.Subtract => value - amount,
				ModificationType.Multiply => value * amount,
				ModificationType.Divide => value / amount,
				ModificationType.Set => amount,
				_ => value,
			};
		}
	}
}
