using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Placeable.Wall
{
    public class PetrifiedWoodWall : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 12;
            item.maxStack = 999;
            item.useTime = 7;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.createWall = ModContent.WallType<Walls.PetrifiedWoodWall>();
            item.consumable = true;
            item.autoReuse = true;
            item.useTurn = true;
        }
    }
}