using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using LethalLib.Modules;
using GameNetcodeStuff;
using BepInEx;
using System.IO;

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

            int numberOfItems = 100;
            if (TimeOfDay.Instance.currentLevel.PlanetName.ToUpper() == "98 GALETRY")
            {
                numberOfItems = 200;
            }
            for (int j = 0; j < numberOfItems; j++)
            {
                Item item = ManageScraps.GetRandomItem(scraps);
                Vector3 spawnPosition = PositionManager();

                if (item != null && item.spawnPrefab != null)
                {
                    ManageScraps.SpawnScrap(item, spawnPosition);
                }
            }

            Plugin.log.LogInfo("LETHAL BATTLE : items spawned successfully !!! UwU");
        }

        private static List<Vector3> GalatryNodesList()
        {
            List<Vector3> artificialNodes = new List<Vector3>();

            artificialNodes.Add(new Vector3(-74, 21, -16));
            artificialNodes.Add(new Vector3(-129, 1, -15));
            artificialNodes.Add(new Vector3(-194, 1, -27));
            artificialNodes.Add(new Vector3(-70, 1, 17));
            artificialNodes.Add(new Vector3(-70, 1, 17));
            artificialNodes.Add(new Vector3(-70, 1, 17));
            artificialNodes.Add(new Vector3(-70, 1, 17));
            artificialNodes.Add(new Vector3(-70, 1, 17));
            artificialNodes.Add(new Vector3(-36, 1, -14));
            artificialNodes.Add(new Vector3(-2, -2, 20));
            artificialNodes.Add(new Vector3(-2, 1, 47));
            artificialNodes.Add(new Vector3(-2, -1, -1));
            artificialNodes.Add(new Vector3(56, -1, 13));
            artificialNodes.Add(new Vector3(-27, 1, 11));
            artificialNodes.Add(new Vector3(23, 1, -26));
            artificialNodes.Add(new Vector3(-8, -2, -27));
            artificialNodes.Add(new Vector3(-104, 11, -16));
            artificialNodes.Add(new Vector3(-51, 20, -13));
            artificialNodes.Add(new Vector3(-31, 11, -2));
            artificialNodes.Add(new Vector3(-31, 11, -26));
            artificialNodes.Add(new Vector3(46, -1, -8));
            artificialNodes.Add(new Vector3(-66, 1, -44));

            return artificialNodes;
        }

        private static Vector3 PositionManager()
        {
            Vector3 spawnPosition;

            if (TimeOfDay.Instance.currentLevel.PlanetName.ToUpper() != "98 GALETRY")
            {
                spawnPosition = RoundManager.Instance.outsideAINodes[Random.Range(0, RoundManager.Instance.outsideAINodes.Length)].transform.position;
            }
            else
            {
                List<Vector3> artificialNodes = GalatryNodesList();
                spawnPosition = artificialNodes[Random.Range(0, artificialNodes.Count)];
            }
            spawnPosition = RoundManager.Instance.GetRandomNavMeshPositionInRadius(spawnPosition, 20);
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
