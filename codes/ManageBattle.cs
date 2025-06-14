using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using LethalLib.Modules;
using GameNetcodeStuff;

namespace Lethal_Battle.NewFolder
{
    internal class ManageBattle
    {
        public static void ItemsSpawner()
        {
            UI.UISpawn();
            HUDManager.Instance.DisplayTip("Time to kill !!!", " And die for some loosers...", true);

            if (!GameNetworkManager.Instance.localPlayerController.IsHost)
                return;

            Plugin.log.LogInfo("LETHAL BATTLE : getting all items... c:");

            List<WeightedItem> scraps = ManageScraps.GetFilteredWeightedItems();

            for (int j = 0; j < 70; j++)
            {
                Item item = ManageScraps.GetRandomItem(scraps);
                Vector3 spawnPosition = PositionManager();

                if (item != null && item.spawnPrefab != null)
                {
                    //Plugin.log.LogError(item.itemName);
                    ManageScraps.SpawnScrap(item, spawnPosition);
                }
            }

            Plugin.log.LogInfo("LETHAL BATTLE : items spawned successfully !!! UwU");
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
                StartOfRound.Instance.ShipLeave();
            }
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
