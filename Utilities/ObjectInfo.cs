using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data;

namespace SeedFinding
{
    public class ObjectInfo
    {
        public struct ObjectData
        {
            public string Name;
            public int Cost;
            public int Id;
            public int Quality;
            public ObjectData(int id, string name, int cost, int quality=0) { Id = id; Name = name; Cost = cost; Quality = quality; }
            public override string ToString()
            {
                return string.Format("[{0:D3} {1}]", Id, Name);
            }
        }
        public static Dictionary<int, ObjectData> Items;
        public static Dictionary<string, int> ItemsToIndex;
        public static Dictionary<string, string> ObjectInformation = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(@"data/object_info.json"));
        static ObjectInfo()
        {
            Items = new Dictionary<int, ObjectData>();
            ItemsToIndex = new Dictionary<string, int>();
            foreach (var kvp in ObjectInformation)
            {
                int id = Int32.Parse(kvp.Key);
                string[] tokens = kvp.Value.Split("/");
                Items.Add(id, new ObjectData(id, tokens[0], Int32.Parse(tokens[1])));
                if (!ItemsToIndex.ContainsKey(tokens[0]))
                    ItemsToIndex.Add(tokens[0], id);
            }
        }

        public static ObjectData Get(int id)
        {
            return Items[id];
        }

        public static int GetIndex(string name)
        {
            if (int.TryParse(name, out int baseIndex))
            {
                return baseIndex;
            }
            if(ItemsToIndex.TryGetValue(name, out int index))
            {
                return index;
            }
            return -1;
        }
    }
}
