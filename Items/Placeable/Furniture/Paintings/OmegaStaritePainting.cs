using Aequus.Content.Town.PhysicistNPC.Analysis;
using Aequus.Tiles.Furniture;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Furniture.Paintings
{
    public class OmegaStaritePainting : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            AnalysisSystem.IgnoreItem.Add(Type);
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<WallPaintings6x4>(), WallPaintings6x4.OmegaStaritePainting);
            Item.value = Item.buyPrice(gold: 2);
        }
    }
}