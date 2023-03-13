using Aequus.Content.WorldGeneration;
using Aequus.Items.Placeable.Furniture.Misc;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Furniture.HardmodeChests
{
    public class HardFrozenChestTile : BaseChest
    {
        public override void SetStaticDefaults()
        {
            HardmodeChestBoost.CountsAsChest[Type] = new(TileID.Containers, ChestType.Frozen);
            ChestType.IsGenericUndergroundChest.Add(new TileKey(Type));
            base.SetStaticDefaults();
            DustType = DustID.t_Frozen;
            ChestDrop = ModContent.ItemType<HardFrozenChest>();
            AddMapEntry(new Color(105, 115, 255), CreateMapEntryName());
        }
    }
}