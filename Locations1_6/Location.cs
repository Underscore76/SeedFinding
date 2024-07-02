using Newtonsoft.Json;
using SeedFinding.Locations;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace SeedFinding.Locations1_6
{
	internal class Location
	{
		public Map map;
		public string Name;
		public Dictionary<Point, Item> ForageSpawns = new();
		public List<(Point,string)> ArtifactSpots = new();
		public LocationData LocationData;
		public Dictionary<Season,List<ForageData>> SeasonalForage = new();
		public Dictionary<Point,Item> Objects = new();
		public Dictionary<Point, Item> Trees = new();
		public List<Bush> Bushes = new();
		public List<ResourceClump> ResourceClumps = new();

		public int Seed;
		public int Day;

		public Location(string name, int seed)
		{
			Name = name;
			Seed = seed;
			Day = 0;
			map = JsonConvert.DeserializeObject<Map>(System.IO.File.ReadAllText($@"Locations1_6/{name}.json"));
			setupDefaultCollections();
			LocationData = Game1.locations1_6[name];

			List<ForageData> spring = new List<ForageData>();
			List<ForageData> summer = new List<ForageData>();
			List<ForageData> fall = new List<ForageData>();
			List<ForageData> winter = new List<ForageData>();

			SeasonalForage.Add(Season.Spring, spring);
			SeasonalForage.Add(Season.Summer, summer);
			SeasonalForage.Add(Season.Fall, fall);
			SeasonalForage.Add(Season.Winter, winter);

			foreach (ForageData spawn in LocationData.Forage)
			{
				if (spawn.Season == null || spawn.Season == "Spring")
				{
					spring.Add(spawn);
				}
				if (spawn.Season == null || spawn.Season == "Summer")
				{
					summer.Add(spawn);
				}
				if (spawn.Season == null || spawn.Season == "Fall")
				{
					fall.Add(spawn);
				}
				if (spawn.Season == null || spawn.Season == "Winter")
				{
					winter.Add(spawn);
				}
			}
		}

		public void setupDefaultCollections()
		{
			Layer paths = map.FindLayer("Paths");
			TileSet tileSet = null;
			for(int y = 0; y < paths.Height; y++)
			{
				for(int x=0; x < paths.Width; x++)
				{
					int index = paths.GetTileIndex(x, y);
					if (index == 0)
					{
						continue;
					}
					if (tileSet == null){
						tileSet = map.FindTileSet(index);
					}
					index -= tileSet.Firstgid;
					Point tile = new Point(x, y);
					switch (index)
					{
						case 9:
						case 10:
						case 11:
						case 12:
						case 31:
						case 32:
						case 34:
							Trees.Add(tile, Item.Get("(O)309"));
							break;
						case 13:
						case 14:
						case 15:
							Objects.Add(tile, Item.Get("(O)784"));
							break;
						case 16:
							Objects.Add(tile, Item.Get("(O)343"));
							break;
						case 17:
							Objects.Add(tile, Item.Get("(O)343"));
							break;
						case 18:
							Objects.Add(tile, Item.Get("(O)294"));
							break;
						case 19:
							ResourceClumps.Add(new ResourceClump() { location = tile });
							break;
						case 20:
							ResourceClumps.Add(new ResourceClump() { location = tile });
							break;
						case 21:
							ResourceClumps.Add(new ResourceClump() { location = tile });
							break;
						case 22:
						case 36:
							Objects.Add(tile, Item.Get("(O)297"));
							break;
						case 23:
							Trees.Add(tile, Item.Get("(O)309"));
							break;
						case 24:
							Bushes.Add(new Bush() { location = tile, size = 2 });
							break;
						case 25:
							Bushes.Add(new Bush() { location = tile, size = 1 });
							break;
						case 26:
							Bushes.Add(new Bush() { location = tile, size = 0 });
							break;
						case 33:
							Bushes.Add(new Bush() { location = tile, size = 4 });
							break;
						case 27:
							break;
						case 29:
						case 30:
							break;
					}
				}
			}
		}
		public bool isBehindBush(Point location)
		{
			foreach (var bush in Bushes)
			{
				Rectangle down = new Rectangle((int)location.X * 64, (int)(location.Y + 1f) * 64, 64, 128);
				if (bush.getBoundingBox().IntersectsWith(down))
				{
					return true;
				}
			}

			return false;
		}

		public bool isBehindTree(Point location)
		{
			Rectangle down = new Rectangle((int)(location.X - 1f) * 64, (int)location.Y * 64, 192, 256);
			foreach (var tree in Trees)
			{
				Rectangle treeTile = new Rectangle((int)tree.Key.X * 64, (int)tree.Key.Y * 64, 64, 64);
				if (treeTile.IntersectsWith(down))
				{
					return true;
				}
			}
			return false;

		}

		public bool CanItemBePlacedHere(Point location)
		{
			if (Objects.ContainsKey(location) || Trees.ContainsKey(location) || ForageSpawns.ContainsKey(location))
			{
				return false;
			}

			if (isBehindBush(location)){
				return false;
			}

			foreach (var bush in Bushes)
			{
				if (bush.location == location)
				{
					return false;
				}
			}

			Rectangle tile = new Rectangle((int)location.X * 64, (int)location.Y * 64, 64, 64);
			foreach (var clump in ResourceClumps)
			{
				if (clump.getBoundingBox().IntersectsWith(tile))
				{
					return false;
				}
			}

			if (!isTilePassable(location))
			{
				return false;
			}

			return true;
		}

		public bool isTilePassable(Point location)
		{
			if (map.doesTileHaveProperty(location.X, location.Y, "Passable", "Back") != null)
			{
				return false;
			}
			if (map.getTileIndexAt(location.X,location.Y,"Buildings") != 0 && map.doesTileHaveProperty(location.X, location.Y, "Shadow", "Buildings") == null && map.doesTileHaveProperty(location.X, location.Y, "Passable", "Buildings") == null)
			{
				return false;
			}
			return true;
		}
		public void ProcessDay()
		{
			int dayOfMonth = (Day - 1) % 28 + 1;
			int dayOfWeek = dayOfMonth % 7;
			if (dayOfWeek == 0)
			{
				ForageSpawns.Clear();
				Spawn(Seed, Day);
				Spawn(Seed, Day);
			}
			Spawn(Seed, Day);
			if (dayOfMonth == 1)
			{
				Spawn(Seed, Day);
			}
			if (Day < 4)
			{
				Spawn(Seed, Day);
			}
		}
		public void RunToDay(int day)
		{
			ForageSpawns.Clear();
			Day = 0;
			//Spawn(Seed, Day);
			while (Day < day)
			{
				Day++;
				ProcessDay();
			}
		}
		public void Spawn(int gameid, int day)
		{
			Season season = Utility.getSeasonFromDay(day);
			var seasonData = SeasonalForage[season];
			Random rand = Utility.CreateDaySaveRandom(day, gameid);
			if (ForageSpawns.Count < LocationData.MaxSpawnedForageAtOnce)
			{
				int numToSpawn = rand.Next(LocationData.MinDailyForageSpawn, LocationData.MaxDailyForageSpawn + 1);
				numToSpawn = Math.Min(numToSpawn, LocationData.MaxSpawnedForageAtOnce - ForageSpawns.Count);
				for (int i = 0; i < numToSpawn; i++)
				{
					for (int t = 0; t < 11; t++)
					{
						Point check = new Point(rand.Next(map.Width), rand.Next(map.Height));

						if (map.IsNoSpawnTile(check.X, check.Y) || map.doesTileHaveProperty(check.X, check.Y, "Spawnable", "Back") == null || map.doesEitherTileOrTileIndexPropertyEqual(check.X, check.Y, "Spawnable", "Back", "F") || !this.CanItemBePlacedHere(check) || map.getTileIndexAt(check.X, check.Y, "AlwaysFront") != 0 || map.getTileIndexAt(check.X, check.Y, "AlwaysFront2") != 0 || map.getTileIndexAt(check.X, check.Y, "AlwaysFront3") != 0 || map.getTileIndexAt(check.X, check.Y, "Front") != 0 || this.isBehindBush(check) || (!rand.NextBool(0.1) && this.isBehindTree(check)))
						{
							continue;
						}

						ForageData index = rand.ChooseFrom(seasonData);

						if (!rand.NextBool(index.Chance))
						{
							continue;
						}

						ForageSpawns[check] = Item.Get(index.Id);
						break;
					}
				}
			}

			for( int i = ArtifactSpots.Count - 1; i >= 0; i--)
			{
				if (rand.NextBool(0.15))
				{
					ArtifactSpots.RemoveAt(i);
				}
			}

			if (ArtifactSpots.Count > ((!(Name.Contains("Farm"))) ? 1 : 0) && (season != Season.Winter || ArtifactSpots.Count > 4))
			{
				return;
			}

			
            double chanceForNewArtifactAttempt = 1.0;
			while (rand.NextDouble() < chanceForNewArtifactAttempt)
			//while (rand.NextBool(chanceForNewArtifactAttempt))
            {
                int xCoord = rand.Next(map.Width);
                int yCoord = rand.Next(map.Height);
                Point location = new Point(xCoord, yCoord);
				if (this.CanItemBePlacedHere(location) && /*!this.IsTileOccupiedBy(location) &&*/ map.getTileIndexAt(xCoord, yCoord, "AlwaysFront") == 0 && map.getTileIndexAt(xCoord, yCoord, "Front") == 0 && !this.isBehindBush(location) && (map.doesTileHaveProperty(xCoord, yCoord, "Diggable", "Back") != null || (season == Season.Winter && map.doesTileHaveProperty(xCoord, yCoord, "Type", "Back") != null && map.doesTileHaveProperty(xCoord, yCoord, "Type", "Back").Equals("Grass"))))
				{
                    if (Name.Equals("Forest") && xCoord >= 93 && yCoord <= 22)
                    {
                        continue;
					}
					ArtifactSpots.Add( (location, rand.NextBool(0.166) ? "(O)SeedSpot" : "(O)590") );
                }
                chanceForNewArtifactAttempt *= 0.75;
                if (season == Season.Winter)
                {
                    chanceForNewArtifactAttempt += 0.10000000149011612;
                }
            }

			
		}
		public void printResults()
		{
			Console.WriteLine($"{Name}	{Day-1}");
			foreach (var forage in ForageSpawns)
			{
				Console.WriteLine($"{forage.Key}	{forage.Value.Name}");
			}

			foreach (var aftifact in ArtifactSpots)
			{
				Console.WriteLine($"{aftifact.Item1}	{aftifact.Item2}");
			}
		}
	}

	public class Bush
	{
		public int size;
		public Point location;

		public Rectangle getBoundingBox()
		{
			Point tileLocation = this.location;
			switch (this.size)
			{
				case 0:
				case 3:
					return new Rectangle((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 64, 64);
				case 1:
				case 4:
					return new Rectangle((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 128, 64);
				case 2:
					return new Rectangle((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 192, 64);
				default:
					return Rectangle.Empty;
			}
		}
	}

	public class ResourceClump
	{
		public Point location;

		public Rectangle getBoundingBox()
		{

			Point tileLocation = this.location;
			return new Rectangle((int)tileLocation.X * 64, (int)tileLocation.Y * 64, (int)2 * 64, (int)2 * 64);
		}
	}
}
