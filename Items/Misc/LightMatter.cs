using Aequus.Common.Configuration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc
{
    public class LightMatter : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(silver: 8);
            Item.maxStack = 999;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var drawPosition = position - origin + frame.Size() / 2f * scale;
            var texture = Aequus.MyTex("Assets/Bloom2");
            var drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
            var drawOrigin = drawFrame.Size() / 2f;
            var itemTexture = TextureAssets.Item[Type].Value;
            float time = Main.GlobalTimeWrappedHourly * 2f;
            float waveFunction = (float)Math.Sin(time);
            scale += waveFunction * 0.1f - 0.1f;
            int b = (int)(110 * scale * ClientConfiguration.Instance.effectIntensity);
            var itemFrame = new Rectangle(0, 0, itemTexture.Width, itemTexture.Height);
            var itemOrigin = itemFrame.Size() / 2f;
            for (float f = 0f; f < 1f; f += 0.125f)
            {
                Main.spriteBatch.Draw(itemTexture, drawPosition + new Vector2(2f * scale, 0f).RotatedBy(Main.GlobalTimeWrappedHourly * 0.9f + f * MathHelper.TwoPi), itemFrame, new Color(20, 20, 50, 0), 0f, itemOrigin, scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(itemTexture, drawPosition, itemFrame, new Color(255, 255, 255, 255), 0f, itemOrigin, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPosition, drawFrame, new Color(230, 230, 255, 200), 0f, drawOrigin, scale / 4f, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            var itemTexture = TextureAssets.Item[Type].Value;
            var drawPosition = new Vector2(Item.position.X - Main.screenPosition.X + itemTexture.Width / 2 + Item.width / 2 - itemTexture.Width / 2, Item.position.Y - Main.screenPosition.Y + itemTexture.Height / 2 + Item.height - itemTexture.Height + 2f);
            var texture = Aequus.MyTex("Assets/Bloom2");
            var drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
            var drawOrigin = drawFrame.Size() / 2f;
            float time = Main.GlobalTimeWrappedHourly * 2f;
            float waveFunction = (float)Math.Sin(time);
            drawPosition = new Vector2(drawPosition.X, drawPosition.Y - (int)(waveFunction * 2f));
            scale += waveFunction * 0.1f - 0.1f;
            float halfScale = scale / 2f;
            float fourthScale = scale / 4f;
            float eighthScale = scale / 8f;
            float sixteenthScale = scale / 16f;
            int b = (int)(60 * scale * ClientConfiguration.Instance.effectIntensity);
            int b2 = (int)(110 * scale * ClientConfiguration.Instance.effectIntensity);
            var itemFrame = new Rectangle(0, 0, itemTexture.Width, itemTexture.Height);
            var itemOrigin = itemFrame.Size() / 2f;
            for (float f = 0f; f < 1f; f += 0.125f)
            {
                Main.spriteBatch.Draw(itemTexture, drawPosition + new Vector2(2f * scale, 0f).RotatedBy(Main.GlobalTimeWrappedHourly * 0.9f + f * MathHelper.TwoPi), itemFrame, new Color(20, 20, 50, 0), 0f, itemOrigin, scale * 0.8f, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(itemTexture, drawPosition, itemFrame, new Color(255, 255, 255, 255), rotation, itemOrigin, scale, SpriteEffects.None, 0f);

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
            texture = Aequus.MyTex("Assets/LightRay");

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
            Lighting.AddLight(Item.Center, new Vector3(0.8f, 0.8f, 0.8f));
        }
    }
}