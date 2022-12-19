using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;

namespace Aequus.UI.CarpenterUI
{
    public class CarpenterTextureUIElement : UIElement
    {
        private Asset<Texture2D> texture;
        public Asset<Texture2D> Texture
        {
            get => texture;

            set
            {
                texture = value;
                recalculate = true;
            }
        }
        private float scale;
        public float Scale
        {
            get => scale;

            set
            {
                scale = value;
                DrawScale = value;
                recalculate = true;
            }
        }
        public float DrawScale { get; private set; }

        public bool recalculate;
        public Action OnRecalculateTextureSize;

        public CarpenterTextureUIElement(Asset<Texture2D> texture)
        {
            this.texture = texture;
            scale = 1f;
            recalculate = true;
            DrawScale = 1f;
        }

        public override void Recalculate()
        {
            Height = new StyleDimension(texture.Value.Height, 0f);
            base.Recalculate();
            DrawScale = Scale;
            if (!texture.IsLoaded)
            {
                recalculate = true;
                return;
            }

            int largestSide = (int)((texture.Value.Width > texture.Value.Height ? texture.Value.Width : texture.Value.Height) * DrawScale);
            int maxSize = (int)GetDimensions().Width;
            int maxSize2 = (int)GetDimensions().Height;
            maxSize = maxSize < maxSize2 ? maxSize : maxSize2;
            maxSize *= 2;
            if (largestSide > maxSize)
            {
                DrawScale = maxSize / (float)largestSide;
            }
            OnRecalculateTextureSize();
            recalculate = false;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (recalculate)
            {
                Recalculate();
            }
            if (texture.IsLoaded)
            {
                var d = GetDimensions();
                var drawCoords = new Vector2(d.X + d.Width / 2f, d.Y + d.Height / 2f);
                var drawOrigin = texture.Value.Size() / 2f;
                drawCoords.Y += (d.Height / 2f - drawOrigin.Y) * 0.9f;
                Utils.DrawInvBG(spriteBatch, drawCoords.X - texture.Value.Width / 2f, drawCoords.Y - texture.Value.Height / 2f, texture.Value.Width, texture.Value.Height, (Color.Blue.SaturationMultiply(0.3f) * 0.3f).UseA(200));
                //AequusHelpers.DrawRectangle(Utils.CenteredRectangle(drawCoords, texture.Value.Bounds.Size()), Color.Red * 0.2f);
                DrawGridCorner(spriteBatch, drawCoords, 1, 1);
                DrawGridCorner(spriteBatch, drawCoords, -1, 1);
                DrawGridCorner(spriteBatch, drawCoords, 1, -1);
                DrawGridCorner(spriteBatch, drawCoords, -1, -1);
                spriteBatch.Draw(texture.Value, drawCoords, null, Color.White, 0f, drawOrigin, DrawScale, SpriteEffects.None, 0f);
            }
            base.DrawSelf(spriteBatch);
        }

        public void DrawGridCorner(SpriteBatch spriteBatch, Vector2 drawCoords, int x, int y)
        {
            var origin = TextureAssets.CursorRadial.Value.Size() / 4f;
            spriteBatch.Draw(TextureAssets.CursorRadial.Value,
                drawCoords + new Vector2((texture.Value.Width * DrawScale / 2f - 8f - origin.X) * x, (texture.Value.Height * DrawScale / 2f - 8f - origin.Y) * y),
                TextureAssets.CursorRadial.Value.Frame(horizontalFrames: 2, verticalFrames: 2, x == -1 ? 1 : 0, y == -1 ? 1 : 0),
                Color.White, 0f, origin, DrawScale, SpriteEffects.None, 0f);
        }
    }
}