using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Biomes.MossBiomes.Radon.Tiles {
    public class RadonMoss : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 25;
            ItemID.Sets.ShimmerCountsAsItem[Type] = ItemID.ArgonMoss;
            ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<RadonMossTile>());
            Item.rare = ItemRarityID.Blue;
        }
    }
}