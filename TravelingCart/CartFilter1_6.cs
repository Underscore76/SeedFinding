using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using _Cart = SeedFinding.Cart1_6.CompressedTravelingCart1_6;

namespace SeedFinding.Cart1_6
{
    public class CartFilter
    {
        public HashSet<int> FilterItems;
        public int MaxPriceTotal;
        public int ItemsToFind;
        public CartFilter(HashSet<int> filterItems,int maxPrice,int quantity)
        {
            FilterItems = filterItems;
            MaxPriceTotal = maxPrice;
            ItemsToFind = quantity;
        } 
        public bool CheckSeedForQuantity(int seed,int quantity)
        {
            var cart = _Cart.GetStock(seed);
            HashSet<int> found = new();
            int currentPrice = 0;
            foreach (var item in cart)
            {   
                if (!FilterItems.Contains(item.Id)) continue;
                currentPrice+=item.Cost;
                found.Add(item.Id);
                if (currentPrice > MaxPriceTotal) return false;
                if (found.Count == ItemsToFind) return true;
            }
            return false;
        }
        public static HashSet<int> CheckSeedRange(int startSeed, int endSeed, Func<int,int,bool> checkSeed,int quantity)
        {
            HashSet<int> validSeeds = new();
            for (int seed = startSeed; seed < endSeed; seed++)
            {
                if (checkSeed(seed,quantity)) validSeeds.Add(seed); 
            }
            return validSeeds;
        }
        public static double ParallelSearch(int startSeed,int numSeeds,int blockSize,HashSet<int> filterItems,int maxPrice, out List<int> cartSeeds,int quantity)
        {
            CartFilter cartFilter = new(filterItems,maxPrice,quantity);
            Stopwatch stopwatch = Stopwatch.StartNew();
            var bag = new ConcurrentBag<int>();
            var partioner = Partitioner.Create(startSeed,numSeeds,blockSize);
            Func<int,int,bool> checkSeed = cartFilter.CheckSeedForQuantity;
            HashSet<int> validSeeds;
            Parallel.ForEach(partioner, (range,loopState) =>
            {
                validSeeds = CheckSeedRange(range.Item1,range.Item2,checkSeed,quantity);
                foreach (var seed in validSeeds) 
                {
                    bag.Add(seed); 
                }
            });
            double time = stopwatch.Elapsed.TotalSeconds;
            cartSeeds = bag.ToList();
            cartSeeds.Sort();
            return time;
        }
    }
}