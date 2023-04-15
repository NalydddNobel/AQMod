using Aequus.Items.Placeable.Furniture.Oblivion;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Furniture.Oblivion
{
    public class OblivionChestTile : BaseChest
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DustType = DustID.Ash;
            AddMapEntry(Helper.ColorFurniture, CreateMapEntryName());
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            DrawBasicGlowmask(i, j, spriteBatch);
        }
    }
}