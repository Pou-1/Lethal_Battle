using System;
using BepInEx;
using BepInEx.Logging;

namespace Lethal_Battle
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        const string GUID = "POUY.LETHAL_BATTLE";
        const string NAME = "Lethal Battle";
        const string VERSION = "0.0.1";
        static Plugin instance;
        static ManualLogSource log;

        public void Awake()
        {
            instance = this;
            log = Logger;
            log.LogMessage("Lethal Battle Loaded !");
        }
    }
}
