using AQMod.Assets.Graphics;
using AQMod.Common.Config;
using AQMod.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;

namespace AQMod.Assets.ItemOverlays
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
            bool resetBatch = false;
            if (AQConfigClient.Instance.OutlineShader)
            {
                Main.spriteBatch.End();
                BatcherMethods.StartShaderBatch_GeneralEntities(Main.spriteBatch);
                resetBatch = true;
                var effect = GameShaders.Misc["AQMod:OutlineColor"];
                effect.UseColor(getOutlineColor(Main.GlobalTime));
                effect.Apply(drawData);
            }
            drawData.Draw(Main.spriteBatch);
            if (resetBatch)
            {
                Main.spriteBatch.End();
                BatcherMethods.StartBatch_GeneralEntities(Main.spriteBatch);
            }
        }

        public override bool PreDrawInventory(Player player, AQPlayer aQPlayer, Item item, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            bool resetBatch = false;
            var drawData = new DrawData(item.GetTexture(), position, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0);
            if (EffectCache.Instance.Outline != null)
            {
                resetBatch = true;
                Main.spriteBatch.End();
                BatcherMethods.StartShaderBatch_UI(Main.spriteBatch);
                var effect = GameShaders.Misc["AQMod:OutlineColor"];
                effect.UseColor(getOutlineColor(Main.GlobalTime * 2f));
                effect.Apply(drawData);
            }
            drawData.Draw(Main.spriteBatch);
            if (resetBatch)
            {
                Main.spriteBatch.End();
                BatcherMethods.StartBatch_UI(Main.spriteBatch);
            }
            return true;
        }
    }
}