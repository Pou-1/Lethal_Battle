using HarmonyLib;
using Lethal_Battle.NewFolder;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Lethal_Battle
{
    [HarmonyPatch(typeof(RoundManager))]

    internal class PatchRoundManager
    {
        [HarmonyPostfix]
        [HarmonyPatch("FinishGeneratingNewLevelClientRpc")]
        public static void Changes()
        {
            if (!Plugin.hasBattleStarted && StartOfRound.Instance.livingPlayers > 1 && TimeOfDay.Instance.daysUntilDeadline == 0 && TimeOfDay.Instance.currentLevel.planetHasTime == false && TimeOfDay.Instance.currentLevel.spawnEnemiesAndScrap == false)
            {
                Plugin.log.LogInfo("LETHAL BATTLE : In a company for the last phase!");
                int potentialBodiesValue = 5 * (StartOfRound.Instance.allPlayerObjects.Length - 1);
                int scrapsValue = UnityEngine.Object.FindObjectsOfType<GrabbableObject>().Where(o => o.itemProperties.isScrap && o.itemProperties.minValue > 0
                    && (!(o is StunGrenadeItem g) || !g.hasExploded || !g.DestroyGrenade)
                    && (o.isInShipRoom == true && o.isInElevator == true)).ToList().Sum(s => s.scrapValue);

                if (scrapsValue + potentialBodiesValue + TimeOfDay.Instance.quotaFulfilled >= TimeOfDay.Instance.profitQuota)
                {
                    Plugin.log.LogInfo("LETHAL BATTLE : No battle because you have the qota ! :3");
                }
                else
                {
                    Plugin.log.LogInfo("LETHAL BATTLE : Battle because ur too poor, the company want blood !");
                    ManageBattle.ItemsSpawner();
                    Plugin.hasBattleStarted = true;
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        public static void ChangesUI_death()
        {
            if (Plugin.hasBattleStarted)
            {
                StartMatchLever shipLever = UnityEngine.Object.FindObjectOfType<StartMatchLever>();
                shipLever.triggerScript.interactable = false;
                //ShipTeleporter shipTeleporter = UnityEngine.Object.FindObjectOfType<ShipTeleporter>();
                //shipTeleporter.cooldownAmount = 9999999f;

                if (Plugin.instance.UI_players_alive_and_kills != null && !Plugin.hasMessageWonShowed)
                {
                    ManageUI.UpdateUI();
                }

                if (StartOfRound.Instance.livingPlayers == 1 && !Plugin.hasMessageWonShowed)
                {
                    string winnerUsername = ManageBattle.GetPlayers()[0].playerUsername;
                    HUDManager.Instance.DisplayTip(winnerUsername + " won !!!", "some loosers are here ...", true);

                    ManageBattle.MakeShipLeave(shipLever);
                    
                    Plugin.hasMessageWonShowed = true;
                }
            }
        }
    }
}
