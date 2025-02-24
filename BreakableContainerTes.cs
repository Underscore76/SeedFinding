using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace SeedFinding
{
	internal class BreakableContainerTes
	{

		public static int getItemLevel()
		{
			float weaponPoints = 0f;
			weaponPoints += (float)(int)((double)((16 + 9) / 2) * (1.0 + 0.03 * (double)(Math.Max(0, -8) + ((2 == 1) ? 15 : 0))));
			weaponPoints += (float)(int)((double)(0 / 2 + 0) + ((double)0.02 - 0.02) * 200.0 + (double)((3 - 3f) * 6f));
			string qualifiedItemId = "(W)24";
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
			weaponPoints += (float)(0 * 2);
			return (int)(weaponPoints / 7f + 1f);
		}
		public static int getItemLevelRod()
		{
			float weaponPoints = 0f;
			weaponPoints += (float)(int)((double)((27 + 18) / 2) * (1.0 + 0.03 * (double)(Math.Max(0, -16) + ((2 == 1) ? 15 : 0))));
			weaponPoints += (float)(int)((double)(0 / 2 + 0) + ((double)0.02 - 0.02) * 200.0 + (double)((3 - 3f) * 6f));
			string qualifiedItemId = "(W)26";
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
			weaponPoints += (float)(0 * 2);
			return (int)(weaponPoints / 7f + 1f);
		}

public static bool attemptAddRandomInnateEnchantment(Random r, int weaponLevel)//, bool force = false, List<BaseEnchantment> enchantsToReroll = null)
		{
			if (r == null)
			{
				//r = Game1.random;
			}
			if (r.NextBool())
			{
				while (true)
				{
					int level = 0;
					if (r.NextDouble() < 0.125 && weaponLevel <= 10)
					{
						return false;
						//level = Math.Max(1, Math.Min(2, r.Next(weaponLevel + 1) / 2 + 1));
						//weapon.enchants.Add($"Defense ({level})");
					}
					else if (r.NextDouble() < 0.125)
					{
						//level = r.Next(1, 6);
						//weapon.enchants.Add($"Lightweight ({level})");
					}
					else if (r.NextDouble() < 0.125)
					{
						//weapon.enchants.Add("SlimeGatherer");
					}
					switch (r.Next(5))
					{
						case 0:
							level = Math.Max(1, Math.Min(5, r.Next(weaponLevel + 1) / 2 + 1));
							//Console.WriteLine(weaponLevel);
							//Console.WriteLine(level);
							if ((weaponLevel == 2 || weaponLevel == 3) && level >= 2 || weaponLevel == 4 && level >= 3)
							{
								return true;
							}
							//weapon.enchants.Add($"Attack ({level})");
							break;
						case 1:
							//level = Math.Max(1, Math.Min(3, r.Next(weaponLevel) / 3));
							//weapon.enchants.Add($"Crit ({level})");
							break;
						case 2:
							//level = Math.Max(1, Math.Min(Math.Max(1, 4 - Game1.weapons[weapon.itemID.Substring(3)].Speed), r.Next(weaponLevel)));
							//weapon.enchants.Add($"Speed ({level})");
							break;
						case 3:
							//weapon.enchants.Add("SlimeSlayer");
							//break;
						case 4:
							//level = Math.Max(1, Math.Min(3, r.Next(weaponLevel) / 3));
							//weapon.enchants.Add($"CritPower ({level})");
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
			return false;
		}

		public static bool getSpecialItemForThisMineLevel16(int day, int level, int x, int y, int number)
		{
			Random r;

			r = Utility.CreateRandom(level, day, x, (double)y * number);
			r.NextDouble();
			r.NextDouble();
			r.NextDouble();

			if (level < 20)
			{
				switch (r.Next(6))
				{
					case 0:
						return false;
					case 1:
						return attemptAddRandomInnateEnchantment( r, 2);
					case 2:
						return false;
					case 3:
						return false;
					case 4:
						return false;
					case 5:
						return false;
				}
			}
/*
			else if (level < 40)
			{
				switch (r.Next(7))
				{
					case 0:
						return MeleeWeapon.attemptAddRandomInnateEnchantment(new MeleeWeapon("(W)22"), r);
					case 1:
						return MeleeWeapon.attemptAddRandomInnateEnchantment(new MeleeWeapon("(W)24"), r);
					case 2:
						return new Boots("(O)504");
					case 3:
						return new Boots("(O)505");
					case 4:
						return new Ring("(O)516");
					case 5:
						return new Ring("(O)518");
					case 6:
						return new MeleeWeapon("(W)15");
				}
			}*/
			else if (level < 60)
			{
				switch (r.Next(7))
				{
					case 0:
						return false;
					case 1:
						return attemptAddRandomInnateEnchantment(r, 4);
					case 2:
						return false;
					case 3:
						return false;
					case 4:
						return false;
					case 5:
						return true;
					case 6:
						return false;
				}
			}
			return false;/*
			else if (level < 80)
			{
				switch (r.Next(7))
				{
					case 0:
						return MeleeWeapon.attemptAddRandomInnateEnchantment(new MeleeWeapon("(W)26"), r);
					case 1:
						return MeleeWeapon.attemptAddRandomInnateEnchantment(new MeleeWeapon("(W)27"), r);
					case 2:
						return new Boots("(O)508");
					case 3:
						return new Boots("(O)510");
					case 4:
						return new Ring("(O)517");
					case 5:
						return new Ring("(O)519");
					case 6:
						return MeleeWeapon.attemptAddRandomInnateEnchantment(new MeleeWeapon("(W)19"), r);
				}
			}
			else if (level < 100)
			{
				switch (r.Next(8))
				{
					case 0:
						return MeleeWeapon.attemptAddRandomInnateEnchantment(new MeleeWeapon("(W)48"), r);
					case 1:
						return MeleeWeapon.attemptAddRandomInnateEnchantment(new MeleeWeapon("(W)48"), r);
					case 2:
						return new Boots("(O)511");
					case 3:
						return new Boots("(O)513");
					case 4:
						return MeleeWeapon.attemptAddRandomInnateEnchantment(new MeleeWeapon("(W)18"), r);
					case 5:
						return MeleeWeapon.attemptAddRandomInnateEnchantment(new MeleeWeapon("(W)28"), r);
					case 6:
						return MeleeWeapon.attemptAddRandomInnateEnchantment(new MeleeWeapon("(W)52"), r);
					case 7:
						return MeleeWeapon.attemptAddRandomInnateEnchantment(new MeleeWeapon("(W)3"), r);
				}
			}
			else if (level < 120)
			{
				switch (r.Next(8))
				{
					case 0:
						return MeleeWeapon.attemptAddRandomInnateEnchantment(new MeleeWeapon("(W)19"), r);
					case 1:
						return MeleeWeapon.attemptAddRandomInnateEnchantment(new MeleeWeapon("(W)50"), r);
					case 2:
						return new Boots("(O)511");
					case 3:
						return new Boots("(O)513");
					case 4:
						return MeleeWeapon.attemptAddRandomInnateEnchantment(new MeleeWeapon("(W)18"), r);
					case 5:
						return MeleeWeapon.attemptAddRandomInnateEnchantment(new MeleeWeapon("(W)46"), r);
					case 6:
						return new Ring("(O)887");
					case 7:
						return MeleeWeapon.attemptAddRandomInnateEnchantment(new MeleeWeapon("(W)3"), r);
				}
			}
			else
			{
				switch (r.Next(12))
				{
					case 0:
						return MeleeWeapon.attemptAddRandomInnateEnchantment(new MeleeWeapon("(W)45"), r);
					case 1:
						return MeleeWeapon.attemptAddRandomInnateEnchantment(new MeleeWeapon("(W)50"), r);
					case 2:
						return new Boots("(O)511");
					case 3:
						return new Boots("(O)513");
					case 4:
						return MeleeWeapon.attemptAddRandomInnateEnchantment(new MeleeWeapon("(W)18"), r);
					case 5:
						return MeleeWeapon.attemptAddRandomInnateEnchantment(new MeleeWeapon("(W)28"), r);
					case 6:
						return MeleeWeapon.attemptAddRandomInnateEnchantment(new MeleeWeapon("(W)52"), r);
					case 7:
						return new SDVObject("(O)787", 1);
					case 8:
						return new Boots("(O)878");
					case 9:
						return new SDVObject("(O)856", 1);
					case 10:
						return new Ring("(O)859");
					case 11:
						return new Ring("(O)887");
				}
			}
			return new SDVObject("(O)78", 1);*/
		}
	}
}
