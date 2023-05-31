using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Boss.OmegaStarite.Rewards {
    public class OmegaStariteTrophy : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<Trophies>(), Trophies.OmegaStarite);
            Item.value = 50000;
            Item.rare = ItemRarityID.Blue;
        }
    }
}