using Aequus.Items.Placeable.Furniture;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Furniture.HardmodeChests
{
    public class AdamantiteChestTile : BaseChest
    {
        public override void SetStaticDefaults()
        {
            ChestType.IsGenericUndergroundChest.Add(new TileKey(Type));
            base.SetStaticDefaults();
            DustType = DustID.Ash;
            ChestDrop = ModContent.ItemType<AdamantiteChest>();
            AddMapEntry(new Color(160, 25, 50), CreateMapEntryName());
        }
    }
}