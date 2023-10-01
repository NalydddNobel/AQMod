using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Tiles.Furniture.Boss.Trophies {
    public class UltraStariteTrophy : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<BossTrophiesTile>(), BossTrophiesTile.UltraStarite);
            Item.value = 50000;
            Item.rare = ItemRarityID.Blue;
        }
    }
}