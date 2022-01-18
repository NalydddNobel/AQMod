using AQMod.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Placeable.Furniture
{
    public class CrabClock : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 12;
            item.value = Item.buyPrice(gold: 5);
            item.maxStack = 999;
            item.useTime = 10;
            item.useAnimation = 15;
            item.rare = ItemRarityID.Blue;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.createTile = ModContent.TileType<WallClocks>();
            item.placeStyle = WallClocks.CrabClock;
            item.consumable = true;
            item.useTurn = true;
            item.autoReuse = true;
        }
    }
}