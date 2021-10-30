using AQMod.Assets;
using AQMod.Common.Config;
using AQMod.Common.Utilities;
using AQMod.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;

namespace AQMod.Assets.ItemOverlays
{
    public class EnergyOverlay : ItemOverlayData
    {
        protected static float energyColorValue => ((float)Math.Sin(Main.GlobalTime) + 1f) * 45f;

        protected readonly Func<float, Color> getOutlineColor;
        protected readonly Func<float, Color> getSpotlightColor;
        protected readonly Vector2 _spotlightOffset;

        public EnergyOverlay(Func<float, Color> getOutlineColor, Func<float, Color> getSpotlightColor, Vector2 spotlightOffset = default(Vector2))
        {
            this.getOutlineColor = getOutlineColor;
            this.getSpotlightColor = getSpotlightColor;
            _spotlightOffset = spotlightOffset;
        }

        public override void DrawWorld(Item item, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            var frame = new Rectangle(0, 0, Main.itemTexture[item.type].Width, Main.itemTexture[item.type].Height);
            var drawPosition = new Vector2(item.position.X - Main.screenPosition.X + frame.Width / 2 + item.width / 2 - frame.Width / 2, item.position.Y - Main.screenPosition.Y + frame.Height / 2 + item.height - frame.Height);
            drawPosition = new Vector2((int)drawPosition.X, drawPosition.Y);
            bool resetBatch = false;
            if (AQConfigClient.Instance.SpotlightShader)
            {
                resetBatch = true;
                Main.spriteBatch.End();
                BatcherMethods.StartShaderBatch_GeneralEntities(Main.spriteBatch);
                SamplerDraw.Light(drawPosition + _spotlightOffset, (scale + (float)Math.Sin(Main.GlobalTime * 3.14f) * 0.1f + 0.65f) * 30f, getSpotlightColor(Main.GlobalTime));
            }
            Vector2 origin = frame.Size() / 2f;
            var drawData = new DrawData(Main.itemTexture[item.type], drawPosition, frame, item.modItem.GetAlpha(default(Color)).GetValueOrDefault(), rotation, origin, scale, SpriteEffects.None, 0);
            if (AQConfigClient.Instance.OutlineShader)
            {
                if (!resetBatch)
                {
                    Main.spriteBatch.End();
                    BatcherMethods.StartShaderBatch_GeneralEntities(Main.spriteBatch);
                }
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
            if (AQConfigClient.Instance.OutlineShader)
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