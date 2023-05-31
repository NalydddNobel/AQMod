using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Boss.SpaceSquidMiniboss.Rewards {
    public class SpaceSquidTrophy : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<Trophies>(), Trophies.SpaceSquid);
            Item.value = 50000;
            Item.rare = ItemRarityID.Blue;
            Item.Aequus().itemGravityCheck = 255;
        }
    }
}