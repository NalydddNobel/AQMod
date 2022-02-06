using AQMod.Tiles.Nature.CrabCrevice;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Materials
{
    public class SeaPickle : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.sellPrice(silver: 10);
            item.maxStack = 999;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 10;
            item.useAnimation = 15;
            item.createTile = ModContent.TileType<SeaPicklesTile>();
            item.autoReuse = true;
            item.useTurn = true;
            item.consumable = true;
        }
    }
}