using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Placeable.Nature
{
    public class SedimentSandBlock : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.maxStack = 999;
            item.useTime = 10;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.createTile = ModContent.TileType<Tiles.Nature.CrabCrevice.SedimentSand>();
            item.consumable = true;
            item.useTurn = true;
            item.autoReuse = true;
        }
    }
}