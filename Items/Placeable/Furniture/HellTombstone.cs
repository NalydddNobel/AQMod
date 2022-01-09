using AQMod.Tiles.Furniture;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Placeable.Furniture
{
    public class HellTombstone : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 12;
            item.maxStack = 999;
            item.rare = ItemRarityID.Blue;
            item.useTime = 10;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.createTile = ModContent.TileType<AQTombstones>();
            item.placeStyle = AQTombstones.HellTombstone;
            item.consumable = true;
            item.useTurn = true;
            item.autoReuse = true;
        }
    }
}