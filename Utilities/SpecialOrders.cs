using Newtonsoft.Json;
using SeedFinding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace SeedFinding
{
    public class SpecialOrders
    {
        public static Dictionary<string, SpecialOrderData> allOrders = JsonConvert.DeserializeObject<Dictionary<string, SpecialOrderData>>(File.ReadAllText(@"data/SpecialOrders.json"));


        [ThreadStatic]
        public static bool sewingMachineAccess;
        [ThreadStatic]
        public static bool islandAccess;
        [ThreadStatic]
        public static bool resortAccess;
        [ThreadStatic]
        public static bool qiBeansActive;
        [ThreadStatic]
        public static bool qiDropBoxActive;

        public static Version version1_6 = new Version("1.6");

        public SpecialOrders()
        {
        }

        public static List<SpecialOrder> GetOrders(int gameId, int day, string version = "1.6", List<string> completedOrders = null, List<string>activeOrders = null)
        {
            var orders = new List<SpecialOrder>();

            day = (day - 1) / 7 * 7 + 1;

            int dayOfMonth = ((day - 1) % 28) + 1;

            Season season = Utility.getSeasonFromDay(day);
            SpecialOrder.workingSeason = season;

            List<String> validOrders = new List<String>();
            foreach (var pair in allOrders) { 
                string key = pair.Key;
                var order = pair.Value;

                bool invalid = false;
                if (!invalid && order.Repeatable != "True" && completedOrders != null && completedOrders.Contains(key))
                {
                    invalid = true;
                }
                if (dayOfMonth >= 16 && order.Duration == "Month")
                {
                    invalid = true;
                }
                if (!invalid && !SpecialOrder.CheckTags(order.RequiredTags))
                {
                    invalid = true;
                }
                if (!invalid && activeOrders != null && activeOrders.Contains(key))
                {
                    invalid = true;
                }

                if (!invalid)
                {
                    validOrders.Add(key);
                }
            }

            Version gameVersion = new Version(version);
            Random r;
            if (gameVersion >= version1_6)
            {
                r = Utility.CreateRandom(gameId, (double)day * 1.3);
            }
            else
            {
                r = new Random(gameId + (int)(day * 1.3f));
            }
            string[] array = new string[2]
            {
                "",
                "Qi"
            };
            foreach (string type_to_find in array)
            {
                if (gameVersion >= version1_6)
                {
                    r = Utility.CreateRandom(gameId, (double)day * 1.3);
                }
                List<string> typed_keys = new List<string>();
                foreach (string key3 in validOrders)
                {
                    if (allOrders[key3].OrderType == type_to_find)
                    {
                        typed_keys.Add(key3);
                    }
                }
                List<string> all_keys = new List<string>(typed_keys);
                if (type_to_find != "Qi")
                {
                    for (int j = 0; j < typed_keys.Count; j++)
                    {
                        if (completedOrders != null && completedOrders.Contains(typed_keys[j]))
                        {
                            typed_keys.RemoveAt(j);
                            j--;
                        }
                    }
                }
                for (int i = 0; i < 2; i++)
                {
                    if (typed_keys.Count == 0)
                    {
                        if (all_keys.Count == 0)
                        {
                            break;
                        }
                        typed_keys = new List<string>(all_keys);
                    }
                    int index = r.Next(typed_keys.Count);
                    string key2 = typed_keys[index];
                    orders.Add(SpecialOrder.GetSpecialOrder(key2, r.Next()));
                    typed_keys.Remove(key2);
                    all_keys.Remove(key2);
                }
            }
            return orders;
        }
    }

    public class SpecialOrderData
    {
        [JsonProperty("Name")]
        public string Name;
        [JsonProperty("Duration")]
        public string Duration;
        [JsonProperty("Repeatable")]
        public string Repeatable;
        [JsonProperty("RequiredTags")]
        public string RequiredTags;
        [JsonProperty("OrderType")]
        public string OrderType;
        [JsonProperty("Text")]
        public string Text;
        [JsonProperty("RandomizedElements")]
        public List<RandomizedElement> RandomizedElements;
        [JsonProperty("Objectives")]
        public List<SpecialOrderObjectiveData> Objectives;

        public SpecialOrderData() { }
    }
    public class RandomizedElement
    {
        [JsonProperty("Name")]
        public string Name;
        [JsonProperty("Values")]
        public List<RandomizedElementItem> Values;

        public RandomizedElement() { }
    }
    public class RandomizedElementItem
    {
        [JsonProperty("RequiredTags")]
        public string RequiredTags;
        [JsonProperty("Value")]
        public string Value;

        public RandomizedElementItem() { }
    }
    public class SpecialOrderObjectiveData
    {
        [JsonProperty("Type")]
        public string Type;
        [JsonProperty("Text")]
        public string Text;
        [JsonProperty("RequiredCount")]
        public string RequiredCount;
        [JsonProperty("Data")]
        public Dictionary<string, string> Data;

        public SpecialOrderObjectiveData() { }
    }

    public class SpecialOrder
    {
        public static Season workingSeason = Season.Spring;
        public int generationSeed;
        public string questKey;
        public string name;
        protected SpecialOrderData _orderData;
        public Dictionary<string, int> selectedRandomElements = new Dictionary<string, int>();
        public Dictionary<string, string> preSelectedItems = new Dictionary<string, string>();
        public string duration;
        public string questDescription;
        public List<OrderObjective> objectives = new List<OrderObjective>();
        public static SpecialOrder GetSpecialOrder(string key, int? generation_seed, string version = "1.6")
        {
            Dictionary<string, SpecialOrderData> order_data = SpecialOrders.allOrders;

            if (order_data.ContainsKey(key))
            {
                Random r;
                Version gameVersion = new Version(version);
                if (gameVersion >= SpecialOrders.version1_6)
                {
                    r = Utility.CreateRandom(generation_seed.Value);
                }
                else
                {
                    r = new Random(generation_seed.Value);
                }
                SpecialOrderData data = order_data[key];
                SpecialOrder order = new SpecialOrder();
                order.generationSeed = generation_seed.Value;
                order._orderData = data;
                order.questKey = key;
                order.name = data.Name;
                order.selectedRandomElements.Clear();
                if (data.RandomizedElements != null)
                {
                    foreach (RandomizedElement randomized_element in data.RandomizedElements)
                    {
                        List<int> valid_indices = new List<int>();
                        for (int i = 0; i < randomized_element.Values.Count; i++)
                        {
                            if (SpecialOrder.CheckTags(randomized_element.Values[i].RequiredTags))
                            {
                                valid_indices.Add(i);
                            }
                        }
                        int selected_index = Utility.GetRandom(valid_indices, r);
                        order.selectedRandomElements[randomized_element.Name] = selected_index;
                        string value = randomized_element.Values[selected_index].Value;
                        if (value.StartsWith("PICK_ITEM"))
                        {
                            value = value.Substring("PICK_ITEM".Length);
                            string[] array = value.Split(',');
                            List<string> valid_item_ids = new List<string>();
                            string[] array2 = array;
                            for (int j = 0; j < array2.Length; j++)
                            {
                                string valid_item_name = array2[j].Trim();
                                if (valid_item_name.Length == 0)
                                {
                                    continue;
                                }
                                if (char.IsDigit(valid_item_name[0]))
                                {
                                    int item_id = -1;
                                    if (int.TryParse(valid_item_name, out item_id))
                                    {
                                        valid_item_ids.Add(Item.Get(item_id.ToString()).Name);
                                    }
                                }
                                else
                                {
                                    valid_item_ids.Add(valid_item_name);
                                }
                            }
                            order.preSelectedItems[randomized_element.Name] = Utility.GetRandom(valid_item_ids, r);
                        }
                        else
                        {
                            int target = -1;
                            string[] array3 = value.Split('|');
                            for (int k = 0; k < array3.Length; k++)
                            {
                                if (array3[k] == "Target" || array3[k] == "Tags")
                                {
                                    target = k + 1;
                                    break;
                                }
                            }

                            if (target != -1)
                            {
                                order.preSelectedItems[randomized_element.Name] = array3[target];
                            }
                        }
                    }
                }
                order.duration = data.Duration;
                order.questDescription = data.Text;
                foreach (SpecialOrderObjectiveData objective_data in data.Objectives)
                {
                    OrderObjective objective = null;
                    Type objective_type = Type.GetType("StardewValley." + objective_data.Type.Trim() + "Objective");
                    if (!(objective_type == null) && objective_type.IsSubclassOf(typeof(OrderObjective)))
                    {
                        objective = (OrderObjective)Activator.CreateInstance(objective_type);
                        if (objective != null)
                        {
                            objective.description = order.Parse(objective_data.Text);
                            objective.maxCount = int.Parse(order.Parse(objective_data.RequiredCount));
                            order.objectives.Add(objective);
                        }
                    }
                }

                return order;

            }
            return null;
        }

        protected static bool CheckTag(string tag)
        {
            if (tag == "NOT_IMPLEMENTED")
            {
                return false;
            }
            if (tag.StartsWith("dropbox_QiChallengeBox"))
            {
                if (SpecialOrders.qiDropBoxActive)
                    return true;
            }
            if (tag.StartsWith("rule_QI_BEANS"))
            {
                if (SpecialOrders.qiBeansActive)
                    return true;
            }
            if (tag.StartsWith("season_"))
            {
                string value4 = tag.Substring("season_".Length);
                if (workingSeason.ToString().ToLower() == value4)
                {
                    return true;
                }
            }
            else if (tag.StartsWith("mail_Island_Resort"))
            {
                if (SpecialOrders.resortAccess)
                    return true;
            }
            else if (tag.StartsWith("event_992559"))
            {
                if (SpecialOrders.sewingMachineAccess)
                    return true;
            }
            else
            {
                if (tag == "island")
                {
                    if (SpecialOrders.islandAccess)
                    {
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }

        public static bool CheckTags(string tag_list)
        {
            if (tag_list == null)
            {
                return true;
            }
            List<string> tags = new List<string>();
            string[] array = tag_list.Split(',');
            foreach (string tag in array)
            {
                tags.Add(tag.Trim());
            }
            foreach (string item in tags)
            {
                string current_tag = item;
                if (current_tag.Length != 0)
                {
                    bool match = true;
                    if (current_tag[0] == '!')
                    {
                        match = false;
                        current_tag = current_tag.Substring(1);
                    }
                    if (CheckTag(current_tag) != match)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public virtual string Parse(string data)
        {
            data = data.Trim();
            int open_index = 0;
            do
            {
                open_index = data.LastIndexOf('{');
                if (open_index < 0)
                {
                    continue;
                }
                int close_index = data.IndexOf('}', open_index);
                if (close_index == -1)
                {
                    return data;
                }
                string inner = data.Substring(open_index + 1, close_index - open_index - 1);
                string value = inner;
                string key = inner;
                string subkey = null;
                if (inner.Contains(":"))
                {
                    string[] split2 = inner.Split(':');
                    key = split2[0];
                    if (split2.Length > 1)
                    {
                        subkey = split2[1];
                    }
                }
                if (this._orderData.RandomizedElements != null)
                {
                    if (this.preSelectedItems.ContainsKey(key))
                    {
                        value = this.preSelectedItems[key];

                    }
                    else if (this.selectedRandomElements.ContainsKey(key))
                    {
                        foreach (RandomizedElement randomized_element in this._orderData.RandomizedElements)
                        {
                            if (randomized_element.Name == key)
                            {
                                value = randomized_element.Values[this.selectedRandomElements[key]].Value;
                                break;
                            }
                        }
                    }
                }
                if (subkey != null)
                {
                    string[] split = value.Split('|');
                    for (int i = 0; i < split.Length; i += 2)
                    {
                        if (i + 1 <= split.Length && split[i] == subkey)
                        {
                            value = split[i + 1];
                            break;
                        }
                    }
                }
                data = data.Remove(open_index, close_index - open_index + 1);
                data = data.Insert(open_index, value);
            }
            while (open_index >= 0);
            return data;
        }

        public string GetDescription()
        {
            string description = name;

            if (preSelectedItems.Count != 0)
            {
                string items = "";
                foreach (KeyValuePair<string, string> pair in preSelectedItems)
                {
                    if (items != "")
                        items += ", ";
                    items += pair.Value;
                }
                description += " (" + items + ")";
            }

            return description;
        }
    }
    public class OrderObjective
    {

        public string description;
        public int maxCount;
    }
}
