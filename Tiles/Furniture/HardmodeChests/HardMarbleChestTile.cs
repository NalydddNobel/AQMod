using Aequus.Items.Placeable.Furniture;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Furniture.HardmodeChests
{
    public class HardMarbleChestTile : BaseChest
    {
        public override void SetStaticDefaults()
        {
            ChestType.IsGenericUndergroundChest.Add(new TileKey(Type));
            base.SetStaticDefaults();
            DustType = DustID.t_Frozen;
            ChestDrop = ModContent.ItemType<HardMarbleChest>();
            AddMapEntry(new Color(200, 185, 100), CreateMapEntryName());
        }
    }
}