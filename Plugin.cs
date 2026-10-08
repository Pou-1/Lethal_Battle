using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Lethal_Battle.codes;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace Lethal_Battle
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        const string GUID = "POUY.LETHAL_BATTLE";
        const string NAME = "Lethal Battle";
        const string VERSION = "1.1.1";
        public static Plugin Instance { get; private set; } = null!;
        public static ManualLogSource? log;
        public readonly Harmony harmony = new Harmony(GUID);

        // _____________Mod_____________ \\
        public static bool verifying = false;
        internal static Config LethalBattleConfig { get; private set; } = null!;
        public GameObject? UI_Lethal_Battle;
        public int numberOfPlayers;
        public static bool hasBattleStarted = false;
        public static bool hasMessageWonShowed = false;
        public GameObject? FootprintPrefab;
        public GameObject? VolumeLightBattle;
        public AudioClip? SoundSonar;

        public void Awake()
        {
            Instance = this;
            log = Logger;
            LethalBattleConfig = new Config(Config);
           
            LoadUI();

            log.LogMessage("Lethal Battle Loaded !");
            ClosestPlayerAndPath.Initialize();

            harmony.PatchAll();
        }

        public void LoadUI()
        {
            try
            {
                string assetDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "lethal_battle");
                AssetBundle bundle = AssetBundle.LoadFromFile(assetDir);
                string path = "Assets/LethalModding/LethalBattle/UIBattle.prefab";

                UI_Lethal_Battle = bundle.LoadAsset<GameObject>(path);

                log?.LogInfo("UI loaded successfully");

                LoadFootprints(bundle);
            }
            catch (Exception e)
            {
                log?.LogError("UI ERROR");
                log?.LogError(e);
            }
        }

        public void LoadFootprints(AssetBundle bundle)
        {
            FootprintPrefab = bundle.LoadAsset<GameObject>("Assets/LethalModding/LethalBattle/FootPrintPrefab.prefab");
            SoundSonar = bundle.LoadAsset<AudioClip>("Assets/LethalModding/LethalBattle/sonar.ogg");
            VolumeLightBattle = bundle.LoadAsset<GameObject>("Assets/LethalModding/LethalBattle/VolumeBattle.prefab");
            if (VolumeLightBattle == null)
            {
                log?.LogError("VolumeLightBattle failed to load");
            }
            foreach (string assetName in bundle.GetAllAssetNames())
            {
                log?.LogInfo("BUNDLE ASSET: " + assetName);
            }
        }
    }
}
