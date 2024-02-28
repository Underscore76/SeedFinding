using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SeedFinding.ObjectInfo;

namespace SeedFinding
{
    internal class Item
    {
        [JsonProperty("Name")]
        public string Name;

        [JsonProperty("Texture")]
        public string Texture;

        [JsonProperty("SpriteIndex")]
        public int SpriteIndex;

        [JsonProperty("ArtifactSpotChances")]
        public Dictionary<string, float> ArtifactSpotChances;

        [JsonProperty("Type")]
        public string Type;

        [JsonProperty("Category")]
        public int Category;

        [JsonProperty("Price")]
        public int Price;

        public string id;

        public static Dictionary<string, Item> Items = SetupItems();

        public static Dictionary<string, Item> SetupItems()
        {
            Dictionary<string, Item> result = JsonConvert.DeserializeObject<Dictionary<string, Item>>(File.ReadAllText(@"data/Objects.json"));
            foreach (var item in result)
            {
                item.Value.id = item.Key;
            }
            return result;
        }

        public static Item Get(string id)
        {
            return Items[id];
        }
    }
}
