using Newtonsoft.Json;
using SeedFinding.StardewClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedFinding.Locations1_6
{
	public class LocationData
	{
		[JsonProperty("DisplayName")]
		public string DisplayName;

		[JsonProperty("ArtifactSpots")]
		public List<ArtifactSpotDropData> ArtifactSpots;

		[JsonProperty("Forage")]
		public List<ForageData> Forage;

		[JsonProperty("MinDailyForageSpawn")]
		public int MinDailyForageSpawn;

		[JsonProperty("MaxDailyForageSpawn")]
		public int MaxDailyForageSpawn;

		[JsonProperty("MaxSpawnedForageAtOnce")]
		public int MaxSpawnedForageAtOnce;


	}

	public class ArtifactSpotDropData : GenericSpawnItemData
	{
		[JsonProperty("Chance")]
		public double Chance;

		[JsonProperty("Precedence")]
		public int Precedence;

		[JsonProperty("Condition")]
		public string Condition;

		[JsonProperty("Id")]
		public string Id;

		[JsonProperty("OneDebrisPerDrop")]
		public bool OneDebrisPerDrop;

		[JsonProperty("ApplyGenerousEnchantment")]
		public bool ApplyGenerousEnchantment;

		[JsonProperty("ContinueOnDrop")]
		public bool ContinueOnDrop;
	}
	public class ForageData
	{
		[JsonProperty("Chance")]
		public double Chance;

		[JsonProperty("Season")]
		public string Season;

		[JsonProperty("Condition")]
		public string Condition;

		[JsonProperty("Id")]
		public string Id;
	}
}
