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

        public static Dictionary<string, float> WeightsByName { get; private set; } = new Dictionary<string, float>();

        private static RootJson _cachedRoot;

        public static void LoadWeightsFromJson(string jsonPath)
        {
            try
            {
                if (!File.Exists(jsonPath))
                {
                    Plugin.log.LogError("Fichier JSON introuvable : " + jsonPath);
                    return;
                }

                string jsonContent = File.ReadAllText(jsonPath);
                _cachedRoot = JsonConvert.DeserializeObject<RootJson>(jsonContent);

                if (_cachedRoot == null || _cachedRoot.items == null)
                {
                    Plugin.log.LogError("Erreur : le fichier JSON est vide ou invalide !");
                    return;
                }

                Plugin.log.LogInfo($"JSON chargé depuis {jsonPath} : {_cachedRoot.items.Count} items et {_cachedRoot.battleWithOneItem.Count} battle items, probabilité battle={_cachedRoot.battleWithOneItemProba}%");

                WeightsByName = _cachedRoot.items.ToDictionary(
                    x => x.name.Trim().ToUpper(),
                    x => x.value
                );

                Plugin.log.LogInfo("Aperçu des 5 premiers items :");
                foreach (var item in _cachedRoot.items.Take(5))
                    Plugin.log.LogInfo($" - {item.name} : {item.value}");
            }
            catch (Exception ex)
            {
                Plugin.log.LogError("Erreur lors du chargement du JSON : " + ex.Message);
            }
        }

        public static float GetWeight(string itemName)
        {
            string key = itemName.Trim().ToUpper();
            return WeightsByName.TryGetValue(key, out float weight) ? weight : 0f;
        }

        public static List<WeightedItem> GetBattleItemsWeighted(List<Item> allItems)
        {
            string jsonPath = Path.Combine(Paths.PluginPath, "Lethal_Battle/items.json");
            if (_cachedRoot == null)
                LoadWeightsFromJson(jsonPath);

            if (_cachedRoot == null)
            {
                Plugin.log.LogError("Impossible de charger les données JSON !");
                return new List<WeightedItem>();
            }

            Random rand = new Random();
            int roll = rand.Next(0, 100);
            bool useBattle = roll < _cachedRoot.battleWithOneItemProba;

            List<WeightedItem> result = new List<WeightedItem>();

            if (!useBattle)
            {
                Plugin.log.LogInfo($"Roll {roll} → utilisation de la liste principale (items)");
                WeightsByName = _cachedRoot.items.ToDictionary(
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
                Plugin.log.LogInfo($"Roll {roll} → mode battleWithOneItem !");
                var list = _cachedRoot.battleWithOneItem;

                if (list == null || list.Count == 0)
                {
                    Plugin.log.LogWarning("battleWithOneItem vide, fallback sur la liste principale !");
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

                Plugin.log.LogInfo($"Item choisi pour la bataille : {chosen.name} (poids {chosen.value})");

                Item selectedItem = allItems.FirstOrDefault(it => it.itemName.Trim().ToUpper() == chosen.name.Trim().ToUpper());
                if (selectedItem != null)
                    result.Add(new WeightedItem(selectedItem, chosen.value));
                else
                    Plugin.log.LogWarning($"Item {chosen.name} non trouvé dans la liste allItems !");
            }

            return result;
        }

        private class RootJson
        {
            public List<ItemWeight> items { get; set; }
            public int battleWithOneItemProba { get; set; }
            public List<ItemWeight> battleWithOneItem { get; set; }
        }

        private class ItemWeight
        {
            public string name { get; set; }
            public float value { get; set; }
        }
    }
}
