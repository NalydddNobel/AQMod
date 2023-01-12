using Aequus.Items.Placeable.Furniture;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Furniture.HardmodeChests
{
    public class HardMushroomChestTile : BaseChest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DustType = DustID.t_Frozen;
            ChestDrop = ModContent.ItemType<HardMushroomChest>();
            AddMapEntry(new Color(0, 50, 215), CreateMapEntryName());
        }
    }
}