using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.NPCs.Boss.DustDevil.Rewards {
    public class DustDevilTrophy : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<Trophies>(), Trophies.DustDevil);
            Item.value = 50000;
            Item.rare = ItemRarityID.Blue;
            Item.Aequus().itemGravityCheck = 255;
        }
    }
}