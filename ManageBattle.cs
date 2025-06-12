using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using LethalLib.Modules;
using GameNetcodeStuff;

namespace Lethal_Battle
{
    internal class ManageBattle
    {
        public static void ItemsSpawner()
        {
            UI.UISpawn();
            HUDManager.Instance.DisplayTip("Time to kill !!!", " And die for some loosers...", true);

            if (GameNetworkManager.Instance.localPlayerController.IsHost == false)
            {
                return;
            }

            Plugin.log.LogError("=====================================");
            Plugin.log.LogError("LETHAL BATTLE : getting all items... c:");

            List<Item> scraps = itemForBattleList();

            string[] banScraps = BanScraps.getBannedScraps();
            Plugin.log.LogError("LETHAL BATTLE : removing non lethal items... qwq");

            scraps.RemoveAll(item =>
                banScraps.Any(banned =>
                    string.Equals(banned.Trim(), item.itemName.Trim().ToUpper(), System.StringComparison.OrdinalIgnoreCase)
                )
            );

            Plugin.log.LogError("LETHAL BATTLE : spawning items... >w<");

            for (int j = 0; j < StartOfRound.Instance.livingPlayers * 25; j++)
            {
                int randomInt = Random.Range(0, scraps.Count);
                Vector3 spawnPosition = PositionManager();
                if(scraps[randomInt] != null)
                {
                    if (scraps[randomInt].spawnPrefab)
                    {
                        Plugin.log.LogError(scraps[randomInt].itemName);
                        SpawnScrap(scraps[randomInt], spawnPosition);
                    }
                }
            }
            Plugin.log.LogError("LETHAL BATTLE : items spawned sucessfully !!! UwU");
            Plugin.log.LogError("=====================================");
        }

        private static List<Item> itemForBattleList()
        {
            List<Item> scraps = new List<Item>();

            foreach (var item in Items.shopItems)
                scraps.Add(item.item);

            foreach (var item in Items.plainItems)
                scraps.Add(item.item);

            foreach (var item in Items.scrapItems)
                scraps.Add(item.item);

            foreach (var item in StartOfRound.Instance.allItemsList.itemsList)
                scraps.Add(item);

            return scraps;
        }

        private static Vector3 PositionManager()
        {
            Vector3 spawnPosition = RoundManager.Instance.outsideAINodes[Random.Range(0, RoundManager.Instance.outsideAINodes.Length)].transform.position;
            Vector3 spawnPositionTemp = spawnPosition;
            spawnPosition += new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
            spawnPosition = RoundManager.Instance.GetNavMeshPosition(spawnPosition);
            if (!RoundManager.Instance.GotNavMeshPositionResult)
            {
                spawnPosition = spawnPositionTemp;
            }
            Plugin.log.LogError(spawnPosition);
            return spawnPosition;
        }

        public static void MakeShipLeave(StartMatchLever shipLever)
        {
            if (shipLever != null)
            {
                shipLever.triggerScript.animationString = "SA_PushLeverBack";
                shipLever.leverHasBeenPulled = false;
                shipLever.triggerScript.interactable = false;
                shipLever.leverAnimatorObject.SetBool("pullLever", false);
            }
            StartOfRound.Instance.ShipLeave();
        }

        private static void SpawnScrap(Item scrap, Vector3 position)
        {
            GameObject gameObject =
                UnityEngine.Object.Instantiate(scrap.spawnPrefab, position, Quaternion.identity, RoundManager.Instance.spawnedScrapContainer);

            GrabbableObject component = gameObject.GetComponent<GrabbableObject>();
            component.transform.rotation = Quaternion.Euler(component.itemProperties.restingRotation);
            component.fallTime = 0f;
            component.scrapValue = 0;
            component.NetworkObject.Spawn();
        }

        public static List<PlayerControllerB> GetPlayers()
        {
            List<PlayerControllerB> rawList = StartOfRound.Instance.allPlayerScripts.ToList();
            List<PlayerControllerB> updatedList = new List<PlayerControllerB>(rawList);
            foreach (var p in rawList)
            {
                if (!p.IsSpawned || !p.isPlayerControlled)
                {
                    updatedList.Remove(p);
                }
            }
            return updatedList;
        }
    }
}
