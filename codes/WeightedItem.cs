using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using Newtonsoft.Json;

namespace Lethal_Battle.NewFolder
{
    internal class WeightedItem
    {
        public Item item;
        public float weight;

        public WeightedItem(Item item, float weight)
        {
            this.item = item;
            this.weight = weight;
        }

        public class ItemWeight
        {
            public string name { get; set; }
            public float value { get; set; }
        }

        public static List<ItemWeight> _reader { get; set; }
        public static int battleWithOneItemProba { get; set; } = 0;
        public static List<ItemWeight> battleWithOneItem { get; set; }

        public static Dictionary<string, float> WeightsByName { get; private set; } = new Dictionary<string, float>();

        public static void LoadWeightsFromJson(string jsonPath)
        {
            try
            {
                if (!File.Exists(jsonPath))
                {
                    Plugin.log.LogError("LETHAL BATTLE : JSON not found : " + jsonPath);
                    return;
                }

                string jsonContent = File.ReadAllText(jsonPath);
                _reader = JsonConvert.DeserializeObject<List<ItemWeight>>(jsonContent);

                if (_reader == null)
                {
                    Plugin.log.LogError("LETHAL BATTLE : JSON not valide");
                    return;
                }

                WeightsByName = _reader.ToDictionary(
                    x => x.name.Trim().ToUpper(),
                    x => x.value
                );

            }
            catch (Exception ex)
            {
                Plugin.log.LogError("LETHAL BATTLE : " + ex.Message);
            }
        }

        public static float GetWeight(string itemName)
        {
            string key = itemName.Trim().ToUpper();
            return WeightsByName.TryGetValue(key, out float weight) ? weight : 0f;
        }

        public static List<WeightedItem> GetBattleItemsWeighted(List<Item> allItems)
        {
            string jsonPath = Path.Combine(Paths.PluginPath, "MikuT4T-LethalBattle/items.json");
            LoadWeightsFromJson(jsonPath);

            if (_reader == null)
            {
                Plugin.log.LogError("LETHAL BATTLE : can't load JSON data !");
                return new List<WeightedItem>();
            }

            Random rand = new Random();
            int roll = rand.Next(0, 100);
            bool useBattle = roll <= battleWithOneItemProba;

            List<WeightedItem> result = new List<WeightedItem>();

            if (!useBattle)
            {
                WeightsByName = _reader.ToDictionary(
                    x => x.name.Trim().ToUpper(),
                    x => x.value
                );

                foreach (Item item in allItems)
                {
                    float weight = GetWeight(item.itemName);
                    result.Add(new WeightedItem(item, weight));
                }
            }
            else
            {
                Plugin.log.LogError("LETHAL BATTLE : battle with one item !");
                var list = battleWithOneItem;

                if (list == null || list.Count == 0)
                {
                    return GetBattleItemsWeighted(allItems);
                }

                float totalWeight = list.Sum(i => i.value);
                float pick = (float)(rand.NextDouble() * totalWeight);

                ItemWeight chosen = null;
                float cumulative = 0f;

                foreach (var i in list)
                {
                    cumulative += i.value;
                    if (pick <= cumulative)
                    {
                        chosen = i;
                        break;
                    }
                }

                if (chosen == null)
                    chosen = list.Last();

                Plugin.log.LogError($"LETHAL BATTLE : battle with {chosen.name} !");

                Item selectedItem = allItems.FirstOrDefault(it => it.itemName.Trim().ToUpper() == chosen.name.Trim().ToUpper());
                if (selectedItem != null)
                    result.Add(new WeightedItem(selectedItem, chosen.value));
                else
                    Plugin.log.LogError($"LETHAL BATTLE : can't find {chosen.name} !");
            }
            return result;
        }
    }
}
