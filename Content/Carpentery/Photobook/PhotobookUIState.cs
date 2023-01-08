using Aequus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Content.Carpentery.Photobook
{
    public class PhotobookUIState : AequusUIState
    {
        public const int PageMove = 4;
        public float scale;

        public UIElement buttonRight;
        public UIElement buttonLeft;
        public int page;
        public PhotobookUIImageViewOverlay view;
        public PhotobookUIElement[] photos;

        public override void OnInitialize()
        {
            float imageScale = scale;
            var book = ModContent.Request<Texture2D>($"{Aequus.AssetsPath}UI/Photobook", AssetRequestMode.ImmediateLoad);
            HAlign = 0.5f;
            VAlign = 0.5f;
            Width.Set(book.Value.Width * imageScale, 0f);
            Height.Set(book.Value.Height * imageScale, 0f);
            photos = new PhotobookUIElement[4];
            for (int k = 0; k < photos.Length; k++)
            {
                if (page + k >= PhotobookPlayer.MyMaxPhotos)
                    break;

                int i = k / 2;
                int j = k % 2;
                photos[k] = new PhotobookUIElement(this, page + k);
                photos[k].Width.Set(190, 0f);
                photos[k].Height.Set(140, 0f);
                photos[k].Left.Set(-photos[k].Width.Pixels / 2, 0.25f + 0.5f * i);
                photos[k].Top.Set(-photos[k].Height.Pixels / 2, 0.25f + 0.4f * j);
                Append(photos[k]);
            }
            LoadCurrentPage();

            buttonLeft = new UIElement();
            buttonLeft.Width.Set(10f, 0f);
            buttonLeft.Height.Set(10f, 0f);
            buttonLeft.Left.Set(23, 0f);
            buttonLeft.VAlign = 0.45f;
            buttonLeft.OnClick += ButtonLeft_OnClick;
            Append(buttonLeft);

            buttonRight = new UIElement();
            buttonRight.Width.Set(10f, 0f);
            buttonRight.Height.Set(10f, 0f);
            buttonRight.Left.Set(-50, 1f);
            buttonRight.VAlign = 0.45f;
            buttonRight.OnClick += ButtonRight_OnClick;
            Append(buttonRight);

            SetupButton(buttonLeft, true);
            SetupButton(buttonRight, false);
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
            var image = ModContent.Request<Texture2D>($"{Aequus.AssetsPath}UI/PhotobookButton{(left ? "Left" : "Right")}", AssetRequestMode.ImmediateLoad);
            element.Width.Set(image.Value.Width, 0f);
            element.Height.Set(image.Value.Height, 0f);
            if (left && page <= 0)
            {
                return;
            }
            if (!left && page >= PhotobookPlayer.MyMaxPhotos - PageMove)
            {
                return;
            }
            var uiImage = new UIImage(image);
            uiImage.HAlign = 0.5f;
            uiImage.VAlign = 0.5f;
            uiImage.Width.Set(image.Value.Width, 0f);
            uiImage.Height.Set(image.Value.Height, 0f);
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

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            var book = ModContent.Request<Texture2D>($"{Aequus.AssetsPath}UI/Photobook", AssetRequestMode.ImmediateLoad);
            Main.spriteBatch.Draw(book.Value, GetDimensions().Center(), null, Color.White, 0f, book.Value.Size() / 2f, scale, SpriteEffects.None, 0f);
            base.DrawSelf(spriteBatch);
        }

        public override void ConsumePlayerControls(Player player)
        {
            if (player.controlInv)
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                player.controlInv = true;
                player.releaseInventory = false;
                if (view == null)
                {
                    player.SetTalkNPC(-1);
                    Aequus.UserInterface.SetState(null);
                }
                else
                {
                    view.Remove();
                    view = null;
                }
            }
            if (Main.mouseLeft && Main.mouseLeftRelease && view != null)
            {
                view.Remove();
                Main.mouseLeftRelease = false;
            }
        }

        public override int GetLayerIndex(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex((l) => l.Name.Equals(AequusUI.InterfaceLayers.Inventory_28));
            if (index == -1)
                return -1;
            return index + 1;
        }

        public override bool ModifyInterfaceLayers(List<GameInterfaceLayer> layers, ref InterfaceScaleType scaleType)
        {
            DisableAnnoyingInventoryLayeringStuff(layers);
            return true;
        }
    }
}