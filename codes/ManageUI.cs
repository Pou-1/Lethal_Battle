using GameNetcodeStuff;
using Lethal_Battle.codes;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lethal_Battle.NewFolder
{
    internal class ManageUI
    {
        public static GameObject? UI_players_alive_and_kills;
        private static GameObject? ScreenWinBattle;
        private static GameObject? ScreenBattle;

        public static void UISpawn()
        {
            if (Plugin.Instance.UI_Lethal_Battle != null)
            {
                Plugin.Instance.numberOfPlayers = StartOfRound.Instance.livingPlayers;
                UI_players_alive_and_kills = Object.Instantiate(Plugin.Instance.UI_Lethal_Battle);

                foreach (Graphic graphic in UI_players_alive_and_kills.GetComponentsInChildren<Graphic>(true))
                {
                    graphic.raycastTarget = false;
                }

                GraphicRaycaster? raycaster = UI_players_alive_and_kills.GetComponent<GraphicRaycaster>();

                if (raycaster != null)
                    raycaster.enabled = false;

                ScreenWinBattle =
                    UI_players_alive_and_kills.transform.Find("ScreenWinBattle")?.gameObject;

                ScreenWinBattle?.SetActive(false);

                ScreenBattle =
                    UI_players_alive_and_kills.transform.Find("Panel")?.gameObject;

                ScreenBattle?.SetActive(true);
            }
        }

        public static void ShowWinScreen()
        {
            ScreenBattle?.SetActive(false);

            if (ScreenWinBattle != null)
                ScreenWinBattle.SetActive(true);
        }

        public static void UpdateUI()
        {
            if (UI_players_alive_and_kills != null)
            {
                TMP_Text? NumberOfPlayerKilled = UI_players_alive_and_kills.transform.Find("Panel/NumberOfPlayerKilled")?.GetComponent<TMP_Text>();

                if (NumberOfPlayerKilled != null)
                    NumberOfPlayerKilled.text = Plugin.Instance.numberOfPlayers - StartOfRound.Instance.livingPlayers + "";

                TMP_Text? NumberOfPlayer = UI_players_alive_and_kills.transform.Find("Panel/NumberOfPlayers")?.GetComponent<TMP_Text>();

                if (NumberOfPlayer != null)
                    NumberOfPlayer.text = "/ " + (Plugin.Instance.numberOfPlayers - 1);

                // Closest player
                PlayerControllerB? closestPlayer = ClosestPlayerAndPath.GetClosestOpponentToLocalPlayer();
                TMP_Text? playerName = UI_players_alive_and_kills.transform.Find("Panel/playerName")?.GetComponent<TMP_Text>();
                TMP_Text? playerDistance = UI_players_alive_and_kills.transform.Find("Panel/PlayerDistance")?.GetComponent<TMP_Text>();
                RawImage? avatar = UI_players_alive_and_kills.transform.Find("Panel/PFPPlayer")?.GetComponent<RawImage>();

                bool isMultiplayer = !GameNetworkManager.Instance.disableSteam;

                if (closestPlayer != null)
                {
                    float distance = Vector3.Distance(
                        GameNetworkManager.Instance.localPlayerController.transform.position,
                        closestPlayer.transform.position
                    );

                    if (playerName != null)
                        playerName.text = closestPlayer.playerUsername;

                    if (playerDistance != null)
                        playerDistance.text = $"- {Mathf.RoundToInt(distance)} m";

                    if (avatar != null)
                    {
                        if (isMultiplayer)
                        {
                            avatar.gameObject.SetActive(true);
                            HUDManager.FillImageWithSteamProfile(
                                avatar,
                                closestPlayer.playerSteamId
                            );
                        }
                        else
                        {
                            avatar.gameObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    if (playerName != null)
                        playerName.text = "No player";

                    if (playerDistance != null)
                        playerDistance.text = "-";

                    if (avatar != null)
                        avatar.gameObject.SetActive(false);
                }
            }
        }

        public static IEnumerator WinScreenCoroutine(StartMatchLever shipLever)
        {
            PlayerControllerB? winner = null;

            foreach (GameObject playerObject in StartOfRound.Instance.allPlayerObjects)
            {
                PlayerControllerB? player = playerObject.GetComponent<PlayerControllerB>();

                if (player != null && !player.isPlayerDead)
                {
                    winner = player;
                    break;
                }
            }

            if (winner != null && GameNetworkManager.Instance.localPlayerController == winner)
            {
                ShowWinScreen();
            }

            /*foreach (GameObject playerObject in StartOfRound.Instance.allPlayerObjects)
            {
                PlayerControllerB? player = playerObject.GetComponent<PlayerControllerB>();

                if (player != null && !player.isPlayerDead)
                {
                    player.DamagePlayer(1000); //AAAAAAAAAAAAAAAAAAAAAAAAA
                }
            }*/

            //yield return new WaitForSeconds(2f);
            yield return new WaitForSeconds(0.5f);

            ItemSpawn.MakeShipLeave(shipLever);
            Plugin.hasMessageWonShowed = true;
        }

        public static void UIDelete()
        {
            if (UI_players_alive_and_kills != null)
            {
                UnityEngine.Object.Destroy(UI_players_alive_and_kills);
            }
        }
    }
}
