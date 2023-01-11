using Aequus.Items.Placeable;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Furniture.HardmodeChests
{
    public class AdamantiteChestTile : BaseChest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DustType = DustID.Ash;
            ChestDrop = ModContent.ItemType<AdamantiteChest>();
            AddMapEntry(ColorHelper.FurnitureColor, CreateMapEntryName());
        }
    }
}