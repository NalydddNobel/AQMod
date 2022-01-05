using AQMod.Common.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Tiles.CrabCrevice
{
    public sealed class ArgonMossSand : GrassType
    {
        public override int? OriginalTile => TileID.Sand;

        public override void SetDefaults()
        {
            Main.tileLighted[Type] = true;
            base.SetDefaults();

            AddMapEntry(new Color(208, 0, 126));
            //AddMapEntry(new Color(114, 254, 2));
            //AddMapEntry(new Color(0, 197, 208));

            dustType = 32;
            drop = ItemID.SandBlock;
            soundType = SoundID.Dig;
            soundStyle = 1;

            mineResist = 2f;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.3f;
            g = 0f;
            b = 0.17f;
        }

        public override void RandomUpdate(int i, int j)
        {
            if (j > 400)
            {
                SpreadGrassToSurroundings(i, j, OriginalTile.Value, Type, 3, Main.tile[i, j].color());
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            AQGraphics.Rendering.TileWithSloping(Main.tile[i, j], ModContent.GetTexture(this.GetPath("_Glow")), new Vector2(i * 16f, j * 16f) + AQGraphics.TileZero - Main.screenPosition, new Color(200, 200, 200, 0), Main.tile[i, j].frameX, Main.tile[i, j].frameY, 16, 16);
        }
    }
}