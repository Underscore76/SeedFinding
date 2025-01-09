using SeedFinding.Bundles1_6;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SeedFinding.Locations1_6;
using Newtonsoft.Json.Linq;
using System.Collections;
using static SeedFinding.Weather;
using SeedFinding.StardewClasses;
using Location = SeedFinding.Locations1_6.Location;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Xna.Framework;
using System.Numerics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace SeedFinding
{
	public class MAXPERFECTION
	{
		public static void checkFile()
		{
			DirectoryInfo directory = new DirectoryInfo("H:\\Video Projects\\Videos\\Maximum Perfection");

			for (int i = 7; i < 400; i++)
			{
				string fileName = "H:\\Video Projects\\Videos\\Maximum Perfection\\" + i.ToString() + "\\MAXIMUM3_655571\\MAXIMUM3_655571";
				if (!File.Exists(fileName))
				{
					continue;
				}
				XmlDocument doc = new XmlDocument();
				doc.Load(fileName);
				XmlNode locations = doc.SelectSingleNode("SaveGame/locations");
				XmlNode location = null;
				foreach (XmlNode gameLocation in locations.ChildNodes)
				{
					if (gameLocation.SelectSingleNode("name").InnerText == "Farm")
					{
						location = gameLocation;
						break;
					}
				}

				if (location == null)
					return;

				XmlNode objects = location.SelectSingleNode("objects");
				int count = 0;
				foreach (XmlNode item in objects.ChildNodes)
				{
					if (item.SelectSingleNode("value/Object/name")?.InnerText == "Stone")
					{
						count++;
					}
				}

				XmlNode stats = doc.SelectSingleNode("SaveGame/player/stats/Values");
				int day = 0;
				foreach (XmlNode value in stats.ChildNodes)
				{
					if (value.SelectSingleNode("key/string").InnerText == "daysPlayed")
					{
						day = int.Parse(value.SelectSingleNode("value/unsignedInt").InnerText);
					}
				}
				Console.WriteLine($"{i} {count} {objects.ChildNodes.Count}	{day}");
			}

		}

		public static void checkWinterStar()
		{
			List<int> tiles = new()
			{
				1,
				2,
				3,
				4,
				6,
				7,
				8,
				9,
				10,
				38,
				39,
				40,
				41,
				42,
				43,
				44,
				45,
				46,
				47,
				48,
				49,
				50,
				51,
				52,
				53,
				54,
			};

			foreach( var x in tiles)
			{
				Console.WriteLine($"{x}:	{winterStar(655571,2,x)}");
			}
		}

		public static string winterStar(uint gameId, int year, int x)
		{
			Random giftRandom = Utility.CreateRandom(gameId / 2uL, year, 25, 3, x);
			List<string> gifts = new List<string>();

			gifts.AddRange(new string[14]
					{
						"(O)608",
						"(O)651",
						"(O)611",
						"(O)517",
						"(O)466",
						"(O)422",
						"(O)392",
						"(O)348",
						"(O)346",
						"(O)341",
						"(O)221",
						"(O)64",
						"(O)60",
						"(O)70"
					});
            return giftRandom.ChooseFrom(gifts);


		}

		public static void PrintGeodes()
		{
			//Mines.PrintGeodeContents(655571, 1, 200, new List<Geode> { Geode.Geode, Geode.FrozenGeode, Geode.MagmaGeode, Geode.OmniGeode }, "	", false, 115, false, false);
		}
		public static BigInteger requiredBundles = (
				CompressedFlags.BOILER_BLACKSMITH |
				CompressedFlags.BOILER_ENGINEER |
				CompressedFlags.PANTRY_RARE
			);

		public static BigInteger KillerBundles = (
				CompressedFlags.CRAFTS_FOREST |
				CompressedFlags.PANTRY_FISH_FARMER |
				CompressedFlags.FISH_QUALITY |
				CompressedFlags.BULLETIN_FORAGER
			);
		public static bool ValidSeed(int gameId, bool curate = false)
		{

			var bundles = RemixedBundles.Generate(gameId);
			if (bundles.Contains(KillerBundles))
			{
				return false;
			}
			if (!bundles.Satisfies(requiredBundles))
			{
				return false;
			}
			if (curate)
			{
				var bundle = string.Join(", \n", bundles.Curate().ToArray());
				Console.WriteLine($"Seed: {gameId}{Environment.NewLine}Bundle Flags:{Environment.NewLine}{bundle}");
			}
			return true;
		}

		public static double Search(int startId, int endId, int blockSize, out List<int> validSeeds)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			var bag = new ConcurrentBag<int>();
			var partioner = Partitioner.Create(startId, endId, blockSize);
			//for(int seed = startId; seed < endId; seed++) { 
			Parallel.ForEach(partioner, (range, loopState) =>
			{
				for (int seed = range.Item1; seed < range.Item2; seed++)
				{
					if (ValidSeed(seed))
					{
						bag.Add(seed);
						FileStream fs = null;
						int i = 0;
						while (fs == null)
						{
							try
							{
								fs = new FileStream($"MaxPerfection_{i}.txt", FileMode.Append);
							}
							catch
							{
								i++;
							}
						}
						StreamWriter stream = new StreamWriter(fs);
						stream.Write($"{seed},");
						Console.WriteLine(seed);
						stream.Close();
					}
				}
			});
			double seconds = stopwatch.Elapsed.TotalSeconds;
			Console.WriteLine($"Found: {bag.Count} sols in {seconds.ToString("F2")} s");
			validSeeds = bag.ToList();
			validSeeds.Sort();
			return seconds;
		}

		public static void findGoodStepCount(int count, int machines)
		{

			GameSave save = new GameSave("C:\\Users\\Blake\\AppData\\Roaming\\StardewValley\\Saves\\MAXIMUM3_655571\\MAXIMUM3_655571");

			for (int steps = save.steps; steps < save.steps + count; steps++)
			{
				// Need to reload objects every pass
				save = new GameSave("C:\\Users\\Blake\\AppData\\Roaming\\StardewValley\\Saves\\MAXIMUM3_655571\\MAXIMUM3_655571");
				save.day++;
				var list = OvernightFarmUpdate(save, steps, machines);
				Console.WriteLine(String.Join("\n", list));
			}

		}
		public static List<(string, Vector2)> OvernightFarmUpdate( GameSave save, int steps, int machines)
		{
			List<(string, Vector2)> destroyedObjects = new();
			Season season = Utility.getSeasonFromDay(save.day);
			int dayOfMonth = Utility.getDayOfMonthFromDay(save.day);
			Location farm = save.farm;

			Random gameRandom;
			var result = StepPredictions.Predict(save.gameId, save.day-1, steps, save.friends, out gameRandom, machines);


			// Special orders board
			if (dayOfMonth == 1 || dayOfMonth == 8 || dayOfMonth == 15 || dayOfMonth == 22)
			{
				int questCount = 0;
				foreach (var order in SpecialOrders.GetOrders(save.gameId, save.day))
				{
					switch (order.questKey)
					{
						case "Demetrius2":
							questCount += 8;
							break;
						case "Lewis":
							questCount += 9;
							break;
						case "Robin2":
							questCount += 4;
							break;
						case "Caroline":
							questCount += 8;
							break;

					}
				}

				for (int i = 0; i <  questCount; i++)
				{
					gameRandom.Next();
				}
			}

			// QuestOfTheDay
			var quest = Quests.GetQuestType(save.gameId, save.day, save.hasSocialiseQuest, save.deepestMineLevel);

			switch (quest)
			{
				case Quests.QuestType.ItemDeliveryQuest:
					gameRandom.Next();
					break;
				case Quests.QuestType.FishingQuest:
					gameRandom.Next();
					break;
				case Quests.QuestType.ResourceCollectionQuest:
					gameRandom.Next();
					break;
				case Quests.QuestType.SlayMonsterQuest:
					// TODO - Randomness in slaymonsterquests
					//gameRandom.Next();
					break;
				case Quests.QuestType.SocializeQuest:
					break;

			}

			// Weather
			WeatherType overmorrow = save.weatherOvermorrow;
			if (season == Season.Summer)
			{
				// Random uses if upgrading rain to storm
				if (overmorrow == WeatherType.Rain)
				{
					if (gameRandom.NextDouble() < 0.85)
					{

					}
					else
					{
						if (!(dayOfMonth == 1 || dayOfMonth == 2))
						{
							gameRandom.Next();
						}
					}
				}
			}else if (season == Season.Spring || season == Season.Fall)
			{
				// Upgrading rain to storm
				if (overmorrow == WeatherType.Rain && save.day > 28)
				{
					if (season != Season.Spring || dayOfMonth != 3)
					{
						gameRandom.Next();
					}
				}

				// Debris check
				if (overmorrow == WeatherType.Sun && save.day > 3)
				{
					gameRandom.Next();
				}
			}

			// Ginger Island Weather
			if (save.visitedIsland)
			{
				gameRandom.Next();
			}



			// FarmHouse - nothing?

			// ###GameLocation.DayUpdate
			// Terrain Features

			KeyValuePair<Vector2, TerrainFeature>[] map_features = farm.TerrainFeatures.ToArray();
			KeyValuePair<Vector2, TerrainFeature>[] array2 = map_features;
			for (int k = 0; k < array2.Length; k++)
			{
				KeyValuePair<Vector2, TerrainFeature> pair5 = array2[k];
				pair5.Value.dayUpdate(gameRandom, season);
			}

			// Large terrain features - nothing?

			// SpawnObjects - standard farm, nothing? Artifact spot spawn?

			// Hoedirt decay - TODO

			// ###Farm.DayUpdate
			// Slimes escaping - nothing

			// Specific farm updates - TODO

			// Mushroom tree conversion
			if (farm.TerrainFeatures.Count > 0 && season == Season.Fall && dayOfMonth > 1 && gameRandom.NextDouble() < 0.05)
			{
				for (int tries = 0; tries < 10; tries++)
				{
					KeyValuePair<Vector2, TerrainFeature> pair = farm.TerrainFeatures.ElementAt(gameRandom.Next(farm.TerrainFeatures.Count));
					var feature = pair.Value;
					if (feature is Tree tree && (int)tree.growthStage >= 5 && !tree.tapped && !tree.isTemporaryGreenRainTree)
					{
						tree.treeType = "7";
						break;
					}
				}
			}

			// Crows - TODO

			// Spawning Weeds/Stones/Twigs
			int numberToSpawn = 0;
			switch (season)
			{
				case Season.Spring:
				case Season.Fall:
					numberToSpawn = 20;
					break;
				case Season.Summer:
					numberToSpawn = 30;
					break;
			}

			bool spawnFromOldWeeds = true;
			bool weedsOnly = false;
			bool isFarm = true;
			Vector2 Vector2Zero = new Vector2(0, 0);
			bool greenRain = save.weatherForTomorrow == WeatherType.GreenRain;

			if (Weather.isRainType(save.weatherForTomorrow))
			{
				numberToSpawn *= 2;
			}
			if (dayOfMonth == 1)
			{
				numberToSpawn *= 5;
			}
			for (int i = 0; i < numberToSpawn; i++)
			{
				Vector2 v = (spawnFromOldWeeds ? new Vector2(gameRandom.Next(-1, 2), gameRandom.Next(-1, 2)) : new Vector2(gameRandom.Next(farm.map.Layers[0].Width), gameRandom.Next(farm.map.Layers[0].Height)));
				//if (!spawnFromOldWeeds && this is IslandWest)
				//{
				//	v = new Vector2(gameRandom.Next(57, 97), gameRandom.Next(44, 68));
				//}
				while (spawnFromOldWeeds && v.Equals(Vector2Zero))
				{
					v = new Vector2(gameRandom.Next(-1, 2), gameRandom.Next(-1, 2));
				}
				Vector2 fromTile = Vector2Zero;
				Item fromObj = null;
				if (spawnFromOldWeeds)
				{
					KeyValuePair<Vector2, Item> pair = farm.Objects.ElementAt(gameRandom.Next(farm.Objects.Count));
					fromTile = pair.Key; fromObj = pair.Value;
					//Utility.TryGetRandom(this.objects, out fromTile, out fromObj);
				}
				Vector2 baseVect = (spawnFromOldWeeds ? fromTile : Vector2Zero);
				//if ((this is Mountain && v.X + baseVect.X > 100f) || this is IslandNorth)
				//{
				//	continue;
				//}
				//bool num = this is Farm || this is IslandWest;
				bool num = true;
				int checked_tile_x = (int)(v.X + baseVect.X);
				int checked_tile_y = (int)(v.Y + baseVect.Y);
				Vector2 checked_tile = new Vector2(checked_tile_x, checked_tile_y);
				int health = 1;
				bool is_valid_tile = false;
				bool tile_is_diggable = farm.map.doesTileHaveProperty(checked_tile_x, checked_tile_y, "Diggable", "Back") != null;
				if (num == tile_is_diggable && !farm.map.IsNoSpawnTile(checked_tile_x, checked_tile_y) && farm.map.doesTileHaveProperty(checked_tile_x, checked_tile_y, "Type", "Back") != "Wood")
				{
					bool is_tile_clear = false;
					if (farm.CanItemBePlacedHere(checked_tile) && !farm.TerrainFeatures.ContainsKey(checked_tile))
					{
						is_tile_clear = true;
					}
					else if (spawnFromOldWeeds)
					{
						if (farm.Objects.TryGetValue(checked_tile, out var tileObj))
						{
							if (greenRain)
							{
								is_tile_clear = false;
							}
							else if (!tileObj.IsTapper())
							{
								is_tile_clear = true;
							}
						}
						if (!is_tile_clear && farm.TerrainFeatures.TryGetValue(checked_tile, out var terrainFeature) && (terrainFeature is HoeDirt || terrainFeature is Flooring))
						{
							is_tile_clear = !greenRain;
						}
					}
					if (is_tile_clear)
					{
						if (spawnFromOldWeeds)
						{
							is_valid_tile = true;
						}
						else if (!farm.Objects.ContainsKey(checked_tile))
						{
							is_valid_tile = true;
						}
					}
				}
				if (!is_valid_tile)
				{
					continue;
				}
				string whatToAdd = null;
				//if (this is Desert)
				//{
				//	whatToAdd = "(O)750";
				//}
				//else
				{
					if (gameRandom.NextBool() && !weedsOnly && (!spawnFromOldWeeds || fromObj.IsBreakableStone() || fromObj.IsTwig()))
					{
						whatToAdd = gameRandom.Choose("(O)294", "(O)295", "(O)343", "(O)450");
					}
					else if (!spawnFromOldWeeds || fromObj.IsWeeds())
					{
						whatToAdd = Location.getWeedForSeason(gameRandom, season);
						if (greenRain)
						{
							if (farm.map.doesTileHaveProperty((int)(v.X + baseVect.X), (int)(v.Y + baseVect.Y), "Type", "Back") == (isFarm ? "Dirt" : "Grass"))
							{
								int which = gameRandom.Next(8);
								whatToAdd = "(O)GreenRainWeeds" + which;
								if (which == 2 || which == 3 || which == 7)
								{
									health = 2;
								}
							}
							else
							{
								whatToAdd = null;
							}
						}
					}
					if (isFarm && !spawnFromOldWeeds && gameRandom.NextDouble() < 0.05 && !farm.TerrainFeatures.ContainsKey(checked_tile))
					{
						farm.TerrainFeatures.Add(checked_tile, new Tree(checked_tile, farm));// (gameRandom.Next(3) + 1).ToString(), gameRandom.Next(3)));
						gameRandom.Next(3);
						gameRandom.Next(3);
						continue;
					}
				}
				if (whatToAdd == null)
				{
					continue;
				}
				bool destroyed = false;
				if (farm.Objects.TryGetValue(checked_tile, out var removedObj))
				{
					//if (greenRain || removedObj is Fence || removedObj is Chest || removedObj.QualifiedItemId == "(O)590" || removedObj.QualifiedItemId == "(BC)MushroomLog")
					//{
					//	continue;
					//}
					string text = removedObj.Name;
					if (text != null && text.Length > 0 && removedObj.Category != -999)
					{
						destroyed = true;
						//Game1.debugOutput = removedObj.Name + " was destroyed";
					}
					destroyedObjects.Add(($"{text} was replaced with {Item.Get(whatToAdd).Name}", checked_tile));
					farm.Objects.Remove(checked_tile);
				}
				if (farm.TerrainFeatures.TryGetValue(checked_tile, out var removedFeature))
				{
					try
					{
						destroyed = removedFeature is HoeDirt || removedFeature is Flooring;
					}
					catch (Exception)
					{
					}
					if (!destroyed || greenRain)
					{
						break;
					}
					farm.TerrainFeatures.Remove(checked_tile);
				}
				//if (destroyed && isFarm && day > 1 && !notified_destruction)
				//{
				//	notified_destruction = true;
				//	Game1.multiplayer.broadcastGlobalMessage("Strings\\Locations:Farm_WeedsDestruction", false, null);
				//}
				Item obj = Item.Get(whatToAdd);
				//obj.minutesUntilReady.Value = health;
				farm.Objects.TryAdd(checked_tile, obj);
			}

			return destroyedObjects;
		}
	}

	public class GameSave
	{
		public Location farm;
		public List<string> friends = new List<string>();
		public bool visitedIsland = false;
		public int gameId;
		public int day;
		public WeatherType weatherForTomorrow;
		public int steps;
		public WeatherType weatherOvermorrow;
		public bool hasSocialiseQuest;
		public int deepestMineLevel;

		public GameSave(string fileName)
		{
			farm = new Location("Farm_Standard", 0, false);

			XmlDocument doc = new XmlDocument();
			doc.Load(fileName);
			XmlNode saveGame = doc.SelectSingleNode("SaveGame");
			gameId = (int)Convert.ToInt32(saveGame.SelectSingleNode("uniqueIDForThisGame").InnerText);

			XmlNode xmlLocations = saveGame.SelectSingleNode("locations");
			XmlNode xmlLocation = null;
			foreach (XmlNode gameLocation in xmlLocations.ChildNodes)
			{
				if (gameLocation.SelectSingleNode("name").InnerText == "Farm")
				{
					xmlLocation = gameLocation;
					break;
				}
			}

			if (xmlLocation == null)
				return;

			XmlNode xmlTerrainFeatures = xmlLocation.SelectSingleNode("terrainFeatures");
			foreach (XmlNode item in xmlTerrainFeatures.ChildNodes)
			{
				XmlNode terrainFeature = item.SelectSingleNode("value/TerrainFeature");
				switch (terrainFeature.Attributes[0].Value)
				{
					case "Tree":
						Tree tree = new Tree(new Vector2(Convert.ToInt32(item.SelectSingleNode("key/Vector2/X").InnerText), Convert.ToInt32(item.SelectSingleNode("key/Vector2/Y").InnerText)), farm);

						tree.growthStage = Convert.ToInt32(terrainFeature.SelectSingleNode("growthStage").InnerText);
						tree.stump = terrainFeature.SelectSingleNode("stump").InnerText == "true";
						tree.hasMoss = terrainFeature.SelectSingleNode("hasMoss").InnerText == "true";
						tree.fertilized = terrainFeature.SelectSingleNode("fertilized").InnerText == "true";
						tree.tapped = terrainFeature.SelectSingleNode("tapped").InnerText == "true";
						tree.isTemporaryGreenRainTree = terrainFeature.SelectSingleNode("isTemporaryGreenRainTree").InnerText == "true";
						tree.treeType = terrainFeature.SelectSingleNode("treeType").InnerText;

						farm.TerrainFeatures.Add(tree.tile, tree);
						farm.TerrainFeatures2.Add(tree);
						break;
					case "Grass":
						Grass grass = new Grass();
						grass.tile = new Vector2(Convert.ToInt32(item.SelectSingleNode("key/Vector2/X").InnerText), Convert.ToInt32(item.SelectSingleNode("key/Vector2/Y").InnerText));
						grass.grassType = Convert.ToInt32(terrainFeature.SelectSingleNode("grassType").InnerText);
						grass.numberOfWeeds = Convert.ToInt32(terrainFeature.SelectSingleNode("numberOfWeeds").InnerText);
						grass.grassSourceOffset = Convert.ToInt32(terrainFeature.SelectSingleNode("grassSourceOffset").InnerText);

						farm.TerrainFeatures.Add(grass.tile, grass);
						farm.TerrainFeatures2.Add(grass);
						break;
					case "HoeDirt":
						HoeDirt hoeDirt = new HoeDirt();
						hoeDirt.tile = new Vector2(Convert.ToInt32(item.SelectSingleNode("key/Vector2/X").InnerText), Convert.ToInt32(item.SelectSingleNode("key/Vector2/Y").InnerText));
						// TODO crops
						break;
				}
			}

			XmlNode objects = xmlLocation.SelectSingleNode("objects");
			foreach (XmlNode item in objects.ChildNodes)
			{
				Vector2 tile = new Vector2(Convert.ToInt32(item.SelectSingleNode("key/Vector2/X").InnerText), Convert.ToInt32(item.SelectSingleNode("key/Vector2/Y").InnerText));
				farm.Objects.Add(tile, Item.Get("(O)" + item.SelectSingleNode("value/Object/itemId").InnerText));
			}

			friends = new List<string>();

			XmlNode player = saveGame.SelectSingleNode("player");
			XmlNode friendshipData = player.SelectSingleNode("friendshipData");
			foreach (XmlNode item in friendshipData.ChildNodes)
			{
				friends.Add(item.SelectSingleNode("key/string").InnerText);
			}

			XmlNode mail = player.SelectSingleNode("mailReceived");
			foreach (XmlNode item in mail.ChildNodes)
			{
				if (item.InnerText == "Visited_Island")
				{
					visitedIsland = true;
					break;
				}
			}


			XmlNode stats = player.SelectSingleNode("stats/Values");
			foreach (XmlNode node in stats)
			{
				if (node.SelectSingleNode("key/string").InnerText == "daysPlayed")
				{
					day = Convert.ToInt32(node.SelectSingleNode("value/unsignedInt").InnerText);
				}
				if (node.SelectSingleNode("key/string").InnerText == "stepsTaken")
				{
					steps = Convert.ToInt32(node.SelectSingleNode("value/unsignedInt").InnerText);
				}
			}

			XmlNode questLog = player.SelectSingleNode("questLog");
			foreach (XmlNode node in questLog.ChildNodes)
			{
				if (node.Attributes[0].Value == "SocializeQuest")
				{
					hasSocialiseQuest = true;
					break;
				}
			}

			deepestMineLevel = Convert.ToInt32(player.SelectSingleNode("deepestMineLevel").InnerText);

			XmlNode LocationWeather = saveGame.SelectSingleNode("locationWeather");
			foreach (XmlNode node in LocationWeather.ChildNodes)
			{
				if (node.SelectSingleNode("key/string").InnerText == "Default")
				{
					weatherForTomorrow = Weather.getWeatherTypeFromName(node.SelectSingleNode("value/LocationWeather/WeatherForTomorrow").InnerText);
					break;
				}
			}

			weatherOvermorrow = Weather.getWeather(day, gameId);
		}
	}
}
