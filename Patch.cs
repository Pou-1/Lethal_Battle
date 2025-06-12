using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace Lethal_Battle
{
    [HarmonyPatch(typeof(RoundManager))]

    internal class Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("FinishGeneratingNewLevelClientRpc")]
        public static void Changes() // is called on the start of a new level
        {
            if (!Plugin.hasBattleStarted && TimeOfDay.Instance.daysUntilDeadline == 0 && TimeOfDay.Instance.currentLevel.PlanetName == "71 Gordion")
            {
                Plugin.log.LogError("In gordion for the last phase!");
                int potentialBodiesValue = 5 * (StartOfRound.Instance.allPlayerObjects.Length - 1); // Cout value of every player except one * 5 
                int scrapsValue = Object.FindObjectsOfType<GrabbableObject>().Where(o => o.itemProperties.isScrap && o.itemProperties.minValue > 0
                    && (!(o is StunGrenadeItem g) || !g.hasExploded || !g.DestroyGrenade)
                    && (o.isInShipRoom == true && o.isInElevator == true)).ToList().Sum(s => s.scrapValue);
                if (scrapsValue + potentialBodiesValue + TimeOfDay.Instance.quotaFulfilled >= TimeOfDay.Instance.profitQuota) // quota not reached and day is quota day
                {
                    Plugin.log.LogError("No battle !");
                }
                else
                {
                    Plugin.log.LogError("battle !");
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
                StartMatchLever shipLever = Object.FindObjectOfType<StartMatchLever>();
                shipLever.triggerScript.interactable = false;

                if (Plugin.instance.UI_players_alive_and_kills != null && !Plugin.hasMessageWonShowed)
                {
                    UI.UpdateUI();
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
