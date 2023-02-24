using Aequus.Content.AnalysisQuests;
using Aequus.Tiles.Furniture;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.NPCs.PhysicistNPC.Shop
{
    public class ExLydSpacePainting : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            AnalysisSystem.IgnoreItem.Add(Type);
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<WallPaintings>(), WallPaintings.ExLydSpacePainting);
            Item.value = Item.buyPrice(gold: 2);
        }
    }
}