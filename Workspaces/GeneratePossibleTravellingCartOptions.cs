using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedFinding.Workspaces
{
    internal class GeneratePossibleTravellingCartOptions
    {
        void generate() {
            int[] offLimits = new int[] { 79,
163,
162,
161,
160,
159,
158,
305,
308,
326,
341,
413,
437,
439,
454,
460,
682,
681,
680,
645,
688,
690,
689,
774,
775,
797,
798,
799,
800,
801,
802,
803,
417,
807,
261,
279,
277,
447,
812,
292,
69,
91,
73,
289,
935};
            Dictionary<string, Item> objects = JsonConvert.DeserializeObject<Dictionary<string, Item>>(File.ReadAllText(@"data/Objects.json"));
            int? rangeMinId = 2;
            int? rangeMaxId = 789;
            bool rangeNegated = false;
            bool? categoryPresenceExpected = true;
            HashSet<int> categoriesRequired = null;
            HashSet<int> categoriesExcluded = new HashSet<int> { -999 };
            bool? objectTypePresenceExpected = null;
            HashSet<string> objectTypesRequired = null;
            HashSet<string> objectTypesExcluded = new HashSet<string> { "Quest", "Minerals", "Arch" };
            List<string> prefixesRequired = null;
            List<string> prefixesExcluded = null;
            HashSet<string> idsRequired = null;
            HashSet<string> idsExcluded = null;
            bool allowMissingPrice = false;
            bool requireExplicitCategory = true;
            int count = 1;
            List<string> possibleItems = new List<string>();
            foreach (KeyValuePair<string, Item> kvp in objects)
            //foreach (string itemId in typeDef.GetAllIds())
            {
                string id = kvp.Key;
                int.TryParse(id, out int index);
                Item data = kvp.Value;
                if (offLimits.Contains(index))
                {
                    continue;
                }
                if (!(index >= rangeMinId && index <= rangeMaxId))
                {
                    continue;
                }
                if ((!categoryPresenceExpected.HasValue || categoryPresenceExpected == data.Category < -1) && (categoriesRequired == null || categoriesRequired.Contains(data.Category)) && (categoriesExcluded == null || !categoriesExcluded.Contains(data.Category)) && (!requireExplicitCategory || data.Category < 0) && (!objectTypePresenceExpected.HasValue || objectTypePresenceExpected != string.IsNullOrWhiteSpace(data.Type)) && (objectTypesRequired == null || objectTypesRequired.Contains(data.Type)) && (objectTypesExcluded == null || !objectTypesExcluded.Contains(data.Type)))
                {
                    possibleItems.Add(id);
                }
            }

            foreach (var item in possibleItems)
            {
                Console.WriteLine(item);
            }
        }
    }
}
