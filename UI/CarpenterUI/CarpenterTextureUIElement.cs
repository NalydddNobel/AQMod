using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
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
            Height = new StyleDimension(GetDimensions().Width, 0f);
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
            maxSize /= 2;
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

                var black = Color.Black.UseA(150);
                if (Aequus.HQ)
                {
                    foreach (var c in AequusHelpers.CircularVector(4))
                    {
                        spriteBatch.Draw(texture.Value, drawCoords + new Vector2(2f) + c * 2f, null, black * 0.1f, 0f, drawOrigin, DrawScale, SpriteEffects.None, 0f);
                    }
                }
                foreach (var c in AequusHelpers.CircularVector(4))
                {
                    spriteBatch.Draw(texture.Value, drawCoords + c * 2f, null, black, 0f, drawOrigin, DrawScale, SpriteEffects.None, 0f);
                }
                spriteBatch.Draw(texture.Value, drawCoords, null, Color.White, 0f, drawOrigin, DrawScale, SpriteEffects.None, 0f);
            }
            base.DrawSelf(spriteBatch);
        }
    }
}