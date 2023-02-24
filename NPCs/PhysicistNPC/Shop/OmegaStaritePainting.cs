using Aequus.Content.AnalysisQuests;
using Aequus.Tiles.Furniture;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.NPCs.PhysicistNPC.Shop
{
    public class OmegaStaritePainting : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            AnalysisSystem.IgnoreItem.Add(Type);
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<WallPaintings6x4>(), WallPaintings6x4.OmegaStaritePainting);
            Item.value = Item.buyPrice(gold: 2);
        }
    }
}