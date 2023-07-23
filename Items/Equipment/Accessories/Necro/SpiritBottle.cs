using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Necro {
    public class SpiritBottle : ModItem {
        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.PygmyNecklace);
            Item.neckSlot = 0;
            Item.hasVanityEffects = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.maxTurrets++;
        }
    }
}