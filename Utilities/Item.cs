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
    public class Item
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

		public int Stack;

        public string id;

        public static Dictionary<string, Item> Items = SetupItems();

        public static Dictionary<string, Item> Hats = SetupHats();

        public static Dictionary<string, Item> SetupItems()
        {
            Dictionary<string, Item> result = JsonConvert.DeserializeObject<Dictionary<string, Item>>(File.ReadAllText(@"data/Objects.json"));
            foreach (var item in result)
            {
                item.Value.id = item.Key;
            }
            return result;
        }

        public static Dictionary<string, Item> SetupHats()
        {
            Dictionary<string, Item> Hats = new();
            Dictionary<string, string> hats = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(@"data/hats.json"));
            foreach (var hat in hats)
            {
                var item = new Item();
                item.Name = hat.Value.Split("/")[0];
                Hats.Add(hat.Key, item);
            }
            return Hats;
        }

		public override string ToString()
		{
			string result = Name;
			if (Stack > 0)
			{
				result += " (" + Stack + ")";
			}
			return result;
		}

        public static Item Get(string id)
        {
            // Assume Object
            if (Items.ContainsKey(id))
            {

                return (Item)Items[id].MemberwiseClone();
            }

            if (id.StartsWith("(O)"))
            {
                return Get(id[3..]);
            }

            if (id.StartsWith("(H)"))
            {
                return Hats[id[3..]];
            }

            return new Item() { Name = id};
        }

		public bool IsBreakableStone()
		{
			if (Category == -999)
			{
				return this.Name == "Stone";
			}
			return false;
		}

		public bool IsTwig()
		{
			if (Category == -999)
			{
				return this.Name == "Twig";
			}
			return false;
		}

		public bool IsWeeds()
		{
			if (Category == -999)
			{
				return this.Name.ToLower().Contains("weeds");
			}
			return false;
		}

		public bool IsTapper()
		{
			// TODO
			return false;
		}
	}
}
