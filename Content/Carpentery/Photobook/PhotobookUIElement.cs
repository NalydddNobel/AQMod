using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Content.Carpentery.Photobook
{
    public class PhotobookUIElement : UIElement
    {
        public int photo;
        public UIElement Text;
        public PhotobookUIState parent;
        public PhotobookUIImageViewOverlay view;

        public PhotobookUIElement(PhotobookUIState parentState, int photo)
        {
            this.photo = photo;
            parent = parentState;
        }

        public override void OnInitialize()
        {
            OnClick += PhotobookUIElement_OnClick;
        }

        private void PhotobookUIElement_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!Main.LocalPlayer.GetModPlayer<PhotobookPlayer>().photos[photo].HasData)
                return;
            view = new PhotobookUIImageViewOverlay(photo, parent, this);
            parent.view = view;
            parent.Append(view);
        }

        public void InitEmptyPhoto()
        {
            var uiText = new UIText("No Photo!")
            {
                HAlign = 0.5f,
                VAlign = 0.5f
            };
            Text.Append(uiText);
        }

        public void LoadPhoto(int photo)
        {
            this.photo = photo;
            RemoveAllChildren();
            var photoPlayer = Main.LocalPlayer.GetModPlayer<PhotobookPlayer>();

            Text = new UIElement();
            Text.Width.Set(0f, 1f);
            Text.Height.Set(0f, 1f);
            Append(Text);

            if (photoPlayer.Photos[photo].tileMap == null)
            {
                InitEmptyPhoto();
            }
            else
            {
                photoPlayer.photos[photo].LoadTexture();
                if (photoPlayer.photos[photo].Texture.Value != null)
                {
                    var uiText2 = new UIText("Loading...")
                    {
                        HAlign = 0.5f,
                        VAlign = 0.5f
                    };
                    Text.Append(uiText2);
                }

                var deleteButton = new UIImageButton(ModContent.Request<Texture2D>($"{Aequus.VanillaTexture}UI/SearchCancel"));
                deleteButton.Left.Set(-28f, 1f);
                deleteButton.Top.Set(8f, 0f);
                deleteButton.OnClick += DeleteButton_OnClick;
                Append(deleteButton);
            }

            var uiText = new UIText($"{photo + 1}.")
            {
                HAlign = 0.05f,
                VAlign = 0.05f
            };
            Append(uiText);
            uiText = new UIText($"{photoPlayer.photos[photo].Date}.", 0.8f)
            {
                HAlign = 0.05f,
                VAlign = 0.95f,
                TextColor = Color.Yellow
            };
            Append(uiText);
        }

        private void DeleteButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            var photoPlayer = Main.LocalPlayer.GetModPlayer<PhotobookPlayer>();
            photoPlayer.photos[photo] = new PhotoData();
            LoadPhoto(photo);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            var r = GetDimensions().ToRectangle();
            Utils.DrawInvBG(Main.spriteBatch, r, new Color(200, 200, 200, 255));
            var photoPlayer = Main.LocalPlayer.GetModPlayer<PhotobookPlayer>();
            if (photoPlayer.Photos[photo].tileMap == null)
                return;

            photoPlayer.photos[photo].LoadTexture();
            if (photoPlayer.photos[photo].Texture.Value == null)
            {
                return;
            }
            Text.RemoveAllChildren();
            float scale = 1f;
            int largestImageSide = Math.Max(photoPlayer.photos[photo].Texture.Value.Width, photoPlayer.photos[photo].Texture.Value.Height);
            int shortestUISide = (int)(Math.Min(r.Width, r.Height) * 0.9f);
            if (largestImageSide > shortestUISide)
            {
                scale = shortestUISide / (float)largestImageSide;
            }
            var origin = photoPlayer.photos[photo].Texture.Value.Size() / 2f;
            var drawCoords = r.Center.ToVector2();
            if (Aequus.HQ)
            {
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        spriteBatch.Draw(photoPlayer.photos[photo].Texture.Value, drawCoords + new Vector2(2f * i, 2f * j), null, Color.Black, 0f, origin, scale, SpriteEffects.None, 0f);
                    }
                }
            }
            spriteBatch.Draw(photoPlayer.photos[photo].Texture.Value, drawCoords, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
        }
    }
}