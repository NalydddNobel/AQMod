using AQMod.Assets;
using AQMod.Content.World.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Effects.GoreNest
{
    public static class GoreNestRenderer
    {
        private const int Length = 100;

        private static ushort[] tileX = new ushort[Length];
        private static ushort[] tileY = new ushort[Length];
        private static int index = -1;
        private static bool reset = false;

        public static void RefreshCoordinates()
        {
            tileX = new ushort[Length];
            tileY = new ushort[Length];
            index = 0;
        }

        public static void AddCorrdinates(int i, int j)
        {
            if (index == -1 || reset)
            {
                RefreshCoordinates();
                reset = false;
            }
            try
            {
                if (index < Length)
                {
                    tileX[index] = (ushort)i;
                    tileY[index] = (ushort)j;
                    index++;
                }
            }
            catch
            {
                index = -1;
            }
        }

        internal static void Render()
        {
            for (int k = 0; k < index; k++)
            {
                var portalPosition = new Vector2(tileX[k] * 16f + 24f, tileY[k] * 16f - 24f + (float)Math.Sin(Main.GlobalTime) * 4f);
                Main.spriteBatch.End();
                BatcherMethods.GeneralEntities.BeginShader(Main.spriteBatch);
                SamplerRenderer.DrawSampler(LegacyEffectCache.s_GoreNestPortal, portalPosition - Main.screenPosition, 0f, new Vector2(80f, 80f), new Color(255, 255, 255, 255));
                Main.spriteBatch.End();
                BatcherMethods.GeneralEntities.Begin(Main.spriteBatch);
                if (DemonSiege.IsActive && DemonSiege.BaseItem != null && DemonSiege.BaseItem.type > ItemID.None && DemonSiege.AltarCorner() == new Point(tileX[k], tileY[k]))
                {
                    var texture = TextureGrabber.GetItem(DemonSiege.BaseItem.type);
                    var frame = new Rectangle(0, 0, texture.Width, texture.Height);
                    var origin = frame.Size() / 2f;
                    float scale = DemonSiege.BaseItem.scale;
                    float rotation = (float)Math.Sin(Main.GlobalTime) * 0.05f;
                    var drawPosition = new Vector2(portalPosition.X, portalPosition.Y);
                    float y2 = texture.Height / 2f;
                    if (y2 > 24f)
                        drawPosition.Y += 24f - y2;
                    if (ItemLoader.PreDrawInWorld(DemonSiege.BaseItem, Main.spriteBatch, Color.White, Color.White, ref rotation, ref scale, 666))
                        Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, frame, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);
                    ItemLoader.PostDrawInWorld(DemonSiege.BaseItem, Main.spriteBatch, Color.White, Color.White, rotation, scale, 666);
                }
            }
            reset = true;
        }
    }
}