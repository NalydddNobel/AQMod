using AQMod.Common.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace AQMod.Effects
{
    internal class TileEffects
    {
        public static void RenderSwayingVine(int x, int y, int vineID)
        {
            int num = 0;
            int num2 = 0;
            Vector2 vector = new Vector2(x * 16 + 8, y * 16 - 2);
            float amount = Math.Abs(Main.windSpeed) / 1.7f;
            amount = MathHelper.Lerp(0.2f, 1f, amount);
            float num3 = -0.08f * amount;
            float windCycle = (float)Math.Sin(Main.GlobalTime + (x + y) * 0.1f) * 0.05f + 0.12f;
            float num4 = 0f;
            float num5 = 0f;
            for (int j = y; j < Main.maxTilesY - 10; j++)
            {
                Tile tile = Main.tile[x, j];
                if (tile != null)
                {
                    ushort type = tile.type;
                    if (!tile.active() || type != vineID)
                    {
                        break;
                    }
                    num++;
                    if (num2 >= 5)
                    {
                        num3 += 0.0075f * amount;
                    }
                    if (num2 >= 2)
                    {
                        num3 += 0.0025f;
                    }
                    float windGridPush = 0f;
                    if (Main.tile[x, j].wall <= 0 && (double)j < Main.worldSurface)
                    {
                        windGridPush = (float)Math.Sin(Main.GlobalTime + (x + y) * 0.1f) * 0.05f + 0.04f;
                        num2++;
                    }
                    windGridPush *= Main.windSpeed;
                    num4 = ((windGridPush != 0f || num5 == 0f) ? (num4 - windGridPush) : (num4 * -0.78f));
                    num5 = windGridPush;
                    short tileFrameX = tile.frameX;
                    short tileFrameY = tile.frameY;
                    Color color = Lighting.GetColor(x, j);
                    Vector2 position = new Vector2(-(int)Main.screenPosition.X, -(int)Main.screenPosition.Y) + AQGraphics.TileZero + vector;
                    if (tile.color() == 31)
                    {
                        color = Color.White;
                    }
                    float num6 = (float)num2 * num3 * windCycle + num4;
                    var tileDrawTexture = Main.tileTexture[type];
                    if (tileDrawTexture == null)
                    {
                        break;
                    }
                    Main.spriteBatch.Draw(tileDrawTexture, position, new Rectangle(tileFrameX, tileFrameY, 16, 16), color, num6, new Vector2(16f / 2f, 0f), 1f, SpriteEffects.None, 0f);
                    vector += (num6 + (float)Math.PI / 2f).ToRotationVector2() * 16f;
                }
            }
        }
    }
}
