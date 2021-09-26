using AQMod.Assets.Enumerators;
using AQMod.Assets.Textures;
using AQMod.Common;
using AQMod.Common.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace AQMod.Assets.ItemOverlays
{
    public class SpectralLensOverlay : ItemOverlay
    {
        private static readonly Color[] pattern = new Color[]
        {
            new Color(255, 0, 0, 128),
            new Color(255, 128, 128, 200),

            new Color(255, 128, 0, 128),
            new Color(255, 255, 128, 200),

            new Color(255, 255, 0, 128),
            new Color(255, 255, 128, 200),

            new Color(0, 255, 10, 128),
            new Color(128, 255, 150, 200),

            new Color(0, 128, 128, 128),
            new Color(128, 255, 255, 200),

            new Color(0, 10, 255, 128),
            new Color(128, 150, 255, 200),

            new Color(128, 0, 128, 128),
            new Color(255, 128, 255, 200),
        };

        private static Color getColor(float time)
        {
            var config = ModContent.GetInstance<AQConfigClient>();
            return AQUtils.colorLerps(pattern, time * 3f * config.EffectIntensity) * config.EffectIntensity;
        }

        private static void DrawEye(Vector2 drawPosition, Item item, float time, float rotation = 0f, float scale3 = 1f)
        {
            var texture = SpriteUtils.Textures.Lights[LightID.Spotlight80x80];
            var drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
            var drawOrigin = drawFrame.Size() / 2f;
            int count = (int)(pattern.Length / 2 * ModContent.GetInstance<AQConfigClient>().EffectQuality);
            float rot = MathHelper.TwoPi / count;
            float rot2 = Main.GlobalTime * 3.22244455f;
            Vector2 scale = new Vector2(scale3 * 0.06f, scale3 * (0.4f - (float)Math.Sin(time) * 0.08f));
            var clr = getColor(Main.GlobalTime);
            Main.spriteBatch.Draw(texture, drawPosition, drawFrame, clr, 0f, drawOrigin, scale3 * 0.25f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPosition, drawFrame, clr * 0.5f, 0f, drawOrigin, scale3 * 0.4f, SpriteEffects.None, 0f);
            for (int i = 0; i < count; i++)
            {
                var drawPos = drawPosition + new Vector2(2, 0f).RotatedBy(rot2 + rot * i);
                var drawColor = getColor(Main.GlobalTime + 0.2f * i) * 0.2f;
                Vector2 scale2 = scale * (0.9f - (float)Math.Sin(Main.GlobalTime * 8f + i) * 0.15f);
                Main.spriteBatch.Draw(texture, drawPos, drawFrame, drawColor, rotation + MathHelper.PiOver2, drawOrigin, scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture, drawPos, drawFrame, drawColor, rotation, drawOrigin, scale2, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture, drawPosition, drawFrame, clr * 0.4f, rotation + MathHelper.PiOver4, drawOrigin, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPosition, drawFrame, clr * 0.4f, rotation + MathHelper.PiOver4 * 3f, drawOrigin, scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, drawPosition, drawFrame, clr * 0.6f, rotation + MathHelper.PiOver2, drawOrigin, scale * 1.5f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, drawPosition, drawFrame, clr * 0.6f, rotation, drawOrigin, scale * 1.25f, SpriteEffects.None, 0);
        }

        private static void DrawEye_DrawData(Vector2 drawPosition, Player player, Item item, SpriteEffects effects)
        {
            var texture = SpriteUtils.Textures.Lights[LightID.Spotlight80x80];
            var drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
            var drawOrigin = drawFrame.Size() / 2f;
            int count = pattern.Length / 2;
            float rot = MathHelper.TwoPi / count;
            float rot2 = Main.GlobalTime * 3.22244455f;
            Vector2 scale = new Vector2(item.scale * 0.06f, item.scale * (0.75f - (float)Math.Sin(player.itemAnimation / (float)player.itemAnimationMax * MathHelper.Pi) * 0.25f));
            var clr = getColor(Main.GlobalTime);
            Main.playerDrawData.Add(new DrawData(texture, drawPosition, drawFrame, clr, 0f, drawOrigin, item.scale * 0.45f, effects, 0));
            Main.playerDrawData.Add(new DrawData(texture, drawPosition, drawFrame, clr * 0.55f, 0f, drawOrigin, item.scale * 0.82f, effects, 0));
            for (int i = 0; i < count; i++)
            {
                var drawPos = drawPosition + new Vector2(2, 0f).RotatedBy(rot2 + rot * i);
                var drawColor = getColor(Main.GlobalTime + 0.2f * i) * 0.2f;
                Vector2 scale2 = scale * (0.9f - (float)Math.Sin(Main.GlobalTime * 8f + i) * 0.15f);
                Main.playerDrawData.Add(new DrawData(texture, drawPos, drawFrame, drawColor, player.itemRotation + MathHelper.PiOver2, drawOrigin, scale, effects, 0));
                Main.playerDrawData.Add(new DrawData(texture, drawPos, drawFrame, drawColor, player.itemRotation, drawOrigin, scale2, effects, 0));
            }
            Main.playerDrawData.Add(new DrawData(texture, drawPosition, drawFrame, clr * 0.4f, player.itemRotation + MathHelper.PiOver4, drawOrigin, scale, effects, 0));
            Main.playerDrawData.Add(new DrawData(texture, drawPosition, drawFrame, clr * 0.4f, player.itemRotation + MathHelper.PiOver4 * 3f, drawOrigin, scale, effects, 0));
            Main.playerDrawData.Add(new DrawData(texture, drawPosition, drawFrame, clr * 0.6f, player.itemRotation + MathHelper.PiOver2, drawOrigin, scale * 1.5f, effects, 0));
            Main.playerDrawData.Add(new DrawData(texture, drawPosition, drawFrame, clr * 0.6f, player.itemRotation, drawOrigin, scale * 1.25f, effects, 0));
        }

        public override void DrawHeld(Player player, AQPlayer aQPlayer, Item item, PlayerDrawInfo info)
        {
            Texture2D texture = SpriteUtils.Textures.Glows[GlowID.SpectralLens];
            var drawColor = getColor(Main.GlobalTime);
            if (player.gravDir == -1f)
            {
                var drawPosition = new Vector2((int)(info.itemLocation.X - Main.screenPosition.X), (int)(info.itemLocation.Y - Main.screenPosition.Y));
                var drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
                var drawOrigin = new Vector2(texture.Width * 0.5f - texture.Width * 0.5f * player.direction, 0f);
                Main.playerDrawData.Add(new DrawData(texture, drawPosition, drawFrame, drawColor, player.itemRotation, drawOrigin, item.scale, info.spriteEffects, 0));
                DrawEye_DrawData(drawPosition - drawOrigin + drawFrame.Size() / 2f * item.scale, player, item, info.spriteEffects);
            }
            else
            {
                var drawPosition = new Vector2((int)(info.itemLocation.X - Main.screenPosition.X), (int)(info.itemLocation.Y - Main.screenPosition.Y));
                var drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
                var drawOrigin = new Vector2(texture.Width * 0.5f - texture.Width * 0.5f * player.direction, texture.Height);
                Main.playerDrawData.Add(new DrawData(texture, drawPosition, drawFrame, drawColor, player.itemRotation, drawOrigin, item.scale, info.spriteEffects, 0));
                DrawEye_DrawData(drawPosition - drawOrigin + drawFrame.Size() / 2f * item.scale, player, item, info.spriteEffects);
            }
        }

        public override void DrawWorld(Item item, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            var texture = SpriteUtils.Textures.Glows[GlowID.SpectralLens];
            var drawColor = getColor(Main.GlobalTime);
            Vector2 drawPosition = new Vector2(item.position.X - Main.screenPosition.X + texture.Width / 2 + item.width / 2 - texture.Width / 2, item.position.Y - Main.screenPosition.Y + texture.Height / 2 + item.height - texture.Height + 2f);
            var drawOrigin = Main.itemTexture[item.type].Size() / 2;
            Main.spriteBatch.Draw(texture, drawPosition, null, drawColor, rotation, drawOrigin, scale, SpriteEffects.None, 0f);
            DrawEye(drawPosition, item, Main.GlobalTime + 1f, rotation, scale);
        }

        public override void DrawInventory(Player player, AQPlayer aQPlayer, Item item, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var texture = SpriteUtils.Textures.Glows[GlowID.SpectralLens];
            drawColor = getColor(Main.GlobalTime);
            Main.spriteBatch.Draw(texture, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            DrawEye(position - origin + frame.Size() / 2f * scale, item, Main.GlobalTime + 1f, 0f, scale);
        }
    }
}