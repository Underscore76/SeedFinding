using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SeedFinding.Locations1_6
{
    public class Map
    {
        public string Name { get; set; }

        [JsonProperty("height")]
        public int Height;

        [JsonProperty("width")]
        public int Width;

        [JsonProperty("layers")]
        public List<Layer> Layers;

        [JsonProperty("tilesets")]
        public List<TileSet> TileSets;

        public Layer FindLayer(string layerName)
        {
            foreach (var layer in Layers)
            {
                if (layer.Name == layerName)
                {
                    return layer;
                }
            }
            return null;

        }

        public bool isTilePassable(int x, int y)
        {
            if (doesTileHaveProperty(x, y, "Passable", "Back") != null)
            {
                return false;
            }

            int index = getTileIndexAt(x, y, "Buildings");
            if (index != 0 && doesTileHaveProperty(x, y, "Shadow", "Buildings") == null && doesTileHaveProperty(x, y, "Passable", "Buildings") == null)
            {
                return false;
            }
            return true;
        }

        public int distanceToLand(int x, int y, bool landMustBeAdjacentToWalkableTile = false)
        {
            Layer layer = FindLayer("Back");
            int distance = 0;
            int startx = x - 1;
            int starty = y - 1;
            int endx = x + 2;
            int endy = y + 2;
            int width = endx - startx;

            bool foundLand = false;

            while (!foundLand && width <= 11)
            {

                List<Point> vector2List = new List<Point>();
                for (int index = startx; index < endx; ++index)
                    vector2List.Add(new Point(index, starty));
                for (int index = starty + 1; index < endy; ++index)
                    vector2List.Add(new Point((endx - 1), index));
                for (int index = endx - 2; index >= startx; --index)
                    vector2List.Add(new Point(index, (endy - 1)));
                for (int index = endy - 2; index >= starty + 1; --index)
                    vector2List.Add(new Point(startx, index));

                foreach (Point position in vector2List)
                {
                    if (position.X > 0 && position.Y > 0 && position.X <= Width - 1 && position.Y <= Height - 1)
                    {
                        int tileIndex = layer.GetTileIndex(position.X, position.Y);
                        Tile tile = FindTile(tileIndex);
                        if (tile == null || (tile.HasProperty("Water") != null))
                        {
                            foundLand = true;
                            distance = width / 2;
                            if (!landMustBeAdjacentToWalkableTile)
                            {
                                break;
                            }
                            foundLand = false;


                            Point[] surroundingTileLocationsArray = Utility.getSurroundingTileLocationsArray(position);
                            foreach (Point surroundings in surroundingTileLocationsArray)
                            {
                                if (isTilePassable(surroundings.X, surroundings.Y) && !isWaterTile(position.X, position.Y))
                                {
                                    foundLand = true;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
                startx--;
                starty--;
                endx++;
                endy++;
                width = endx - startx;
            }
            if (width > 11)
                distance = 6;

            distance--;
            return distance;
        }

        public string doesTileHaveProperty(int x, int y, string property, string layerName, bool ignoreTileSheetProperties = false)
        {
            // Hardcoded due to laziness
            if (Name == "Mountains" && property == "NoFishing" && layerName == "Back")
            {
				if ((x == 47 || x == 48) && (y == 3 || y == 4 || y == 5))
				{
					return "Y";
				}
				return null;
            }
            Tile tile = FindTile(getTileIndexAt(x, y, layerName));
            if (tile == null)
            {
                return null;
            }
            return tile.HasProperty(property);
        }

        public int getTileIndexAt(int x, int y, string layerName)
        {
            Layer layer = FindLayer(layerName);
            if (layer == null)
            {
                return 0;
            }
            return layer.GetTileIndex(x, y);
        }

        public string getTileSheetIDAt(int x, int y, string layerName)
        {
            Layer layer = FindLayer(layerName);
            if (layer == null)
            {
                return "";
            }
            int index = layer.GetTileIndex(x, y);

            TileSet tileSet = FindTileSet(index);

            if (tileSet == null)
            {
                return "";
            }
            return tileSet.Name;
        }

        public bool isOpenWater(int xTile, int yTile)
        {
            if (!this.isWaterTile(xTile, yTile))
            {
                return false;
            }
            int tile_index = this.getTileIndexAt(xTile, yTile, "Buildings");
            if (tile_index != 0)
            {
                bool tile_blocked = true;
                if (this.getTileSheetIDAt(xTile, yTile, "Buildings") == "outdoors" && (tile_index == 759 || tile_index == 628 || tile_index == 629 || tile_index == 734))
                {
                    tile_blocked = false;
                }
                if (tile_blocked)
                {
                    return false;
                }
            }
            return true;
        }

        public bool isWaterTile(int xTile, int yTile)
        {
            int tileIndex = getTileIndexAt(xTile, yTile, "Back");
            Tile tile = FindTile(tileIndex);
            return (tile != null && tile.HasProperty("Water") != null);
        }

        public TileSet FindTileSet(int id)
        {
            TileSet previous = null;
            foreach (var tileSet in TileSets)
            {
                // Skip until in range
                if (id > tileSet.Firstgid)
                {
                    previous = tileSet;
                    continue;
                }
                break;
            }
            return previous;
        }

        public Tile FindTile(int id)
        {
            var tileSet = FindTileSet(id);

            if (tileSet == null)
            {
                return null;
            }

            // Tileset has no specific tile information
            if (tileSet.Tiles == null)
            {
                return null;
            }

            // Loop through tiles until a match is found
            foreach (var tile in tileSet.Tiles)
            {
                if (tile.Id + tileSet.Firstgid == id)
                {
                    return tile;
                }
            }
            return null;
        }

		public virtual bool IsNoSpawnTile(int x, int y, string type = "All", bool ignoreTileSheetProperties = false)
		{
			string noSpawn = this.doesTileHaveProperty(x, y, "NoSpawn", "Back", ignoreTileSheetProperties);
			switch (noSpawn)
			{
				case "Grass":
				case "Tree":
					if (type == noSpawn)
					{
						return true;
					}
					break;
				default:
					{
						if (!bool.TryParse(noSpawn, out var isBanned) || isBanned)
						{
							return true;
						}
						break;
					}
				case null:
					break;
			}
			return false;
		}

		public virtual bool doesEitherTileOrTileIndexPropertyEqual(int xTile, int yTile, string propertyName, string layerName, string propertyValue)
		{
			Layer layer = FindLayer(layerName);
			if (layer != null)
			{
				Tile tmp = FindTile(getTileIndexAt(xTile, yTile, layerName));
				//if (tmp != null && tmp.TileIndexProperties.TryGetValue(propertyName, out var property2) && property2 == propertyValue)
				//{
				//	return true;
				//}
				if (tmp != null && tmp.HasProperty(propertyName) == propertyValue)
				{
					return true;
				}
			}
			return propertyValue == null;
		}
	}
    public class Layer
    {
        [JsonProperty("data")]
        public int[] Data;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("height")]
        public int Height;

        [JsonProperty("width")]
        public int Width;

        public int GetTileIndex(int x, int y)
        {
            int index = x + Width * y;
            if (index > Data.Length)
            {
                return 0;
            }
            return Data[index];
        }
    }

    public class TileSet
    {
        [JsonProperty("firstgid")]
        public int Firstgid { get; set; }

        [JsonProperty("tiles")]
        public List<Tile> Tiles;

        [JsonProperty("name")]
        public string Name;
    }

    public class Tile
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("properties")]
        public List<Dictionary<string, string>> Properties;

        public string HasProperty(string name)
        {
            if (Properties == null)
            {
                return null;
            }
            foreach (var prop in Properties)
            {

                if (prop["name"] == name)
                {
                    return prop["value"];
                }
            }
            return null;
        }
    }
}
