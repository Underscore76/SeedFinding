/* Used to debug remix bundles */
using System;
using SeedFinding;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace SeedFinding.Bundles
{
    public class BundleGenerator
    {
        public static List<RandomBundleData> randomBundleData;

        public Dictionary<string, List<int>> bundleData;

        public Dictionary<string, int> itemNameLookup;

        public Random random;
        static BundleGenerator()
        {
            randomBundleData = JsonConvert.DeserializeObject<List<RandomBundleData>>(File.ReadAllText(@"data/RandomBundles.json"));
        }

        public Dictionary<string, List<int>> Generate(int seed)
        {
            random = new Random(seed*9);
            bundleData = new Dictionary<string, List<int>>();
            foreach (RandomBundleData area_data in randomBundleData)
            {
                List<int> index_lookups = new List<int>();
                string[] array = area_data.Keys.Trim().Split(' ');
                Dictionary<int, BundleData> selected_bundles = new Dictionary<int, BundleData>();
                string[] array2 = array;
                foreach (string index_string in array2)
                {
                    index_lookups.Add(int.Parse(index_string));
                }
                BundleSetData bundle_set = area_data.BundleSets.GetRandom(random);
                if (bundle_set != null)
                {
                    foreach (BundleData bundle_data4 in bundle_set.Bundles)
                    {
                        selected_bundles[bundle_data4.Index] = bundle_data4;
                    }
                }
                List<BundleData> random_bundle_pool = new List<BundleData>();
                foreach (BundleData bundle_data3 in area_data.Bundles)
                {
                    random_bundle_pool.Add(bundle_data3);
                }
                for (int i = 0; i < index_lookups.Count; i++)
                {
                    if (!selected_bundles.ContainsKey(i))
                    {
                        List<BundleData> index_bundles = new List<BundleData>();
                        foreach (BundleData bundle_data2 in random_bundle_pool)
                        {
                            if (bundle_data2.Index == i)
                            {
                                index_bundles.Add(bundle_data2);
                            }
                        }
                        if (index_bundles.Count > 0)
                        {
                            BundleData selected_bundle2 = index_bundles.GetRandom(random);
                            random_bundle_pool.Remove(selected_bundle2);
                            selected_bundles[i] = selected_bundle2;
                        }
                        else
                        {
                            foreach (BundleData bundle_data in random_bundle_pool)
                            {
                                if (bundle_data.Index == -1)
                                {
                                    index_bundles.Add(bundle_data);
                                }
                            }
                            if (index_bundles.Count > 0)
                            {
                                BundleData selected_bundle = index_bundles.GetRandom(random);
                                random_bundle_pool.Remove(selected_bundle);
                                selected_bundles[i] = selected_bundle;
                            }
                        }
                    }
                }
                foreach (int key in selected_bundles.Keys)
                {
                    BundleData data = selected_bundles[key];
                    StringBuilder string_data = new StringBuilder();
                    string_data.Append(data.Name);
                    string_data.Append("/");
                    var selectedItems = ParseItemList(string_data, data.Items, data.Pick, data.RequiredItems);
                    bundleData[data.Name] = selectedItems;
                }
            }
            return bundleData;
        }

        public string ParseRandomTags(string data)
        {
            int open_index2 = 0;
            do
            {
                open_index2 = data.LastIndexOf('[');
                if (open_index2 >= 0)
                {
                    int close_index = data.IndexOf(']', open_index2);
                    if (close_index == -1)
                    {
                        return data;
                    }
                    string value = (new List<string>(data.Substring(open_index2 + 1, close_index - open_index2 - 1).Split('|'))).GetRandom(random);
                    data = data.Remove(open_index2, close_index - open_index2 + 1);
                    data = data.Insert(open_index2, value);
                }
            }
            while (open_index2 >= 0);
            return data;
        }

        public string ParseItemString(string item_string)
        {
            string[] parts = item_string.Trim().Split(' ');
            int index2 = 0;
            int count = int.Parse(parts[index2]);
            index2++;
            //int quality = 0;
            if (parts[index2] == "NQ")
            {
                //quality = 0;
                index2++;
            }
            else if (parts[index2] == "SQ")
            {
                //quality = 1;
                index2++;
            }
            else if (parts[index2] == "GQ")
            {
                //quality = 2;
                index2++;
            }
            else if (parts[index2] == "IQ")
            {
                //quality = 3;
                index2++;
            }
            string item_name = string.Join(" ", parts, index2, parts.Length - index2);
            return item_name;
        }

        public List<int> ParseItemList(StringBuilder builder, string item_list, int pick_count, int required_items)
        {
            item_list = ParseRandomTags(item_list);
            string[] items = item_list.Split(',');
            List<string> item_strings = new List<string>();
            for (int j = 0; j < items.Length; j++)
            {
                string item = ParseItemString(items[j]);
                item_strings.Add(item);
            }
            if (pick_count < 0)
            {
                pick_count = item_strings.Count;
            }
            if (required_items < 0)
            {
                required_items = pick_count;
            }
            while (item_strings.Count > pick_count)
            {
                int index_to_remove = random.Next(item_strings.Count);
                item_strings.RemoveAt(index_to_remove);
            }
            return item_strings.Select(o => ObjectInfo.GetIndex(o)).ToList();
        }
    }
}
