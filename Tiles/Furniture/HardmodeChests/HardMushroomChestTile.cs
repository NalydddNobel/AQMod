using Aequus.Items.Placeable.Furniture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Furniture.HardmodeChests
{
    public class HardMushroomChestTile : BaseChest
    {
        public override void SetStaticDefaults()
        {
            ChestType.IsGenericUndergroundChest.Add(new TileKey(Type));
            base.SetStaticDefaults();
            DustType = DustID.t_Frozen;
            ChestDrop = ModContent.ItemType<HardMushroomChest>();
            AddMapEntry(new Color(0, 50, 215), CreateMapEntryName());
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            DrawBasicGlowmask(i, j, spriteBatch);
        }
    }
}