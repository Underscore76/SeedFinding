using Newtonsoft.Json;
using SeedFinding.Locations;
using SeedFinding.Locations1_6;
using SeedFinding.StardewClasses;
using SeedFinding.Utilities;
using System;
using System.Collections.Generic;
using SkiaSharp;
using System.Drawing;
using Layer = SeedFinding.Locations1_6.Layer;
using Map = SeedFinding.Locations1_6.Map;
using Tile = SeedFinding.Locations1_6.Tile;

namespace SeedFinding.Volcano
{
	public struct VolcanoChest
	{
		public double upgradeLuck;
		public bool upgraded;
		public Item basicItem;
		public Item upgradedItem;
		public Vector2 location;

		public VolcanoChest(bool upgraded, double upgradeLuck, Item basicItem, Item upgradedItem, Vector2 location)
		{
			this.upgraded = upgraded;
			this.upgradeLuck = upgradeLuck;
			this.basicItem = basicItem;
			this.upgradedItem = upgradedItem;
			this.location = location;
		}
	}

	public struct SetPiece
	{
		public int size;
		public Vector2 coordinates;
		public Vector2 location;

		public SetPiece(int size, Vector2 coordinates, Vector2 location)
		{
			this.size = size;
			this.coordinates = coordinates;
			this.location = location;
		}
	}
	public class VolcanoFloor
	{
		public int level = -1;
		public int layout = -1;
		public bool flipped = false;
		Random generationRandom;
		List<Rectangle> setPieceAreas = new List<Rectangle>();
		PixelMap pixelMap = null;

		public int[] heightMap;
		public List<Vector2> barrelLocations = new List<Vector2>();
		public List<Vector2> teethLocations = new List<Vector2>();
		public List<Vector2> magmaCapLocations = new List<Vector2>();
		public Dictionary<Vector2, string> stones = new Dictionary<Vector2, string>();
		public Dictionary<Vector2, VolcanoChest> volcanoChests = new();
		public int monsterCount = 0;
		public Dictionary<Vector2, string> monsters = new Dictionary<Vector2, string>();
		public List<Item> items = new List<Item>();
		public Dictionary<string, int> monsterCounts = new();

		private const int mapWidth = 64;
		private const int mapHeight = 64;

		public Dictionary<int, List<Point>> possibleSwitchPositions = new Dictionary<int, List<Point>>();
		public List<Point> switches = new();

		public Dictionary<int, List<Point>> possibleGatePositions = new Dictionary<int, List<Point>>();

		public HashSet<Point> dirtTiles = new HashSet<Point>();

		public Point? startPosition;
		public Point? endPosition;

		public HashSet<Point> blockedTiles = new HashSet<Point>();
		public HashSet<Point> buildingTiles = new HashSet<Point>();
		public HashSet<Point> frontTiles = new HashSet<Point>();
		public HashSet<Point> dirtAdjacentTiles = new HashSet<Point>();
		public HashSet<Point> dungeonBackTiles = new HashSet<Point>();

		protected Dictionary<int, Point> _blobIndexLookup = null;
		protected Dictionary<int, Point> _lavaBlobIndexLookup = null;

		private Dictionary<Point, List<Item>> itemsList = new Dictionary<Point, List<Item>>();

		//public Bitmap FloorImage = null;

		public List<SetPiece> SetPieces = new();

		public VolcanoFloor(int level, int layout, int generationSeed)
		{
			this.level = level;
			this.layout = layout;

			if (level == 0)
			{
				return;
			}

			generationRandom = Utility.CreateRandom(generationSeed);
			generationRandom.Next();

			flipped = generationRandom.Next(2) == 1;
			if (level == 0 || level == 5)
			{
				flipped = false;
			}

			pixelMap = VolcanoFloorPixelMaps.GetPixelMap(layout, flipped);

			PlaceGroundTiles();
			this.ApplyToPixelType(PixelType.StartPosition, delegate (int x, int y)
			{
				if (!this.startPosition.HasValue)
				{
					this.startPosition = new Point(x, y);
				}
				if (this.level == 0)
				{
					//base.warps.Add(new Warp(x, y + 2, "IslandNorth", 40, 24, flipFarmer: false));
				}
				else
				{
					//base.warps.Add(new Warp(x, y + 2, VolcanoDungeon.GetLevelName(this.level.Value - 1), x - this.startPosition.Value.X, 0, flipFarmer: false));
				}
			});

			this.ApplyToPixelType(PixelType.Empty, delegate (int x, int y)
			{
				this.SetPixelType(x, y, PixelType.Open);
			});


			this.ApplyToPixelType(PixelType.EndPosition, delegate (int x, int y)
			{
				if (!this.endPosition.HasValue)
				{
					this.endPosition = new Point(x, y);
				}
				if (this.level == 9)
				{
					//base.warps.Add(new Warp(x, y - 2, "Caldera", 21, 39, flipFarmer: false));
				}
				else
				{
					//base.warps.Add(new Warp(x, y - 2, VolcanoDungeon.GetLevelName(this.level.Value + 1), x - this.endPosition.Value.X, 1, flipFarmer: false));
				}
			});

			this.ApplyToPixelType(PixelType.PossibleSwitchPosition, delegate (int x, int y)
			{
				this.AddPossibleSwitchLocation(0, x, y);
			});

			PopulateSetPieces();
			ApplySetPieces();

			this.GenerateWalls(PixelType.OutOfBounds, 0, 4, 4, 4, start_in_wall: true, delegate (int x, int y)
			{
				this.SetPixelType(x, y, PixelType.Wall);
			}, use_corner_hack: true);
			this.GenerateWalls(PixelType.Wall, 0, 13, 1);
			this.ApplyToPixelType(PixelType.Lava, delegate (int x, int y)
			{
				//base.waterTiles[x, y] = true;
				//this.SetTile(this.backLayer, x, y, 4);
				//base.setTileProperty(x, y, "Back", "Water", "T");

				blockedTiles.Add(new Point(x, y));
				if (this.generationRandom.NextDouble() < 0.1)
				{
					//base.sharedLights.AddLight(new LightSource($"VolcanoDungeon_{this.level.Value}_Lava_{x}_{y}", 4, new Vector2(x, y) * 64f, 2f, new Color(0, 50, 50), LightSource.LightContext.None, 0L, base.NameOrUniqueName));
				}
			});
			this.GenerateBlobs(PixelType.Lava, 0, 16, fill_center: true, is_lava_pool: true);
			if (this.startPosition.HasValue)
			{
				this.CreateEntrance(this.startPosition.Value);
			}
			if (this.endPosition.HasValue)
			{
				this.CreateExit(this.endPosition);
			}




			if (this.level != 0)
			{
				this.GenerateDirtTiles();
			}
			if ((this.level == 9 || this.generationRandom.NextDouble() < (this.isMonsterLevel() ? 1.0 : 0.2)) && this.possibleSwitchPositions.TryGetValue(0, out var endSwitchPositions) && endSwitchPositions.Count > 0)
			{
				this.AddPossibleGateLocation(0, this.endPosition.Value.X, this.endPosition.Value.Y);
			}
			foreach (int index in this.possibleGatePositions.Keys)
			{
				if (this.possibleGatePositions[index].Count > 0 && this.possibleSwitchPositions.TryGetValue(index, out var dwarfSwitchPositions) && dwarfSwitchPositions.Count > 0)
				{
					Point gate_point = this.generationRandom.ChooseFrom(this.possibleGatePositions[index]);
					this.CreateDwarfGate(index, gate_point);
				}
			}

			GenerateEntities();
			// pixelMap. origin is top left.  Rows first.


		}

		public static int shortestPath(int seed, int level, int layout)
		{
			VolcanoFloor floor = new VolcanoFloor(level, layout, seed);

			// Dictionary of checked tiles, key of tile location, value of distance
			Dictionary<Point, int> checkedTiles = new();

			List<Size> directions = new() {
				new Size(-1,0),
				new Size(1,0),
				new Size(0,-1),
				new Size(0,1)
			};

			// Queue of points to check
			Queue<Point> tilesToCheck = new();

			tilesToCheck.Enqueue((Point)floor.startPosition);
			checkedTiles.Add((Point)floor.startPosition, 1);
			while (tilesToCheck.Count != 0)
			{
				Point tile = tilesToCheck.Dequeue();
				int distance = checkedTiles[tile] + 1;
				foreach (var direction in directions)
				{
					Point neighbour = Point.Add(tile, direction);

					// Found exit
					if (neighbour == floor.endPosition)
					{
						return distance;
					}

					// Already checked
					if (checkedTiles.ContainsKey(neighbour))
					{
						continue;
					}

					// Blocked
					if (floor.blockedTiles.Contains(neighbour))
					{
						continue;
					}

					// Queue neighbour
					tilesToCheck.Enqueue(neighbour);
					checkedTiles.Add(neighbour, distance);
				}
			}

			return -1;
		}

		public void AddToBuildingTile(Point point)
		{
			buildingTiles.Add(point);
		}
		public virtual void CreateEntrance(Point? position)
		{
			for (int x = -1; x <= 1; x++)
			{
				for (int y = 0; y <= 3; y++)
				{
					if (isTileOnMap(position.Value.X + x, position.Value.Y + y))
					{
						Point point = new Point(position.Value.X + x, position.Value.Y + y);
						//base.removeTile(position.Value.X + x, position.Value.Y + y, "Back");
						//base.removeTile(position.Value.X + x, position.Value.Y + y, "Buildings");
						//base.removeTile(position.Value.X + x, position.Value.Y + y, "Front");
						if (blockedTiles.Contains(point))
						{
							blockedTiles.Remove(point);
						}
						if (buildingTiles.Contains(point))
						{
							buildingTiles.Remove(point);
						}
						if (frontTiles.Contains(point))
						{
							frontTiles.Remove(point);
						}
					}
				}
			}
			//if (base.hasTileAt(position.Value.X - 1, position.Value.Y - 1, "Front"))
			//{
			//	this.SetTile(this.frontLayer, position.Value.X - 1, position.Value.Y - 1, VolcanoDungeon.GetTileIndex(13, 16));
			//}
			//base.removeTile(position.Value.X, position.Value.Y - 1, "Front");
			frontTiles.Remove(new Point(position.Value.X, position.Value.Y - 1));
			//this.SetTile(this.buildingsLayer, position.Value.X - 1, position.Value.Y, VolcanoDungeon.GetTileIndex(13, 17));
			//this.SetTile(this.buildingsLayer, position.Value.X - 1, position.Value.Y + 1, VolcanoDungeon.GetTileIndex(13, 18));
			//this.SetTile(this.buildingsLayer, position.Value.X - 1, position.Value.Y + 2, VolcanoDungeon.GetTileIndex(13, 19));

			blockedTiles.Add(new Point(position.Value.X - 1, position.Value.Y));
			blockedTiles.Add(new Point(position.Value.X - 1, position.Value.Y + 1));
			blockedTiles.Add(new Point(position.Value.X - 1, position.Value.Y + 2));
			AddToBuildingTile(new Point(position.Value.X - 1, position.Value.Y));
			AddToBuildingTile(new Point(position.Value.X - 1, position.Value.Y + 1));
			AddToBuildingTile(new Point(position.Value.X - 1, position.Value.Y + 2));
			//if (base.hasTileAt(position.Value.X + 1, position.Value.Y - 1, "Front"))
			//{
			//	this.SetTile(this.frontLayer, position.Value.X + 1, position.Value.Y - 1, VolcanoDungeon.GetTileIndex(15, 16));
			//}
			//this.SetTile(this.buildingsLayer, position.Value.X + 1, position.Value.Y, VolcanoDungeon.GetTileIndex(15, 17));
			//this.SetTile(this.buildingsLayer, position.Value.X + 1, position.Value.Y + 1, VolcanoDungeon.GetTileIndex(15, 18));
			//this.SetTile(this.buildingsLayer, position.Value.X + 1, position.Value.Y + 2, VolcanoDungeon.GetTileIndex(15, 19));
			blockedTiles.Add(new Point(position.Value.X + 1, position.Value.Y));
			blockedTiles.Add(new Point(position.Value.X + 1, position.Value.Y + 1));
			blockedTiles.Add(new Point(position.Value.X + 1, position.Value.Y + 2));
			AddToBuildingTile(new Point(position.Value.X + 1, position.Value.Y));
			AddToBuildingTile(new Point(position.Value.X + 1, position.Value.Y + 1));
			AddToBuildingTile(new Point(position.Value.X + 1, position.Value.Y + 2));
			//this.SetTile(this.backLayer, position.Value.X, position.Value.Y, VolcanoDungeon.GetTileIndex(14, 17));
			//this.SetTile(this.backLayer, position.Value.X, position.Value.Y + 1, VolcanoDungeon.GetTileIndex(14, 18));
			//this.SetTile(this.frontLayer, position.Value.X, position.Value.Y + 2, VolcanoDungeon.GetTileIndex(14, 19));
			//this.SetTile(this.buildingsLayer, position.Value.X - 1, position.Value.Y + 3, VolcanoDungeon.GetTileIndex(12, 4));
			//this.SetTile(this.buildingsLayer, position.Value.X, position.Value.Y + 3, VolcanoDungeon.GetTileIndex(12, 4));
			//this.SetTile(this.buildingsLayer, position.Value.X + 1, position.Value.Y + 3, VolcanoDungeon.GetTileIndex(12, 4));
			frontTiles.Add(new Point(position.Value.X, position.Value.Y + 2));
			blockedTiles.Add(new Point(position.Value.X - 1, position.Value.Y + 3));
			blockedTiles.Add(new Point(position.Value.X, position.Value.Y + 3));
			blockedTiles.Add(new Point(position.Value.X + 1, position.Value.Y + 3));
			AddToBuildingTile(new Point(position.Value.X - 1, position.Value.Y + 3));
			AddToBuildingTile(new Point(position.Value.X, position.Value.Y + 3));
			AddToBuildingTile(new Point(position.Value.X + 1, position.Value.Y + 3));
		}

		private void CreateExit(Point? position, bool draw_stairs = true)
		{
			for (int x = -1; x <= 1; x++)
			{
				for (int y = -4; y <= 0; y++)
				{
					if (isTileOnMap(position.Value.X + x, position.Value.Y + y))
					{
						//if (draw_stairs)
						//{
						//	base.removeTile(position.Value.X + x, position.Value.Y + y, "Back");
						//}
						//base.removeTile(position.Value.X + x, position.Value.Y + y, "Buildings");
						//base.removeTile(position.Value.X + x, position.Value.Y + y, "Front");
						if (blockedTiles.Contains(new Point(position.Value.X + x, position.Value.Y + y)))
						{
							blockedTiles.Remove(new Point(position.Value.X + x, position.Value.Y + y));
						}
						if (buildingTiles.Contains(new Point(position.Value.X + x, position.Value.Y + y)))
						{
							buildingTiles.Remove(new Point(position.Value.X + x, position.Value.Y + y));
						}
						if (frontTiles.Contains(new Point(position.Value.X + x, position.Value.Y + y)))
						{
							frontTiles.Remove(new Point(position.Value.X + x, position.Value.Y + y));
						}
					}
				}
			}
			//this.SetTile(this.buildingsLayer, position.Value.X - 1, position.Value.Y, VolcanoDungeon.GetTileIndex(9, 19));
			//this.SetTile(this.buildingsLayer, position.Value.X - 1, position.Value.Y - 1, VolcanoDungeon.GetTileIndex(9, 18));
			//this.SetTile(this.buildingsLayer, position.Value.X - 1, position.Value.Y - 2, VolcanoDungeon.GetTileIndex(9, 17));
			//this.SetTile(this.buildingsLayer, position.Value.X - 1, position.Value.Y - 3, VolcanoDungeon.GetTileIndex(9, 16));
			blockedTiles.Add(new Point(position.Value.X - 1, position.Value.Y));
			blockedTiles.Add(new Point(position.Value.X - 1, position.Value.Y - 1));
			blockedTiles.Add(new Point(position.Value.X - 1, position.Value.Y - 2));
			blockedTiles.Add(new Point(position.Value.X - 1, position.Value.Y - 3));
			AddToBuildingTile(new Point(position.Value.X - 1, position.Value.Y));
			AddToBuildingTile(new Point(position.Value.X - 1, position.Value.Y - 1));
			AddToBuildingTile(new Point(position.Value.X - 1, position.Value.Y - 2));
			AddToBuildingTile(new Point(position.Value.X - 1, position.Value.Y - 3));
			//this.SetTile(this.alwaysFrontLayer, position.Value.X - 1, position.Value.Y - 4, VolcanoDungeon.GetTileIndex(12, 4));
			//this.SetTile(this.alwaysFrontLayer, position.Value.X, position.Value.Y - 4, VolcanoDungeon.GetTileIndex(12, 4));
			//this.SetTile(this.alwaysFrontLayer, position.Value.X + 1, position.Value.Y - 4, VolcanoDungeon.GetTileIndex(12, 4));
			frontTiles.Add(new Point(position.Value.X - 1, position.Value.Y - 4));
			frontTiles.Add(new Point(position.Value.X, position.Value.Y - 4));
			frontTiles.Add(new Point(position.Value.X + 1, position.Value.Y - 4));
			//this.SetTile(this.buildingsLayer, position.Value.X, position.Value.Y - 3, VolcanoDungeon.GetTileIndex(10, 16));
			//this.SetTile(this.buildingsLayer, position.Value.X + 1, position.Value.Y, VolcanoDungeon.GetTileIndex(11, 19));
			//this.SetTile(this.buildingsLayer, position.Value.X + 1, position.Value.Y - 1, VolcanoDungeon.GetTileIndex(11, 18));
			//this.SetTile(this.buildingsLayer, position.Value.X + 1, position.Value.Y - 2, VolcanoDungeon.GetTileIndex(11, 17));
			//this.SetTile(this.buildingsLayer, position.Value.X + 1, position.Value.Y - 3, VolcanoDungeon.GetTileIndex(11, 16));
			blockedTiles.Add(new Point(position.Value.X, position.Value.Y - 3));
			blockedTiles.Add(new Point(position.Value.X + 1, position.Value.Y));
			blockedTiles.Add(new Point(position.Value.X + 1, position.Value.Y - 1));
			blockedTiles.Add(new Point(position.Value.X + 1, position.Value.Y - 2));
			blockedTiles.Add(new Point(position.Value.X + 1, position.Value.Y - 3));
			AddToBuildingTile(new Point(position.Value.X, position.Value.Y - 3));
			AddToBuildingTile(new Point(position.Value.X + 1, position.Value.Y));
			AddToBuildingTile(new Point(position.Value.X + 1, position.Value.Y - 1));
			AddToBuildingTile(new Point(position.Value.X + 1, position.Value.Y - 2));
			AddToBuildingTile(new Point(position.Value.X + 1, position.Value.Y - 3));
			//if (draw_stairs)
			//{
			//	this.SetTile(this.backLayer, position.Value.X, position.Value.Y, VolcanoDungeon.GetTileIndex(12, 19));
			//	this.SetTile(this.backLayer, position.Value.X, position.Value.Y - 1, VolcanoDungeon.GetTileIndex(12, 18));
			//	this.SetTile(this.backLayer, position.Value.X, position.Value.Y - 2, VolcanoDungeon.GetTileIndex(12, 17));
			//	this.SetTile(this.backLayer, position.Value.X, position.Value.Y - 3, VolcanoDungeon.GetTileIndex(12, 16));
			//}
			//this.SetTile(this.buildingsLayer, position.Value.X - 1, position.Value.Y - 4, VolcanoDungeon.GetTileIndex(12, 4));
			//this.SetTile(this.buildingsLayer, position.Value.X, position.Value.Y - 4, VolcanoDungeon.GetTileIndex(12, 4));
			//this.SetTile(this.buildingsLayer, position.Value.X + 1, position.Value.Y - 4, VolcanoDungeon.GetTileIndex(12, 4));
			blockedTiles.Add(new Point(position.Value.X - 1, position.Value.Y - 4));
			blockedTiles.Add(new Point(position.Value.X, position.Value.Y - 4));
			blockedTiles.Add(new Point(position.Value.X + 1, position.Value.Y - 4));
			AddToBuildingTile(new Point(position.Value.X - 1, position.Value.Y - 4));
			AddToBuildingTile(new Point(position.Value.X, position.Value.Y - 4));
			AddToBuildingTile(new Point(position.Value.X + 1, position.Value.Y - 4));
		}

