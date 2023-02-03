using Aequus.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Content.Carpentery.Photobook
{
    public class PhotobookUIImageViewOverlay : UIElement
    {
        public int photo;
        public PhotobookUIState parent;
        public PhotobookUIElement pictureElement;
        public float vignette;
        public float timer;

        public PhotobookUIImageViewOverlay(int photo, PhotobookUIState parentState, PhotobookUIElement parentElement)
        {
            this.photo = photo;
            parent = parentState;
            pictureElement = parentElement;
        }

        public override void OnInitialize()
        {
            timer = 0f;
            vignette = 0f;
        }

        public override void Update(GameTime gameTime)
        {
            timer++;
            if (vignette < 1f)
            {
                vignette += 0.033f;
                if (vignette > 1f)
                {
                    vignette = 1f;
                }
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            Begin.UI.Begin(spriteBatch);
            spriteBatch.Draw(ModContent.Request<Texture2D>($"{Aequus.AssetsPath}VignetteSmall").Value,
                new Rectangle(-2, -2, Main.screenWidth + 4, Main.screenHeight + 4), Color.White * vignette);

            var photoPlayer = Main.LocalPlayer.GetModPlayer<PhotobookPlayer>();
            if (photoPlayer.Photos[photo].tileMap == null)
                return;

            photoPlayer.photos[photo].LoadTexture();
            if (photoPlayer.photos[photo].Texture.Value == null)
            {
                return;
            }

            var texture = photoPlayer.photos[photo].Texture.Value;
            var r = pictureElement.GetDimensions().ToRectangle();
            var photoNewR = new Rectangle(Main.screenWidth / 2 - texture.Width / 2, Main.screenHeight / 2 - texture.Width / 2, texture.Width, texture.Height);
            float lerpAmt = (float)Math.Pow(Math.Min(timer / 30f, 1f), 2f);
            r.X = (int)MathHelper.Lerp(r.X, photoNewR.X, lerpAmt);
            r.Y = (int)MathHelper.Lerp(r.Y, photoNewR.Y, lerpAmt);
            r.Width = (int)MathHelper.Lerp(r.Width, photoNewR.Width, lerpAmt);
            r.Height = (int)MathHelper.Lerp(r.Height, photoNewR.Height, lerpAmt);

            float scale = 1f;
            int largestImageSide = Math.Max(photoPlayer.photos[photo].Texture.Value.Width, photoPlayer.photos[photo].Texture.Value.Height);
            int shortestUISide = (int)(Math.Min(r.Width, r.Height) * 0.9f);
            if (largestImageSide > shortestUISide)
            {
                scale = shortestUISide / (float)largestImageSide;
            }

            float newScale = 1f;
            shortestUISide = (int)(Math.Min(Main.screenWidth, Main.screenHeight) * 0.9f);
            if (largestImageSide > shortestUISide)
            {
                newScale = shortestUISide / (float)largestImageSide;
            }
            scale = MathHelper.Lerp(scale, newScale, lerpAmt);
            var origin = texture.Size() / 2f;
            var drawCoords = r.Center.ToVector2();
            if (Aequus.HQ)
            {
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        spriteBatch.Draw(texture, drawCoords + new Vector2(2f * i, 2f * j), null, Color.Black, 0f, origin, scale, SpriteEffects.None, 0f);
                    }
                }
            }
            spriteBatch.Draw(texture, drawCoords, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
            base.DrawSelf(spriteBatch);
        }
    }
}