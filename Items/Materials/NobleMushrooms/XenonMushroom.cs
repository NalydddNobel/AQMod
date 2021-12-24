using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Materials.NobleMushrooms
{
    public class XenonMushroom : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.sellPrice(silver: 20);
            item.maxStack = 999;
            item.useTime = 10;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.createTile = ModContent.TileType<Tiles.Nature.NobleMushrooms>();
            item.placeStyle = 2;
            item.consumable = true;
            item.useTurn = true;
            item.autoReuse = true;
        }
    }
}
