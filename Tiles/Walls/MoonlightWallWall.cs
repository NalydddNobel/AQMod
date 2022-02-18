using AQMod.Common.Graphics;
using AQMod.Items.Placeable.Wall;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Tiles.Walls
{
    public class MoonlightWallWall : ModWall
    {
        public static Color GetColor(float value = 0f)
        {
            return AQUtils.LerpColors(new Color[] { new Color(53, 163, 255, 0), new Color(0, 16, 102, 0), new Color(8, 165, 137, 0) }, value * 0.157f + Main.GlobalTime);
        }

        public override void SetDefaults()
        {
            Main.wallHouse[Type] = true;
            Main.wallLight[Type] = true;
            dustType = 203;
            drop = ModContent.ItemType<MoonlightWall>();
            AddMapEntry(new Color(12, 12, 48));
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            var c = GetColor(i + j) * 0.25f;
            r = c.R / 255f;
            g = c.G / 255f;
            b = c.B / 255f;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            AQGraphics.Rendering.Wall(i, j, ModContent.GetTexture(this.GetPath("_Glow")), GetColor(i + j));
        }
    }
}