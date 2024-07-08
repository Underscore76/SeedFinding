using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedFinding.StardewClasses
{
	public class Farmer
	{
		//public Stats stats = new Stats();
		public Double DailyLuck;
		public int MiningLevel;
		public int LuckLevel;
		public int FarmingLevel;
		public int ForagingLevel;
		public int uniqueMultiplayerID;
		public int UniqueMultiplayerID;
		public int timesReachedMineBottom = 0;
		public List<int> professions = new List<int>();
		//public FarmerTeam team = new FarmerTeam();

		public Farmer()
		{
			timesReachedMineBottom = 0;
		}

		public void gainExperience(int which, int howMuch)
		{

		}

		public int getEffectiveSkillLevel(int which)
		{
			switch (which)
			{
				case 0:
					return FarmingLevel;
				case 1:
					return 0;
				case 2:
					return ForagingLevel;
				case 3:
					return MiningLevel;
				case 4:
					return 0;
				case 5:
					return LuckLevel;


			}


			return 0;
		}
	}
}
