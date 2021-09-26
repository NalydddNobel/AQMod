using AQMod.Assets.Enumerators;
using AQMod.Assets.Textures;
using AQMod.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace AQMod.Assets.ItemOverlays
{
    public class EnergyOverlay : ItemOverlay
    {
        protected static float energyColorValue => ((float)Math.Sin(Main.GlobalTime) + 1f) * 45f;

        public readonly GlowID type;
        public readonly Func<float, Color> getOutlineColor;
        public readonly Func<float, Color> getSpotlightColor;

        public EnergyOverlay(GlowID type, Func<float, Color> getOutlineColor, Func<float, Color> getSpotlightColor)
        {
            this.type = type;
            this.getOutlineColor = getOutlineColor;
            this.getSpotlightColor = getSpotlightColor;
        }

        public override void DrawWorld(Item item, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Rectangle frame = new Rectangle(0, 0, Main.itemTexture[item.type].Width, Main.itemTexture[item.type].Height);
            Vector2 drawPosition = new Vector2(item.position.X - Main.screenPosition.X + frame.Width / 2 + item.width / 2 - frame.Width / 2, item.position.Y - Main.screenPosition.Y + frame.Height / 2 + item.height - frame.Height + (int)(Math.Sin(Main.GlobalTime) * 2f));
            float colorOffset = energyColorValue;
            var texture = SpriteUtils.Textures.Lights[LightID.Spotlight30x30];
            Main.spriteBatch.Draw(texture, drawPosition - new Vector2(0.5f, 1.5f), null, getSpotlightColor(colorOffset), rotation, new Vector2(texture.Width, texture.Height) / 2f, scale + (float)Math.Sin(Main.GlobalTime * 3.14f) * 0.1f + 0.65f, SpriteEffects.None, 0f);
            texture = SpriteUtils.Textures.Glows[type];
            Main.spriteBatch.Draw(texture, drawPosition, null, getOutlineColor(colorOffset), rotation, new Vector2(texture.Width, texture.Height) / 2f, scale, SpriteEffects.None, 0f);
            Vector2 origin = frame.Size() / 2f;
            Main.spriteBatch.Draw(Main.itemTexture[item.type], drawPosition, frame, item.modItem.GetAlpha(default(Color)).GetValueOrDefault(), rotation, origin, scale, SpriteEffects.None, 0f);
        }

        public override void DrawInventory(Player player, AQPlayer aQPlayer, Item item, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = SpriteUtils.Textures.Glows[type];
            Main.spriteBatch.Draw(texture, position, null, getOutlineColor(energyColorValue), 0f, origin + new Vector2(2, 2), scale, SpriteEffects.None, 0f);
        }
    }
}