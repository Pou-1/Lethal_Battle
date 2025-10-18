using GameNetcodeStuff;
using HarmonyLib;
using Lethal_Battle.NewFolder;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Lethal_Battle
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class PatchStartOfRound
    {
        [HarmonyPrefix]
        [HarmonyPatch("EndOfGame")]
        public static void ChangesDeleteUI()
        {
            if (Plugin.hasBattleStarted && Plugin.instance.UI_players_alive_and_kills != null)
            {
                UI.UIDelete();
                Plugin.hasMessageWonShowed = false;
                Plugin.hasBattleStarted = false;
            }
        }
    }
}
