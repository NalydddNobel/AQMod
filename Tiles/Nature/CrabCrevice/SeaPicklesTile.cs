using AQMod.Common.Graphics;
using AQMod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Tiles.Nature.CrabCrevice
{
    public class SeaPicklesTile : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileObsidianKill[Type] = true;

            AddMapEntry(new Color(10, 82, 22), CreateMapEntryName("SeaPickle"));
            dustType = DustID.Dirt;
            disableSmartCursor = true;
            drop = ModContent.ItemType<SeaPickle>();
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.tile[i, j].liquid < 100)
            {
                r = 0.01f;
                g = 0.01f;
                b = 0.01f;
            }
            else
            {
                r = 0.3f;
                g = 0.4f;
                b = 0.25f;
            }
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileUtils.GemFrame(i, j);
            return false;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Main.tile[i, j].frameY >= 108)
            {
                return true;
            }
            bool anchorTop = Main.tile[i, j].frameY >= 54;
            var drawFrame = new Rectangle(Main.tile[i, j].frameX, Main.tile[i, j].frameY, 16, 16);
            var drawOrigin = new Vector2(8f, 8f);
            var drawPosition = new Vector2(i * 16f + drawOrigin.X, j * 16f + drawOrigin.Y + 2 * (anchorTop ? -1 : 1));
            float rotation = 0f;
            if (j < (int)Main.worldSurface && Main.tile[i, j].wall == 0)
            {
                float windPower = ((float)Math.Cos(Main.GlobalTime * MathHelper.Pi + i * 0.1f) + 1f) / 2f * Main.windSpeed * (anchorTop ? -1 : 1);
                drawPosition.X += windPower;
                drawPosition.Y += Math.Abs(windPower);
                rotation = windPower * 0.1f;
            }

            Main.spriteBatch.Draw(Main.tileTexture[Type], drawPosition - Main.screenPosition + AQGraphics.TileZero, drawFrame,
                Lighting.GetColor(i, j), rotation, drawOrigin, 1f, SpriteEffects.None, 0f);
            return false;
        }
    }
}