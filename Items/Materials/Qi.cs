using AQMod.Assets;
using AQMod.Common.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Materials
{
    public class Qi : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.rare = ItemRarityID.Red;
            item.value = Item.sellPrice(silver: 20);
            item.maxStack = 999;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255);
        }

        private static void DrawEye(Vector2 drawPosition, Item item, float time, float rotation = 0f, float scale = 1f)
        {
            var config = ModContent.GetInstance<AQConfigClient>();
            var texture = AQTextures.Lights[LightTex.Spotlight80x80];
            var drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
            var drawOrigin = drawFrame.Size() / 2f;
            float timeSine = (float)Math.Sin(time);
            drawPosition = new Vector2(drawPosition.X, drawPosition.Y - (int)(timeSine * 2f));
            scale += timeSine * 0.1f - 0.1f;
            float halfScale = scale / 2f;
            float fourthScale = scale / 4f;
            float eighthScale = scale / 8f;
            float sixteenthScale = scale / 16f;
            int b = (int)(110 * scale * config.EffectIntensity);
            Main.spriteBatch.Draw(Main.itemTexture[item.type], drawPosition, new Rectangle(0, 0, Main.itemTexture[item.type].Width, Main.itemTexture[item.type].Height), new Color(255, 255, 255, 255), rotation, new Vector2(Main.itemTexture[item.type].Width * 0.5f, Main.itemTexture[item.type].Height * 0.5f), scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPosition, drawFrame, new Color(255, 255, 255, 200), rotation, drawOrigin, fourthScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPosition, drawFrame, new Color(b, b, b, b / 2), rotation, drawOrigin, halfScale, SpriteEffects.None, 0f);

            var scale2 = new Vector2(eighthScale, halfScale);
            float scale3 = 1f - MathHelper.Clamp(((float)Math.Sin(time * 2f) + 1f) / 2f, 0.3f, 0.8f);
            Main.spriteBatch.Draw(texture, drawPosition, drawFrame, new Color(b, b, b, 0), rotation + time, drawOrigin, scale2 * scale3, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPosition, drawFrame, new Color(b, b, b, 0), rotation + MathHelper.PiOver2 + time, drawOrigin, scale2 * scale3, SpriteEffects.None, 0f);
            scale3 = MathHelper.Clamp(((float)Math.Sin(time * 2f) + 1f) / 2f, 0.3f, 0.8f);
            Main.spriteBatch.Draw(texture, drawPosition, drawFrame, new Color(b, b, b, 0), rotation + MathHelper.PiOver4 + time, drawOrigin, scale2 * scale3, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPosition, drawFrame, new Color(b, b, b, 0), rotation - MathHelper.PiOver4 + time, drawOrigin, scale2 * scale3, SpriteEffects.None, 0f);

            if (config.EffectQuality < 1f)
                return;

            texture = AQTextures.Lights[LightTex.Spotlight80x80HalfCropped];

            scale3 = MathHelper.Clamp(((float)Math.Sin(time * 1.1f) + 1f) / 2f, 0.151f, 0.8f) * 0.9f;
            Main.spriteBatch.Draw(texture, drawPosition, drawFrame, new Color(b, b, b, 0) * scale3, rotation + time * 1.1f, drawOrigin, scale2 * scale3, SpriteEffects.None, 0f);

            scale3 = MathHelper.Clamp(((float)Math.Sin(time * 0.32111f) + 1f) / 2f, 0.1f, 0.8f) * 1.2f;
            Main.spriteBatch.Draw(texture, drawPosition, drawFrame, new Color(b, b, b, 0) * scale3, rotation + time * 0.4f, drawOrigin, scale2 * scale3, SpriteEffects.None, 0f);

            scale3 = MathHelper.Clamp(((float)Math.Sin(time * 0.741f) + 1f) / 2f, 0.4f, 0.9f) * 1.1f;
            Main.spriteBatch.Draw(texture, drawPosition, drawFrame, new Color(b, b, b, 0) * scale3, rotation + time * 0.6553322f, drawOrigin, scale2 * scale3, SpriteEffects.None, 0f);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            DrawEye(position - origin + frame.Size() / 2f * scale, item, Main.GlobalTime * 2f, 0f, scale);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            var texture = Main.itemTexture[item.type];
            var drawPosition = new Vector2(item.position.X - Main.screenPosition.X + texture.Width / 2 + item.width / 2 - texture.Width / 2, item.position.Y - Main.screenPosition.Y + texture.Height / 2 + item.height - texture.Height + 2f);
            DrawEye(drawPosition, item, Main.GlobalTime * 2f + whoAmI, rotation, scale);
            float distance = (Main.player[Main.myPlayer].Center - item.Center).Length();
            var config = ModContent.GetInstance<AQConfigClient>();
            if (distance > 200f && config.EffectIntensity > 0.5f)
            {
                if (distance > 888f)
                    distance = 888f;
                distance -= 200f;
                float mult = distance / 688f;
                texture = AQTextures.Lights[LightTex.Spotlight80x80];
                var drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
                var drawOrigin = drawFrame.Size() / 2f;
                Main.spriteBatch.Draw(texture, drawPosition, drawFrame, new Color(90, 90, 90, 60) * mult, rotation + MathHelper.PiOver4, drawOrigin, new Vector2(scale / 4f * mult, scale * 1.65f * mult), SpriteEffects.None, 0f);
            }
            return false;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(item.Center, new Vector3(0.8f, 0.8f, 0.8f));
        }
    }
}