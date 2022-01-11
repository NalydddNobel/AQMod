using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Placeable.Nature
{
    public class GlimmeringStatue : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 12;
            item.maxStack = 999;
            item.rare = ItemRarityID.Green;
            item.useTime = 10;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.createTile = ModContent.TileType<Tiles.Nature.GlimmeringStatue>();
            item.consumable = true;
            item.useTurn = true;
            item.autoReuse = true;
        }
    }
}