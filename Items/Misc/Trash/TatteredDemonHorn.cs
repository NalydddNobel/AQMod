using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Trash {
    public class TatteredDemonHorn : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.FishingSeaweed);
            Item.width = 10;
            Item.height = 16;
        }
    }
}
