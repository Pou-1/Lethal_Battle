using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using Newtonsoft.Json;

namespace Lethal_Battle.NewFolder
{
    internal class ManageJson
    {
        public Item item;
        public float weight;

        public ManageJson(Item item, float weight)
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
        public static int battleWithOneItemProba { get; set; } = 30;
        public static List<ItemWeight> battleWithOneItem { get; set; }

        public static bool battleJsonLoaded = false;

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

                if (_reader == null || _reader.Count == 0)
                {
                    Plugin.log.LogError("LETHAL BATTLE : JSON not valid");
                    return;
                }

                WeightsByName = _reader.ToDictionary(
                    x => x.name.Trim().ToUpper(),
                    x => x.value
                );
            }
            catch (Exception ex)
            {
                Plugin.log.LogError("LETHAL BATTLE : " + ex);
            }
        }

        public static void LoadBattleWithOneItemJson(string jsonPath)
        {
            try
            {
                if (!File.Exists(jsonPath))
                {
                    Plugin.log.LogError("LETHAL BATTLE : Battle JSON not found : " + jsonPath);
                    return;
                }

                string jsonContent = File.ReadAllText(jsonPath);
                battleWithOneItem = JsonConvert.DeserializeObject<List<ItemWeight>>(jsonContent);

                if (battleWithOneItem == null || battleWithOneItem.Count == 0)
                {
                    Plugin.log.LogError("LETHAL BATTLE : Battle JSON invalid or empty");
                    battleWithOneItem = new List<ItemWeight>();
                    return;
                }

                battleJsonLoaded = true;
                Plugin.log.LogInfo($"LETHAL BATTLE : Loaded {battleWithOneItem.Count} battle items");
            }
            catch (Exception ex)
            {
                Plugin.log.LogError("LETHAL BATTLE : " + ex);
                battleWithOneItem = new List<ItemWeight>();
            }
        }

        public static float GetWeight(string itemName)
        {
            string key = itemName.Trim().ToUpper();
            return WeightsByName.TryGetValue(key, out float weight) ? weight : 0f;
        }

        public static List<ManageJson> GetBattleItemsWeighted(List<Item> allItems)
        {
            string baseJsonPath = Path.Combine(Paths.PluginPath, "MikuT4T-LethalBattle/items.json");
            string battleJsonPath = Path.Combine(Paths.PluginPath, "MikuT4T-LethalBattle/itemsForBattleWithOneItem.json");

            LoadWeightsFromJson(baseJsonPath);

            if (!battleJsonLoaded)
                LoadBattleWithOneItemJson(battleJsonPath);

            Random rand = new Random();
            int roll = rand.Next(0, 100);
            bool useBattle = roll < battleWithOneItemProba;

            List<ManageJson> result = new List<ManageJson>();

            // Battle with all items
            if (!useBattle)
            {
                foreach (Item item in allItems)
                {
                    float weight = GetWeight(item.itemName);
                    result.Add(new ManageJson(item, weight));
                }
                return result;
            }

            // Battle with one item
            if (battleWithOneItem == null || battleWithOneItem.Count == 0)
            {
                Plugin.log.LogError("LETHAL BATTLE : No battle items loaded");
                return result;
            }

            float totalWeight = battleWithOneItem.Sum(i => i.value);
            if (totalWeight <= 0f)
            {
                Plugin.log.LogError("LETHAL BATTLE : Battle total weight is zero");
                return result;
            }

            float pick = (float)(rand.NextDouble() * totalWeight);
            float cumulative = 0f;
            ItemWeight chosen = null;

            foreach (var i in battleWithOneItem)
            {
                cumulative += i.value;
                if (pick <= cumulative)
                {
                    chosen = i;
                    break;
                }
            }

            chosen ??= battleWithOneItem.Last();

            Item selectedItem = allItems.FirstOrDefault(
                it => it.itemName.Trim().ToUpper() == chosen.name.Trim().ToUpper()
            );

            if (selectedItem == null)
            {
                Plugin.log.LogError($"LETHAL BATTLE : Item not found in game : {chosen.name}");
                return result;
            }

            Plugin.log.LogInfo($"LETHAL BATTLE : battle with {chosen.name}");
            result.Add(new ManageJson(selectedItem, chosen.value));

            return result;
        }

    }
}