		public int GetNeighborValue(int x, int y, PixelType matched_type, bool is_lava_pool = false)
		{
			int neighbor_value = 0;
			if (this.GetPixelType(x, y - 1) == matched_type)
			{
				neighbor_value++;
			}
			if (this.GetPixelType(x, y + 1) == matched_type)
			{
				neighbor_value += 2;
			}
			if (this.GetPixelType(x + 1, y) == matched_type)
			{
				neighbor_value += 4;
			}
			if (this.GetPixelType(x - 1, y) == matched_type)
			{
				neighbor_value += 8;
			}
			if (is_lava_pool && neighbor_value == 15)
			{
				if (this.GetPixelType(x - 1, y - 1) == matched_type)
				{
					neighbor_value += 16;
				}
				if (this.GetPixelType(x + 1, y - 1) == matched_type)
				{
					neighbor_value += 32;
				}
			}
			return neighbor_value;
		}

		public Dictionary<int, Point> GetBlobLookup()
		{
			if (_blobIndexLookup == null)
			{
				_blobIndexLookup = new Dictionary<int, Point>();
				_blobIndexLookup[0] = new Point(0, 0);
				_blobIndexLookup[6] = new Point(1, 0);
				_blobIndexLookup[14] = new Point(2, 0);
				_blobIndexLookup[10] = new Point(3, 0);
				_blobIndexLookup[7] = new Point(1, 1);
				_blobIndexLookup[11] = new Point(3, 1);
				_blobIndexLookup[5] = new Point(1, 2);
				_blobIndexLookup[13] = new Point(2, 2);
				_blobIndexLookup[9] = new Point(3, 2);
				_blobIndexLookup[2] = new Point(0, 1);
				_blobIndexLookup[3] = new Point(0, 2);
				_blobIndexLookup[1] = new Point(0, 3);
				_blobIndexLookup[4] = new Point(1, 3);
				_blobIndexLookup[12] = new Point(2, 3);
				_blobIndexLookup[8] = new Point(3, 3);
				_blobIndexLookup[15] = new Point(2, 1);
			}
			return _blobIndexLookup;
		}
		public Dictionary<int, Point> GetLavaBlobLookup()
		{
			if (_lavaBlobIndexLookup == null)
			{
				_lavaBlobIndexLookup = new Dictionary<int, Point>(this.GetBlobLookup());
				_lavaBlobIndexLookup[63] = new Point(2, 1);
				_lavaBlobIndexLookup[47] = new Point(4, 3);
				_lavaBlobIndexLookup[31] = new Point(4, 2);
				_lavaBlobIndexLookup[15] = new Point(4, 1);
			}
			return _lavaBlobIndexLookup;
		}

		public virtual void GenerateBlobs(PixelType match, int tile_x, int tile_y, bool fill_center = true, bool is_lava_pool = false)
		{
			for (int x = 0; x < VolcanoFloor.mapWidth; x++)
			{
				for (int y = 0; y < VolcanoFloor.mapHeight; y++)
				{
					if (this.GetPixelType(x, y) != match)
					{
						continue;
					}
					int value = this.GetNeighborValue(x, y, match, is_lava_pool);
					if (fill_center || value != 15)
					{
						Dictionary<int, Point> blob_lookup = this.GetBlobLookup();
						if (is_lava_pool)
						{
							blob_lookup = this.GetLavaBlobLookup();
						}
						if (blob_lookup.TryGetValue(value, out var offset))
						{
							//this.SetTile(this.buildingsLayer, x, y, VolcanoDungeon.GetTileIndex(tile_x + offset.X, tile_y + offset.Y));
							blockedTiles.Add(new Point(x, y));
							AddToBuildingTile(new Point(x, y));
						}
					}
				}
			}
		}

		public bool isMushroomLevel()
		{
			if (this.layout >= 32)
			{
				return this.layout <= 34;
			}
			return false;
		}

		private void adjustLevelChances(ref double stoneChance, ref double monsterChance, ref double itemChance, ref double gemStoneChance)
		{
			if (this.level == 0 || this.level == 5)
			{
				monsterChance = 0.0;
				itemChance = 0.0;
				gemStoneChance = 0.0;
				stoneChance = 0.0;
			}
			if (this.isMushroomLevel())
			{
				monsterChance = 0.025;
				itemChance *= 35.0;
				stoneChance = 0.0;
			}
			else if (this.isMonsterLevel())
			{
				stoneChance = 0.0;
				itemChance = 0.0;
				monsterChance *= 2.0;
			}
			bool has_avoid_monsters_buff = false;
			bool has_spawn_monsters_buff = Volcano.monsterMuskActive;
			/*foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
			{
				if (onlineFarmer.hasBuff("23"))
				{
					has_avoid_monsters_buff = true;
				}
				if (onlineFarmer.hasBuff("24"))
				{
					has_spawn_monsters_buff = true;
				}
				if (has_spawn_monsters_buff && has_avoid_monsters_buff)
				{
					break;
				}
			}*/
			if (has_spawn_monsters_buff)
			{
				monsterChance *= 2.0;
			}
			gemStoneChance /= 2.0;
		}
		public bool isTileOnMap(int x, int y)
		{
			if (x >= 0 && x < VolcanoFloor.mapWidth && y >= 0)
			{
				return y < VolcanoFloor.mapHeight;
			}
			return false;
		}


		public virtual void GenerateEntities()
		{
			// List<Point> spawn_points = new List<Point>();
			// this.ApplyToPixelType(PixelType.SpawnTile, delegate (int x, int y)
			// {
			// 	spawn_points.Add(new Point(x, y));
			// });
			// List<Point> spiker_spawn_points = new List<Point>();
			// this.ApplyToPixelType(PixelType.SpikerSpawnTile, delegate (int x, int y)
			// {
			// 	spiker_spawn_points.Add(new Point(x, y));
			// });
			double stoneChance = (double)this.generationRandom.Next(11, 18) / 150.0;
			double monsterChance = 0.0008 + (double)this.generationRandom.Next(70) / 10000.0;
			double itemChance = 0.001;
			double gemStoneChance = 0.003;
			this.adjustLevelChances(ref stoneChance, ref monsterChance, ref itemChance, ref gemStoneChance);
			if (this.level > 0 && this.level != 5 && (this.generationRandom.NextBool() || this.isMushroomLevel()))
			{
				int numBarrels = this.generationRandom.Next(5) + (int)(Volcano.dailyLuck * 20.0);
				if (this.isMushroomLevel())
				{
					numBarrels += 50;
				}
				for (int i = 0; i < numBarrels; i++)
				{
					Point p;
					Point motion;
					if (this.generationRandom.NextDouble() < 0.33)
					{
						p = new Point(this.generationRandom.Next(VolcanoFloor.mapWidth), 0);
						motion = new Point(0, 1);
					}
					else if (this.generationRandom.NextBool())
					{
						p = new Point(0, this.generationRandom.Next(VolcanoFloor.mapHeight));
						motion = new Point(1, 0);
					}
					else
					{
						p = new Point(VolcanoFloor.mapWidth - 1, this.generationRandom.Next(VolcanoFloor.mapHeight));
						motion = new Point(-1, 0);
					}
					while (isTileOnMap(p.X, p.Y))
					{
						p.X += motion.X;
						p.Y += motion.Y;
						Vector2 objectPos = new Vector2(p.X, p.Y);
						if (GetPixelType(p.X, p.Y) == PixelType.Open)
							//if (this.isTileClearForMineObjects(new Vector2(p.X, p.Y)))
							if (!volcanoChests.ContainsKey(objectPos) && !frontTiles.Contains(p))
							{
								if (this.isMushroomLevel())
								{
									//base.terrainFeatures.Add(objectPos, new CosmeticPlant(6 + this.generationRandom.Next(3)));
									blockedTiles.Add(p);
									this.generationRandom.Next(3);
								}
								else
								{
									barrelLocations.Add(objectPos);
									blockedTiles.Add(p);
									//base.objects.Add(objectPos, BreakableContainer.GetBarrelForVolcanoDungeon(objectPos));
								}
								break;
							}
					}
				}
			}
			if (this.level != 5)
			{
				for (int j = 0; j < VolcanoFloor.mapWidth; j++)
				{
					for (int k = 0; k < VolcanoFloor.mapHeight; k++)
					{
						Vector2 objectPos2 = new Vector2(j, k);
						Point point = new Point(j, k);
						if ((Math.Abs((float)this.startPosition.Value.X - objectPos2.X) <= 5f && Math.Abs((float)this.startPosition.Value.Y - objectPos2.Y) <= 5f) || (Math.Abs((float)this.endPosition.Value.X - objectPos2.X) <= 5f && Math.Abs((float)this.endPosition.Value.Y - objectPos2.Y) <= 5f))
						{
							continue;
						}
						if (GetPixelType(j, k) != PixelType.OutOfBounds)
						{
							if (!blockedTiles.Contains(point) && generationRandom.NextDouble() < monsterChance)
							//if (this.CanItemBePlacedHere(objectPos2) && this.generationRandom.NextDouble() < monsterChance)
							{
								//if (base.getTileIndexAt((int)objectPos2.X, (int)objectPos2.Y, "Back", "dungeon") == 25)
								if (dirtTiles.Contains(point))
								{
									if (!this.isMushroomLevel())
									{
										if (!monsters.ContainsKey(objectPos2))
										{
											monsters.Add(objectPos2, "(O)MagmaDuggy");
											IncrememntMonsterCount("(O)MagmaDuggy");
										}
										//base.characters.Add(new Duggy(objectPos2 * 64f, magmaDuggy: true));
									}
								}
								else if (this.isMushroomLevel())
								{
									//base.characters.Add(new RockCrab(objectPos2 * 64f, "False Magma Cap"));
									monsters.Add(objectPos2, "(O)False Magma Cap");
									IncrememntMonsterCount("(O)False Magma Cap");
								}
								else
								{
									//base.characters.Add(new Bat(objectPos2 * 64f, (this.level.Value > 5 && this.generationRandom.NextBool()) ? (-556) : (-555)));
									string name = "(O)Magma Sprite";
									if (level > 5 && this.generationRandom.NextBool())
									{
										name = "(O)Magma Sparker";
									}
									monsters.Add(objectPos2, name);
									IncrememntMonsterCount(name);
								}
							}
							else
							{
								if (volcanoChests.ContainsKey(objectPos2) || dungeonBackTiles.Contains(point) || dirtAdjacentTiles.Contains(point) || blockedTiles.Contains(point) || dirtTiles.Contains(point) || barrelLocations.Contains(objectPos2) || teethLocations.Contains(objectPos2) || frontTiles.Contains(point))
								{
									continue;
								}
								//if (!this.isTileClearForMineObjects(objectPos2, ignoreRuins: true))
								//{
								//	continue;
								//}
								if (this.GetPixelType((int)point.X, (int)point.Y) == PixelType.PossibleSwitchPosition)
								{
									continue;
								}
								double chance = stoneChance;
								if (chance > 0.0)
								{
									foreach (Vector2 v in Utility.getAdjacentTileLocations(objectPos2))
									{
										if (teethLocations.Contains(v) || barrelLocations.Contains(v) || stones.ContainsKey(v))
										{
											chance += 0.1;
										}
									}
								}
								int stoneIndex = this.chooseStoneTypeIndexOnly(objectPos2);
								bool basicStone = stoneIndex >= 845 && stoneIndex <= 847;
								if (chance > 0.0 && (!basicStone || this.generationRandom.NextDouble() < chance))
								{
									this.createStone(stoneIndex, objectPos2);
								}
								else if (this.generationRandom.NextDouble() < itemChance)
								{
									magmaCapLocations.Add(objectPos2);
									blockedTiles.Add(point);
								}
							}
						}
					}
				}
				while (stoneChance != 0.0 && this.generationRandom.NextDouble() < 0.2)
				{
					this.tryToAddOreClumps();
				}
			}
			// TODO below code.  This is after sprite/sparkers so don't care right now.
			/*
			for (int l = 0; l < 7; l++)
			{
				if (spawn_points.Count == 0)
				{
					break;
				}
				int index = this.generationRandom.Next(0, spawn_points.Count);
				Point spawn_point = spawn_points[index];
				if (this.CanItemBePlacedHere(new Vector2(spawn_point.X, spawn_point.Y)))
				{
					Monster monster = null;
					if (this.generationRandom.NextDouble() <= 0.25)
					{
						for (int m = 0; m < 20; m++)
						{
							Point point = spawn_point;
							point.X += this.generationRandom.Next(-10, 11);
							point.Y += this.generationRandom.Next(-10, 11);
							bool fail = false;
							for (int check_x = -1; check_x <= 1; check_x++)
							{
								for (int check_y = -1; check_y <= 1; check_y++)
								{
									if (!LavaLurk.IsLavaTile(this, point.X + check_x, point.Y + check_y))
									{
										fail = true;
										break;
									}
								}
							}
							if (!fail)
							{
								monster = new LavaLurk(Utility.PointToVector2(point) * 64f);
								break;
							}
						}
					}
					if (monster == null && this.generationRandom.NextDouble() <= 0.20000000298023224)
					{
						monster = new HotHead(Utility.PointToVector2(spawn_point) * 64f);
					}
					if (monster == null)
					{
						GreenSlime greenSlime = new GreenSlime(Utility.PointToVector2(spawn_point) * 64f, 0);
						greenSlime.makeTigerSlime();
						monster = greenSlime;
					}
					if (monster != null)
					{
						base.characters.Add(monster);
					}
				}
				spawn_points.RemoveAt(index);
			}
			foreach (Point p2 in spiker_spawn_points)
			{
				if (this.CanSpawnCharacterHere(new Vector2(p2.X, p2.Y)))
				{
					int direction = 1;
					switch (base.getTileIndexAt(p2, "Back", "dungeon"))
					{
						case 537:
						case 538:
							direction = 2;
							break;
						case 552:
						case 569:
							direction = 3;
							break;
						case 553:
						case 570:
							direction = 0;
							break;
					}
					base.characters.Add(new Spiker(new Vector2(p2.X, p2.Y) * 64f, direction));
				}
			}*/
		}

		public void tryToAddOreClumps()
		{
			if (!(this.generationRandom.NextDouble() < 0.55 + Volcano.dailyLuck))
			{
				return;
			}
			// getRandomTile uses Game1.random
			/*
			Vector2 endPoint = base.getRandomTile();
			Point point = new Point((int)endPoint.X, (int)endPoint.Y);
			for (int tries = 0; tries < 1 || this.generationRandom.NextDouble() < 0.25 + Game1.player.DailyLuck; tries++)
			{
				if (this.CanItemBePlacedHere(endPoint) && /*this.isTileOnClearAndSolidGround(endPoint) && */ /*!dirtTiles.Contains(point))
				{
					Utility.recursiveObjectPlacement(new Object(this.generationRandom.Next(843, 845).ToString(), 1)
					{
						MinutesUntilReady = 12
					}, (int)endPoint.X, (int)endPoint.Y, 0.949999988079071, 0.30000001192092896, this, "Dirt", 0, 0.05000000074505806);
				}
				endPoint = base.getRandomTile();
			}*/
		}

		private void createStone(int stone, Vector2 tile)
		{
			string whichStone = this.chooseStoneTypeIndexOnly(tile).ToString() ?? "";
			int stoneHealth = 1;
			switch (whichStone)
			{
				case "1095382":
					whichStone = "VolcanoCoalNode0";
					stoneHealth = 10;
					break;
				case "845":
				case "846":
				case "847":
					stoneHealth = 6;
					break;
				case "843":
				case "844":
					stoneHealth = 12;
					break;
				case "765":
					stoneHealth = 16;
					break;
				case "764":
					whichStone = "VolcanoGoldNode";
					stoneHealth = 8;
					break;
				case "290":
					stoneHealth = 8;
					break;
				case "751":
					stoneHealth = 8;
					break;
				case "819":
					stoneHealth = 8;
					break;
			}

			stones.Add(tile, "(O)" + whichStone);
			blockedTiles.Add(new Point((int)tile.X, (int)tile.Y));
		}

		private int chooseStoneTypeIndexOnly(Vector2 tile)
		{
			int whichStone = this.generationRandom.Next(845, 848);
			float levelMod = 1f + (float)this.level / 7f;
			float masterMultiplier = 0.8f;
			float luckMultiplier = 1f + (float)Volcano.luckLevel * 0.035f + (float)Volcano.dailyLuck / 2f;
			double chance = 0.008 * (double)levelMod * (double)masterMultiplier * (double)luckMultiplier;
			foreach (Vector2 v in Utility.getAdjacentTileLocations(tile))
			{
				if (stones.TryGetValue(v, out var obj) && (obj == "(O)843" || obj == "(O)844"))
				{
					chance += 0.15;
				}
			}
			if (this.generationRandom.NextDouble() < chance)
			{
				whichStone = this.generationRandom.Next(843, 845);
			}
			else
			{
				chance = 0.0025 * (double)levelMod * (double)masterMultiplier * (double)luckMultiplier;
				foreach (Vector2 v2 in Utility.getAdjacentTileLocations(tile))
				{
					if (stones.TryGetValue(v2, out var obj2) && obj2 == "(O)765")
					{
						chance += 0.1;
					}
				}
				if (this.generationRandom.NextDouble() < chance)
				{
					whichStone = 765;
				}
				else
				{
					chance = 0.01 * (double)levelMod * (double)masterMultiplier;
					foreach (Vector2 v3 in Utility.getAdjacentTileLocations(tile))
					{
						if (stones.TryGetValue(v3, out var obj3) && obj3 == "(O)VolcanoGoldNode")
						{
							chance += 0.2;
						}
					}
					if (this.generationRandom.NextDouble() < chance)
					{
						whichStone = 764;
					}
					else
					{
						chance = 0.012 * (double)levelMod * (double)masterMultiplier;
						foreach (Vector2 v4 in Utility.getAdjacentTileLocations(tile))
						{
							if (stones.TryGetValue(v4, out var obj4) && obj4.StartsWith("(O)VolcanoCoalNode"))
							{
								chance += 0.2;
							}
						}
						if (this.generationRandom.NextDouble() < chance)
						{
							whichStone = 1095382;
						}
						else
						{
							chance = 0.015 * (double)levelMod * (double)masterMultiplier;
							foreach (Vector2 v5 in Utility.getAdjacentTileLocations(tile))
							{
								if (stones.TryGetValue(v5, out var obj5) && obj5 == "(O)850")
								{
									chance += 0.25;
								}
							}
							if (this.generationRandom.NextDouble() < chance)
							{
								whichStone = 850;
							}
							else
							{
								chance = 0.018 * (double)levelMod * (double)masterMultiplier;
								foreach (Vector2 v6 in Utility.getAdjacentTileLocations(tile))
								{
									if (stones.TryGetValue(v6, out var obj6) && obj6 == "(O)849")
									{
										chance += 0.25;
									}
								}
								if (this.generationRandom.NextDouble() < chance)
								{
									whichStone = 849;
								}
							}
						}
					}
				}
			}
			if (this.generationRandom.NextDouble() < 0.0005)
			{
				whichStone = 819;
			}
			if (this.generationRandom.NextDouble() < 0.0007)
			{
				whichStone = 44;
			}
			if (this.level > 2 && this.generationRandom.NextDouble() < 0.0002)
			{
				whichStone = 46;
			}
			return whichStone;
		}

