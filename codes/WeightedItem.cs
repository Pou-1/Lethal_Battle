using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lethal_Battle.NewFolder
{
    internal class WeightedItem
    {
        public Item item;
        public float weight;

        public WeightedItem(Item item, float weight)
        {
            this.item = item;
            this.weight = weight;
        }

        public static readonly Dictionary<string, float> WeightsByName = new Dictionary<string, float>
        {
            { "YIELD SIGN", 50f },
            { "STOP SIGN", 50f },
            { "KITCHEN KNIFE", 10f },
            { "STUN GRENADE", 5f },
            { "SHOVEL", 50f },
            { "DOUBLE-BARREL", 10f },
            { "SHOTGUN", 10f },
            { "TRAGEDY", 15f },
            { "COMEDY", 15f },

            // ============= MODED ITEMS ============= \\
            { "COMICALLY LARGE SPOON", 30f },
            { "AIRHORN", 30f },
            { "FRIENDSHIP ENDER", 10f },
            { "STICK", 20f },
            { "GALVANIZED SQUARE STEEL", 10f },
            { "BOMB", 30f },
            { "CONTROLLER", 10f },
            { "MAJORA'S MASK", 10f },
            { "DEATH NOTE", 10f },
            { "UNO REVERSE CARD", 10f },
            { "MASTER SWORD", 30f },
            { "OCARINA", 10f },
            { "TOTEM OF UNDYING", 1f },
            { "DANCE NOTE", 10f },
            { "UNO REVERSE CARD DX", 10f },
            { "BOOMBOX", 10f },
        };

        public static float GetWeight(string itemName)
        {
            string key = itemName.Trim().ToUpper();
            return WeightsByName.TryGetValue(key, out float weight) ? weight : 0f;
        }

        public static List<WeightedItem> GetBattleItemsWeighted(List<Item> allitems)
        {
            List<WeightedItem> result = new List<WeightedItem>();

            foreach (Item item in allitems)
            {
                Plugin.log.LogError(item.itemName.ToUpper());
                float weight = GetWeight(item.itemName);
                result.Add(new WeightedItem(item, weight));
            }

            return result;
        }
    }
}
