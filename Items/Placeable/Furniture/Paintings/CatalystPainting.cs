using Aequus.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Furniture.Paintings
{
    public class CatalystPainting : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<WallPaintings>(), WallPaintings.RockFromAnAlternateUniversePainting);
            Item.maxStack = 99;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ItemRarityID.Blue;
        }
    }
}