		public virtual void CreateDwarfGate(int gate_index, Point tile_position)
		{
			blockedTiles.Add(new Point(tile_position.X - 1, tile_position.Y + 1));
			blockedTiles.Add(new Point(tile_position.X + 1, tile_position.Y + 1));
			blockedTiles.Add(new Point(tile_position.X - 1, tile_position.Y));
			blockedTiles.Add(new Point(tile_position.X + 1, tile_position.Y));
			AddToBuildingTile(new Point(tile_position.X - 1, tile_position.Y + 1));
			AddToBuildingTile(new Point(tile_position.X + 1, tile_position.Y + 1));
			AddToBuildingTile(new Point(tile_position.X - 1, tile_position.Y));
			AddToBuildingTile(new Point(tile_position.X + 1, tile_position.Y));
			frontTiles.Add(new Point(tile_position.X - 1, tile_position.Y - 1));
			frontTiles.Add(new Point(tile_position.X + 1, tile_position.Y - 1));
			/*this.SetTile(this.backLayer, tile_position.X, tile_position.Y + 1, VolcanoDungeon.GetTileIndex(3, 34));
			this.SetTile(this.buildingsLayer, tile_position.X - 1, tile_position.Y + 1, VolcanoDungeon.GetTileIndex(2, 34));
			this.SetTile(this.buildingsLayer, tile_position.X + 1, tile_position.Y + 1, VolcanoDungeon.GetTileIndex(4, 34));
			this.SetTile(this.buildingsLayer, tile_position.X - 1, tile_position.Y, VolcanoDungeon.GetTileIndex(2, 33));
			this.SetTile(this.buildingsLayer, tile_position.X + 1, tile_position.Y, VolcanoDungeon.GetTileIndex(4, 33));
			this.SetTile(this.frontLayer, tile_position.X - 1, tile_position.Y - 1, VolcanoDungeon.GetTileIndex(2, 32));
			this.SetTile(this.frontLayer, tile_position.X + 1, tile_position.Y - 1, VolcanoDungeon.GetTileIndex(4, 32));
			this.SetTile(this.alwaysFrontLayer, tile_position.X - 1, tile_position.Y - 1, VolcanoDungeon.GetTileIndex(2, 32));
			this.SetTile(this.alwaysFrontLayer, tile_position.X, tile_position.Y - 1, VolcanoDungeon.GetTileIndex(3, 32));
			this.SetTile(this.alwaysFrontLayer, tile_position.X + 1, tile_position.Y - 1, VolcanoDungeon.GetTileIndex(4, 32));
			if (gate_index == 0)
			{
				this.SetTile(this.alwaysFrontLayer, tile_position.X - 1, tile_position.Y - 2, VolcanoDungeon.GetTileIndex(0, 32));
				this.SetTile(this.alwaysFrontLayer, tile_position.X + 1, tile_position.Y - 2, VolcanoDungeon.GetTileIndex(0, 32));
			}
			else
			{
				this.SetTile(this.alwaysFrontLayer, tile_position.X - 1, tile_position.Y - 2, VolcanoDungeon.GetTileIndex(9, 25));
				this.SetTile(this.alwaysFrontLayer, tile_position.X + 1, tile_position.Y - 2, VolcanoDungeon.GetTileIndex(10, 25));
			}*/
			int seed = this.generationRandom.Next();
			Random r = Utility.CreateRandom(seed);
			if (possibleSwitchPositions.TryGetValue(gate_index, out var positions))
			{
				int max_points = Math.Min(positions.Count, 3);
				if (gate_index > 0)
				{
					max_points = 1;
				}
				List<Point> points = new List<Point>(positions);
				Utility.Shuffle(r, points);
				int points_to_choose = r.Next(1, Math.Max(1, max_points));
				points_to_choose = Math.Min(points_to_choose, max_points);
				if (isMonsterLevel())
				{
					points_to_choose = max_points;
				}
				for (int i = 0; i < points_to_choose; i++)
				{
					switches.Add(points[i]);
					//this.switches[points[i]] = false;
				}
			}
			//if (Game1.IsMasterGame)
			//{
			//	DwarfGate gate = new DwarfGate(this, gate_index, tile_position.X, tile_position.Y, seed);
			//	this.dwarfGates.Add(gate);
			//}
		}

		public bool isMonsterLevel()
		{
			if (this.layout >= 35)
			{
				return this.layout <= 37;
			}
			return false;
		}

		public virtual void ErodeInvalidDirtTiles()
		{
			Point[] neighboring_tiles = new Point[8]
			{
				new Point(-1, -1),
				new Point(0, -1),
				new Point(1, -1),
				new Point(-1, 0),
				new Point(1, 0),
				new Point(-1, 1),
				new Point(0, 1),
				new Point(1, 1)
			};
			Dictionary<Point, bool> visited_tiles = new Dictionary<Point, bool>();
			List<Point> dirt_to_remove = new List<Point>();
			foreach (Point dirt_tile in this.dirtTiles)
			{
				bool fail = false;
				foreach (var setPieceArea in setPieceAreas)
				{
					if (setPieceArea.Contains(dirt_tile))
					{
						fail = true;
						break;
					}
				}
				if (!fail && buildingTiles.Contains(dirt_tile))// base.hasTileAt(dirt_tile, "Buildings"))
				{
					fail = true;
				}
				if (!fail)
				{
					Point[] array = neighboring_tiles;
					for (int i = 0; i < array.Length; i++)
					{
						Point offset = array[i];
						Point neighbor = new Point(dirt_tile.X + offset.X, dirt_tile.Y + offset.Y);
						if (visited_tiles.TryGetValue(neighbor, out var prevSucceeded))
						{
							if (!prevSucceeded)
							{
								fail = true;
								break;
							}
						}
						else if (!this.dirtTiles.Contains(neighbor))
						{
							if (!this.GetDirtNeighborTile(neighbor.X, neighbor.Y).HasValue)
							{
								fail = true;
							}
							visited_tiles[neighbor] = !fail;
							if (fail)
							{
								break;
							}
						}
					}
				}
				if (fail)
				{
					dirt_to_remove.Add(dirt_tile);
				}
			}
			foreach (Point remove in dirt_to_remove)
			{
				this.dirtTiles.Remove(remove);
			}
		}

		public Point? GetDirtNeighborTile(int tile_x, int tile_y)
		{
			if (this.GetPixelType(tile_x, tile_y) != PixelType.Open)
			{
				return null;
			}
			if (buildingTiles.Contains(new Point(tile_x, tile_y)))
			{
				return null;
			}
			if (this.dirtTiles.Contains(new Point(tile_x, tile_y - 1)) && this.dirtTiles.Contains(new Point(tile_x, tile_y + 1)))
			{
				return null;
			}
			if (this.dirtTiles.Contains(new Point(tile_x - 1, tile_y)) && this.dirtTiles.Contains(new Point(tile_x + 1, tile_y)))
			{
				return null;
			}
			if (this.dirtTiles.Contains(new Point(tile_x - 1, tile_y)) && !this.dirtTiles.Contains(new Point(tile_x + 1, tile_y)))
			{
				if (this.dirtTiles.Contains(new Point(tile_x, tile_y - 1)))
				{
					return new Point(3, 3);
				}
				if (this.dirtTiles.Contains(new Point(tile_x, tile_y + 1)))
				{
					return new Point(3, 1);
				}
				return new Point(2, 1);
			}
			if (this.dirtTiles.Contains(new Point(tile_x + 1, tile_y)) && !this.dirtTiles.Contains(new Point(tile_x - 1, tile_y)))
			{
				if (this.dirtTiles.Contains(new Point(tile_x, tile_y - 1)))
				{
					return new Point(3, 2);
				}
				if (this.dirtTiles.Contains(new Point(tile_x, tile_y + 1)))
				{
					return new Point(3, 0);
				}
				return new Point(0, 1);
			}
			if (this.dirtTiles.Contains(new Point(tile_x, tile_y - 1)) && !this.dirtTiles.Contains(new Point(tile_x, tile_y + 1)))
			{
				return new Point(1, 2);
			}
			if (this.dirtTiles.Contains(new Point(tile_x, tile_y + 1)) && !this.dirtTiles.Contains(new Point(tile_x, tile_y - 1)))
			{
				return new Point(1, 0);
			}
			if (this.dirtTiles.Contains(new Point(tile_x - 1, tile_y - 1)))
			{
				return new Point(2, 2);
			}
			if (this.dirtTiles.Contains(new Point(tile_x + 1, tile_y - 1)))
			{
				return new Point(0, 2);
			}
			if (this.dirtTiles.Contains(new Point(tile_x - 1, tile_y + 1)))
			{
				return new Point(0, 2);
			}
			if (this.dirtTiles.Contains(new Point(tile_x + 1, tile_y + 1)))
			{
				return new Point(2, 2);
			}
			return null;
		}

		public virtual void GenerateDirtTiles()
		{
			if (this.level == 5)
			{
				return;
			}
			for (int i = 0; i < 8; i++)
			{
				int center_x = this.generationRandom.Next(0, 64);
				int center_y = this.generationRandom.Next(0, 64);
				int travel_distance = this.generationRandom.Next(2, 8);
				int radius = this.generationRandom.Next(1, 3);
				int direction_x = ((this.generationRandom.Next(2) != 0) ? 1 : (-1));
				int direction_y = ((this.generationRandom.Next(2) != 0) ? 1 : (-1));
				bool x_oriented = this.generationRandom.Next(2) == 0;
				for (int j = 0; j < travel_distance; j++)
				{
					for (int x = center_x - radius; x <= center_x + radius; x++)
					{
						for (int y = center_y - radius; y <= center_y + radius; y++)
						{
							if (this.GetPixelType(x, y) == PixelType.Open)
							{
								this.dirtTiles.Add(new Point(x, y));
							}
						}
					}
					if (x_oriented)
					{
						direction_y += ((this.generationRandom.Next(2) != 0) ? 1 : (-1));
					}
					else
					{
						direction_x += ((this.generationRandom.Next(2) != 0) ? 1 : (-1));
					}
					center_x += direction_x;
					center_y += direction_y;
					radius += ((this.generationRandom.Next(2) != 0) ? 1 : (-1));
					if (radius < 1)
					{
						radius = 1;
					}
					if (radius > 4)
					{
						radius = 4;
					}
				}
			}
			for (int k = 0; k < 2; k++)
			{
				this.ErodeInvalidDirtTiles();
			}
			HashSet<Point> visited_neighbors = new HashSet<Point>();
			Point[] neighboring_tiles = new Point[8]
			{
				new Point(-1, -1),
				new Point(0, -1),
				new Point(1, -1),
				new Point(-1, 0),
				new Point(1, 0),
				new Point(-1, 1),
				new Point(0, 1),
				new Point(1, 1)
			};
			foreach (Point point in this.dirtTiles)
			{
				//blockedTiles.Add(point);

				//this.SetTile(this.backLayer, point.X, point.Y, VolcanoDungeon.GetTileIndex(9, 1));
				if (this.generationRandom.NextDouble() < 0.015)
				{
					monsters.Add(new Vector2(point.X, point.Y), "(O)MagmaDuggy");
					IncrememntMonsterCount("(O)MagmaDuggy");
					//base.characters.Add(new Duggy(Utility.PointToVector2(point) * 64f, magmaDuggy: true));
				}
				Point[] array = neighboring_tiles;
				for (int l = 0; l < array.Length; l++)
				{
					Point offset = array[l];
					Point neighbor = new Point(point.X + offset.X, point.Y + offset.Y);
					if (!this.dirtTiles.Contains(neighbor) && !visited_neighbors.Contains(neighbor))
					{
						visited_neighbors.Add(neighbor);
						//Point? neighbor_tile_offset = this.GetDirtNeighborTile(neighbor.X, neighbor.Y);
						//if (neighbor_tile_offset.HasValue)
						//{
						//	this.SetTile(this.backLayer, neighbor.X, neighbor.Y, VolcanoDungeon.GetTileIndex(8 + neighbor_tile_offset.Value.X, neighbor_tile_offset.Value.Y));
						//}
						dirtAdjacentTiles.Add(neighbor);
					}
				}
			}
		}

		public virtual void PlaceSingleWall(int x, int y)
		{
			int index = this.generationRandom.Next(0, 4);
			//this.SetTile(this.frontLayer, x, y - 1, VolcanoDungeon.GetTileIndex(index, 2));
			//this.SetTile(this.buildingsLayer, x, y, VolcanoDungeon.GetTileIndex(index, 3));
			frontTiles.Add(new Point(x, y - 1));
			blockedTiles.Add(new Point(x, y));
			AddToBuildingTile(new Point(x, y));
		}

		public int GetPixelClearance(int x, int y, int wall_height, PixelType match)
		{
			int current_height = 0;
			if (this.GetPixelType(x, y, PixelType.Open) == match)
			{
				current_height++;
				for (int i = 1; i < wall_height; i++)
				{
					if (current_height >= wall_height)
					{
						break;
					}
					if (y + i >= VolcanoFloor.mapHeight)
					{
						return wall_height;
					}
					if (this.GetPixelType(x, y + i, PixelType.Open) != match)
					{
						break;
					}
					current_height++;
				}
				for (int j = 1; j < wall_height; j++)
				{
					if (current_height >= wall_height)
					{
						break;
					}
					if (y - j < 0)
					{
						return wall_height;
					}
					if (this.GetPixelType(x, y - j, PixelType.Open) != match)
					{
						break;
					}
					current_height++;
				}
				return current_height;
			}
			return 0;
		}
		public int GetHeight(int x, int y, int max_height)
		{
			if (x < 0 || x >= VolcanoFloor.mapWidth || y < 0 || y >= VolcanoFloor.mapHeight)
			{
				return max_height + 1;
			}
			return this.heightMap[x + y * VolcanoFloor.mapWidth];
		}


		public virtual void GenerateWalls(PixelType match, int source_x, int source_y, int wall_height = 4, int random_wall_variants = 1, bool start_in_wall = false, Action<int, int> on_insufficient_wall_height = null, bool use_corner_hack = false)
		{
			this.heightMap = new int[VolcanoFloor.mapWidth * VolcanoFloor.mapHeight];
			for (int i = 0; i < this.heightMap.Length; i++)
			{
				this.heightMap[i] = -1;
			}
			for (int pass = 0; pass < 2; pass++)
			{
				for (int x = 0; x < VolcanoFloor.mapWidth; x++)
				{
					int last_y = -1;
					int clearance = 0;
					if (start_in_wall)
					{
						clearance = wall_height;
					}
					for (int current_y = 0; current_y <= VolcanoFloor.mapHeight; current_y++)
					{
						if (this.GetPixelType(x, current_y, match) != match || current_y >= VolcanoFloor.mapHeight)
						{
							int current_height = 0;
							int wall_variant_index = 0;
							if (random_wall_variants > 1 && this.generationRandom.NextBool())
							{
								wall_variant_index = this.generationRandom.Next(1, random_wall_variants);
							}
							if (current_y >= VolcanoFloor.mapHeight)
							{
								current_height = wall_height;
								clearance = wall_height;
							}
							for (int curr_y = current_y - 1; curr_y > last_y; curr_y--)
							{
								if (clearance < wall_height)
								{
									if (on_insufficient_wall_height != null)
									{
										on_insufficient_wall_height(x, curr_y);
									}
									else
									{
										this.SetPixelType(x, curr_y, PixelType.Open);
										this.PlaceSingleWall(x, curr_y);
									}
									current_height--;
								}
								else
								{
									switch (pass)
									{
										case 0:
											if (this.GetPixelClearance(x - 1, curr_y, wall_height, match) < wall_height && this.GetPixelClearance(x + 1, curr_y, wall_height, match) < wall_height)
											{
												if (on_insufficient_wall_height != null)
												{
													on_insufficient_wall_height(x, curr_y);
												}
												else
												{
													this.SetPixelType(x, curr_y, PixelType.Open);
													this.PlaceSingleWall(x, curr_y);
												}
												current_height--;
											}
											break;
										case 1:
											this.heightMap[x + curr_y * VolcanoFloor.mapWidth] = current_height + 1;
											if (current_height < wall_height || wall_height == 0)
											{
												if (wall_height > 0)
												{
													//this.SetTile(this.buildingsLayer, x, curr_y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants + wall_variant_index, source_y + 1 + random_wall_variants + wall_height - current_height - 1));
													blockedTiles.Add(new Point(x, curr_y));
													AddToBuildingTile(new Point(x, curr_y));
												}
											}
											else
											{
												//this.SetTile(this.buildingsLayer, x, curr_y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 3, source_y));
												blockedTiles.Add(new Point(x, curr_y));
												AddToBuildingTile(new Point(x, curr_y));
											}
											break;
									}
								}
								if (current_height < wall_height)
								{
									current_height++;
								}
							}
							last_y = current_y;
							clearance = 0;
						}
						else
						{
							clearance++;
						}
					}
				}
			}
			List<Point> corner_tiles = new List<Point>();
			for (int y = 0; y < VolcanoFloor.mapHeight; y++)
			{
				for (int j = 0; j < VolcanoFloor.mapWidth; j++)
				{
					int height = this.GetHeight(j, y, wall_height);
					int left_height = this.GetHeight(j - 1, y, wall_height);
					int right_height = this.GetHeight(j + 1, y, wall_height);
					int top_height = this.GetHeight(j, y - 1, wall_height);
					int index = this.generationRandom.Next(0, random_wall_variants);
					if (right_height < height)
					{
						if (right_height == wall_height)
						{
							if (use_corner_hack)
							{
								corner_tiles.Add(new Point(j, y));
								//this.SetTile(this.buildingsLayer, j, y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 3, source_y));
								blockedTiles.Add(new Point(j, y));
								AddToBuildingTile(new Point(j, y));
							}
							else
							{
								//this.SetTile(this.buildingsLayer, j, y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 3, source_y + 1));
								blockedTiles.Add(new Point(j, y));
								AddToBuildingTile(new Point(j, y));
							}
						}
						else
						{
							//Layer target_layer = this.buildingsLayer;
							HashSet<Point> target = buildingTiles;
							if (right_height >= 0)
							{
								//this.SetTile(this.buildingsLayer, j, y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants, source_y + 1 + random_wall_variants + wall_height - right_height));
								//target_layer = this.frontLayer;
								target = frontTiles;
								blockedTiles.Add(new Point(j, y));
								AddToBuildingTile(new Point(j, y));
							}
							if (height > wall_height)
							{
								//this.SetTile(target_layer, j, y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 3 - 1, source_y + 1 + index));
								target.Add(new Point(j, y));
							}
							else
							{
								//this.SetTile(target_layer, j, y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 2 + index, source_y + 1 + random_wall_variants * 2 + 1 - height - 1));
								target.Add(new Point(j, y));
							}
							if (wall_height > 0 && y + 1 < VolcanoFloor.mapHeight && right_height == -1 && this.GetHeight(j + 1, y + 1, wall_height) >= 0 && this.GetHeight(j, y + 1, wall_height) >= 0)
							{
								if (use_corner_hack)
								{
									corner_tiles.Add(new Point(j, y));
									//this.SetTile(this.buildingsLayer, j, y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 3, source_y));
									blockedTiles.Add(new Point(j, y));
									AddToBuildingTile(new Point(j, y));
								}
								else
								{
									//this.SetTile(this.frontLayer, j, y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 3, source_y + 2));
									frontTiles.Add(new Point(j, y));
								}
							}
						}
					}
					else if (left_height < height)
					{
						if (left_height == wall_height)
						{
							if (use_corner_hack)
							{
								corner_tiles.Add(new Point(j, y));
								//this.SetTile(this.buildingsLayer, j, y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 3, source_y));
								blockedTiles.Add(new Point(j, y));
								AddToBuildingTile(new Point(j, y));
							}
							else
							{
								//this.SetTile(this.buildingsLayer, j, y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 3 + 1, source_y + 1));
								blockedTiles.Add(new Point(j, y));
								AddToBuildingTile(new Point(j, y));
							}
						}
						else
						{
							//Layer target_layer2 = this.buildingsLayer;
							HashSet<Point> target = buildingTiles;
							if (left_height >= 0)
							{
								//this.SetTile(this.buildingsLayer, j, y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants, source_y + 1 + random_wall_variants + wall_height - left_height));
								//target_layer2 = this.frontLayer;
								target = frontTiles;
								blockedTiles.Add(new Point(j, y));
								AddToBuildingTile(new Point(j, y));
							}
							if (height > wall_height)
							{
								//this.SetTile(target_layer2, j, y, VolcanoDungeon.GetTileIndex(source_x, source_y + 1 + index));
								target.Add(new Point(j, y));
							}
							else
							{
								//this.SetTile(target_layer2, j, y, VolcanoDungeon.GetTileIndex(source_x + index, source_y + 1 + random_wall_variants * 2 + 1 - height - 1));
								target.Add(new Point(j, y));
							}
							if (wall_height > 0 && y + 1 < VolcanoFloor.mapHeight && left_height == -1 && this.GetHeight(j - 1, y + 1, wall_height) >= 0 && this.GetHeight(j, y + 1, wall_height) >= 0)
							{
								if (use_corner_hack)
								{
									corner_tiles.Add(new Point(j, y));
									//this.SetTile(this.buildingsLayer, j, y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 3, source_y));
									blockedTiles.Add(new Point(j, y));
									AddToBuildingTile(new Point(j, y));
								}
								else
								{
									//this.SetTile(this.frontLayer, j, y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 3 + 1, source_y + 2));
									frontTiles.Add(new Point(j, y));
								}
							}
						}
					}
					if (height < 0 || top_height != -1)
					{
						continue;
					}
					if (wall_height > 0)
					{
						if (right_height == -1)
						{
							//this.SetTile(this.frontLayer, j, y - 1, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 2 + index, source_y));
							frontTiles.Add(new Point(j, y - 1));
						}
						else if (left_height == -1)
						{
							//this.SetTile(this.frontLayer, j, y - 1, VolcanoDungeon.GetTileIndex(source_x + index, source_y));
							frontTiles.Add(new Point(j, y - 1));
						}
						else
						{
							//this.SetTile(this.frontLayer, j, y - 1, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants + index, source_y));
							frontTiles.Add(new Point(j, y - 1));
						}
					}
					else if (right_height == -1)
					{
						//this.SetTile(this.buildingsLayer, j, y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 2 + index, source_y));
						blockedTiles.Add(new Point(j, y));
						AddToBuildingTile(new Point(j, y));
					}
					else if (left_height == -1)
					{
						//this.SetTile(this.buildingsLayer, j, y, VolcanoDungeon.GetTileIndex(source_x + index, source_y));
						blockedTiles.Add(new Point(j, y));
						AddToBuildingTile(new Point(j, y));
					}
					else
					{
						//this.SetTile(this.buildingsLayer, j, y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants + index, source_y));
						blockedTiles.Add(new Point(j, y));
						AddToBuildingTile(new Point(j, y));
					}
				}
			}
			if (use_corner_hack)
			{
				foreach (Point corner_tile in corner_tiles)
				{
					if (this.GetHeight(corner_tile.X - 1, corner_tile.Y, wall_height) == -1)
					{
						//this.SetTile(this.frontLayer, corner_tile.X, corner_tile.Y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 3 + 1, source_y + 2));
						frontTiles.Add(new Point(corner_tile.X, corner_tile.Y));
					}
					else if (this.GetHeight(corner_tile.X + 1, corner_tile.Y, wall_height) == -1)
					{
						//this.SetTile(this.frontLayer, corner_tile.X, corner_tile.Y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 3, source_y + 2));
						frontTiles.Add(new Point(corner_tile.X, corner_tile.Y));
					}
					if (this.GetHeight(corner_tile.X - 1, corner_tile.Y, wall_height) == wall_height)
					{
						//this.SetTile(this.alwaysFrontLayer, corner_tile.X, corner_tile.Y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 3 + 1, source_y + 1));
						frontTiles.Add(new Point(corner_tile.X, corner_tile.Y));
					}
					else if (this.GetHeight(corner_tile.X + 1, corner_tile.Y, wall_height) == wall_height)
					{
						//this.SetTile(this.alwaysFrontLayer, corner_tile.X, corner_tile.Y, VolcanoDungeon.GetTileIndex(source_x + random_wall_variants * 3, source_y + 1));
						frontTiles.Add(new Point(corner_tile.X, corner_tile.Y));
					}
				}
			}
			this.heightMap = null;
		}

