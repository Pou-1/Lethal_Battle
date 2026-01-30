using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using HarmonyLib.Tools;
using Lethal_Battle.NewFolder;
using LethalLib;
using LethalLib.Modules;
using System;
using System.IO;
using System.Linq;
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
        const string VERSION = "1.0.1";
        public static Plugin instance;
        public static bool hasBattleStarted = false;
        public static ManualLogSource log;
        public static bool hasMessageWonShowed = false;

        public readonly Harmony harmony = new Harmony(GUID);
        public GameObject? UI_Lethal_Battle;
        public GameObject? UI_players_alive_and_kills;
        public int numberOfPlayers;

        public static bool verifying = false;

        public void Awake()
        {
            instance = this;
            log = Logger;

            LoadUI();
            LoadItem();

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

                log.LogInfo("LETHAL BATTLE : UI loaded successfully");
            }
            catch (Exception e)
            {
                log.LogError("LETHAL BATTLE : UI ERROR");
                log.LogError(e);
            }
        }

        public void LoadItem()
        {
            try
            {
                string assetDirPhone = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "forcefemmod"
                );

                AssetBundle bundlePhone = AssetBundle.LoadFromFile(assetDirPhone);
                if (bundlePhone == null)
                {
                    log.LogError("LETHAL BATTLE : Phone bundle not found");
                    return;
                }

                Item PhoneItem = bundlePhone.LoadAsset<Item>("Assets/LethalModding/PhoneItem.asset");
                if (PhoneItem == null)
                {
                    log.LogError("LETHAL BATTLE : PhoneItem is NULL");
                }
                else
                {
                    log.LogError("LETHAL BATTLE : PhoneItem OK");

                    if (PhoneItem.spawnPrefab == null)
                        log.LogError("LETHAL BATTLE : spawnPrefab is NULL");
                    else
                        log.LogError("LETHAL BATTLE : spawnPrefab OK");
                }

                ConfigEntry<int> configPhoneSpawnWeight =
                    Config.Bind("Phone", "Spawn Weight", 15);

                ConfigEntry<Levels.LevelTypes> configPhoneMoonSpawns =
                    Config.Bind("Phone", "Spawn Locations", Levels.LevelTypes.Vanilla);

                NetworkPrefabs.RegisterNetworkPrefab(PhoneItem.spawnPrefab);
                Utilities.FixMixerGroups(PhoneItem.spawnPrefab);
                Items.RegisterScrap(PhoneItem, configPhoneSpawnWeight.Value, configPhoneMoonSpawns.Value);

                log.LogInfo("LETHAL BATTLE : Phone item loaded successfully");
            }
            catch (Exception e)
            {
                log.LogError("LETHAL BATTLE : PHONE ERROR");
                log.LogError(e);
            }
        }
    }
}
