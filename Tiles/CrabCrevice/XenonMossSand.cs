using AQMod.Common.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Tiles.CrabCrevice
{
    public sealed class XenonMossSand : GrassType
    {
        public override int? OriginalTile => TileID.Sand;

        public override void SetDefaults()
        {
            Main.tileLighted[Type] = true;
            base.SetDefaults();

            AddMapEntry(new Color(0, 197, 208));

            dustType = 32;
            drop = ItemID.SandBlock;
            soundType = SoundID.Dig;
            soundStyle = 1;

            mineResist = 2f;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0f;
            g = 0.16f;
            b = 0.34f;
        }

        public override void RandomUpdate(int i, int j)
        {
            if (j > 500)
            {
                SpreadGrassToSurroundings(i, j, OriginalTile.Value, Type, 3, Main.tile[i, j].color());
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            AQGraphics.Rendering.DrawTileWithSloping(Main.tile[i, j], ModContent.GetTexture(this.GetPath("_Glow")), new Vector2(i * 16f, j * 16f) + AQGraphics.Data.TileZero - Main.screenPosition, new Color(200, 200, 200, 0), Main.tile[i, j].frameX, Main.tile[i, j].frameY, 16, 16);
        }
    }
}