using BepInEx.Configuration;

namespace Lethal_Battle
{
    internal class Config
    {
        public ConfigEntry<int> SingItemBattleRarity
        {
            get; private set;
        }

        public Config(ConfigFile configFile)
        {
            configFile.SaveOnConfigSet = false;
            SingItemBattleRarity = configFile.Bind(
                "Spawn Rates",
                "SingleItemBattleRarity",
                10,
                "Rarity of the Single Item Battle (higher = more common)."
            );

            configFile.Save();
            configFile.SaveOnConfigSet = true;
        }
    }
}
