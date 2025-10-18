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

                var list = JsonConvert.DeserializeObject<List<ItemWeight>>(jsonContent);

                if (list == null || list.Count == 0)
                {
                    Plugin.log.LogError("Aucun item trouvé dans le JSON !");
                    return;
                }

                WeightsByName = list.ToDictionary(
                    x => x.name.Trim().ToUpper(),
                    x => x.value
                );

                Plugin.log.LogInfo(WeightsByName.Count + " items chargés depuis " + jsonPath);
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

        public static List<WeightedItem> GetBattleItemsWeighted(List<Item> allitems)
        {
            string jsonPath = Path.Combine(Paths.PluginPath, "Lethal_Battle/items.json");
            LoadWeightsFromJson(jsonPath);
            List<WeightedItem> result = new List<WeightedItem>();

            foreach (Item item in allitems)
            {
                Plugin.log.LogInfo(item.itemName.ToUpper());
                float weight = GetWeight(item.itemName);
                result.Add(new WeightedItem(item, weight));
            }

            return result;
        }
        
        private class ItemWeight
        {
            public string name { get; set; }
            public float value { get; set; }
        }
    }
}
