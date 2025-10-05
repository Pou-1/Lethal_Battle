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
            { "KITCHEN KNIFE", 20f },
            { "STUN GRENADE", 2 },
            { "SHOVEL", 50f },
            { "DOUBLE-BARREL", 10f },
            { "TRAGEDY", 20f },
            { "COMEDY", 20f },

            // ============= MODED ITEMS ============= \\
            { "COMICALLY LARGE SPOON", 10f },
            { "AIRHORN", 5f },
            { "FRIENDSHIP ENDER", 20f },
            { "STICK", 15f },
            { "GALVANIZED SQUARE STEEL", 10f },
            { "BOMB", 20f },
            { "CONTROLLER", 1f },
            { "MAJORA'S MASK", 10f },
            { "DEATH NOTE", 0.5f },
            { "UNO REVERSE CARD", 10f },
            { "MASTER SWORD", 5f },
            { "OCARINA", 10f },
            { "TOTEM OF UNDYING", 1f },
            { "DANCE NOTE", 5f },
            { "UNO REVERSE CARD DX", 20f },
            { "BOOMBOX", 25f },

            { "NUCLEAR BOMB", 1f },

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
