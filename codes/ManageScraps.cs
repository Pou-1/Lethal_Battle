using System.Collections.Generic;
using System.Linq;
using LethalLib.Modules;
using UnityEngine;

namespace Lethal_Battle.NewFolder
{
    internal class ManageScraps
    {
        private static readonly float shopWeight = 1.0f;
        private static readonly float plainWeight = 1.0f;
        private static readonly float scrapWeight = 0.1f;
        private static readonly float allItemsWeight = 4f;

        public static List<WeightedItem> GetFilteredWeightedItems()
        {
            List<WeightedItem> items = new List<WeightedItem>();

            Plugin.log.LogError("shopItems");
            foreach (var item in Items.shopItems)
            {
                items.Add(new WeightedItem(item.item, GetItemWeight(item.item, shopWeight)));
                Plugin.log.LogError(item.item.itemName);

            }

            Plugin.log.LogError("plainItems");
            foreach (var item in Items.plainItems)
            {
                items.Add(new WeightedItem(item.item, GetItemWeight(item.item, plainWeight)));
                Plugin.log.LogError(item.item.itemName);
            }

            Plugin.log.LogError("scrapItems");
            foreach (var item in Items.scrapItems)
            {
                items.Add(new WeightedItem(item.item, GetItemWeight(item.item, scrapWeight)));
                Plugin.log.LogError(item.item.itemName);
            }

            Plugin.log.LogError("allItemsList");
            foreach (var item in StartOfRound.Instance.allItemsList.itemsList)
            {
                items.Add(new WeightedItem(item, GetItemWeight(item, allItemsWeight)));
                Plugin.log.LogError(item.itemName);
            }
            Plugin.log.LogError("==============================");

            string[] banned = BanScraps.getBannedScraps();
            items.RemoveAll(wi =>
                banned.Any(b =>
                    string.Equals(b.Trim(), wi.item.itemName.Trim().ToUpper(), System.StringComparison.OrdinalIgnoreCase)
                )
            );

            return items;
        }

        public static Item GetRandomItem(List<WeightedItem> weightedItems)
        {
            float totalWeight = weightedItems.Sum(wi => wi.weight);
            float rand = Random.Range(0f, totalWeight);
            float current = 0f;

            foreach (var wi in weightedItems)
            {
                current += wi.weight;
                if (rand <= current)
                {
                    Plugin.log.LogError(wi.item.itemName + " - " + wi.weight);
                    return wi.item;
                }
            }

            return weightedItems[0].item;
        }

        private static float GetItemWeight(Item item, float baseWeight)
        {
            string name = item.itemName.Trim().ToLower();

            if (name.Contains("shovel")) return baseWeight * 40f;

            return baseWeight;
        }

        public static void SpawnScrap(Item scrap, Vector3 position)
        {
            GameObject gameObject =
                Object.Instantiate(scrap.spawnPrefab, position, Quaternion.identity, RoundManager.Instance.spawnedScrapContainer);

            GrabbableObject component = gameObject.GetComponent<GrabbableObject>();
            component.transform.rotation = Quaternion.Euler(component.itemProperties.restingRotation);
            component.fallTime = 0f;
            component.scrapValue = 0;
            component.NetworkObject.Spawn();
        }
    }
}
