using Aequus.NPCs.Town.PhysicistNPC.Analysis;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Tiles.Paintings.Canvas3x3 {
    public class ExLydSpacePainting : ModItem {
        public override void SetStaticDefaults() {
            AnalysisSystem.IgnoreItem.Add(Type);
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<WallPaintings3x3>(), WallPaintings3x3.ExLydSpacePainting);
            Item.value = Item.buyPrice(gold: 2);
        }
    }
}