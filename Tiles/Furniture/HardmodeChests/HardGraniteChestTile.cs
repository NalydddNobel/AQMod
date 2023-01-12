using Aequus.Items.Placeable.Furniture;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Furniture.HardmodeChests
{
    public class HardGraniteChestTile : BaseChest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DustType = DustID.t_Frozen;
            ChestDrop = ModContent.ItemType<HardGraniteChest>();
            AddMapEntry(new Color(100, 255, 255), CreateMapEntryName());
        }
    }
}