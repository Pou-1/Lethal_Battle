using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;

namespace Lethal_Battle.NewFolder
{
    internal class UI
    {
        public static void UISpawn()
        {
            if (Plugin.instance.UI_Lethal_Battle != null)
            {
                Plugin.instance.numberOfPlayers = StartOfRound.Instance.livingPlayers;
                Plugin.instance.UI_players_alive_and_kills = UnityEngine.Object.Instantiate(Plugin.instance.UI_Lethal_Battle);
            }
        }

        public static void UpdateUI()
        {
            if (Plugin.instance.UI_players_alive_and_kills != null)
            {
                var texts = Plugin.instance.UI_players_alive_and_kills.GetComponentsInChildren<TMPro.TMP_Text>();
                texts[0].text = "/ " + (Plugin.instance.numberOfPlayers - 1);
                texts[1].text = Plugin.instance.numberOfPlayers - StartOfRound.Instance.livingPlayers + "";
            }
        }
        public static void UIDelete()
        {
            if (Plugin.instance.UI_players_alive_and_kills != null)
            {
                UnityEngine.Object.Destroy(Plugin.instance.UI_players_alive_and_kills);
            }
        }
    }
}
