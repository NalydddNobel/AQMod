using AQMod.Tiles.Nature.CrabCrevice;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Placeable.Nature
{
    public class XenonMushroom : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.sellPrice(silver: 2);
            item.maxStack = 999;
            item.useTime = 10;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.createTile = ModContent.TileType<NobleMushroomsNew>();
            item.placeStyle = 6;
            item.consumable = true;
            item.useTurn = true;
            item.autoReuse = true;
        }
    }
}