using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Lethal_Battle.NewFolder;

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
