using Aequus.Content.WorldGeneration;
using Aequus.Items.Placeable.Furniture.Misc;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Furniture.HardmodeChests
{
    public class HardJungleChestTile : BaseChest
    {
        public override void SetStaticDefaults()
        {
            HardmodeChestBoost.CountsAsChest[Type] = new(TileID.Containers, ChestType.Ivy);
            ChestType.IsGenericUndergroundChest.Add(new TileKey(Type));
            base.SetStaticDefaults();
            DustType = DustID.t_Frozen;
            ChestDrop = ModContent.ItemType<HardJungleChest>();
            AddMapEntry(new Color(170, 105, 70), CreateMapEntryName());
        }
    }
}