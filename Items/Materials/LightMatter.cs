using AQMod.Assets;
using AQMod.Common.Graphics;
using AQMod.Common.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Materials
{
    public class LightMatter : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.rare = ItemRarityID.Orange;
            item.value = Item.sellPrice(silver: 8);
            item.maxStack = 999;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var drawPosition = position - origin + frame.Size() / 2f * scale;
            var texture = LegacyTextureCache.Lights[LightTex.Spotlight80x80];
            var drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
            var drawOrigin = drawFrame.Size() / 2f;
            float time = Main.GlobalTime * 2f;
            float waveFunction = (float)Math.Sin(time);
            scale += waveFunction * 0.1f - 0.1f;
            int b = (int)(110 * scale * AQConfigClient.Instance.EffectIntensity);
            var itemFrame = new Rectangle(0, 0, Main.itemTexture[item.type].Width, Main.itemTexture[item.type].Height);
            var itemOrigin = itemFrame.Size() / 2f;
            for (float f = 0f; f < 1f; f += 0.125f)
            {
                Main.spriteBatch.Draw(Main.itemTexture[item.type], drawPosition + new Vector2(2f * scale, 0f).RotatedBy(AQGraphics.TimerBasedOnTimeOfDay * 0.9f + f * MathHelper.TwoPi), itemFrame, new Color(20, 20, 50, 0), 0f, itemOrigin, scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(Main.itemTexture[item.type], drawPosition, itemFrame, new Color(255, 255, 255, 255), 0f, itemOrigin, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPosition, drawFrame, new Color(230, 230, 255, 200), 0f, drawOrigin, scale / 4f, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            var texture = Main.itemTexture[item.type];
            var drawPosition = new Vector2(item.position.X - Main.screenPosition.X + texture.Width / 2 + item.width / 2 - texture.Width / 2, item.position.Y - Main.screenPosition.Y + texture.Height / 2 + item.height - texture.Height + 2f);
            texture = LegacyTextureCache.Lights[LightTex.Spotlight80x80];
            var drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
            var drawOrigin = drawFrame.Size() / 2f;
            float time = Main.GlobalTime * 2f;
            float waveFunction = (float)Math.Sin(time);
            drawPosition = new Vector2(drawPosition.X, drawPosition.Y - (int)(waveFunction * 2f));
            scale += waveFunction * 0.1f - 0.1f;
            float halfScale = scale / 2f;
            float fourthScale = scale / 4f;
            float eighthScale = scale / 8f;
            float sixteenthScale = scale / 16f;
            int b = (int)(60 * scale * AQConfigClient.Instance.EffectIntensity);
            int b2 = (int)(110 * scale * AQConfigClient.Instance.EffectIntensity);
            var itemFrame = new Rectangle(0, 0, Main.itemTexture[item.type].Width, Main.itemTexture[item.type].Height);
            var itemOrigin = itemFrame.Size() / 2f;
            for (float f = 0f; f < 1f; f += 0.125f)
            {
                Main.spriteBatch.Draw(Main.itemTexture[item.type], drawPosition + new Vector2(2f * scale, 0f).RotatedBy(AQGraphics.TimerBasedOnTimeOfDay * 0.9f + f * MathHelper.TwoPi), itemFrame, new Color(20, 20, 50, 0), 0f, itemOrigin, scale * 0.8f, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(Main.itemTexture[item.type], drawPosition, itemFrame, new Color(255, 255, 255, 255), rotation, itemOrigin, scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(texture, drawPosition, drawFrame, new Color(255, 255, 255, 200), rotation, drawOrigin, fourthScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPosition, drawFrame, new Color(b, b, b2, b / 2), rotation, drawOrigin, halfScale, SpriteEffects.None, 0f);

            var scale2 = new Vector2(eighthScale, halfScale);
            float scale3 = 1f - MathHelper.Clamp(((float)Math.Sin(time * 2f) + 1f) / 2f, 0.3f, 0.8f);
            Main.spriteBatch.Draw(texture, drawPosition, drawFrame, new Color(b, b, b2, 0), rotation + time, drawOrigin, scale2 * scale3, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPosition, drawFrame, new Color(b, b, b2, 0), rotation + MathHelper.PiOver2 + time, drawOrigin, scale2 * scale3, SpriteEffects.None, 0f);
            scale3 = MathHelper.Clamp(((float)Math.Sin(time * 2f) + 1f) / 2f, 0.3f, 0.8f);
            Main.spriteBatch.Draw(texture, drawPosition, drawFrame, new Color(b, b, b2, 0), rotation + MathHelper.PiOver4 + time, drawOrigin, scale2 * scale3, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPosition, drawFrame, new Color(b, b, b2, 0), rotation - MathHelper.PiOver4 + time, drawOrigin, scale2 * scale3, SpriteEffects.None, 0f);

            drawFrame.Height = 40;
            texture = ModContent.GetTexture("AQMod/Assets/Lights/LightRay");

            scale3 = MathHelper.Clamp(((float)Math.Sin(time * 1.1f) + 1f) / 2f, 0.151f, 0.8f) * 0.9f;
            Main.spriteBatch.Draw(texture, drawPosition, drawFrame, new Color(b, b, b2, 0) * scale3, rotation + time * 1.1f, drawOrigin, scale2 * scale3, SpriteEffects.None, 0f);

            scale3 = MathHelper.Clamp(((float)Math.Sin(time * 0.32111f) + 1f) / 2f, 0.1f, 0.8f) * 1.2f;
            Main.spriteBatch.Draw(texture, drawPosition, drawFrame, new Color(b, b, b2, 0) * scale3, rotation + time * 0.4f, drawOrigin, scale2 * scale3, SpriteEffects.None, 0f);

            scale3 = MathHelper.Clamp(((float)Math.Sin(time * 0.741f) + 1f) / 2f, 0.4f, 0.9f) * 1.1f;
            Main.spriteBatch.Draw(texture, drawPosition, drawFrame, new Color(b, b, b2, 0) * scale3, rotation + time * 0.6553322f, drawOrigin, scale2 * scale3, SpriteEffects.None, 0f);
            return false;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(item.Center, new Vector3(0.8f, 0.8f, 0.8f));
        }
    }
}