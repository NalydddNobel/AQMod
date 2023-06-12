using Aequus.NPCs.Town.PhysicistNPC.Analysis;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Blocks.GravityBlocks.Ancient {
    public class AncientAntiGravityBlock : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 100;
            AnalysisSystem.IgnoreItem.Add(Type);
            ItemID.Sets.ShimmerTransformToItem[ModContent.ItemType<AntiGravityBlock>()] = Type;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<AncientAntiGravityBlockTile>());
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 2, copper: 50);
        }
    }

    public class AncientAntiGravityBlockTile : AntiGravityBlockTile {
    }
}