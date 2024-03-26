// Generator class for Compressed Bundle Configurations
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Numerics;

namespace SeedFinding.Bundles1_6
{
    public class RemixedBundles
    {
        public static List<RandomBundleData> RandomBundles;
        public static List<CompRandomBundleData> CompRandomBundles;

        static RemixedBundles()
		{
            RandomBundles = JsonConvert.DeserializeObject<List<RandomBundleData>>(File.ReadAllText(@"data/RandomBundles1_6.json"));
            CompRandomBundles = new List<CompRandomBundleData>(RandomBundles.Select(o => new CompRandomBundleData(o)));
        }
        public static CompressedRemixBundles Generate(int seed)
        {
            Random random = Utility.CreateRandom((double)seed * 9.0);
            var result = new CompressedRemixBundles(0);
            foreach (var area_data in CompRandomBundles)
            {
                Dictionary<int, CompBundleData> selected_bundles = new Dictionary<int, CompBundleData>();
                CompBundleSetData bundle_set = random.ChooseFrom(area_data.BundleSets);
                if (bundle_set != null)
                {
                    foreach (CompBundleData bundle_data4 in bundle_set.Bundles)
                    {
                        selected_bundles[bundle_data4.Index] = bundle_data4;
                    }
                }
                List<CompBundleData> random_bundle_pool = new List<CompBundleData>(area_data.Bundles);
                for (int i = 0; i < area_data.Keys.Length; i++)
                {
                    if (selected_bundles.ContainsKey(i))
                    {
                        continue;
                    }
                    List<CompBundleData> index_bundles = new List<CompBundleData>();
                    foreach (var data in random_bundle_pool)
                    {
                        if (data.Index == i)
                        {
                            index_bundles.Add(data);
                        }
                    }
                    if (index_bundles.Count > 0)
                    {
                        CompBundleData selected_bundle = random.ChooseFrom(index_bundles);
                        random_bundle_pool.Remove(selected_bundle);
                        selected_bundles[i] = selected_bundle;
                        continue;
                    }
                    foreach (CompBundleData bundle_data in random_bundle_pool)
                    {
                        if (bundle_data.Index == -1)
                        {
                            index_bundles.Add(bundle_data);
                        }
                    }
                    if(index_bundles.Count > 0)
                    {
                        CompBundleData selected_bundle2 = random.ChooseFrom(index_bundles);
                        random_bundle_pool.Remove(selected_bundle2);
                        selected_bundles[i] = selected_bundle2;
                    }

                }
                foreach(int key in selected_bundles.Keys)
                {
                    CompBundleData data = selected_bundles[key];
                    result.State |= data.BaseFlag;
                    // handle random flags
                    List<BigInteger> flags = new List<BigInteger>();
                    if (data.HasRandom)
                    {
                        foreach (var flag in data.Flags)
                        {
                            flags.Add(random.ChooseFrom(flag));
                        }
                        flags.Reverse();
                    }
                    else
                    {
                        if (data.Flags.Count > 0)
                        {
                            flags.AddRange(data.Flags[0]);
                        }
                    }
                    // down select
                    int pick_count = data.Pick;
                    int require_items = data.RequiredItems;
                    if (pick_count < 0) pick_count = flags.Count;
                    if (require_items < 0) require_items = pick_count;
                    while (flags.Count > pick_count)
                    {
                        int index = random.Next(flags.Count);
                        flags.RemoveAt(index);
                    }
                    // set the flags
                    foreach(var flag in flags)
                    {
                        result.State |= flag;
                    }
                }
            }
            return result;
        }
	}

    
}

