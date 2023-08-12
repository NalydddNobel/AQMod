using Aequus.NPCs.Town.PhysicistNPC.Analysis;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Blocks.GravityBlocks.Ancient {
    public class AncientGravityBlock : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 100;
            AnalysisSystem.IgnoreItem.Add(Type);
            ItemID.Sets.ShimmerTransformToItem[ModContent.ItemType<GravityBlock>()] = Type;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<AncientGravityBlockTile>());
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 1);
        }
    }

    public class AncientGravityBlockTile : GravityBlockTile {
    }
}