using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using Unity.Netcode;

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

            foreach (var item in StartOfRound.Instance.allItemsList.itemsList)
                scraps.Add(item);

            string[] banScraps = BanScraps.getBannedScraps();

            scraps.RemoveAll(item =>
                banScraps.Any(banned =>
                    string.Equals(banned.Trim(), item.itemName.Trim().ToUpper(), StringComparison.OrdinalIgnoreCase)
                )
            );

            Vector3 spawnPosition = new Vector3(24, -11, 12);

            Plugin.log.LogError("scraps======================");
            for (int i = 0; i < scraps.Count; i++)
            {
                Plugin.log.LogError(scraps[i].name);
                //SpawnScrap(scraps[i], spawnPosition);
            }

            Plugin.log.LogError("SPAWN==========================");
            Plugin.log.LogError(scraps[1].itemName);
            SpawnScrap(scraps[1], spawnPosition);

            //StartOfRound.Instance.allPlayerObjects.Length;
        }

        private static void SpawnScrap(Item scrap, Vector3 position)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate(scrap.spawnPrefab, position,
                Quaternion.identity, RoundManager.Instance.spawnedScrapContainer.transform);

            GrabbableObject component = gameObject.GetComponent<GrabbableObject>();
            component.transform.rotation = Quaternion.Euler(component.itemProperties.restingRotation);
            component.fallTime = 0f;
            component.scrapValue = 0;
            component.FallToGround(true);
            NetworkObject netObj = gameObject.GetComponent<NetworkObject>();
            netObj.Spawn();
            //component.NetworkObject.Spawn();
            component.FallToGround(true);
        }
    }
}
