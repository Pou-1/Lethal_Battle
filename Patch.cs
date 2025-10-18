using System.Linq;
using GameNetcodeStuff;
using HarmonyLib;
using Lethal_Battle.codes;
using Lethal_Battle.NewFolder;
using LethalLib.Modules;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static LethalLib.Modules.ContentLoader;

namespace Lethal_Battle
{
    [HarmonyPatch(typeof(RoundManager))]

    internal class Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("FinishGeneratingNewLevelClientRpc")]
        public static void Changes() // is called on the start of a new level
        {
            if (!Plugin.hasBattleStarted && TimeOfDay.Instance.daysUntilDeadline == 0 && TimeOfDay.Instance.currentLevel.planetHasTime == false && TimeOfDay.Instance.currentLevel.spawnEnemiesAndScrap == false)
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

        [HarmonyPatch(typeof(HUDManager), "SubmitChat_performed")]
        [HarmonyPrefix]
        public static bool Prefix(HUDManager __instance, InputAction.CallbackContext context)
        {
            if (!context.performed)
                return true;

            string text = __instance.chatTextField.text;
            if (string.IsNullOrEmpty(text))
                return true;

            PlayerControllerB local = GameNetworkManager.Instance.localPlayerController;
            if (local == null)
                return true;

            StartOfRound manager = local.playersManager;
            if (manager == null)
                return true;

            if (Plugin.verifying)
            {
                if (text.Equals("CONFIRM", System.StringComparison.OrdinalIgnoreCase))
                {
                    ResetTextbox(__instance, local);
                    ManageChat.AcceptRestart(manager);
                    ManageBattle.ItemsSpawner();
                    Plugin.hasBattleStarted = true;
                    return false;
                }

                if (text.Equals("DENY", System.StringComparison.OrdinalIgnoreCase))
                {
                    ResetTextbox(__instance, local);
                    ManageChat.DeclineRestart();
                    return false;
                }
            }

            if (text.Equals("/battle", System.StringComparison.OrdinalIgnoreCase))
            {
                ResetTextbox(__instance, local);
                if (!GameNetworkManager.Instance.isHostingGame)
                {
                    ManageChat.SendChatMessage("Only the host can start a battle.");
                    return false;
                }

                if (TimeOfDay.Instance.currentLevel.planetHasTime || TimeOfDay.Instance.currentLevel.spawnEnemiesAndScrap)
                {
                    ManageChat.SendChatMessage("Must be in a company battle.");
                    return false;
                }

                if (Plugin.hasBattleStarted)
                {
                    ManageChat.SendChatMessage("Must not be in a battle.");
                    return false;
                }

                ManageChat.ConfirmRestart();

                return false;
            }
            return true;
        }

        private static void ResetTextbox(HUDManager manager, PlayerControllerB local)
        {
            local.isTypingChat = false;
            manager.chatTextField.text = "";
            EventSystem.current.SetSelectedGameObject(null);
            manager.PingHUDElement(manager.Chat, 2f, 1f, 0.2f);
            manager.typingIndicator.enabled = false;
        }
    }
}
