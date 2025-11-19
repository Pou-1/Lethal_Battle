using GameNetcodeStuff;
using HarmonyLib;
using Lethal_Battle.codes;
using Lethal_Battle.NewFolder;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Lethal_Battle
{
    [HarmonyPatch(typeof(HUDManager))]
    internal class DebugCommandsManager
    {
        [HarmonyPostfix]
        [HarmonyPatch("AddChatMessage")]
        public static void AddChatMessageDebugCommand(string chatMessage)
        {
            if (!GameNetworkManager.Instance.isHostingGame || (GameNetworkManager.Instance.isHostingGame && (chatMessage != "/battle" || chatMessage != "CONFIRM" || chatMessage != "DENY")))
            {
                Plugin.log.LogError(chatMessage);
                return;
            }
            if (TimeOfDay.Instance.currentLevel.planetHasTime == false && TimeOfDay.Instance.currentLevel.spawnEnemiesAndScrap == false)
            {
                    if (chatMessage == "/battle")
                    {
                        ManageChat.ConfirmRestart();
                    }
                    if (chatMessage == "CONFIRM")
                    {
                        ManageBattle.ItemsSpawner();
                        Plugin.hasBattleStarted = true;
                    }

                    if (chatMessage == "DENY")
                    {
                        ManageChat.DeclineRestart();
                    }
            }
            else
            {
                ManageChat.SendChatMessage("Must be in a company.");
            }
        }
    }
}
