using AQMod.Assets;
using AQMod.Assets.Effects;
using AQMod.Items.DrawOverlays;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace AQMod.Items.Materials.Energies
{
    public class EnergyOverlay : IOverlayDrawWorld, IOverlayDrawInventory
    {
        protected readonly Func<Color> getOutlineColor;
        protected readonly Func<Color> getSpotlightColor;
        protected readonly Vector2 _spotlightOffset;

        public EnergyOverlay(Func<Color> getOutlineColor, Func<Color> getSpotlightColor, Vector2 spotlightOffset = default(Vector2))
        {
            this.getOutlineColor = getOutlineColor;
            this.getSpotlightColor = getSpotlightColor;
            _spotlightOffset = spotlightOffset;
        }

        bool IOverlayDrawWorld.PreDrawWorld(Item item, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return false;
        }

        void IOverlayDrawWorld.PostDrawWorld(Item item, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            var texture = TextureAssets.Item[item.type].Value;
            var frame = new Rectangle(0, 0, texture.Width, texture.Height);
            var drawPosition = new Vector2(item.position.X - Main.screenPosition.X + frame.Width / 2 + item.width / 2 - frame.Width / 2, item.position.Y - Main.screenPosition.Y + frame.Height / 2 + item.height - frame.Height);
            drawPosition = new Vector2((int)drawPosition.X, drawPosition.Y);
            Main.spriteBatch.End();
            BatcherMethods.GeneralEntities.BeginShader(Main.spriteBatch);
            SamplerDraw.Light(drawPosition + _spotlightOffset, (scale + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3.14f) * 0.1f + 0.65f) * 30f, getSpotlightColor());
            Vector2 origin = frame.Size() / 2f;
            var drawData = new DrawData(texture, drawPosition, frame, item.GetAlpha(default(Color)), rotation, origin, scale, SpriteEffects.None, 0);
            var effect = EffectCache.s_Outline;
            effect.UseColor(getOutlineColor());
            effect.Apply(drawData);
            drawData.Draw(Main.spriteBatch);
            Main.spriteBatch.End();
            BatcherMethods.GeneralEntities.Begin(Main.spriteBatch);
        }

        bool IOverlayDrawInventory.PreDrawInv(Player player, AQPlayer aQPlayer, Item item, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var texture = TextureAssets.Item[item.type].Value;
            var drawData = new DrawData(texture, position, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            BatcherMethods.UI.Begin(Main.spriteBatch, BatcherMethods.Shader);
            var effect = EffectCache.s_Outline;
            effect.UseColor(getOutlineColor());
            effect.Apply(drawData);
            drawData.Draw(Main.spriteBatch);
            Main.spriteBatch.End();
            BatcherMethods.UI.Begin(Main.spriteBatch, BatcherMethods.Regular);
            return true;
        }

        void IOverlayDrawInventory.PostDrawInv(Player player, AQPlayer aQPlayer, Item item, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
        }
    }
}