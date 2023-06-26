using Aequus.NPCs.Town.PhysicistNPC.Analysis;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Tiles.Paintings.Canvas6x4 {
    public class OmegaStaritePainting : ModItem {
        public override void SetStaticDefaults() {
            AnalysisSystem.IgnoreItem.Add(Type);
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<WallPaintings6x4>(), WallPaintings6x4.OmegaStaritePainting);
            Item.value = Item.buyPrice(gold: 2);
        }
    }
}