		public Item ChestItem(Random chest_random, bool upgraded)
		{
			if (!upgraded)
			{
				int random_count2 = 7;
				int random2 = chest_random.Next(random_count2);
				if (!Volcano.crackedGoldenCoconut)
				{
					while (random2 == 1)
					{
						random2 = chest_random.Next(random_count2);
					}
				}
				/*if (Game1.random.NextBool() && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
				{
					int num2 = chest_random.Next(2, 6);
					for (int l = 0; l < num2; l++)
					{
						items.Add(ItemRegistry.Create("(O)890"));
					}
				}*/
				switch (random2)
				{
					case 0:
						{
							Item item = Item.Get("(O)848");
							item.Stack = 3;
							return item;
							//for (int num3 = 0; num3 < 3; num3++)
							//{
							//	items.Add(ItemRegistry.Create("(O)848"));
							//}
							break;
						}
					case 1:
						{
							Item item = Item.Get("(O)791");
							return item;
							//items.Add(ItemRegistry.Create("(O)791"));
							break;
						}
					case 2:
						{
							Item item = Item.Get("(O)831");
							item.Stack = 8;
							return item;
							//for (int n = 0; n < 8; n++)
							//{
							//	items.Add(ItemRegistry.Create("(O)831"));
							//}
							break;
						}
					case 3:
						{
							Item item = Item.Get("(O)833");
							item.Stack = 5;
							return item;
							//for (int m = 0; m < 5; m++)
							//{
							//	items.Add(ItemRegistry.Create("(O)833"));
							//}
							break;
						}
					case 4:
						{
							Item item = Item.Get("(O)861");
							return item;
							//items.Add(new Ring("861"));
							break;
						}
					case 5:
						{
							Item item = Item.Get("(O)862");
							return item;
							//items.Add(new Ring("862"));
							break;
						}
					default:
						{
							MeleeWeapon weapon = MeleeWeapon.GetWeapon("(W)" + chest_random.Next(54, 57));
							weapon.attemptAddRandomInnateEnchantment(chest_random);
							return weapon;

							//items.Add(MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)" + chest_random.Next(54, 57)), chest_random));
							break;
						}
				}
			}
			else
			{
				int random_count = 9;
				int random = chest_random.Next(random_count);
				if (!Volcano.crackedGoldenCoconut)
				{
					while (random == 3)
					{
						random = chest_random.Next(random_count);
					}
				}
				/*if (Game1.random.NextDouble() <= 1.0 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
				{
					int num = chest_random.Next(4, 6);
					for (int i = 0; i < num; i++)
					{
						items.Add(ItemRegistry.Create("(O)890"));
					}
				}*/
				switch (random)
				{
					case 0:
						{
							Item item = Item.Get("(O)848");
							item.Stack = 10;
							return item;
							//{
							//	for (int k = 0; k < 10; k++)
							//	{
							//		items.Add(ItemRegistry.Create("(O)848"));
							//	}
							//	break;
							//}
						}
					case 1:
						{
							Item item = Item.Get("(B)854");
							return item;
							//items.Add(ItemRegistry.Create("(B)854"));
							//break;
						}
					case 2:
						{
							Item item = Item.Get("(B)855");
							return item;
							//items.Add(ItemRegistry.Create("(B)855"));
							//break;
						}
					case 3:
						{
							Item item = Item.Get("(O)791");
							item.Stack = 3;
							return item;
							//for (int j = 0; j < 3; j++)
							//{
							//	items.Add(ItemRegistry.Create<Object>("(O)791"));
							//}
							//break;
						}
					case 4:
						{
							Item item = Item.Get("(O)863");
							return item;
							//items.Add(new Ring("863"));
							//break;
						}
					case 5:
						{
							Item item = Item.Get("(O)860");
							return item;
							//items.Add(new Ring("860"));
							//break;
						}
					case 6:
						{
							MeleeWeapon weapon = MeleeWeapon.GetWeapon("(W)" + chest_random.Next(57, 60));
							weapon.attemptAddRandomInnateEnchantment(chest_random);
							return weapon;
							//items.Add(MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)" + chest_random.Next(57, 60)), chest_random));
							//break;
						}
					case 7:
						{
							Item item = Item.Get("(H)76");
							return item;
							//items.Add(ItemRegistry.Create("(H)76"));
							//break;
						}
					default:
						{
							Item item = Item.Get("(O)289");
							return item;
							//items.Add(ItemRegistry.Create("(O)289"));
							//break;
						}
				}
			}
		}

		public virtual void SpawnChest(int tile_x, int tile_y)
		{

			int chestRandomSeed = generationRandom.Next();
			Random chest_random = Utility.CreateRandom(chestRandomSeed);
			double extraRare_luckboost = Volcano.luckLevel * 0.035f + Volcano.dailyLuck / 2f;
			//if (Game1.IsMasterGame)
			{
				//Vector2 position = new Vector2(tile_x, tile_y);
				//Chest chest = new Chest(playerChest: false, position);
				//chest.dropContents.Value = true;
				//chest.synchronized.Value = true;
				//chest.type.Value = "interactive";
				double result = chest_random.NextDouble();
				bool isUpgraded = result < (double)((this.level == 9) ? (0.5f + extraRare_luckboost) : (0.1f + extraRare_luckboost));
				double luckRequired = result - ((level == 9) ? 0.5 : 0.1);

				Item basicItem = ChestItem(chest_random, false);

				chest_random = Utility.CreateRandom(chestRandomSeed);
				chest_random.NextDouble();

				Item upgradedItem = ChestItem(chest_random, true);

				VolcanoChest chest = new VolcanoChest(isUpgraded, luckRequired, basicItem, upgradedItem, new Vector2(tile_x, tile_y));
				volcanoChests.Add(chest.location, chest);

				/*if (chest_random.NextDouble() < (double)((this.level == 9) ? (0.5f + extraRare_luckboost) : (0.1f + extraRare_luckboost)))
				{
					chest.SetBigCraftableSpriteIndex(227);
					this.PopulateChest(chest.Items, chest_random, 1);
				}
				else
				{
					chest.SetBigCraftableSpriteIndex(223);
					this.PopulateChest(chest.Items, chest_random, 0);
				}
				base.setObject(position, chest);*/
			}
		}
		public static int GetTileIndex(int x, int y)
		{
			return x + y * 16;
		}

		private PixelType GetPixelType(int x, int y, PixelType default_type = PixelType.OutOfBounds)
		{
			return this.pixelMap.GetPixel(x, y, default_type);
		}

		private void SetPixelType(int x, int y, PixelType type)
		{
			this.pixelMap.SetPixel(x, y, type);
		}

		public virtual void ApplyToPixelType(PixelType type, Action<int, int> action)
		{
			for (int x = 0; x < VolcanoFloor.mapWidth; x++)
			{
				for (int y = 0; y < VolcanoFloor.mapHeight; y++)
				{
					if (this.GetPixelType(x, y, type) == type)
					{
						action?.Invoke(x, y);
					}
				}
			}
		}

		private void ApplySetPieces()
		{
			for (int i = 0; i < setPieceAreas.Count; i++)
			{
				Rectangle rectangle = setPieceAreas[i];
				Map override_map = Volcano.SetLayouts3;
				int size = 3;
				if (rectangle.Width >= 32)
				{
					size = 32;
					override_map = Volcano.SetLayouts32;
				}
				else if (rectangle.Width >= 16)
				{
					size = 16;
					override_map = Volcano.SetLayouts16;
				}
				else if (rectangle.Width >= 8)
				{
					size = 8;
					override_map = Volcano.SetLayouts8;
				}
				else if (rectangle.Width >= 4)
				{
					size = 4;
					override_map = Volcano.SetLayouts4;
				}
				//Map override_map = JsonConvert.DeserializeObject<Map>(System.IO.File.ReadAllText($@"Volcano/Volcano_SetPieces_{size}.json"));
				int cols = override_map.Layers[0].Width / size;
				int rows = override_map.Layers[0].Height / size;
				int selected_col = this.generationRandom.Next(0, cols);
				int selected_row = this.generationRandom.Next(0, rows);
				Layer paths_layer = override_map.FindLayer("Paths");
				Layer building_layer = override_map.FindLayer("Buildings");
				Layer front_layer = override_map.FindLayer("Front");
				Layer back_layer = override_map.FindLayer("Back");

				SetPieces.Add(new SetPiece(size, new Vector2(selected_col, selected_row), new Vector2(rectangle.Left, rectangle.Top)));

				if (paths_layer == null)
				{
					continue;
				}
				for (int x = 0; x < size; x++)
				{
					for (int y = 0; y <= size; y++)
					{
						int source_x = selected_col * size + x;
						int source_y = selected_row * size + y;

						int dest_x = rectangle.Left + x;
						int dest_y = rectangle.Top + y;

						if (y < size && building_layer.GetTileIndex(source_x, source_y) != 0)
						{
							blockedTiles.Add(new Point(dest_x, dest_y));
							AddToBuildingTile(new Point(dest_x, dest_y));
						}
						if (y < size && front_layer.GetTileIndex(source_x, source_y) != 0)
						{
							frontTiles.Add(new Point(dest_x, dest_y));
						}
						if (y < size && back_layer.GetTileIndex(source_x, source_y) != 0)
						{
							dungeonBackTiles.Add(new Point(dest_x, dest_y));
						}
						if (!paths_layer.IsValidTileLocation(source_x, source_y))
						{
							continue;
						}
						//Tile tile = paths_layer.Tiles[source_x, source_y];
						int path_index = paths_layer.GetTileIndex(source_x, source_y) - 1;
						//tile?.TileIndex ?? (-1);
						if (path_index >= GetTileIndex(10, 14) && path_index <= GetTileIndex(15, 14))
						{
							int index = path_index - GetTileIndex(10, 14);
							if (index > 0)
							{
								index += i * 10;
							}
							double chance = 1.0;
							if (source_x == 5 && source_y == 16)
							{
								chance = 0.5;
							}
							if (this.generationRandom.NextDouble() < chance)
							{
								this.AddPossibleGateLocation(index, dest_x, dest_y);
							}
						}
						else if (path_index >= GetTileIndex(10, 15) && path_index <= GetTileIndex(15, 15))
						{
							int index2 = path_index - GetTileIndex(10, 15);
							if (index2 > 0)
							{
								index2 += i * 10;
							}
							this.AddPossibleSwitchLocation(index2, dest_x, dest_y);
						}
						else if (path_index == GetTileIndex(10, 20))
						{
							this.SetPixelType(dest_x, dest_y, PixelType.SpawnTile);
						}
						else if (path_index == GetTileIndex(11, 20))
						{
							this.SetPixelType(dest_x, dest_y, PixelType.Lava);
						}
						else if (path_index == GetTileIndex(12, 20))
						{
							this.SpawnChest(dest_x, dest_y);
						}
						else if (path_index == GetTileIndex(13, 20))
						{
							this.SetPixelType(dest_x, dest_y, PixelType.OutOfBounds);
						}
						else if (path_index == GetTileIndex(14, 20) && this.generationRandom.NextBool())
						{
							//if (Game1.IsMasterGame)
							{
								barrelLocations.Add(new Vector2(dest_x, dest_y));
								blockedTiles.Add(new Point(dest_x, dest_y));
								//base.objects.Add(new Vector2(dest_x, dest_y), BreakableContainer.GetBarrelForVolcanoDungeon(new Vector2(dest_x, dest_y)));
							}
						}
						else if (path_index == GetTileIndex(15, 20) && this.generationRandom.NextBool())
						{
							//if (Game1.IsMasterGame)
							{

								Vector2 objTile = new Vector2(dest_x, dest_y);
								teethLocations.Add(objTile);
								blockedTiles.Add(new Point(dest_x, dest_y));
								//base.objects.Add(objTile, new Object("852", 1)
								//{
								//	IsSpawnedObject = true,
								//	CanBeGrabbed = true
								//});
							}
						}
						else if (path_index == GetTileIndex(10, 21))
						{
							this.SetPixelType(dest_x, dest_y, PixelType.SpikerSpawnTile);
						}
					}
				}
			}
		}
		private void PopulateSetPieces()
		{
			switch (layout)
			{
				case 1:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(3, 3, 8, 8));
						setPieceAreas.Add(new Rectangle(8, 27, 3, 3));
						setPieceAreas.Add(new Rectangle(11, 18, 4, 4));
						setPieceAreas.Add(new Rectangle(20, 18, 3, 3));
						setPieceAreas.Add(new Rectangle(21, 35, 3, 3));
						setPieceAreas.Add(new Rectangle(23, 47, 3, 3));
						setPieceAreas.Add(new Rectangle(35, 31, 3, 3));
						setPieceAreas.Add(new Rectangle(41, 47, 3, 3));
						setPieceAreas.Add(new Rectangle(45, 48, 3, 3));
						setPieceAreas.Add(new Rectangle(46, 14, 4, 4));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(14, 14, 4, 4));
						setPieceAreas.Add(new Rectangle(16, 48, 3, 3));
						setPieceAreas.Add(new Rectangle(20, 47, 3, 3));
						setPieceAreas.Add(new Rectangle(26, 31, 3, 3));
						setPieceAreas.Add(new Rectangle(38, 47, 3, 3));
						setPieceAreas.Add(new Rectangle(40, 35, 3, 3));
						setPieceAreas.Add(new Rectangle(41, 18, 3, 3));
						setPieceAreas.Add(new Rectangle(49, 18, 4, 4));
						setPieceAreas.Add(new Rectangle(53, 3, 8, 8));
						setPieceAreas.Add(new Rectangle(53, 27, 3, 3));

					}

					break;


