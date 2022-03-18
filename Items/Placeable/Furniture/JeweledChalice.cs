using AQMod.Tiles.ExporterQuest;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Placeable.Furniture
{
    public class JeweledChalice : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 24;
            item.rare = ItemRarityID.Quest;
            item.questItem = true;
            item.consumable = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 10;
            item.useAnimation = 15;
            item.createTile = ModContent.TileType<JeweledChaliceTile>();
            item.autoReuse = true;
            item.maxStack = 99;
        }
    }
}