using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using Unity.Netcode;
using LethalLib.Modules;

namespace Lethal_Battle
{
    internal class ManageBattle
    {
        public static void ItemsSpawner()
        {
            if (GameNetworkManager.Instance.localPlayerController.IsHost == false)
            {
                return;
            }

            List<Item> scraps = new List<Item>();


            foreach (var item in Items.shopItems)
                scraps.Add(item.item);

            foreach (var item in Items.plainItems)
                scraps.Add(item.item);

            foreach (var item in Items.scrapItems)
                scraps.Add(item.item);

            foreach (var item in StartOfRound.Instance.allItemsList.itemsList)
                scraps.Add(item);

            string[] banScraps = BanScraps.getBannedScraps();

            scraps.RemoveAll(item =>
                banScraps.Any(banned =>
                    string.Equals(banned.Trim(), item.itemName.Trim().ToUpper(), StringComparison.OrdinalIgnoreCase)
                )
            );

            Vector3 spawnPosition = new Vector3(24, -11, 12);
            System.Random random = new System.Random();

            Plugin.log.LogError("scraps======================");

            for (int i = 0; i < scraps.Count; i++)
            {
                    Plugin.log.LogError(scraps[i].name);
            }

            Plugin.log.LogError("SPAWN==========================");

            int randomInt = random.Next(scraps.Count);
            for (int j = 0; j < StartOfRound.Instance.allPlayerObjects.Length * 2.5; j++)
            {
                if(scraps[j].spawnPrefab && scraps[j] != null)
                {
                    SpawnScrap(scraps[j], spawnPosition);
                }
            }
        }

        private static void SpawnScrap(Item scrap, Vector3 position)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate(scrap.spawnPrefab, position,
                Quaternion.identity, RoundManager.Instance.spawnedScrapContainer);

            GrabbableObject component = gameObject.GetComponent<GrabbableObject>();
            component.transform.rotation = Quaternion.Euler(component.itemProperties.restingRotation);
            component.fallTime = 0f;
            component.scrapValue = 0;
            NetworkObject netObj = gameObject.GetComponent<NetworkObject>();
            component.NetworkObject.Spawn();
            component.FallToGround(true);
        }
    }
}