				case 2:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(5, 38, 4, 4));
						setPieceAreas.Add(new Rectangle(16, 15, 3, 3));
						setPieceAreas.Add(new Rectangle(23, 31, 5, 5));
						setPieceAreas.Add(new Rectangle(24, 18, 3, 3));
						setPieceAreas.Add(new Rectangle(35, 35, 5, 5));
						setPieceAreas.Add(new Rectangle(40, 5, 8, 8));
						setPieceAreas.Add(new Rectangle(43, 14, 16, 16));
						setPieceAreas.Add(new Rectangle(47, 31, 3, 3));

					}
					else
					{

						setPieceAreas.Add(new Rectangle(5, 14, 16, 16));
						setPieceAreas.Add(new Rectangle(14, 31, 3, 3));
						setPieceAreas.Add(new Rectangle(16, 5, 8, 8));
						setPieceAreas.Add(new Rectangle(24, 35, 5, 5));
						setPieceAreas.Add(new Rectangle(36, 31, 5, 5));
						setPieceAreas.Add(new Rectangle(37, 18, 3, 3));
						setPieceAreas.Add(new Rectangle(45, 15, 3, 3));
						setPieceAreas.Add(new Rectangle(55, 38, 4, 4));
					}
					break;
				case 3:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(19, 26, 16, 16));
						setPieceAreas.Add(new Rectangle(41, 28, 16, 16));

					}
					else
					{
						setPieceAreas.Add(new Rectangle(7, 28, 16, 16));
						setPieceAreas.Add(new Rectangle(29, 26, 16, 16));
					}
					break;
				case 4:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(13, 40, 4, 4));
						setPieceAreas.Add(new Rectangle(18, 22, 16, 16));
						setPieceAreas.Add(new Rectangle(38, 44, 4, 4));

					}
					else
					{
						setPieceAreas.Add(new Rectangle(22, 44, 4, 4));
						setPieceAreas.Add(new Rectangle(30, 22, 16, 16));
						setPieceAreas.Add(new Rectangle(47, 40, 4, 4));
					}
					break;
				case 5:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(22, 26, 4, 4));
						setPieceAreas.Add(new Rectangle(23, 47, 4, 4));
						setPieceAreas.Add(new Rectangle(38, 48, 8, 8));
						setPieceAreas.Add(new Rectangle(39, 17, 8, 8));
						setPieceAreas.Add(new Rectangle(41, 42, 4, 4));
						setPieceAreas.Add(new Rectangle(50, 41, 4, 4));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(10, 41, 4, 4));
						setPieceAreas.Add(new Rectangle(17, 17, 8, 8));
						setPieceAreas.Add(new Rectangle(18, 48, 8, 8));
						setPieceAreas.Add(new Rectangle(19, 42, 4, 4));
						setPieceAreas.Add(new Rectangle(37, 47, 4, 4));
						setPieceAreas.Add(new Rectangle(38, 26, 4, 4));
					}
					break;
				case 6:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(10, 30, 4, 4));
						setPieceAreas.Add(new Rectangle(35, 32, 4, 4));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(25, 32, 4, 4));
						setPieceAreas.Add(new Rectangle(50, 30, 4, 4));
					}
					break;
				case 7:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(22, 30, 4, 4));
						setPieceAreas.Add(new Rectangle(28, 22, 4, 4));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(32, 22, 4, 4));
						setPieceAreas.Add(new Rectangle(38, 30, 4, 4));
					}
					break;
				case 8:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(17, 24, 4, 4));
						setPieceAreas.Add(new Rectangle(26, 32, 4, 4));
						setPieceAreas.Add(new Rectangle(38, 12, 4, 4));
						setPieceAreas.Add(new Rectangle(39, 30, 4, 4));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(21, 30, 4, 4));
						setPieceAreas.Add(new Rectangle(22, 12, 4, 4));
						setPieceAreas.Add(new Rectangle(34, 32, 4, 4));
						setPieceAreas.Add(new Rectangle(43, 24, 4, 4));
					}
					break;
				case 9:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(24, 14, 16, 16));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(24, 14, 16, 16));
					}
					break;
				case 10:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(10, 30, 4, 4));
						setPieceAreas.Add(new Rectangle(19, 48, 3, 3));
						setPieceAreas.Add(new Rectangle(23, 46, 4, 4));
						setPieceAreas.Add(new Rectangle(24, 29, 8, 8));
						setPieceAreas.Add(new Rectangle(27, 9, 8, 8));
						setPieceAreas.Add(new Rectangle(39, 33, 16, 16));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(9, 33, 16, 16));
						setPieceAreas.Add(new Rectangle(29, 9, 8, 8));
						setPieceAreas.Add(new Rectangle(32, 29, 8, 8));
						setPieceAreas.Add(new Rectangle(37, 46, 4, 4));
						setPieceAreas.Add(new Rectangle(42, 48, 3, 3));
						setPieceAreas.Add(new Rectangle(50, 30, 4, 4));
					}
					break;
				case 11:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(3, 11, 3, 3));
						setPieceAreas.Add(new Rectangle(7, 4, 16, 16));
						setPieceAreas.Add(new Rectangle(11, 48, 3, 3));
						setPieceAreas.Add(new Rectangle(13, 53, 3, 3));
						setPieceAreas.Add(new Rectangle(15, 46, 4, 4));
						setPieceAreas.Add(new Rectangle(17, 51, 4, 4));
						setPieceAreas.Add(new Rectangle(24, 9, 3, 3));
						setPieceAreas.Add(new Rectangle(28, 22, 8, 8));
						setPieceAreas.Add(new Rectangle(37, 22, 4, 4));
						setPieceAreas.Add(new Rectangle(42, 42, 8, 8));
						setPieceAreas.Add(new Rectangle(45, 9, 3, 3));
						setPieceAreas.Add(new Rectangle(52, 17, 4, 4));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(8, 17, 4, 4));
						setPieceAreas.Add(new Rectangle(14, 42, 8, 8));
						setPieceAreas.Add(new Rectangle(16, 9, 3, 3));
						setPieceAreas.Add(new Rectangle(23, 22, 4, 4));
						setPieceAreas.Add(new Rectangle(28, 22, 8, 8));
						setPieceAreas.Add(new Rectangle(37, 9, 3, 3));
						setPieceAreas.Add(new Rectangle(41, 4, 16, 16));
						setPieceAreas.Add(new Rectangle(43, 51, 4, 4));
						setPieceAreas.Add(new Rectangle(45, 46, 4, 4));
						setPieceAreas.Add(new Rectangle(48, 53, 3, 3));
						setPieceAreas.Add(new Rectangle(50, 48, 3, 3));
						setPieceAreas.Add(new Rectangle(58, 11, 3, 3));
					}
					break;
				case 12:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(16, 21, 4, 4));
						setPieceAreas.Add(new Rectangle(22, 15, 16, 16));
						setPieceAreas.Add(new Rectangle(23, 44, 4, 4));
						setPieceAreas.Add(new Rectangle(30, 47, 3, 3));
						setPieceAreas.Add(new Rectangle(33, 36, 8, 8));
						setPieceAreas.Add(new Rectangle(39, 20, 3, 3));
						setPieceAreas.Add(new Rectangle(41, 24, 3, 3));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(20, 24, 3, 3));
						setPieceAreas.Add(new Rectangle(22, 20, 3, 3));
						setPieceAreas.Add(new Rectangle(23, 36, 8, 8));
						setPieceAreas.Add(new Rectangle(26, 15, 16, 16));
						setPieceAreas.Add(new Rectangle(31, 47, 3, 3));
						setPieceAreas.Add(new Rectangle(37, 44, 4, 4));
						setPieceAreas.Add(new Rectangle(44, 21, 4, 4));
					}
					break;
				case 13:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(10, 13, 16, 16));
						setPieceAreas.Add(new Rectangle(20, 40, 8, 8));
						setPieceAreas.Add(new Rectangle(29, 40, 4, 4));
						setPieceAreas.Add(new Rectangle(31, 22, 3, 3));
						setPieceAreas.Add(new Rectangle(32, 46, 4, 4));
						setPieceAreas.Add(new Rectangle(37, 40, 8, 8));
						setPieceAreas.Add(new Rectangle(39, 13, 16, 16));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(9, 13, 16, 16));
						setPieceAreas.Add(new Rectangle(19, 40, 8, 8));
						setPieceAreas.Add(new Rectangle(28, 46, 4, 4));
						setPieceAreas.Add(new Rectangle(30, 22, 3, 3));
						setPieceAreas.Add(new Rectangle(31, 40, 4, 4));
						setPieceAreas.Add(new Rectangle(36, 40, 8, 8));
						setPieceAreas.Add(new Rectangle(38, 13, 16, 16));
					}
					break;
				case 14:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(7, 23, 8, 8));
						setPieceAreas.Add(new Rectangle(12, 51, 3, 3));
						setPieceAreas.Add(new Rectangle(15, 42, 8, 8));
						setPieceAreas.Add(new Rectangle(16, 28, 3, 3));
						setPieceAreas.Add(new Rectangle(17, 22, 4, 4));
						setPieceAreas.Add(new Rectangle(26, 14, 8, 8));
						setPieceAreas.Add(new Rectangle(38, 43, 8, 8));
						setPieceAreas.Add(new Rectangle(41, 37, 4, 4));
						setPieceAreas.Add(new Rectangle(42, 23, 8, 8));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(14, 23, 8, 8));
						setPieceAreas.Add(new Rectangle(18, 43, 8, 8));
						setPieceAreas.Add(new Rectangle(19, 37, 4, 4));
						setPieceAreas.Add(new Rectangle(30, 14, 8, 8));
						setPieceAreas.Add(new Rectangle(41, 42, 8, 8));
						setPieceAreas.Add(new Rectangle(43, 22, 4, 4));
						setPieceAreas.Add(new Rectangle(45, 28, 3, 3));
						setPieceAreas.Add(new Rectangle(49, 23, 8, 8));
						setPieceAreas.Add(new Rectangle(49, 51, 3, 3));
					}
					break;
				case 15:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(6, 7, 8, 8));
						setPieceAreas.Add(new Rectangle(7, 16, 4, 4));
						setPieceAreas.Add(new Rectangle(15, 11, 4, 4));
						setPieceAreas.Add(new Rectangle(15, 18, 3, 3));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(45, 11, 4, 4));
						setPieceAreas.Add(new Rectangle(46, 18, 3, 3));
						setPieceAreas.Add(new Rectangle(50, 7, 8, 8));
						setPieceAreas.Add(new Rectangle(53, 16, 4, 4));
					}
					break;
				case 16:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(5, 39, 3, 3));
						setPieceAreas.Add(new Rectangle(11, 31, 4, 4));
						setPieceAreas.Add(new Rectangle(16, 8, 3, 3));
						setPieceAreas.Add(new Rectangle(18, 56, 3, 3));
						setPieceAreas.Add(new Rectangle(21, 4, 4, 4));
						setPieceAreas.Add(new Rectangle(23, 23, 16, 16));
						setPieceAreas.Add(new Rectangle(29, 5, 3, 3));
						setPieceAreas.Add(new Rectangle(32, 46, 4, 4));
						setPieceAreas.Add(new Rectangle(43, 13, 4, 4));
						setPieceAreas.Add(new Rectangle(45, 29, 3, 3));
						setPieceAreas.Add(new Rectangle(53, 37, 8, 8));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(3, 37, 8, 8));
						setPieceAreas.Add(new Rectangle(16, 29, 3, 3));
						setPieceAreas.Add(new Rectangle(17, 13, 4, 4));
						setPieceAreas.Add(new Rectangle(25, 23, 16, 16));
						setPieceAreas.Add(new Rectangle(28, 46, 4, 4));
						setPieceAreas.Add(new Rectangle(32, 5, 3, 3));
						setPieceAreas.Add(new Rectangle(39, 4, 4, 4));
						setPieceAreas.Add(new Rectangle(43, 56, 3, 3));
						setPieceAreas.Add(new Rectangle(45, 8, 3, 3));
						setPieceAreas.Add(new Rectangle(49, 31, 4, 4));
						setPieceAreas.Add(new Rectangle(56, 39, 3, 3));
					}
					break;
				case 17:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(5, 8, 8, 8));
						setPieceAreas.Add(new Rectangle(9, 21, 3, 3));
						setPieceAreas.Add(new Rectangle(16, 37, 3, 3));
						setPieceAreas.Add(new Rectangle(21, 45, 3, 3));
						setPieceAreas.Add(new Rectangle(22, 25, 3, 3));
						setPieceAreas.Add(new Rectangle(30, 32, 4, 4));
						setPieceAreas.Add(new Rectangle(38, 14, 4, 4));
						setPieceAreas.Add(new Rectangle(39, 20, 4, 4));
						setPieceAreas.Add(new Rectangle(47, 11, 3, 3));
						setPieceAreas.Add(new Rectangle(48, 42, 4, 4));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(12, 42, 4, 4));
						setPieceAreas.Add(new Rectangle(14, 11, 3, 3));
						setPieceAreas.Add(new Rectangle(21, 20, 4, 4));
						setPieceAreas.Add(new Rectangle(22, 14, 4, 4));
						setPieceAreas.Add(new Rectangle(30, 32, 4, 4));
						setPieceAreas.Add(new Rectangle(39, 25, 3, 3));
						setPieceAreas.Add(new Rectangle(40, 45, 3, 3));
						setPieceAreas.Add(new Rectangle(45, 37, 3, 3));
						setPieceAreas.Add(new Rectangle(51, 8, 8, 8));
						setPieceAreas.Add(new Rectangle(52, 21, 3, 3));
					}
					break;
				case 18:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(11, 28, 3, 3));
						setPieceAreas.Add(new Rectangle(22, 35, 3, 3));
						setPieceAreas.Add(new Rectangle(24, 50, 4, 4));
						setPieceAreas.Add(new Rectangle(25, 16, 3, 3));
						setPieceAreas.Add(new Rectangle(28, 26, 4, 4));
						setPieceAreas.Add(new Rectangle(29, 15, 4, 4));
						setPieceAreas.Add(new Rectangle(34, 16, 3, 3));
						setPieceAreas.Add(new Rectangle(35, 50, 4, 4));
						setPieceAreas.Add(new Rectangle(48, 30, 3, 3));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(13, 30, 3, 3));
						setPieceAreas.Add(new Rectangle(25, 50, 4, 4));
						setPieceAreas.Add(new Rectangle(27, 16, 3, 3));
						setPieceAreas.Add(new Rectangle(31, 15, 4, 4));
						setPieceAreas.Add(new Rectangle(32, 26, 4, 4));
						setPieceAreas.Add(new Rectangle(36, 16, 3, 3));
						setPieceAreas.Add(new Rectangle(36, 50, 4, 4));
						setPieceAreas.Add(new Rectangle(39, 35, 3, 3));
						setPieceAreas.Add(new Rectangle(50, 28, 3, 3));
					}
					break;
				case 19:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(25, 39, 4, 4));
						setPieceAreas.Add(new Rectangle(38, 40, 4, 4));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(22, 40, 4, 4));
						setPieceAreas.Add(new Rectangle(35, 39, 4, 4));
					}
					break;
				case 20:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(18, 23, 8, 8));
						setPieceAreas.Add(new Rectangle(28, 38, 4, 4));
						setPieceAreas.Add(new Rectangle(28, 43, 4, 4));
						setPieceAreas.Add(new Rectangle(33, 38, 4, 4));
						setPieceAreas.Add(new Rectangle(33, 43, 4, 4));
						setPieceAreas.Add(new Rectangle(42, 20, 3, 3));
						setPieceAreas.Add(new Rectangle(42, 25, 3, 3));
						setPieceAreas.Add(new Rectangle(47, 20, 3, 3));
						setPieceAreas.Add(new Rectangle(47, 25, 3, 3));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(14, 20, 3, 3));
						setPieceAreas.Add(new Rectangle(14, 25, 3, 3));
						setPieceAreas.Add(new Rectangle(19, 20, 3, 3));
						setPieceAreas.Add(new Rectangle(19, 25, 3, 3));
						setPieceAreas.Add(new Rectangle(27, 38, 4, 4));
						setPieceAreas.Add(new Rectangle(27, 43, 4, 4));
						setPieceAreas.Add(new Rectangle(32, 38, 4, 4));
						setPieceAreas.Add(new Rectangle(32, 43, 4, 4));
						setPieceAreas.Add(new Rectangle(38, 23, 8, 8));
					}
					break;
				case 21:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(16, 43, 3, 3));
						setPieceAreas.Add(new Rectangle(17, 16, 4, 4));
						setPieceAreas.Add(new Rectangle(19, 26, 8, 8));
						setPieceAreas.Add(new Rectangle(21, 36, 3, 3));
						setPieceAreas.Add(new Rectangle(30, 20, 8, 8));
						setPieceAreas.Add(new Rectangle(38, 41, 8, 8));
						setPieceAreas.Add(new Rectangle(40, 11, 4, 4));
						setPieceAreas.Add(new Rectangle(45, 12, 3, 3));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(16, 12, 3, 3));
						setPieceAreas.Add(new Rectangle(18, 41, 8, 8));
						setPieceAreas.Add(new Rectangle(20, 11, 4, 4));
						setPieceAreas.Add(new Rectangle(26, 20, 8, 8));
						setPieceAreas.Add(new Rectangle(37, 26, 8, 8));
						setPieceAreas.Add(new Rectangle(40, 36, 3, 3));
						setPieceAreas.Add(new Rectangle(43, 16, 4, 4));
						setPieceAreas.Add(new Rectangle(45, 43, 3, 3));
					}
					break;
				case 22:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(4, 52, 8, 8));
						setPieceAreas.Add(new Rectangle(14, 20, 8, 8));
						setPieceAreas.Add(new Rectangle(17, 29, 8, 8));
						setPieceAreas.Add(new Rectangle(24, 44, 3, 3));
						setPieceAreas.Add(new Rectangle(29, 50, 3, 3));
						setPieceAreas.Add(new Rectangle(40, 20, 16, 16));
						setPieceAreas.Add(new Rectangle(53, 9, 3, 3));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(8, 9, 3, 3));
						setPieceAreas.Add(new Rectangle(8, 20, 16, 16));
						setPieceAreas.Add(new Rectangle(32, 50, 3, 3));
						setPieceAreas.Add(new Rectangle(37, 44, 3, 3));
						setPieceAreas.Add(new Rectangle(39, 29, 8, 8));
						setPieceAreas.Add(new Rectangle(42, 20, 8, 8));
						setPieceAreas.Add(new Rectangle(52, 52, 8, 8));
					}
					break;
				case 23:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(10, 30, 8, 8));
						setPieceAreas.Add(new Rectangle(21, 36, 4, 4));
						setPieceAreas.Add(new Rectangle(21, 50, 3, 3));
						setPieceAreas.Add(new Rectangle(31, 29, 4, 4));
						setPieceAreas.Add(new Rectangle(32, 52, 3, 3));
						setPieceAreas.Add(new Rectangle(33, 47, 3, 3));
						setPieceAreas.Add(new Rectangle(37, 37, 4, 4));
						setPieceAreas.Add(new Rectangle(43, 29, 8, 8));
						setPieceAreas.Add(new Rectangle(50, 7, 3, 3));
						setPieceAreas.Add(new Rectangle(50, 24, 3, 3));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(11, 7, 3, 3));
						setPieceAreas.Add(new Rectangle(11, 24, 3, 3));
						setPieceAreas.Add(new Rectangle(13, 29, 8, 8));
						setPieceAreas.Add(new Rectangle(23, 37, 4, 4));
						setPieceAreas.Add(new Rectangle(28, 47, 3, 3));
						setPieceAreas.Add(new Rectangle(29, 29, 4, 4));
						setPieceAreas.Add(new Rectangle(29, 52, 3, 3));
						setPieceAreas.Add(new Rectangle(39, 36, 4, 4));
						setPieceAreas.Add(new Rectangle(40, 50, 3, 3));
						setPieceAreas.Add(new Rectangle(46, 30, 8, 8));
					}
					break;
				case 24:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(18, 55, 3, 3));
						setPieceAreas.Add(new Rectangle(24, 25, 16, 16));
						setPieceAreas.Add(new Rectangle(28, 42, 4, 4));
						setPieceAreas.Add(new Rectangle(28, 58, 3, 3));
						setPieceAreas.Add(new Rectangle(29, 8, 4, 4));
						setPieceAreas.Add(new Rectangle(41, 17, 3, 3));
						setPieceAreas.Add(new Rectangle(41, 24, 8, 8));
						setPieceAreas.Add(new Rectangle(52, 30, 3, 3));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(9, 30, 3, 3));
						setPieceAreas.Add(new Rectangle(15, 24, 8, 8));
						setPieceAreas.Add(new Rectangle(20, 17, 3, 3));
						setPieceAreas.Add(new Rectangle(24, 25, 16, 16));
						setPieceAreas.Add(new Rectangle(31, 8, 4, 4));
						setPieceAreas.Add(new Rectangle(32, 42, 4, 4));
						setPieceAreas.Add(new Rectangle(33, 58, 3, 3));
						setPieceAreas.Add(new Rectangle(43, 55, 3, 3));
					}
					break;
				case 25:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(6, 42, 3, 3));
						setPieceAreas.Add(new Rectangle(8, 48, 3, 3));
						setPieceAreas.Add(new Rectangle(15, 13, 4, 4));
						setPieceAreas.Add(new Rectangle(22, 35, 3, 3));
						setPieceAreas.Add(new Rectangle(41, 47, 4, 4));
						setPieceAreas.Add(new Rectangle(42, 40, 4, 4));
						setPieceAreas.Add(new Rectangle(43, 12, 8, 8));
						setPieceAreas.Add(new Rectangle(44, 52, 3, 3));
						setPieceAreas.Add(new Rectangle(46, 45, 3, 3));
						setPieceAreas.Add(new Rectangle(48, 49, 4, 4));
						setPieceAreas.Add(new Rectangle(50, 41, 4, 4));
						setPieceAreas.Add(new Rectangle(53, 46, 3, 3));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(8, 46, 3, 3));
						setPieceAreas.Add(new Rectangle(10, 41, 4, 4));
						setPieceAreas.Add(new Rectangle(12, 49, 4, 4));
						setPieceAreas.Add(new Rectangle(13, 12, 8, 8));
						setPieceAreas.Add(new Rectangle(15, 45, 3, 3));
						setPieceAreas.Add(new Rectangle(17, 52, 3, 3));
						setPieceAreas.Add(new Rectangle(18, 40, 4, 4));
						setPieceAreas.Add(new Rectangle(19, 47, 4, 4));
						setPieceAreas.Add(new Rectangle(39, 35, 3, 3));
						setPieceAreas.Add(new Rectangle(45, 13, 4, 4));
						setPieceAreas.Add(new Rectangle(53, 48, 3, 3));
						setPieceAreas.Add(new Rectangle(55, 42, 3, 3));
					}
					break;
				case 26:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(13, 10, 8, 8));
						setPieceAreas.Add(new Rectangle(22, 13, 8, 8));
						setPieceAreas.Add(new Rectangle(23, 27, 4, 4));
						setPieceAreas.Add(new Rectangle(28, 30, 3, 3));
						setPieceAreas.Add(new Rectangle(29, 26, 3, 3));
						setPieceAreas.Add(new Rectangle(35, 30, 4, 4));
						setPieceAreas.Add(new Rectangle(38, 40, 3, 3));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(23, 40, 3, 3));
						setPieceAreas.Add(new Rectangle(25, 30, 4, 4));
						setPieceAreas.Add(new Rectangle(32, 26, 3, 3));
						setPieceAreas.Add(new Rectangle(33, 30, 3, 3));
						setPieceAreas.Add(new Rectangle(34, 13, 8, 8));
						setPieceAreas.Add(new Rectangle(37, 27, 4, 4));
						setPieceAreas.Add(new Rectangle(43, 10, 8, 8));
					}
					break;
				case 27:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(8, 21, 3, 3));
						setPieceAreas.Add(new Rectangle(8, 31, 8, 8));
						setPieceAreas.Add(new Rectangle(14, 16, 3, 3));
						setPieceAreas.Add(new Rectangle(14, 50, 4, 4));
						setPieceAreas.Add(new Rectangle(17, 44, 4, 4));
						setPieceAreas.Add(new Rectangle(27, 20, 3, 3));
						setPieceAreas.Add(new Rectangle(29, 6, 8, 8));
						setPieceAreas.Add(new Rectangle(43, 20, 4, 4));
						setPieceAreas.Add(new Rectangle(47, 51, 4, 4));
						setPieceAreas.Add(new Rectangle(48, 21, 4, 4));
						setPieceAreas.Add(new Rectangle(50, 30, 4, 4));
						setPieceAreas.Add(new Rectangle(53, 20, 4, 4));
						setPieceAreas.Add(new Rectangle(54, 34, 3, 3));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(7, 20, 4, 4));
						setPieceAreas.Add(new Rectangle(7, 34, 3, 3));
						setPieceAreas.Add(new Rectangle(10, 30, 4, 4));
						setPieceAreas.Add(new Rectangle(12, 21, 4, 4));
						setPieceAreas.Add(new Rectangle(13, 51, 4, 4));
						setPieceAreas.Add(new Rectangle(17, 20, 4, 4));
						setPieceAreas.Add(new Rectangle(27, 6, 8, 8));
						setPieceAreas.Add(new Rectangle(34, 20, 3, 3));
						setPieceAreas.Add(new Rectangle(43, 44, 4, 4));
						setPieceAreas.Add(new Rectangle(46, 50, 4, 4));
						setPieceAreas.Add(new Rectangle(47, 16, 3, 3));
						setPieceAreas.Add(new Rectangle(48, 31, 8, 8));
						setPieceAreas.Add(new Rectangle(53, 21, 3, 3));
					}
					break;
				case 28:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(4, 13, 4, 4));
						setPieceAreas.Add(new Rectangle(6, 32, 8, 8));
						setPieceAreas.Add(new Rectangle(9, 9, 16, 16));
						setPieceAreas.Add(new Rectangle(11, 45, 4, 4));
						setPieceAreas.Add(new Rectangle(16, 26, 3, 3));
						setPieceAreas.Add(new Rectangle(16, 39, 16, 16));
						setPieceAreas.Add(new Rectangle(20, 26, 4, 4));
						setPieceAreas.Add(new Rectangle(22, 35, 3, 3));
						setPieceAreas.Add(new Rectangle(24, 56, 3, 3));
						setPieceAreas.Add(new Rectangle(30, 6, 3, 3));
						setPieceAreas.Add(new Rectangle(33, 27, 3, 3));
						setPieceAreas.Add(new Rectangle(34, 39, 3, 3));
						setPieceAreas.Add(new Rectangle(34, 50, 3, 3));
						setPieceAreas.Add(new Rectangle(37, 16, 16, 16));
						setPieceAreas.Add(new Rectangle(41, 34, 3, 3));
						setPieceAreas.Add(new Rectangle(42, 11, 4, 4));
						setPieceAreas.Add(new Rectangle(45, 33, 4, 4));
						setPieceAreas.Add(new Rectangle(54, 19, 3, 3));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(7, 19, 3, 3));
						setPieceAreas.Add(new Rectangle(11, 16, 16, 16));
						setPieceAreas.Add(new Rectangle(15, 33, 4, 4));
						setPieceAreas.Add(new Rectangle(18, 11, 4, 4));
						setPieceAreas.Add(new Rectangle(20, 34, 3, 3));
						setPieceAreas.Add(new Rectangle(27, 39, 3, 3));
						setPieceAreas.Add(new Rectangle(27, 50, 3, 3));
						setPieceAreas.Add(new Rectangle(28, 27, 3, 3));
						setPieceAreas.Add(new Rectangle(31, 6, 3, 3));
						setPieceAreas.Add(new Rectangle(32, 39, 16, 16));
						setPieceAreas.Add(new Rectangle(37, 56, 3, 3));
						setPieceAreas.Add(new Rectangle(39, 9, 16, 16));
						setPieceAreas.Add(new Rectangle(39, 35, 3, 3));
						setPieceAreas.Add(new Rectangle(40, 26, 4, 4));
						setPieceAreas.Add(new Rectangle(45, 26, 3, 3));
						setPieceAreas.Add(new Rectangle(49, 45, 4, 4));
						setPieceAreas.Add(new Rectangle(50, 32, 8, 8));
						setPieceAreas.Add(new Rectangle(56, 13, 4, 4));
					}
					break;
				case 29:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(15, 44, 8, 8));
						setPieceAreas.Add(new Rectangle(19, 26, 4, 4));
						setPieceAreas.Add(new Rectangle(20, 21, 4, 4));
						setPieceAreas.Add(new Rectangle(21, 31, 3, 3));
						setPieceAreas.Add(new Rectangle(25, 20, 4, 4));
						setPieceAreas.Add(new Rectangle(26, 25, 3, 3));
						setPieceAreas.Add(new Rectangle(27, 36, 4, 4));
						setPieceAreas.Add(new Rectangle(28, 45, 3, 3));
						setPieceAreas.Add(new Rectangle(29, 32, 3, 3));
						setPieceAreas.Add(new Rectangle(30, 21, 8, 8));
						setPieceAreas.Add(new Rectangle(30, 50, 3, 3));
						setPieceAreas.Add(new Rectangle(35, 35, 4, 4));
						setPieceAreas.Add(new Rectangle(38, 31, 3, 3));
						setPieceAreas.Add(new Rectangle(39, 21, 3, 3));
						setPieceAreas.Add(new Rectangle(40, 26, 4, 4));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(20, 26, 4, 4));
						setPieceAreas.Add(new Rectangle(22, 21, 3, 3));
						setPieceAreas.Add(new Rectangle(23, 31, 3, 3));
						setPieceAreas.Add(new Rectangle(25, 35, 4, 4));
						setPieceAreas.Add(new Rectangle(26, 21, 8, 8));
						setPieceAreas.Add(new Rectangle(31, 50, 3, 3));
						setPieceAreas.Add(new Rectangle(32, 32, 3, 3));
						setPieceAreas.Add(new Rectangle(33, 36, 4, 4));
						setPieceAreas.Add(new Rectangle(33, 45, 3, 3));
						setPieceAreas.Add(new Rectangle(35, 20, 4, 4));
						setPieceAreas.Add(new Rectangle(35, 25, 3, 3));
						setPieceAreas.Add(new Rectangle(40, 21, 4, 4));
						setPieceAreas.Add(new Rectangle(40, 31, 3, 3));
						setPieceAreas.Add(new Rectangle(41, 26, 4, 4));
						setPieceAreas.Add(new Rectangle(41, 44, 8, 8));
					}
					break;
				case 30:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(16, 17, 32, 32));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(16, 17, 32, 32));
					}
					break;
				case 31:
					if (!flipped)
					{
					}
					break;
				case 32:
					if (!flipped)
					{
					}
					else
					{
					}
					break;
				case 33:
					if (!flipped)
					{
					}
					else
					{
					}
					break;
				case 34:
					if (!flipped)
					{
					}
					else
					{
					}
					break;
				case 35:
					if (!flipped)
					{
					}
					else
					{
					}
					break;
				case 36:
					if (!flipped)
					{
					}
					else
					{
					}
					break;
				case 37:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(21, 31, 4, 4));
						setPieceAreas.Add(new Rectangle(25, 38, 3, 3));
						setPieceAreas.Add(new Rectangle(32, 26, 3, 3));
						setPieceAreas.Add(new Rectangle(32, 39, 3, 3));
						setPieceAreas.Add(new Rectangle(39, 28, 3, 3));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(22, 28, 3, 3));
						setPieceAreas.Add(new Rectangle(29, 26, 3, 3));
						setPieceAreas.Add(new Rectangle(29, 39, 3, 3));
						setPieceAreas.Add(new Rectangle(36, 38, 3, 3));
						setPieceAreas.Add(new Rectangle(39, 31, 4, 4));
					}
					break;
				case 38:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(9, 43, 3, 3));
						setPieceAreas.Add(new Rectangle(11, 28, 3, 3));
						setPieceAreas.Add(new Rectangle(12, 22, 4, 4));
						setPieceAreas.Add(new Rectangle(12, 53, 4, 4));
						setPieceAreas.Add(new Rectangle(14, 39, 3, 3));
						setPieceAreas.Add(new Rectangle(15, 18, 3, 3));
						setPieceAreas.Add(new Rectangle(17, 52, 3, 3));
						setPieceAreas.Add(new Rectangle(25, 33, 4, 4));
						setPieceAreas.Add(new Rectangle(29, 49, 4, 4));
						setPieceAreas.Add(new Rectangle(31, 44, 3, 3));
						setPieceAreas.Add(new Rectangle(33, 23, 3, 3));
						setPieceAreas.Add(new Rectangle(38, 24, 4, 4));
						setPieceAreas.Add(new Rectangle(42, 12, 3, 3));
						setPieceAreas.Add(new Rectangle(53, 44, 3, 3));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(8, 44, 3, 3));
						setPieceAreas.Add(new Rectangle(19, 12, 3, 3));
						setPieceAreas.Add(new Rectangle(22, 24, 4, 4));
						setPieceAreas.Add(new Rectangle(28, 23, 3, 3));
						setPieceAreas.Add(new Rectangle(30, 44, 3, 3));
						setPieceAreas.Add(new Rectangle(31, 49, 4, 4));
						setPieceAreas.Add(new Rectangle(35, 33, 4, 4));
						setPieceAreas.Add(new Rectangle(44, 52, 3, 3));
						setPieceAreas.Add(new Rectangle(46, 18, 3, 3));
						setPieceAreas.Add(new Rectangle(47, 39, 3, 3));
						setPieceAreas.Add(new Rectangle(48, 22, 4, 4));
						setPieceAreas.Add(new Rectangle(48, 53, 4, 4));
						setPieceAreas.Add(new Rectangle(50, 28, 3, 3));
						setPieceAreas.Add(new Rectangle(52, 43, 3, 3));
					}
					break;
				case 39:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(4, 26, 3, 3));
						setPieceAreas.Add(new Rectangle(5, 36, 3, 3));
						setPieceAreas.Add(new Rectangle(5, 54, 3, 3));
						setPieceAreas.Add(new Rectangle(6, 43, 3, 3));
						setPieceAreas.Add(new Rectangle(7, 11, 3, 3));
						setPieceAreas.Add(new Rectangle(10, 19, 3, 3));
						setPieceAreas.Add(new Rectangle(10, 45, 3, 3));
						setPieceAreas.Add(new Rectangle(10, 58, 4, 4));
						setPieceAreas.Add(new Rectangle(14, 29, 4, 4));
						setPieceAreas.Add(new Rectangle(14, 42, 8, 8));
						setPieceAreas.Add(new Rectangle(15, 51, 3, 3));
						setPieceAreas.Add(new Rectangle(16, 55, 3, 3));
						setPieceAreas.Add(new Rectangle(18, 12, 3, 3));
						setPieceAreas.Add(new Rectangle(20, 38, 3, 3));
						setPieceAreas.Add(new Rectangle(22, 9, 4, 4));
						setPieceAreas.Add(new Rectangle(23, 30, 3, 3));
						setPieceAreas.Add(new Rectangle(23, 45, 3, 3));
						setPieceAreas.Add(new Rectangle(27, 46, 3, 3));
						setPieceAreas.Add(new Rectangle(28, 55, 4, 4));
						setPieceAreas.Add(new Rectangle(29, 4, 4, 4));
						setPieceAreas.Add(new Rectangle(29, 19, 3, 3));
						setPieceAreas.Add(new Rectangle(30, 23, 4, 4));
						setPieceAreas.Add(new Rectangle(31, 47, 3, 3));
						setPieceAreas.Add(new Rectangle(33, 29, 3, 3));
						setPieceAreas.Add(new Rectangle(37, 35, 8, 8));
						setPieceAreas.Add(new Rectangle(37, 50, 3, 3));
						setPieceAreas.Add(new Rectangle(39, 21, 4, 4));
						setPieceAreas.Add(new Rectangle(41, 51, 3, 3));
						setPieceAreas.Add(new Rectangle(42, 12, 3, 3));
						setPieceAreas.Add(new Rectangle(43, 45, 3, 3));
						setPieceAreas.Add(new Rectangle(44, 3, 3, 3));
						setPieceAreas.Add(new Rectangle(45, 58, 3, 3));
						setPieceAreas.Add(new Rectangle(47, 22, 3, 3));
						setPieceAreas.Add(new Rectangle(48, 28, 4, 4));
						setPieceAreas.Add(new Rectangle(54, 42, 4, 4));
						setPieceAreas.Add(new Rectangle(55, 52, 3, 3));
						setPieceAreas.Add(new Rectangle(57, 18, 3, 3));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(4, 18, 3, 3));
						setPieceAreas.Add(new Rectangle(6, 42, 4, 4));
						setPieceAreas.Add(new Rectangle(6, 52, 3, 3));
						setPieceAreas.Add(new Rectangle(12, 28, 4, 4));
						setPieceAreas.Add(new Rectangle(14, 22, 3, 3));
						setPieceAreas.Add(new Rectangle(16, 58, 3, 3));
						setPieceAreas.Add(new Rectangle(17, 3, 3, 3));
						setPieceAreas.Add(new Rectangle(18, 45, 3, 3));
						setPieceAreas.Add(new Rectangle(19, 12, 3, 3));
						setPieceAreas.Add(new Rectangle(19, 35, 8, 8));
						setPieceAreas.Add(new Rectangle(20, 51, 3, 3));
						setPieceAreas.Add(new Rectangle(21, 21, 4, 4));
						setPieceAreas.Add(new Rectangle(24, 50, 3, 3));
						setPieceAreas.Add(new Rectangle(28, 29, 3, 3));
						setPieceAreas.Add(new Rectangle(30, 23, 4, 4));
						setPieceAreas.Add(new Rectangle(30, 47, 3, 3));
						setPieceAreas.Add(new Rectangle(31, 4, 4, 4));
						setPieceAreas.Add(new Rectangle(32, 19, 3, 3));
						setPieceAreas.Add(new Rectangle(32, 55, 4, 4));
						setPieceAreas.Add(new Rectangle(34, 46, 3, 3));
						setPieceAreas.Add(new Rectangle(38, 9, 4, 4));
						setPieceAreas.Add(new Rectangle(38, 30, 3, 3));
						setPieceAreas.Add(new Rectangle(38, 45, 3, 3));
						setPieceAreas.Add(new Rectangle(41, 38, 3, 3));
						setPieceAreas.Add(new Rectangle(42, 42, 8, 8));
						setPieceAreas.Add(new Rectangle(43, 12, 3, 3));
						setPieceAreas.Add(new Rectangle(45, 55, 3, 3));
						setPieceAreas.Add(new Rectangle(46, 29, 4, 4));
						setPieceAreas.Add(new Rectangle(46, 51, 3, 3));
						setPieceAreas.Add(new Rectangle(50, 58, 4, 4));
						setPieceAreas.Add(new Rectangle(51, 19, 3, 3));
						setPieceAreas.Add(new Rectangle(51, 45, 3, 3));
						setPieceAreas.Add(new Rectangle(54, 11, 3, 3));
						setPieceAreas.Add(new Rectangle(55, 43, 3, 3));
						setPieceAreas.Add(new Rectangle(56, 36, 3, 3));
						setPieceAreas.Add(new Rectangle(56, 54, 3, 3));
						setPieceAreas.Add(new Rectangle(57, 26, 3, 3));
					}
					break;
				case 40:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(5, 43, 3, 3));
						setPieceAreas.Add(new Rectangle(8, 37, 3, 3));
						setPieceAreas.Add(new Rectangle(9, 11, 3, 3));
						setPieceAreas.Add(new Rectangle(10, 16, 4, 4));
						setPieceAreas.Add(new Rectangle(14, 43, 3, 3));
						setPieceAreas.Add(new Rectangle(14, 48, 8, 8));
						setPieceAreas.Add(new Rectangle(15, 8, 3, 3));
						setPieceAreas.Add(new Rectangle(23, 51, 4, 4));
						setPieceAreas.Add(new Rectangle(27, 35, 3, 3));
						setPieceAreas.Add(new Rectangle(27, 47, 3, 3));
						setPieceAreas.Add(new Rectangle(29, 29, 4, 4));
						setPieceAreas.Add(new Rectangle(30, 21, 4, 4));
						setPieceAreas.Add(new Rectangle(31, 9, 2, 2));
						setPieceAreas.Add(new Rectangle(32, 11, 1, 1));
						setPieceAreas.Add(new Rectangle(33, 9, 1, 1));
						setPieceAreas.Add(new Rectangle(33, 10, 1, 1));
						setPieceAreas.Add(new Rectangle(33, 11, 1, 1));
						setPieceAreas.Add(new Rectangle(35, 13, 8, 8));
						setPieceAreas.Add(new Rectangle(37, 24, 3, 3));
						setPieceAreas.Add(new Rectangle(37, 38, 3, 3));
						setPieceAreas.Add(new Rectangle(37, 57, 3, 3));
						setPieceAreas.Add(new Rectangle(38, 48, 3, 3));
						setPieceAreas.Add(new Rectangle(41, 8, 3, 3));
						setPieceAreas.Add(new Rectangle(41, 39, 4, 4));
						setPieceAreas.Add(new Rectangle(45, 19, 3, 3));
						setPieceAreas.Add(new Rectangle(46, 12, 4, 4));
						setPieceAreas.Add(new Rectangle(48, 37, 8, 8));
						setPieceAreas.Add(new Rectangle(49, 32, 4, 4));
						setPieceAreas.Add(new Rectangle(50, 17, 4, 4));
						setPieceAreas.Add(new Rectangle(53, 23, 3, 3));
						setPieceAreas.Add(new Rectangle(55, 29, 3, 3));
						setPieceAreas.Add(new Rectangle(57, 35, 3, 3));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(4, 35, 3, 3));
						setPieceAreas.Add(new Rectangle(6, 29, 3, 3));
						setPieceAreas.Add(new Rectangle(8, 23, 3, 3));
						setPieceAreas.Add(new Rectangle(8, 37, 8, 8));
						setPieceAreas.Add(new Rectangle(10, 17, 4, 4));
						setPieceAreas.Add(new Rectangle(11, 32, 4, 4));
						setPieceAreas.Add(new Rectangle(14, 12, 4, 4));
						setPieceAreas.Add(new Rectangle(16, 19, 3, 3));
						setPieceAreas.Add(new Rectangle(19, 39, 4, 4));
						setPieceAreas.Add(new Rectangle(20, 8, 3, 3));
						setPieceAreas.Add(new Rectangle(21, 13, 8, 8));
						setPieceAreas.Add(new Rectangle(23, 48, 3, 3));
						setPieceAreas.Add(new Rectangle(24, 24, 3, 3));
						setPieceAreas.Add(new Rectangle(24, 38, 3, 3));
						setPieceAreas.Add(new Rectangle(24, 57, 3, 3));
						setPieceAreas.Add(new Rectangle(30, 9, 3, 3));
						setPieceAreas.Add(new Rectangle(30, 21, 4, 4));
						setPieceAreas.Add(new Rectangle(31, 29, 4, 4));
						setPieceAreas.Add(new Rectangle(34, 35, 3, 3));
						setPieceAreas.Add(new Rectangle(34, 47, 3, 3));
						setPieceAreas.Add(new Rectangle(37, 51, 4, 4));
						setPieceAreas.Add(new Rectangle(42, 48, 8, 8));
						setPieceAreas.Add(new Rectangle(46, 8, 3, 3));
						setPieceAreas.Add(new Rectangle(47, 43, 3, 3));
						setPieceAreas.Add(new Rectangle(50, 16, 4, 4));
						setPieceAreas.Add(new Rectangle(52, 11, 3, 3));
						setPieceAreas.Add(new Rectangle(53, 37, 3, 3));
						setPieceAreas.Add(new Rectangle(56, 43, 3, 3));
					}
					break;
				case 41:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(17, 27, 1, 1));
						setPieceAreas.Add(new Rectangle(18, 25, 3, 3));
						setPieceAreas.Add(new Rectangle(19, 31, 3, 3));
						setPieceAreas.Add(new Rectangle(26, 40, 3, 3));
						setPieceAreas.Add(new Rectangle(27, 24, 4, 4));
						setPieceAreas.Add(new Rectangle(38, 35, 3, 3));
						setPieceAreas.Add(new Rectangle(40, 19, 3, 3));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(21, 19, 3, 3));
						setPieceAreas.Add(new Rectangle(23, 35, 3, 3));
						setPieceAreas.Add(new Rectangle(33, 24, 4, 4));
						setPieceAreas.Add(new Rectangle(35, 40, 3, 3));
						setPieceAreas.Add(new Rectangle(42, 31, 3, 3));
						setPieceAreas.Add(new Rectangle(43, 25, 3, 3));
						setPieceAreas.Add(new Rectangle(46, 27, 1, 1));
					}
					break;
				case 42:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(6, 27, 4, 4));
						setPieceAreas.Add(new Rectangle(9, 12, 8, 8));
						setPieceAreas.Add(new Rectangle(16, 42, 4, 4));
						setPieceAreas.Add(new Rectangle(18, 24, 4, 4));
						setPieceAreas.Add(new Rectangle(21, 7, 4, 4));
						setPieceAreas.Add(new Rectangle(23, 33, 8, 8));
						setPieceAreas.Add(new Rectangle(31, 46, 4, 4));
						setPieceAreas.Add(new Rectangle(34, 28, 4, 4));
						setPieceAreas.Add(new Rectangle(36, 7, 4, 4));
						setPieceAreas.Add(new Rectangle(37, 41, 4, 4));
						setPieceAreas.Add(new Rectangle(38, 19, 4, 4));
						setPieceAreas.Add(new Rectangle(45, 12, 4, 4));
						setPieceAreas.Add(new Rectangle(46, 43, 4, 4));
						setPieceAreas.Add(new Rectangle(49, 20, 8, 8));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(7, 20, 8, 8));
						setPieceAreas.Add(new Rectangle(14, 43, 4, 4));
						setPieceAreas.Add(new Rectangle(15, 12, 4, 4));
						setPieceAreas.Add(new Rectangle(22, 19, 4, 4));
						setPieceAreas.Add(new Rectangle(23, 41, 4, 4));
						setPieceAreas.Add(new Rectangle(24, 7, 4, 4));
						setPieceAreas.Add(new Rectangle(26, 28, 4, 4));
						setPieceAreas.Add(new Rectangle(29, 46, 4, 4));
						setPieceAreas.Add(new Rectangle(33, 33, 8, 8));
						setPieceAreas.Add(new Rectangle(39, 7, 4, 4));
						setPieceAreas.Add(new Rectangle(42, 24, 4, 4));
						setPieceAreas.Add(new Rectangle(44, 42, 4, 4));
						setPieceAreas.Add(new Rectangle(47, 12, 8, 8));
						setPieceAreas.Add(new Rectangle(54, 27, 4, 4));
					}
					break;
				case 43:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(7, 37, 3, 3));
						setPieceAreas.Add(new Rectangle(12, 18, 3, 3));
						setPieceAreas.Add(new Rectangle(14, 35, 3, 3));
						setPieceAreas.Add(new Rectangle(16, 19, 8, 8));
						setPieceAreas.Add(new Rectangle(16, 29, 4, 4));
						setPieceAreas.Add(new Rectangle(21, 34, 8, 8));
						setPieceAreas.Add(new Rectangle(22, 29, 3, 3));
						setPieceAreas.Add(new Rectangle(30, 30, 16, 16));
						setPieceAreas.Add(new Rectangle(40, 13, 4, 4));
						setPieceAreas.Add(new Rectangle(47, 27, 4, 4));
						setPieceAreas.Add(new Rectangle(48, 18, 8, 8));
						setPieceAreas.Add(new Rectangle(49, 14, 3, 3));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(8, 18, 8, 8));
						setPieceAreas.Add(new Rectangle(12, 14, 3, 3));
						setPieceAreas.Add(new Rectangle(13, 27, 4, 4));
						setPieceAreas.Add(new Rectangle(18, 30, 16, 16));
						setPieceAreas.Add(new Rectangle(20, 13, 4, 4));
						setPieceAreas.Add(new Rectangle(35, 34, 8, 8));
						setPieceAreas.Add(new Rectangle(39, 29, 3, 3));
						setPieceAreas.Add(new Rectangle(40, 19, 8, 8));
						setPieceAreas.Add(new Rectangle(44, 29, 4, 4));
						setPieceAreas.Add(new Rectangle(47, 35, 3, 3));
						setPieceAreas.Add(new Rectangle(49, 18, 3, 3));
						setPieceAreas.Add(new Rectangle(54, 37, 3, 3));
					}
					break;
				case 44:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(6, 12, 4, 4));
						setPieceAreas.Add(new Rectangle(6, 48, 4, 4));
						setPieceAreas.Add(new Rectangle(9, 19, 3, 3));
						setPieceAreas.Add(new Rectangle(11, 58, 3, 3));
						setPieceAreas.Add(new Rectangle(18, 30, 3, 3));
						setPieceAreas.Add(new Rectangle(20, 57, 4, 4));
						setPieceAreas.Add(new Rectangle(24, 17, 4, 4));
						setPieceAreas.Add(new Rectangle(26, 48, 8, 8));
						setPieceAreas.Add(new Rectangle(33, 33, 3, 3));
						setPieceAreas.Add(new Rectangle(38, 13, 3, 3));
						setPieceAreas.Add(new Rectangle(42, 49, 3, 3));
						setPieceAreas.Add(new Rectangle(51, 38, 3, 3));
						setPieceAreas.Add(new Rectangle(52, 52, 3, 3));
						setPieceAreas.Add(new Rectangle(54, 25, 4, 4));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(6, 25, 4, 4));
						setPieceAreas.Add(new Rectangle(9, 52, 3, 3));
						setPieceAreas.Add(new Rectangle(10, 38, 3, 3));
						setPieceAreas.Add(new Rectangle(19, 49, 3, 3));
						setPieceAreas.Add(new Rectangle(23, 13, 3, 3));
						setPieceAreas.Add(new Rectangle(28, 33, 3, 3));
						setPieceAreas.Add(new Rectangle(30, 48, 8, 8));
						setPieceAreas.Add(new Rectangle(36, 17, 4, 4));
						setPieceAreas.Add(new Rectangle(40, 57, 4, 4));
						setPieceAreas.Add(new Rectangle(43, 30, 3, 3));
						setPieceAreas.Add(new Rectangle(50, 58, 3, 3));
						setPieceAreas.Add(new Rectangle(52, 19, 3, 3));
						setPieceAreas.Add(new Rectangle(54, 12, 4, 4));
						setPieceAreas.Add(new Rectangle(54, 48, 4, 4));
					}
					break;
				case 45:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(9, 22, 8, 8));
						setPieceAreas.Add(new Rectangle(12, 36, 16, 16));
						setPieceAreas.Add(new Rectangle(22, 11, 8, 8));
						setPieceAreas.Add(new Rectangle(38, 11, 16, 16));
						setPieceAreas.Add(new Rectangle(40, 39, 8, 8));
						setPieceAreas.Add(new Rectangle(44, 32, 3, 3));
						setPieceAreas.Add(new Rectangle(55, 33, 3, 3));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(6, 33, 3, 3));
						setPieceAreas.Add(new Rectangle(10, 11, 16, 16));
						setPieceAreas.Add(new Rectangle(16, 39, 8, 8));
						setPieceAreas.Add(new Rectangle(17, 32, 3, 3));
						setPieceAreas.Add(new Rectangle(34, 11, 8, 8));
						setPieceAreas.Add(new Rectangle(36, 36, 16, 16));
						setPieceAreas.Add(new Rectangle(47, 22, 8, 8));
					}
					break;
				case 46:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(16, 39, 3, 3));
						setPieceAreas.Add(new Rectangle(19, 25, 3, 3));
						setPieceAreas.Add(new Rectangle(23, 26, 4, 4));
						setPieceAreas.Add(new Rectangle(24, 22, 3, 3));
						setPieceAreas.Add(new Rectangle(28, 23, 8, 8));
						setPieceAreas.Add(new Rectangle(37, 22, 3, 3));
						setPieceAreas.Add(new Rectangle(37, 26, 4, 4));
						setPieceAreas.Add(new Rectangle(42, 25, 3, 3));
						setPieceAreas.Add(new Rectangle(45, 39, 3, 3));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(16, 39, 3, 3));
						setPieceAreas.Add(new Rectangle(19, 25, 3, 3));
						setPieceAreas.Add(new Rectangle(23, 26, 4, 4));
						setPieceAreas.Add(new Rectangle(24, 22, 3, 3));
						setPieceAreas.Add(new Rectangle(28, 23, 8, 8));
						setPieceAreas.Add(new Rectangle(37, 22, 3, 3));
						setPieceAreas.Add(new Rectangle(37, 26, 4, 4));
						setPieceAreas.Add(new Rectangle(42, 25, 3, 3));
						setPieceAreas.Add(new Rectangle(45, 39, 3, 3));
					}
					break;
				case 47:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(19, 41, 3, 3));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(42, 41, 3, 3));
					}
					break;
				case 48:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(13, 35, 3, 3));
						setPieceAreas.Add(new Rectangle(19, 23, 3, 3));
						setPieceAreas.Add(new Rectangle(28, 39, 3, 3));
						setPieceAreas.Add(new Rectangle(34, 27, 3, 3));
						setPieceAreas.Add(new Rectangle(42, 19, 3, 3));
						setPieceAreas.Add(new Rectangle(46, 31, 3, 3));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(15, 31, 3, 3));
						setPieceAreas.Add(new Rectangle(19, 19, 3, 3));
						setPieceAreas.Add(new Rectangle(27, 27, 3, 3));
						setPieceAreas.Add(new Rectangle(33, 39, 3, 3));
						setPieceAreas.Add(new Rectangle(42, 23, 3, 3));
						setPieceAreas.Add(new Rectangle(48, 35, 3, 3));
					}
					break;
				case 49:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(6, 6, 16, 16));
						setPieceAreas.Add(new Rectangle(9, 23, 4, 4));
						setPieceAreas.Add(new Rectangle(29, 41, 3, 3));
						setPieceAreas.Add(new Rectangle(31, 25, 4, 4));
						setPieceAreas.Add(new Rectangle(31, 50, 3, 3));
						setPieceAreas.Add(new Rectangle(32, 46, 3, 3));
						setPieceAreas.Add(new Rectangle(33, 19, 3, 3));
						setPieceAreas.Add(new Rectangle(45, 22, 3, 3));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(16, 22, 3, 3));
						setPieceAreas.Add(new Rectangle(28, 19, 3, 3));
						setPieceAreas.Add(new Rectangle(29, 25, 4, 4));
						setPieceAreas.Add(new Rectangle(29, 46, 3, 3));
						setPieceAreas.Add(new Rectangle(30, 50, 3, 3));
						setPieceAreas.Add(new Rectangle(32, 41, 3, 3));
						setPieceAreas.Add(new Rectangle(42, 6, 16, 16));
						setPieceAreas.Add(new Rectangle(51, 23, 4, 4));
					}
					break;
				case 50:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(6, 37, 16, 16));
						setPieceAreas.Add(new Rectangle(7, 14, 8, 8));
						setPieceAreas.Add(new Rectangle(8, 33, 3, 3));
						setPieceAreas.Add(new Rectangle(19, 18, 3, 3));
						setPieceAreas.Add(new Rectangle(19, 33, 3, 3));
						setPieceAreas.Add(new Rectangle(23, 24, 8, 8));
						setPieceAreas.Add(new Rectangle(26, 20, 3, 3));
						setPieceAreas.Add(new Rectangle(44, 10, 8, 8));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(12, 10, 8, 8));
						setPieceAreas.Add(new Rectangle(33, 24, 8, 8));
						setPieceAreas.Add(new Rectangle(35, 20, 3, 3));
						setPieceAreas.Add(new Rectangle(42, 18, 3, 3));
						setPieceAreas.Add(new Rectangle(42, 33, 3, 3));
						setPieceAreas.Add(new Rectangle(42, 37, 16, 16));
						setPieceAreas.Add(new Rectangle(49, 14, 8, 8));
						setPieceAreas.Add(new Rectangle(53, 33, 3, 3));
					}
					break;
				case 51:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(14, 13, 8, 8));
						setPieceAreas.Add(new Rectangle(16, 34, 8, 8));
						setPieceAreas.Add(new Rectangle(19, 43, 4, 4));
						setPieceAreas.Add(new Rectangle(23, 27, 4, 4));
						setPieceAreas.Add(new Rectangle(26, 49, 4, 4));
						setPieceAreas.Add(new Rectangle(27, 10, 4, 4));
						setPieceAreas.Add(new Rectangle(28, 32, 3, 3));
						setPieceAreas.Add(new Rectangle(30, 22, 8, 8));
						setPieceAreas.Add(new Rectangle(33, 16, 3, 3));
						setPieceAreas.Add(new Rectangle(37, 49, 3, 3));
						setPieceAreas.Add(new Rectangle(40, 26, 3, 3));
						setPieceAreas.Add(new Rectangle(41, 43, 4, 4));
						setPieceAreas.Add(new Rectangle(42, 15, 8, 8));
						setPieceAreas.Add(new Rectangle(45, 36, 4, 4));
						setPieceAreas.Add(new Rectangle(52, 17, 3, 3));
						setPieceAreas.Add(new Rectangle(54, 29, 3, 3));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(7, 29, 3, 3));
						setPieceAreas.Add(new Rectangle(9, 17, 3, 3));
						setPieceAreas.Add(new Rectangle(14, 15, 8, 8));
						setPieceAreas.Add(new Rectangle(15, 36, 4, 4));
						setPieceAreas.Add(new Rectangle(19, 43, 4, 4));
						setPieceAreas.Add(new Rectangle(21, 26, 3, 3));
						setPieceAreas.Add(new Rectangle(24, 49, 3, 3));
						setPieceAreas.Add(new Rectangle(26, 22, 8, 8));
						setPieceAreas.Add(new Rectangle(28, 16, 3, 3));
						setPieceAreas.Add(new Rectangle(33, 10, 4, 4));
						setPieceAreas.Add(new Rectangle(33, 32, 3, 3));
						setPieceAreas.Add(new Rectangle(34, 49, 4, 4));
						setPieceAreas.Add(new Rectangle(37, 27, 4, 4));
						setPieceAreas.Add(new Rectangle(40, 34, 8, 8));
						setPieceAreas.Add(new Rectangle(41, 43, 4, 4));
						setPieceAreas.Add(new Rectangle(42, 13, 8, 8));
					}
					break;
				case 52:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(3, 36, 16, 16));
						setPieceAreas.Add(new Rectangle(5, 55, 3, 3));
						setPieceAreas.Add(new Rectangle(6, 7, 16, 16));
						setPieceAreas.Add(new Rectangle(7, 25, 3, 3));
						setPieceAreas.Add(new Rectangle(14, 32, 3, 3));
						setPieceAreas.Add(new Rectangle(16, 25, 4, 4));
						setPieceAreas.Add(new Rectangle(18, 54, 3, 3));
						setPieceAreas.Add(new Rectangle(24, 21, 32, 32));
						setPieceAreas.Add(new Rectangle(41, 57, 4, 4));
						setPieceAreas.Add(new Rectangle(51, 55, 4, 4));
						setPieceAreas.Add(new Rectangle(57, 47, 3, 3));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(4, 47, 3, 3));
						setPieceAreas.Add(new Rectangle(8, 21, 32, 32));
						setPieceAreas.Add(new Rectangle(9, 55, 4, 4));
						setPieceAreas.Add(new Rectangle(19, 57, 4, 4));
						setPieceAreas.Add(new Rectangle(42, 7, 16, 16));
						setPieceAreas.Add(new Rectangle(43, 54, 3, 3));
						setPieceAreas.Add(new Rectangle(44, 25, 4, 4));
						setPieceAreas.Add(new Rectangle(45, 36, 16, 16));
						setPieceAreas.Add(new Rectangle(47, 32, 3, 3));
						setPieceAreas.Add(new Rectangle(54, 25, 3, 3));
						setPieceAreas.Add(new Rectangle(56, 55, 3, 3));
					}
					break;
				case 53:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(7, 16, 4, 4));
						setPieceAreas.Add(new Rectangle(7, 39, 3, 3));
						setPieceAreas.Add(new Rectangle(13, 24, 3, 3));
						setPieceAreas.Add(new Rectangle(15, 9, 3, 3));
						setPieceAreas.Add(new Rectangle(16, 31, 4, 4));
						setPieceAreas.Add(new Rectangle(19, 37, 3, 3));
						setPieceAreas.Add(new Rectangle(22, 13, 3, 3));
						setPieceAreas.Add(new Rectangle(28, 17, 3, 3));
						setPieceAreas.Add(new Rectangle(30, 28, 3, 3));
						setPieceAreas.Add(new Rectangle(33, 40, 4, 4));
						setPieceAreas.Add(new Rectangle(36, 9, 3, 3));
						setPieceAreas.Add(new Rectangle(36, 21, 4, 4));
						setPieceAreas.Add(new Rectangle(43, 6, 3, 3));
						setPieceAreas.Add(new Rectangle(43, 28, 3, 3));
						setPieceAreas.Add(new Rectangle(44, 15, 3, 3));
						setPieceAreas.Add(new Rectangle(45, 41, 3, 3));
						setPieceAreas.Add(new Rectangle(51, 28, 4, 4));
						setPieceAreas.Add(new Rectangle(54, 18, 4, 4));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(6, 18, 4, 4));
						setPieceAreas.Add(new Rectangle(9, 28, 4, 4));
						setPieceAreas.Add(new Rectangle(16, 41, 3, 3));
						setPieceAreas.Add(new Rectangle(17, 15, 3, 3));
						setPieceAreas.Add(new Rectangle(18, 6, 3, 3));
						setPieceAreas.Add(new Rectangle(18, 28, 3, 3));
						setPieceAreas.Add(new Rectangle(24, 21, 4, 4));
						setPieceAreas.Add(new Rectangle(25, 9, 3, 3));
						setPieceAreas.Add(new Rectangle(27, 40, 4, 4));
						setPieceAreas.Add(new Rectangle(31, 28, 3, 3));
						setPieceAreas.Add(new Rectangle(33, 17, 3, 3));
						setPieceAreas.Add(new Rectangle(39, 13, 3, 3));
						setPieceAreas.Add(new Rectangle(42, 37, 3, 3));
						setPieceAreas.Add(new Rectangle(44, 31, 4, 4));
						setPieceAreas.Add(new Rectangle(46, 9, 3, 3));
						setPieceAreas.Add(new Rectangle(48, 24, 3, 3));
						setPieceAreas.Add(new Rectangle(53, 16, 4, 4));
						setPieceAreas.Add(new Rectangle(54, 39, 3, 3));
					}
					break;
				case 54:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(9, 13, 8, 8));
						setPieceAreas.Add(new Rectangle(9, 35, 8, 8));
						setPieceAreas.Add(new Rectangle(10, 31, 3, 3));
						setPieceAreas.Add(new Rectangle(11, 44, 4, 4));
						setPieceAreas.Add(new Rectangle(12, 22, 8, 8));
						setPieceAreas.Add(new Rectangle(14, 49, 8, 8));
						setPieceAreas.Add(new Rectangle(18, 32, 16, 16));
						setPieceAreas.Add(new Rectangle(23, 28, 3, 3));
						setPieceAreas.Add(new Rectangle(24, 50, 4, 4));
						setPieceAreas.Add(new Rectangle(32, 49, 4, 4));
						setPieceAreas.Add(new Rectangle(34, 16, 3, 3));
						setPieceAreas.Add(new Rectangle(35, 38, 8, 8));
						setPieceAreas.Add(new Rectangle(38, 10, 16, 16));
						setPieceAreas.Add(new Rectangle(38, 48, 8, 8));
						setPieceAreas.Add(new Rectangle(40, 34, 3, 3));
						setPieceAreas.Add(new Rectangle(41, 27, 4, 4));
						setPieceAreas.Add(new Rectangle(42, 6, 3, 3));
						setPieceAreas.Add(new Rectangle(47, 27, 3, 3));
						setPieceAreas.Add(new Rectangle(47, 47, 4, 4));
						setPieceAreas.Add(new Rectangle(47, 52, 3, 3));
						setPieceAreas.Add(new Rectangle(55, 20, 3, 3));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(6, 20, 3, 3));
						setPieceAreas.Add(new Rectangle(10, 10, 16, 16));
						setPieceAreas.Add(new Rectangle(13, 47, 4, 4));
						setPieceAreas.Add(new Rectangle(14, 27, 3, 3));
						setPieceAreas.Add(new Rectangle(14, 52, 3, 3));
						setPieceAreas.Add(new Rectangle(18, 48, 8, 8));
						setPieceAreas.Add(new Rectangle(19, 6, 3, 3));
						setPieceAreas.Add(new Rectangle(19, 27, 4, 4));
						setPieceAreas.Add(new Rectangle(21, 34, 3, 3));
						setPieceAreas.Add(new Rectangle(21, 38, 8, 8));
						setPieceAreas.Add(new Rectangle(27, 16, 3, 3));
						setPieceAreas.Add(new Rectangle(28, 49, 4, 4));
						setPieceAreas.Add(new Rectangle(30, 32, 16, 16));
						setPieceAreas.Add(new Rectangle(36, 50, 4, 4));
						setPieceAreas.Add(new Rectangle(38, 28, 3, 3));
						setPieceAreas.Add(new Rectangle(42, 49, 8, 8));
						setPieceAreas.Add(new Rectangle(44, 22, 8, 8));
						setPieceAreas.Add(new Rectangle(47, 13, 8, 8));
						setPieceAreas.Add(new Rectangle(47, 35, 8, 8));
						setPieceAreas.Add(new Rectangle(49, 44, 4, 4));
						setPieceAreas.Add(new Rectangle(51, 31, 3, 3));
					}
					break;
				case 55:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(5, 8, 3, 3));
						setPieceAreas.Add(new Rectangle(8, 33, 3, 3));
						setPieceAreas.Add(new Rectangle(10, 47, 3, 3));
						setPieceAreas.Add(new Rectangle(14, 27, 8, 8));
						setPieceAreas.Add(new Rectangle(17, 10, 3, 3));
						setPieceAreas.Add(new Rectangle(21, 8, 8, 8));
						setPieceAreas.Add(new Rectangle(22, 45, 8, 8));
						setPieceAreas.Add(new Rectangle(33, 12, 3, 3));
						setPieceAreas.Add(new Rectangle(34, 27, 3, 3));
						setPieceAreas.Add(new Rectangle(39, 49, 3, 3));
						setPieceAreas.Add(new Rectangle(40, 26, 4, 4));
						setPieceAreas.Add(new Rectangle(42, 53, 4, 4));
						setPieceAreas.Add(new Rectangle(44, 21, 3, 3));
						setPieceAreas.Add(new Rectangle(47, 48, 8, 8));
						setPieceAreas.Add(new Rectangle(56, 46, 3, 3));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(5, 46, 3, 3));
						setPieceAreas.Add(new Rectangle(9, 48, 8, 8));
						setPieceAreas.Add(new Rectangle(17, 21, 3, 3));
						setPieceAreas.Add(new Rectangle(18, 53, 4, 4));
						setPieceAreas.Add(new Rectangle(20, 26, 4, 4));
						setPieceAreas.Add(new Rectangle(22, 49, 3, 3));
						setPieceAreas.Add(new Rectangle(27, 27, 3, 3));
						setPieceAreas.Add(new Rectangle(28, 12, 3, 3));
						setPieceAreas.Add(new Rectangle(34, 45, 8, 8));
						setPieceAreas.Add(new Rectangle(35, 8, 8, 8));
						setPieceAreas.Add(new Rectangle(42, 27, 8, 8));
						setPieceAreas.Add(new Rectangle(44, 10, 3, 3));
						setPieceAreas.Add(new Rectangle(51, 47, 3, 3));
						setPieceAreas.Add(new Rectangle(53, 33, 3, 3));
						setPieceAreas.Add(new Rectangle(56, 8, 3, 3));
					}
					break;
				case 56:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(10, 40, 3, 3));
						setPieceAreas.Add(new Rectangle(12, 27, 3, 3));
						setPieceAreas.Add(new Rectangle(13, 18, 3, 3));
						setPieceAreas.Add(new Rectangle(16, 46, 4, 4));
						setPieceAreas.Add(new Rectangle(22, 49, 3, 3));
						setPieceAreas.Add(new Rectangle(25, 24, 4, 4));
						setPieceAreas.Add(new Rectangle(27, 46, 4, 4));
						setPieceAreas.Add(new Rectangle(29, 18, 3, 3));
						setPieceAreas.Add(new Rectangle(34, 12, 4, 4));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(26, 12, 4, 4));
						setPieceAreas.Add(new Rectangle(32, 18, 3, 3));
						setPieceAreas.Add(new Rectangle(33, 46, 4, 4));
						setPieceAreas.Add(new Rectangle(35, 24, 4, 4));
						setPieceAreas.Add(new Rectangle(39, 49, 3, 3));
						setPieceAreas.Add(new Rectangle(44, 46, 4, 4));
						setPieceAreas.Add(new Rectangle(48, 18, 3, 3));
						setPieceAreas.Add(new Rectangle(49, 27, 3, 3));
						setPieceAreas.Add(new Rectangle(51, 40, 3, 3));
					}
					break;
				case 57:
					if (!flipped)
					{
						setPieceAreas.Add(new Rectangle(8, 18, 3, 3));
						setPieceAreas.Add(new Rectangle(8, 54, 4, 4));
						setPieceAreas.Add(new Rectangle(13, 7, 3, 3));
						setPieceAreas.Add(new Rectangle(13, 41, 3, 3));
						setPieceAreas.Add(new Rectangle(19, 27, 4, 4));
						setPieceAreas.Add(new Rectangle(31, 23, 3, 3));
						setPieceAreas.Add(new Rectangle(32, 37, 8, 8));
						setPieceAreas.Add(new Rectangle(36, 22, 3, 3));
						setPieceAreas.Add(new Rectangle(41, 54, 3, 3));
						setPieceAreas.Add(new Rectangle(44, 13, 3, 3));
						setPieceAreas.Add(new Rectangle(49, 42, 4, 4));
					}
					else
					{
						setPieceAreas.Add(new Rectangle(11, 42, 4, 4));
						setPieceAreas.Add(new Rectangle(17, 13, 3, 3));
						setPieceAreas.Add(new Rectangle(20, 54, 3, 3));
						setPieceAreas.Add(new Rectangle(24, 37, 8, 8));
						setPieceAreas.Add(new Rectangle(25, 22, 3, 3));
						setPieceAreas.Add(new Rectangle(30, 23, 3, 3));
						setPieceAreas.Add(new Rectangle(41, 27, 4, 4));
						setPieceAreas.Add(new Rectangle(48, 7, 3, 3));
						setPieceAreas.Add(new Rectangle(48, 41, 3, 3));
						setPieceAreas.Add(new Rectangle(52, 54, 4, 4));
						setPieceAreas.Add(new Rectangle(53, 18, 3, 3));

					}
					break;

				default:
					break;

			}
		}

		private void PlaceGroundTiles()
		{
			for (int j = 0; j < 64; j++)
			{
				for (int k = 0; k < 64; k++)
				{
					if (generationRandom.NextDouble() < 0.30000001192092896)
					{
						//this.SetTile(this.backLayer, x, y, VolcanoDungeon.GetTileIndex(1 + this.generationRandom.Next(0, 3), this.generationRandom.Next(0, 2)));
						generationRandom.Next();
						generationRandom.Next();
					}
					else
					{
						//this.SetTile(this.backLayer, x, y, VolcanoDungeon.GetTileIndex(1, 0));
					}
				}
			}
		}


		public virtual void AddPossibleSwitchLocation(int switch_index, int x, int y)
		{
			if (!this.possibleSwitchPositions.TryGetValue(switch_index, out var positions))
			{
				positions = (this.possibleSwitchPositions[switch_index] = new List<Point>());
			}
			positions.Add(new Point(x, y));
		}

		public virtual void AddPossibleGateLocation(int gate_index, int x, int y)
		{
			if (!this.possibleGatePositions.TryGetValue(gate_index, out var positions))
			{
				positions = (this.possibleGatePositions[gate_index] = new List<Point>());
			}
			positions.Add(new Point(x, y));
		}

		public void IncrememntMonsterCount(string monster)
		{
			if (!monsterCounts.ContainsKey(monster))
			{
				monsterCounts[monster] = 0;
			}
			monsterCounts[monster]++;
		}


	}
	public class Volcano
	{
		public static bool crackedGoldenCoconut = true;
		public static double dailyLuck = -0.1;
		public static int luckLevel = 0;
		public static int day = 0;
		public static long gameId = 0;
		public static bool monsterMuskActive = false;

		public static Map SetLayouts3 = JsonConvert.DeserializeObject<Map>(System.IO.File.ReadAllText($@"Volcano/Volcano_SetPieces_3.json"));
		public static Map SetLayouts4 = JsonConvert.DeserializeObject<Map>(System.IO.File.ReadAllText($@"Volcano/Volcano_SetPieces_4.json"));
		public static Map SetLayouts8 = JsonConvert.DeserializeObject<Map>(System.IO.File.ReadAllText($@"Volcano/Volcano_SetPieces_8.json"));
		public static Map SetLayouts16 = JsonConvert.DeserializeObject<Map>(System.IO.File.ReadAllText($@"Volcano/Volcano_SetPieces_16.json"));
		public static Map SetLayouts32 = JsonConvert.DeserializeObject<Map>(System.IO.File.ReadAllText($@"Volcano/Volcano_SetPieces_32.json"));

		public static List<Item> BarrelContents(int x, int y)
		{
			List<Item> items = new List<Item>();
			Random r = Utility.CreateRandom(x, y * 10000.0, day, 0);
			if (r.NextDouble() < 0.1)
			{
				items.Add(Item.Get("(O)73"));
				//Game1.player.team.RequestLimitedNutDrops("VolcanoBarrel", location, x * 64, y * 64, 5);
			}
			//if (location is VolcanoDungeon dungeon && dungeon.level.Value == 5 && x == 34)
			//{
			//	Item item = ItemRegistry.Create("(O)851");
			//	item.Quality = 2;
			//	Game1.createItemDebris(item, new Vector2(x, y) * 64f, 1);
			//}
			/*else*/
			Item item = null;
			if (r.NextDouble() < 0.75)
			{
				if (r.NextDouble() < 0.8)
				{
					switch (r.Next(7))
					{
						case 0:
							item = Item.Get("(O)382");
							item.Stack = r.Next(1, 3);
							break;
						case 1:
							item = Item.Get("(O)384");
							item.Stack = r.Next(1, 4);
							break;
						case 2:
							item = Item.Get("DwarvishSentry");
							break;
						case 3:
							item = Item.Get("(O)380");
							item.Stack = r.Next(2, 6);
							break;
						case 4:
							item = Item.Get("(O)378");
							item.Stack = r.Next(2, 6);
							break;
						case 5:
							item = Item.Get("(O)66");
							break;
						case 6:
							item = Item.Get("(O)709");
							item.Stack = r.Next(2, 6);
							break;
					}
				}
				else
				{
					switch (r.Next(5))
					{
						case 0:
							item = Item.Get("(O)78");
							item.Stack = r.Next(1, 3);
							break;
						case 1:
							item = Item.Get("(O)749");
							item.Stack = r.Next(1, 3);
							break;
						case 2:
							item = Item.Get("(O)60");
							break;
						case 3:
							item = Item.Get("(O)64");
							break;
						case 4:
							item = Item.Get("(O)68");
							break;
					}
				}
			}
			else if (r.NextDouble() < 0.4)
			{
				switch (r.Next(9))
				{
					case 0:
						item = Item.Get("(O)72");
						break;
					case 1:
						item = Item.Get("(O)831");
						item.Stack = r.Next(1, 4);
						break;
					case 2:
						item = Item.Get("(O)833");
						item.Stack = r.Next(1, 3);
						break;
					case 3:
						item = Item.Get("(O)749");
						break;
					case 4:
						item = Item.Get("(O)386");
						break;
					case 5:
						item = Item.Get("(O)848");
						break;
					case 6:
						item = Item.Get("(O)856");
						break;
					case 7:
						item = Item.Get("(O)886");
						break;
					case 8:
						item = Item.Get("(O)688");
						break;
				}
			}
			else
			{
				item = Item.Get("DwarvishSentry");
			}

			items.Add(item);
			return items;
		}
		public static int GetSingleLevelSeed(int seed, int level, int lastLayout, bool specialExists = false, double dailyLuck = 0.0, int luckLevel = 0, bool shortcutUnlocked = false)
		{
			if (level == 5)
			{
				return 31;
			}
			if (level == 9)
			{
				return 30;
			}

			Volcano.dailyLuck = dailyLuck;
			Volcano.luckLevel = luckLevel;
			List<int> levels = new();
			float luckMultiplier = 1f + luckLevel * 0.035f + (float)dailyLuck / 2f;


			List<int> valid_layouts = new();
			for (int i = 1; i < 30; i++)
			{
				valid_layouts.Add(i);
			}

			Random layout_random = new Random(seed);
			if (level > 1 && layout_random.NextDouble() < 0.5 * luckMultiplier)
			{
				if (!specialExists)
				{
					for (int k = 32; k < 38; k++)
					{
						valid_layouts.Add(k);
					}
				}
			}

			if (shortcutUnlocked && layout_random.NextDouble() < 0.75)
			{
				for (int l = 38; l < 58; l++)
				{
					valid_layouts.Add(l);
				}
			}

			if (lastLayout != 0)
			{
				valid_layouts.Remove(lastLayout);
			}

			return valid_layouts[layout_random.Next(valid_layouts.Count)];

		}
		public static List<VolcanoFloor> GetLevels16(uint gameId, int day, double dailyLuck = 0.0, int luckLevel = 0, bool shortcutUnlocked = false, bool crackedGoldenCoconut = false, bool hasMonsterMuskActive = false)
		{
			Volcano.crackedGoldenCoconut = crackedGoldenCoconut;
			Volcano.dailyLuck = dailyLuck;
			Volcano.luckLevel = luckLevel;
			Volcano.monsterMuskActive = monsterMuskActive;
			List<VolcanoFloor> floors = new List<VolcanoFloor>();
			List<int> levels = new();
			bool foundSpecialLevel = false;
			float luckMultiplier = 1f + luckLevel * 0.035f + (float)dailyLuck / 2f;
			int lastLayout = 0;
			for (int level = 1; level < 10; level++)
			{
				int seed = Utility.CreateRandomSeed(day * (level + 1), level * 5152, (int)gameId / 2);
				if (level == 5)
				{
					lastLayout = 31;
					levels.Add(lastLayout);
					continue;
				}
				if (level == 9)
				{
					lastLayout = 30;
					levels.Add(lastLayout);
					VolcanoFloor floor = new VolcanoFloor(level, lastLayout, seed);
					floors.Add(floor);
					continue;
				}
				List<int> valid_layouts = new();
				for (int i = 1; i < 30; i++)
				{
					valid_layouts.Add(i);
				}

				Random layout_random = new Random(seed);
				if (level > 1 && layout_random.NextDouble() < 0.5 * luckMultiplier)
				{
					if (!foundSpecialLevel)
					{
						for (int k = 32; k < 38; k++)
						{
							valid_layouts.Add(k);
						}
					}
				}

				if (shortcutUnlocked && layout_random.NextDouble() < 0.75)
				{
					for (int l = 38; l < 58; l++)
					{
						valid_layouts.Add(l);
					}
				}

				if (lastLayout != 0)
				{
					valid_layouts.Remove(lastLayout);
				}

				lastLayout = valid_layouts[layout_random.Next(valid_layouts.Count)];

				if (lastLayout >= 32)
				{
					foundSpecialLevel = true;
				}

				levels.Add(lastLayout);
				{
					VolcanoFloor floor = new VolcanoFloor(level, lastLayout, seed);
					floors.Add(floor);
				}
			}
			return floors;
		}


		public static List<int> GetLevels(uint gameId, int day, double dailyLuck = 0.0, int luckLevel = 0)
		{
			List<int> levels = new();
			bool foundSpecialLevel = false;
			float luckMultiplier = 1f + luckLevel * 0.035f + (float)dailyLuck / 2f;
			int lastLayout = 0;
			for (int level = 1; level < 10; level++)
			{
				if (level == 5)
				{
					lastLayout = 31;
					levels.Add(lastLayout);
					continue;
				}
				if (level == 9)
				{
					lastLayout = 30;
					levels.Add(lastLayout);
					continue;
				}
				List<int> valid_layouts = new();
				for (int i = 1; i < 30; i++)
				{
					valid_layouts.Add(i);
				}


				Random layout_random = new Random(day * level + level * 5152 + (int)gameId / 2);
				if (level > 1 && layout_random.NextDouble() < 0.5 * luckMultiplier)
				{
					if (!foundSpecialLevel)
					{
						for (int k = 32; k < 38; k++)
						{
							valid_layouts.Add(k);
						}
					}
				}
				if (lastLayout != 0)
				{
					valid_layouts.Remove(lastLayout);
				}

				lastLayout = valid_layouts[layout_random.Next(valid_layouts.Count)];

				if (lastLayout >= 32)
				{
					foundSpecialLevel = true;
				}

				levels.Add(lastLayout);
			}
			return levels;
		}

	}
}
