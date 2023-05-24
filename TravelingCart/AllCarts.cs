/* this was an attempt to dump all cart items once, and then shift
 * from compute to IO, turns out at least for me I'm way more IO than compute bound
 * so it ended up not being very useful.
 */
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace SeedFinding.Cart
{
	public class AllCartGenerator
	{
		public static int SeedMin, SeedMax;
		public static List<List<CompressedCartItem>> Carts;
		public static string[] SeedFiles;

		public static void DumpSeeds(int seedStart, int seedEnd)
		{
			using (var stream = File.Open($"./carts/{seedStart.ToString("D10")}.bin", FileMode.Create))
			{
				using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
				{
					writer.Write(seedStart);
					writer.Write(seedEnd);
					for (int seed = seedStart; seed < seedEnd; seed++)
					{
						foreach (var item in CompressedTravelingCart.GetStock(seed))
						{
							writer.Write(item.State);
						}
					}
				}
			}
			SeedFiles = GetFiles();
		}
		static AllCartGenerator()
		{
			SeedFiles = GetFiles();
		}
		private static string[] GetFiles()
		{
			return Directory.GetFiles("./carts/", "*.bin").OrderBy(s => s).ToArray();
        }

		private static string GetDumpFile(int seed)
		{
			string file = $"./carts/{seed.ToString("D10")}.bin";
			for (int i = 0; i < SeedFiles.Length; i++)
			{
				if (String.Compare(SeedFiles[i], file) > 0) return SeedFiles[i - 1];
                if (String.Compare(SeedFiles[i], file) == 0) return SeedFiles[i];

            }
			throw new ArgumentOutOfRangeException("could not find seed file");

        }
		private static IEnumerable<List<CompressedCartItem>> GetCarts(BinaryReader reader)
		{
			while (reader.BaseStream.Position < reader.BaseStream.Length)
			{
				yield return new(){
					new CompressedCartItem(reader.ReadUInt16()),
                    new CompressedCartItem(reader.ReadUInt16()),
                    new CompressedCartItem(reader.ReadUInt16()),
                    new CompressedCartItem(reader.ReadUInt16()),
                    new CompressedCartItem(reader.ReadUInt16()),
                    new CompressedCartItem(reader.ReadUInt16()),
                    new CompressedCartItem(reader.ReadUInt16()),
                    new CompressedCartItem(reader.ReadUInt16()),
                    new CompressedCartItem(reader.ReadUInt16()),
                    new CompressedCartItem(reader.ReadUInt16()),
                };
			}
		}
		private static void LoadDumpFile(string fileName)
		{
			using (var stream = File.Open(fileName, FileMode.Open))
			{
				using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
				{
					SeedMin = reader.ReadInt32();
                    SeedMax = reader.ReadInt32();
					int numSeeds = SeedMax - SeedMin + 1;
                    Carts = new List<List<CompressedCartItem>>();
					byte[] raw = reader.ReadBytes(2 * 10 * numSeeds);
					ushort[] states = new ushort[10 * numSeeds];
					Buffer.BlockCopy(raw, 0, states, 0, raw.Length);
					for (int i = 0; i < numSeeds; i++)
					{
						var items = new List<CompressedCartItem>();
						for (int j = 0; j < 10; j++)
						{
							items.Add(new CompressedCartItem(states[i * 10 + j]));
						}
						Carts.Add(items);
					}
				}
            }
        }

		public static List<CompressedCartItem> GetSeed(int seed)
		{
			if (seed < SeedMin || SeedMax <= seed)
			{
				LoadDumpFile(GetDumpFile(seed));
			}
			return Carts[seed - SeedMin];
		}

		public static List<List<CompressedCartItem>> GetSeeds(int seedStart, int seedEnd)
		{
			if (seedEnd < seedStart)
			{
				throw new ArgumentOutOfRangeException("seedEnd must be >= than seedStart");
			}
			List<List<CompressedCartItem>> carts = new List<List<CompressedCartItem>>();
			for (int seed = seedStart; seed < seedEnd; seed++)
			{
				carts.Add(GetSeed(seed));
			}
			return carts;
        }
	}
}

