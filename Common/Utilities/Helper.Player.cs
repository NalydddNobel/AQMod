using Terraria;
using Terraria.ModLoader;

namespace Aequus {
    public static partial class Helper {
        public static Item GetAccessory(this Player player, int slot, bool modded = false) {
            if (modded) {
                return ModContent.GetInstance<AccessorySlotLoader>().Get(slot, player).FunctionalItem;
            }

            return player.armor[slot];
        }
    }
}