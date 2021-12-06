using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Placeable.Furniture
{
    public class AStrangeIdea : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 12;
            item.value = Item.sellPrice(silver: 5);
            item.maxStack = 999;
            item.useTime = 10;
            item.useAnimation = 15;
            item.rare = ItemRarityID.Purple;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.createTile = ModContent.TileType<Tiles.Furniture.Paintings3x3>();
            item.placeStyle = Tiles.Furniture.Paintings3x3.AStrangeIdea;
            item.consumable = true;
            item.useTurn = true;
            item.autoReuse = true;
        }
    }
}