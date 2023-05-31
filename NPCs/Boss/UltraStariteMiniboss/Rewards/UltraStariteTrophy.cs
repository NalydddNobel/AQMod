using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Boss.UltraStariteMiniboss.Rewards {
    public class UltraStariteTrophy : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<Trophies>(), Trophies.UltraStarite);
            Item.value = 50000;
            Item.rare = ItemRarityID.Blue;
        }
    }
}