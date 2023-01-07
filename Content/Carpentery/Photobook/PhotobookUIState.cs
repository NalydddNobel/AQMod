using Aequus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Content.Carpentery.Photobook
{
    public class PhotobookUIState : AequusUIState
    {
        public const int PageMove = 4;

        public UIElement buttonRight;
        public UIElement buttonLeft;
        public int page;
        public PhotobookUIElement[] photos;

        public override void OnInitialize()
        {
            HAlign = 0.5f;
            VAlign = 0.5f;
            float imageScale = 1.5f;
            var image = ModContent.Request<Texture2D>($"{Aequus.AssetsPath}UI/Photobook", AssetRequestMode.ImmediateLoad);
            var uiImage = new UIImage(image);
            uiImage.HAlign = 0.379f * imageScale;
            uiImage.VAlign = 0.5f * imageScale;
            uiImage.Width.Set(image.Value.Width, 0f);
            uiImage.Height.Set(image.Value.Height, 0f);
            uiImage.NormalizedOrigin = new Vector2(0f);
            uiImage.ImageScale = imageScale;
            Width.Set(image.Value.Width * imageScale, 0f);
            Height.Set(image.Value.Height * imageScale, 0f);
            Append(uiImage);

            photos = new PhotobookUIElement[4];
            for (int k = 0; k < photos.Length; k++)
            {
                if (page + k >= PhotobookPlayer.MyMaxPhotos)
                    break;

                int i = k / 2;
                int j = k % 2;
                photos[k] = new PhotobookUIElement(page + k);
                photos[k].Width.Set(190, 0f);
                photos[k].Height.Set(140, 0f);
                photos[k].Left.Set(-82, 0.25f + 0.5f * i);
                photos[k].Top.Set(-36, 0.25f + 0.4f * j);
                Append(photos[k]);
            }
            LoadCurrentPage();

            var exitButton = new UIImageButton(TextureAssets.Cd);
            exitButton.Left.Set(40f, 0f);
            exitButton.Top.Set(20f, 0f);
            exitButton.OnClick += ExitButton_OnClick;
            Append(exitButton);

            buttonLeft = new UIElement();
            buttonLeft.Width.Set(10f, 0f);
            buttonLeft.Height.Set(10f, 0f);
            buttonLeft.Left.Set(30, 0f);
            buttonLeft.VAlign = 0.535f;
            buttonLeft.OnClick += ButtonLeft_OnClick;
            Append(buttonLeft);

            buttonRight = new UIElement();
            buttonRight.Width.Set(10f, 0f);
            buttonRight.Height.Set(10f, 0f);
            buttonRight.Left.Set(-30, 1f);
            buttonRight.VAlign = 0.535f;
            buttonRight.OnClick += ButtonRight_OnClick;
            Append(buttonRight);

            SetupButton(buttonLeft, true);
            SetupButton(buttonRight, false);
        }

        private void ExitButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            Aequus.UserInterface.SetState(null);
        }

        private void ButtonLeft_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            page -= PageMove;
            if (page < 0)
            {
                page = 0;
                return;
            }
            SoundEngine.PlaySound(SoundID.MenuTick);
            SetupButton(buttonRight, false);
            SetupButton(buttonLeft, true);
            LoadCurrentPage();
        }

        private void ButtonRight_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            page += PageMove;
            if (page > PhotobookPlayer.MyMaxPhotos - PageMove)
            {
                page = PhotobookPlayer.MyMaxPhotos - PageMove;
                return;
            }
            SoundEngine.PlaySound(SoundID.MenuTick);
            SetupButton(buttonRight, false);
            SetupButton(buttonLeft, true);
            LoadCurrentPage();
        }

        public void SetupButton(UIElement element, bool left)
        {
            element.RemoveAllChildren();
            if (left && page <= 0)
            {
                return;
            }
            if (!left && page >= PhotobookPlayer.MyMaxPhotos - PageMove)
            {
                return;
            }
            var image = ModContent.Request<Texture2D>($"{Aequus.AssetsPath}UI/PhotobookButton{(left ? "Left" : "Right")}", AssetRequestMode.ImmediateLoad);
            var uiImage = new UIImage(image);
            uiImage.HAlign = 0.5f;
            uiImage.VAlign = 0.5f;
            uiImage.Width.Set(image.Value.Width, 0f);
            uiImage.Height.Set(image.Value.Height, 0f);
            element.Width = uiImage.Width;
            element.Height = uiImage.Height;
            element.Append(uiImage);
        }

        public void LoadCurrentPage()
        {
            for (int i = 0; i < 4; i++)
            {
                photos[i].LoadPhoto(page + i);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (GetDimensions().ToRectangle().Contains(Main.mouseX, Main.mouseY))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            base.Update(gameTime);
        }
    }
}