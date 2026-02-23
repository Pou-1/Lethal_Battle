using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LethalLib.Modules;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Lethal_Battle
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        const string GUID = "POUY.LETHAL_BATTLE";
        const string NAME = "Lethal Battle";
        const string VERSION = "1.0.1";
        public static Plugin instance;
        public static bool hasBattleStarted = false;
        public static ManualLogSource log;
        public static bool hasMessageWonShowed = false;

        public readonly Harmony harmony = new Harmony(GUID);
        public GameObject? UI_Lethal_Battle;
        public GameObject? UI_players_alive_and_kills;
        public int numberOfPlayers;
        public static int OneItemBattle;

        public static bool verifying = false;

        public void Awake()
        {
            instance = this;
            log = Logger;
            Config config = new Config(Config);
            OneItemBattle = config.SingItemBattleRarity.Value;

            LoadUI();

            log.LogMessage("Lethal Battle Loaded !");

            harmony.PatchAll();
        }

        public void LoadUI()
        {
            try
            {
                string assetDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "lethal_battle");
                AssetBundle bundle = AssetBundle.LoadFromFile(assetDir);
                string path = "Assets/Lethal_Battle/UI/Lethal_Battle_Death_UI.prefab";

                UI_Lethal_Battle = bundle.LoadAsset<GameObject>(path);

                log.LogInfo("UI loaded successfully");
            }
            catch (Exception e)
            {
                log.LogError("UI ERROR");
                log.LogError(e);
            }
        }
    }
}
