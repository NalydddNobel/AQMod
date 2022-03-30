using Aequus.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable
{
    public class Origin : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.value = Item.sellPrice(silver: 5);
            Item.maxStack = 999;
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.rare = ItemRarityID.Pink;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.createTile = ModContent.TileType<WallPaintings>();
            Item.placeStyle = WallPaintings.TheOrigin;
            Item.consumable = true;
            Item.useTurn = true;
            Item.autoReuse = true;
        }
    }
}