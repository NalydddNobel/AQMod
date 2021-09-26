using AQMod.Assets.Enumerators;
using AQMod.Assets.Textures;
using AQMod.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace AQMod.Assets.ItemOverlays
{
    public class UltimateEnergyOverlay : EnergyOverlay
    {
        public readonly GlowID type2;

        public UltimateEnergyOverlay(GlowID type, GlowID type2, Func<float, Color> getOutlineColor, Func<float, Color> getSpotlightColor) : base(type, getSpotlightColor, getSpotlightColor)
        {
            this.type2 = type2;
        }

        public override void DrawWorld(Item item, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Rectangle frame = new Rectangle(0, 0, Main.itemTexture[item.type].Width, Main.itemTexture[item.type].Height);
            Vector2 drawPosition = new Vector2(item.position.X - Main.screenPosition.X + frame.Width / 2 + item.width / 2 - frame.Width / 2, item.position.Y - Main.screenPosition.Y + frame.Height / 2 + item.height - frame.Height + (int)Math.Sin(Main.GlobalTime) * 2f);
            float colorOffset = energyColorValue;
            var texture = SpriteUtils.Textures.Lights[LightID.Spotlight30x30];
            Main.spriteBatch.Draw(texture, drawPosition - new Vector2(0.5f, 1.5f), null, getSpotlightColor(colorOffset), rotation, new Vector2(texture.Width, texture.Height) / 2f, scale + (float)Math.Sin(Main.GlobalTime * 3.14f) * 0.1f + 0.65f, SpriteEffects.None, 0f);
            texture = SpriteUtils.Textures.Glows[type];
            Main.spriteBatch.Draw(texture, drawPosition, null, getOutlineColor(colorOffset), rotation, new Vector2(texture.Width, texture.Height) / 2f, scale, SpriteEffects.None, 0f);
            Vector2 origin = frame.Size() / 2f;
            Main.spriteBatch.Draw(Main.itemTexture[item.type], drawPosition, frame, item.modItem.GetAlpha(default(Color)).GetValueOrDefault(), rotation, origin, scale, SpriteEffects.None, 0f);
            texture = SpriteUtils.Textures.Glows[type2];
            float offset = ((float)Math.Sin(Main.GlobalTime * 4f) + 1f) * 2f * scale;
            Main.spriteBatch.Draw(texture, drawPosition - new Vector2(offset, 0f), null, getOutlineColor(((float)Math.Cos(Main.GlobalTime * 2f) + 1f) * 45f) * 0.25f, 0f, origin, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPosition - new Vector2(-offset, 0f), null, getOutlineColor(((float)Math.Cos(Main.GlobalTime * 2f + MathHelper.PiOver2) + 1f) * 45f) * 0.25f, 0f, origin, scale, SpriteEffects.None, 0f);
        }

        public override void DrawInventory(Player player, AQPlayer aQPlayer, Item item, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = SpriteUtils.Textures.Glows[type];
            Main.spriteBatch.Draw(texture, position, null, getOutlineColor(energyColorValue), 0f, origin + new Vector2(2, 2), scale, SpriteEffects.None, 0f);
            texture = SpriteUtils.Textures.Glows[type2];
            float offset = ((float)Math.Sin(Main.GlobalTime * 4f) + 1f) * 2f * scale;
            Main.spriteBatch.Draw(texture, position - new Vector2(offset, 0f), null, getOutlineColor(((float)Math.Cos(Main.GlobalTime * 2f) + 1f) * 45f) * 0.25f, 0f, origin, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, position - new Vector2(-offset, 0f), null, getOutlineColor(((float)Math.Cos(Main.GlobalTime * 2f + MathHelper.PiOver2) + 1f) * 45f) * 0.25f, 0f, origin, scale, SpriteEffects.None, 0f);
        }
    }
}