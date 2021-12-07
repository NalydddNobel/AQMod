using AQMod.Common.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;

namespace AQMod.Assets.LegacyItemOverlays
{
    public class OutlineOverlay : ItemOverlayData
    {
        protected readonly Func<float, Color> getOutlineColor;

        public override void PostDrawWorld(Item item, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            var frame = new Rectangle(0, 0, Main.itemTexture[item.type].Width, Main.itemTexture[item.type].Height);
            var drawPosition = new Vector2(item.position.X - Main.screenPosition.X + frame.Width / 2 + item.width / 2 - frame.Width / 2, item.position.Y - Main.screenPosition.Y + frame.Height / 2 + item.height - frame.Height);
            drawPosition = new Vector2((int)drawPosition.X, drawPosition.Y);
            Vector2 origin = frame.Size() / 2f;
            var drawData = new DrawData(Main.itemTexture[item.type], drawPosition, frame, item.modItem.GetAlpha(default(Color)).GetValueOrDefault(), rotation, origin, scale, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            BatcherMethods.GeneralEntities.BeginShader(Main.spriteBatch);
            var effect = EffectCache.s_OutlineColor;
            effect.UseColor(getOutlineColor(Main.GlobalTime));
            effect.Apply(drawData);
            drawData.Draw(Main.spriteBatch);
            Main.spriteBatch.End();
            BatcherMethods.GeneralEntities.Begin(Main.spriteBatch);
        }

        public override bool PreDrawInventory(Player player, AQPlayer aQPlayer, Item item, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var drawData = new DrawData(item.GetTexture(), position, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            BatcherMethods.UI.Begin(Main.spriteBatch, BatcherMethods.Shader);
            var effect = EffectCache.s_OutlineColor;
            effect.UseColor(getOutlineColor(Main.GlobalTime * 2f));
            effect.Apply(drawData);
            drawData.Draw(Main.spriteBatch);
            Main.spriteBatch.End();
            BatcherMethods.UI.Begin(Main.spriteBatch, BatcherMethods.Regular);
            return true;
        }
    }
}