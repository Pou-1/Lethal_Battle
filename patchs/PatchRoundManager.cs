using GameNetcodeStuff;
using HarmonyLib;
using Lethal_Battle.codes;
using Lethal_Battle.NewFolder;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Lethal_Battle.patchs
{
    [HarmonyPatch(typeof(RoundManager))]

    internal class PatchRoundManager
    {
        private static bool winSequenceStarted = false;

        [HarmonyPostfix]
        [HarmonyPatch("FinishGeneratingNewLevelClientRpc")]
        public static void Changes()
        {
            if (!Plugin.hasBattleStarted && StartOfRound.Instance.livingPlayers >= 2 && TimeOfDay.Instance.daysUntilDeadline == 0 && TimeOfDay.Instance.currentLevel.planetHasTime == false && TimeOfDay.Instance.currentLevel.spawnEnemiesAndScrap == false)
            {
                Plugin.log?.LogInfo("In a company for the last phase!");

                int potentialBodiesValue = 5 * (StartOfRound.Instance.allPlayerObjects.Length - 1);

                int scrapsValue = UnityEngine.Object.FindObjectsOfType<GrabbableObject>().Where(o => o.itemProperties.isScrap && o.itemProperties.minValue > 0
                    && (!(o is StunGrenadeItem g) || !g.hasExploded || !g.DestroyGrenade)
                    && (o.isInShipRoom == true && o.isInElevator == true)).ToList().Sum(s => s.scrapValue);

                winSequenceStarted = false;

                if (scrapsValue + potentialBodiesValue + TimeOfDay.Instance.quotaFulfilled >= TimeOfDay.Instance.profitQuota)
                {
                    Plugin.log?.LogInfo(scrapsValue + potentialBodiesValue + TimeOfDay.Instance.quotaFulfilled >= TimeOfDay.Instance.profitQuota);
                }
                else
                {
                    Plugin.log?.LogInfo("Battle because ur too poor, the company want blood !");
                    RoundManager.Instance.StartCoroutine(ItemSpawn.SpawnItems());
                    Plugin.hasBattleStarted = true;
                }
            }
            else
            {
                Plugin.log?.LogError("FUCK U");

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

                if (ManageUI.UI_players_alive_and_kills != null && !Plugin.hasMessageWonShowed)
                {
                    ManageUI.UpdateUI();
                }

                if (StartOfRound.Instance.livingPlayers == 1 && !winSequenceStarted)
                {
                    winSequenceStarted = true;

                    RoundManager.Instance.StartCoroutine(ManageUI.WinScreenCoroutine(shipLever));
                }
            }
        }
    }
}
