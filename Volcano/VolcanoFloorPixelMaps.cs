using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using SkiaSharp;
using System.Threading.Tasks;

namespace SeedFinding.Volcano
{
	public enum PixelType
	{
		StartPosition,
		EndPosition,
		Empty,
		PossibleSwitchPosition,
		OutOfBounds,
		Wall,
		Lava,
		Open,
		Unknown,
		SpawnTile,
		SpikerSpawnTile
	}
	public static class PixelTypeColor
	{
		public readonly static SKColor OutOfBounds = new(0, 0, 0, 255);
		public readonly static SKColor Open = new(255, 255, 255, 255);

		public readonly static SKColor EndPosition = new(255, 0, 0, 255);
		public readonly static SKColor StartPosition = new(0, 255, 0, 255);
		public readonly static SKColor Lava = new(0, 0, 255, 255);

		public readonly static SKColor PossibleSwitchPosition = new(128, 128, 128, 255);
		public readonly static SKColor Wall = new(127, 255, 0, 255);

		public readonly static SKColor Empty = new(255, 255, 0, 255);
		public readonly static SKColor Unknown = new(255, 0, 255, 255);
		public readonly static SKColor SpawnTile = new(0, 255, 255, 255);
		public readonly static SKColor SpikerSpawnTile = new(0, 128, 255, 255);
	}
	public class PixelMap
	{
		public int Layout;
		public Point StartPosition;
		public Point EndPosition;
		public List<Point> PossibleSwitchPositions;
		public PixelType[,] Map;
		public PixelMap(int layout)
		{
			Layout = layout;
			Map = new PixelType[64, 64];
			PossibleSwitchPositions = new List<Point>();
		}
		public void SetPixel(int x, int y, PixelType type, bool flipped = false)
		{
			if (x < 0 || x >= 64 || y < 0 || y >= 64)
			{
				return;
			}
			if (flipped)
			{
				x = 63 - x;
			}
			Map[x, y] = type;
		}
		public PixelType GetPixel(int x, int y, PixelType default_type = PixelType.OutOfBounds)
		{
			if (x < 0 || x >= 64 || y < 0 || y >= 64)
			{
				return default_type;
			}
			return Map[x, y];
		}
		public PixelMap Clone()
		{
			PixelMap clone = new PixelMap(Layout);
			for (int x = 0; x < 64; x++)
			{
				for (int y = 0; y < 64; y++)
				{
					clone.Map[x, y] = Map[x, y];
				}
			}
			clone.StartPosition = StartPosition;
			clone.EndPosition = EndPosition;
			clone.PossibleSwitchPositions = new List<Point>(PossibleSwitchPositions);
			return clone;
		}
	}
	public class VolcanoFloorPixelMaps
	{

		public static Dictionary<int, Tuple<PixelMap, PixelMap>> PixelMaps;

		static VolcanoFloorPixelMaps()
		{
			Initialize();
		}
		public static void Initialize()
		{
			PixelMaps = new();
			SetupLayouts();
		}

		public static void SetupLayouts()
		{
			SKBitmap LayoutImage = SKBitmap.Decode($@"Volcano/Layouts.png");
			for (int layout = 1; layout <= 57; layout++)
			{
				SKBitmap image = new();
				PixelMap pixelMap = new(layout);
				PixelMap pixelMapFlipped = new(layout);
				if (LayoutImage.ExtractSubset(image, new SKRectI(layout % 10 * 64, layout / 10 * 64, layout % 10 * 64 + 64, layout / 10 * 64 + 64)))
				{
					for (int x = 0; x < 64; x++)
					{
						for (int y = 0; y < 64; y++)
						{
							SKColor c = image.GetPixel(x, y);
							switch (c)
							{
								case var _ when c.Equals(PixelTypeColor.StartPosition):
									pixelMap.SetPixel(x, y, PixelType.StartPosition);
									pixelMapFlipped.SetPixel(x, y, PixelType.StartPosition, flipped: true);
									pixelMap.StartPosition = new Point(x, y);
									pixelMapFlipped.StartPosition = new Point(63 - x, y);
									break;
								case var _ when c.Equals(PixelTypeColor.EndPosition):
									pixelMap.SetPixel(x, y, PixelType.EndPosition);
									pixelMapFlipped.SetPixel(x, y, PixelType.EndPosition, flipped: true);
									pixelMap.EndPosition = new Point(x, y);
									pixelMapFlipped.EndPosition = new Point(63 - x, y);
									break;
								case var _ when c.Equals(PixelTypeColor.Empty):
									// This gets immediately set to an open tile on generation
									pixelMap.SetPixel(x, y, PixelType.Open);
									pixelMapFlipped.SetPixel(x, y, PixelType.Open, flipped: true);
									break;
								case var _ when c.Equals(PixelTypeColor.PossibleSwitchPosition):
									pixelMap.SetPixel(x, y, PixelType.PossibleSwitchPosition);
									pixelMapFlipped.SetPixel(x, y, PixelType.PossibleSwitchPosition, flipped: true);
									pixelMap.PossibleSwitchPositions.Add(new Point(x, y));
									pixelMapFlipped.PossibleSwitchPositions.Add(new Point(63 - x, y));
									break;
								case var _ when c.Equals(PixelTypeColor.Wall):
									pixelMap.SetPixel(x, y, PixelType.Wall);
									pixelMapFlipped.SetPixel(x, y, PixelType.Wall, flipped: true);
									Console.WriteLine($"Wall at ({x}, {y}) in layout {layout}");
									break;
								case var _ when c.Equals(PixelTypeColor.Lava):
									pixelMap.SetPixel(x, y, PixelType.Lava);
									pixelMapFlipped.SetPixel(x, y, PixelType.Lava, flipped: true);
									break;
								case var _ when c.Equals(PixelTypeColor.OutOfBounds):
									pixelMap.SetPixel(x, y, PixelType.OutOfBounds);
									pixelMapFlipped.SetPixel(x, y, PixelType.OutOfBounds, flipped: true);
									break;
								case var _ when c.Equals(PixelTypeColor.Open):
									pixelMap.SetPixel(x, y, PixelType.Open);
									pixelMapFlipped.SetPixel(x, y, PixelType.Open, flipped: true);
									break;
								case var _ when c.Equals(PixelTypeColor.SpawnTile):
									pixelMap.SetPixel(x, y, PixelType.SpawnTile);
									pixelMapFlipped.SetPixel(x, y, PixelType.SpawnTile, flipped: true);
									break;
								default:
									throw new Exception($"Unknown pixel color [{c.Red},{c.Green},{c.Blue},{c.Alpha}] at ({x}, {y}) in layout {layout}");
							}
						}
					}
					PixelMaps[layout] = Tuple.Create(pixelMap, pixelMapFlipped);
				}
				else
				{
					throw new Exception($"Failed to extract pixel map for layout {layout}");
				}
			}
		}

		public static PixelMap GetPixelMap(int layout, bool flipped)
		{
			if (PixelMaps.TryGetValue(layout, out var maps))
			{
				return flipped ? maps.Item2.Clone() : maps.Item1.Clone();
			}
			throw new Exception($"Pixel map for layout {layout} not found");
		}
	}
}
