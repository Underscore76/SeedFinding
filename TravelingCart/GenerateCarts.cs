using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using SeedFinding.CartEncoding;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace SeedFinding.Cart1_6
{
	public class CartGenerator
	{
		public int SeedMin;
		public int SeedMax;
		public string Filter;
		public object lockobj = new();
		public BitMaskCartEncode CartEncode;
		public CartGenerator(int seedMin, int seedMax, string filter)
		{
			BMCPP.getFilter(filter,out var filterItems, out var filterItemsMultiple);
			SeedMin = seedMin;
			SeedMax = seedMax;
			Filter = filter;
			CartEncode = new(filterItems,filterItemsMultiple);
		}
		//file writing
		public void FileWrite(string fileName, List<string> lines, bool newFile=false)
		{
			if (newFile && File.Exists(fileName))
			{
				File.Delete(fileName);
			}
			using StreamWriter streamWriter = new StreamWriter(fileName);
			{
				foreach (string line in lines)
				{
					streamWriter.WriteLine(line);
				}
			}
		}
		//read file and rewrite to file in sorted order
		public void SortFile(string fileName)
		{
			if (!File.Exists(fileName))
			{
				throw new Exception("this file does not exist");
			}
			var lines = File.ReadLines(fileName);
			lines = lines.OrderBy(x => x.Split(",")[0]); //comma seperated in encoding
			FileWrite(fileName, lines.ToList(), true);
		}
		public void CartGen(int minSeed,int maxSeed,string fileName)
		{
			List<string> seedData = new();
			uint seedCart = 0;
			for (int seed=minSeed; seed<=maxSeed; seed++)
			{
				seedCart = CartEncode.ReducedStock(seed);
				if (seedCart == 0) continue;
				string seedValuePair = seed.ToString()+","+seedCart;
				seedData.Add(seedValuePair);
			}
			lock (lockobj) 
			{
				FileWrite(fileName, seedData);
			}
		}
		public double Search(int numSeeds,int blockSize,string fileName)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			var partitioner = Partitioner.Create(0,numSeeds,blockSize);
			Parallel.ForEach(partitioner, (range,blockSize) => 
			{				
				CartGen(range.Item1,range.Item2,fileName);
			});
			SortFile(fileName);
			return stopwatch.Elapsed.TotalSeconds;
		}
	}
}

