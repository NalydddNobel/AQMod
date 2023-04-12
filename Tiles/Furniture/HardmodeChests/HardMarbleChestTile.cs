using Aequus.Content.WorldGeneration;
using Aequus.Items.Placeable.Furniture.Misc;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Furniture.HardmodeChests
{
    public class HardMarbleChestTile : BaseChest
    {
        public override void SetStaticDefaults()
        {
            HardmodeChestBoost.CountsAsChest[Type] = new(TileID.Containers, ChestType.Gold);
            ChestType.IsGenericUndergroundChest.Add(new TileKey(Type));
            base.SetStaticDefaults();
            DustType = DustID.t_Frozen;
            ItemDrop = ModContent.ItemType<HardMarbleChest>();
            AddMapEntry(new Color(200, 185, 100), CreateMapEntryName());
        }
    }
}