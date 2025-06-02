using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SeedFinding.Utilities
{
	public class MeleeWeapon : Item
	{
		


		[JsonProperty("MaxDamage")]
		public int maxDamage;

		[JsonProperty("MinDamage")]
		public int minDamage;

		[JsonProperty("Speed")]
		public int speed;

		[JsonProperty("Type")]
		public int type;

		[JsonProperty("AddedPrecision")]
		public int addedPrecision;

		[JsonProperty("Defense")]
		public int addedDefense;

		[JsonProperty("CritChance")]
		public double critChance;

		[JsonProperty("CritMultiplier")]
		public double critMultiplier;

		public List<string> innateEnchants = new List<string>();


		public static Dictionary<string, MeleeWeapon> Weapons = SetupWeapons();
		public static Dictionary<string, MeleeWeapon> SetupWeapons()
		{
			Dictionary<string, MeleeWeapon> result = JsonConvert.DeserializeObject<Dictionary<string, MeleeWeapon>>(File.ReadAllText(@"data/Weapons.json"));
			foreach (var item in result)
			{
				item.Value.id = item.Key;
			}
			return result;
		}

		public override string ToString()
		{
			string result = Name;
			foreach (var enchant in innateEnchants)
			{
				result += " " + enchant.ToString();
			}

			return result;
		}

		public static MeleeWeapon GetWeapon(string id)
		{
			// Assume Object
			if (Weapons.ContainsKey(id))
			{
				MeleeWeapon weapon = Weapons[id];
				weapon = (MeleeWeapon)weapon.MemberwiseClone();
				weapon.innateEnchants = new List<string>();
				return weapon;
			}

			if (id.StartsWith("(W)"))
			{
				return GetWeapon(id[3..]);
			}

			return new MeleeWeapon() { Name = id };
		}

		public virtual int getItemLevel()
		{
			float weaponPoints = 0f;
			weaponPoints += (float)(int)((double)((this.maxDamage + this.minDamage) / 2) * (1.0 + 0.03 * (double)(Math.Max(0, this.speed) + ((this.type == 1) ? 15 : 0))));
			weaponPoints += (float)(int)((double)(this.addedPrecision / 2 + this.addedDefense) + ((double)this.critChance - 0.02) * 200.0 + (double)((this.critMultiplier - 3f) * 6f));
			string qualifiedItemId = "(W)" + id;
			if (!(qualifiedItemId == "(W)2"))
			{
				if (qualifiedItemId == "(W)3")
				{
					weaponPoints += 15f;
				}
			}
			else
			{
				weaponPoints += 20f;
			}
			weaponPoints += (float)(this.addedDefense * 2);
			return (int)(weaponPoints / 7f + 1f);
		}

		public void attemptAddRandomInnateEnchantment(Random r)
		{
			if (r == null)
			{
				//r = Game1.random;
			}
			if (r.NextBool())
			{
				while (true)
				{
					int weaponLevel = getItemLevel();
					if (r.NextDouble() < 0.125 && weaponLevel <= 10)
					{
						innateEnchants.Add($"DefenseEnchantment ({Math.Max(1, Math.Min(2, r.Next(weaponLevel + 1) / 2 + 1))})");
						//weapon.AddEnchantment(new DefenseEnchantment
						//{
						//	Level = Math.Max(1, Math.Min(2, r.Next(weaponLevel + 1) / 2 + 1))
						//});
					}
					else if (r.NextDouble() < 0.125)
					{
						innateEnchants.Add($"LightweightEnchantment ({r.Next(1, 6)})");
						//weapon.AddEnchantment(new LightweightEnchantment
						//{
						//	Level = r.Next(1, 6)
						//});
					}
					else if (r.NextDouble() < 0.125)
					{
						innateEnchants.Add($"SlimeGathererEnchantment");
						//weapon.AddEnchantment(new SlimeGathererEnchantment());
					}
					switch (r.Next(5))
					{
						case 0:
							innateEnchants.Add($"AttackEnchantment ({Math.Max(1, Math.Min(5, r.Next(weaponLevel + 1) / 2 + 1))})");
							//weapon.AddEnchantment(new AttackEnchantment
							//{
							//	Level = Math.Max(1, Math.Min(5, r.Next(weaponLevel + 1) / 2 + 1))
							//});
							break;
						case 1:
							innateEnchants.Add($"CritEnchantment ({Math.Max(1, Math.Min(3, r.Next(weaponLevel) / 3))})");
							//weapon.AddEnchantment(new CritEnchantment
							//{
							//	Level = Math.Max(1, Math.Min(3, r.Next(weaponLevel) / 3))
							//});
							break;
						case 2:
							innateEnchants.Add($"WeaponSpeedEnchantment ({Math.Max(1, Math.Min(Math.Max(1, 4 - speed), r.Next(weaponLevel)))})");
							//weapon.AddEnchantment(new WeaponSpeedEnchantment
							//{
							//	Level = Math.Max(1, Math.Min(Math.Max(1, 4 - weapon.speed.Value), r.Next(weaponLevel)))
							//});
							break;
						case 3:
							innateEnchants.Add($"SlimeSlayerEnchantment");
							//weapon.AddEnchantment(new SlimeSlayerEnchantment());
							break;
						case 4:
							innateEnchants.Add($"CritPowerEnchantment ({Math.Max(1, Math.Min(3, r.Next(weaponLevel) / 3))})");
							//weapon.AddEnchantment(new CritPowerEnchantment
							//{
							//	Level = Math.Max(1, Math.Min(3, r.Next(weaponLevel) / 3))
							//});
							break;
					}
					//if (enchantsToReroll == null)
					//{
						break;
					//}
					/*bool foundMatch = false;
					foreach (BaseEnchantment e in enchantsToReroll)
					{
						foreach (BaseEnchantment w_e in weapon.enchantments)
						{
							if (e.GetType().Equals(w_e.GetType()))
							{
								foundMatch = true;
								break;
							}
						}
						if (foundMatch)
						{
							break;
						}
					}
					if (!foundMatch)
					{
						break;
					}
					weapon.enchantments.RemoveWhere((BaseEnchantment enchantment) => enchantment.IsSecondaryEnchantment() && !(enchantment is GalaxySoulEnchantment));*/
				}
			}
			//return item;
		}
	}
}
