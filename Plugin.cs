using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace Lethal_Battle
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        const string GUID = "POUY.LETHAL_BATTLE";
        const string NAME = "Lethal Battle";
        const string VERSION = "0.0.1";
        static Plugin instance;
        public static ManualLogSource log;

        public readonly Harmony harmony = new Harmony(GUID);

        public void Awake()
        {
            instance = this;

            harmony.PatchAll();

            log = Logger;
            log.LogMessage("Lethal Battle Loaded !");
        }
    }
}
