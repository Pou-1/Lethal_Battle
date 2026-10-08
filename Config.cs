using BepInEx.Configuration;

namespace Lethal_Battle
{
    internal class Config
    {
        public ConfigEntry<int> SingItemBattleRarity
        {
            get; private set;
        }

        public ConfigEntry<bool> IsVolumeInBattle
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
                "% of chances to have a battle with a single item (higher = more common)."
            );

            IsVolumeInBattle = configFile.Bind(
                "true = the battle has a volume making a reddish tone to the screen",
                "IsVolumeInBattle",
                true,
                "Whether the battle volume is in the battle."
            );

            configFile.Save();
            configFile.SaveOnConfigSet = true;
        }
    }
}
