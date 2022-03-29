using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable
{
    public class CrabsonTrophy : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = 50000;
            Item.rare = ItemRarityID.Blue;
            //Item.createTile = ModContent.TileType<Tiles.Furniture.Trophies>();
            //Item.placeStyle = Tiles.Furniture.Trophies.Crabson;
        }
    }
}