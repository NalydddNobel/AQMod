using Aequus.Content.WorldGeneration;
using Aequus.Items.Placeable.Furniture.Misc;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Furniture.HardmodeChests
{
    public class HardSandstoneChestTile : BaseChest
    {
        public override void SetStaticDefaults()
        {
            HardmodeChestBoost.CountsAsChest[Type] = new(TileID.Containers2, ChestType.Sandstone);
            ChestType.IsGenericUndergroundChest.Add(new TileKey(Type));
            base.SetStaticDefaults();
            DustType = DustID.t_Frozen;
            ChestDrop = ModContent.ItemType<HardSandstoneChest>();
            AddMapEntry(new Color(180, 130, 20), CreateMapEntryName());
        }
    }
}