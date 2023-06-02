using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Materials.Fish {
    public class IcebergFish : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.FrostMinnow);
        }
    }
}