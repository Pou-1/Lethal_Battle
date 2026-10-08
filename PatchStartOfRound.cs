using HarmonyLib;
using Lethal_Battle.codes;
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
            if (Plugin.hasBattleStarted && ManageUI.UI_players_alive_and_kills != null && Plugin.hasMessageWonShowed)
            {
                ItemSpawn.DisableBattleVolume();
                ClosestPlayerAndPath.StopClosestOpponentPath();
                ManageUI.UIDelete();
                Plugin.hasMessageWonShowed = false;
                Plugin.hasBattleStarted = false;
            }
        }
    }
}
