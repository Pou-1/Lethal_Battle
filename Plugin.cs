using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Assertions;

namespace Lethal_Battle
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        const string GUID = "POUY.LETHAL_BATTLE";
        const string NAME = "Lethal Battle";
        const string VERSION = "0.0.6";
        public static Plugin instance;
        public static bool hasBattleStarted = false;
        public static ManualLogSource log;
        public static bool hasMessageWonShowed = false;

        public readonly Harmony harmony = new Harmony(GUID);
        public GameObject? UI_Lethal_Battle;
        public GameObject? UI_players_alive_and_kills;
        public int numberOfPlayers;

        public void Awake()
        {
            instance = this;

            string assetDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "lethal_battle");
            AssetBundle bundle = AssetBundle.LoadFromFile(assetDir);
            string path = "Assets/Lethal_Battle/UI/Lethal_Battle_Death_UI.prefab";

            UI_Lethal_Battle = bundle.LoadAsset<GameObject>(path);

            harmony.PatchAll();

            log = Logger;
            log.LogMessage("Lethal Battle Loaded !");
        }
    }
}
