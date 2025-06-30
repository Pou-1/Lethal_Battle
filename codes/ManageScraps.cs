using System.Collections.Generic;
using System.Linq;
using LethalLib.Modules;
using UnityEngine;

namespace Lethal_Battle.NewFolder
{
    internal class ManageScraps
    {
        public static List<WeightedItem> GetFilteredWeightedItems()
        {
            List<Item> items = new List<Item>();

            foreach (var item in Items.shopItems)
                items.Add(item.item);

            foreach (var item in Items.plainItems)
                items.Add(item.item);

            foreach (var item in Items.scrapItems)
                items.Add(item.item);

            foreach (var item in StartOfRound.Instance.allItemsList.itemsList)
                items.Add(item);

            List<WeightedItem> itemsweighted = new List<WeightedItem>();
            itemsweighted = WeightedItem.GetBattleItemsWeighted(items);

            return itemsweighted;
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
                    return wi.item;
                }
            }

            return weightedItems[0].item;
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
