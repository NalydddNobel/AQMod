using Aequus.Content.CarpenterBounties;
using Aequus.Items.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.UI.CarpenterUI
{
    public class CarpenterUIElement : UIElement
    {
        public CarpenterUIState bountyUICache;

        public CarpenterBounty bounty;
        public Item listItem;
        public CarpenterBountyItem ListItem => listItem.ModItem<CarpenterBountyItem>();
        public Item rewardItem;

        public UIPanel panel;
        public UIPanel textPanel;

        private bool hoverTick;

        public static Color BasePanelColor => (AequusUI.invBackColor * 0.8f).UseA(255) * AequusUI.invBackColorMultipler;
        public static Color BasePanelColor_Hover => AequusUI.invBackColor * 1.2f * AequusUI.invBackColorMultipler;

        public CarpenterUIElement(CarpenterUIState bountyUI, CarpenterBounty bounty)
        {
            this.bounty = bounty;
            bountyUICache = bountyUI;
            listItem = bounty.ProvidePortableBounty().Item;
            rewardItem = bounty.ProvideBountyRewardItems()[0];
        }

        public void Setup()
        {
            listItem.buy = true;
            panel = new UIPanel();
            panel.Width.Set(Width.Pixels + 60, Width.Percent);
            panel.Height.Set(Height.Pixels, Height.Percent);
            Append(panel);

            textPanel = new UIPanel(Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Stat_Panel"), null, 12, 7)
            {
                Top = new StyleDimension(10, 0f),
                Left = new StyleDimension((int)(64 * 0.8f) + 20, 0f),
                Width = new StyleDimension(-84, 1f),
                BackgroundColor = new Color(33, 46, 80, 255),
                PaddingLeft = 4f,
                PaddingRight = 4f
            };
            textPanel.Height = new StyleDimension(-textPanel.Top.Pixels - 8, 1f);
            Append(textPanel);

            var title = new UIText(ListItem.BountyFancyName, 0.5f, large: true)
            {
                HAlign = 0f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
                PaddingLeft = 4f,
                PaddingTop = 0f,
                TextOriginX = 0.5f,
                TextOriginY = 0f,
            };
            textPanel.Append(title);

            var description = new UIText(ListItem.BountyDescription, 0.9f)
            {
                HAlign = 0f,
                VAlign = 0f,
                Top = new StyleDimension(28, 0f),
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
                PaddingLeft = 4f,
                PaddingTop = 0f,
                TextOriginX = 0f,
                TextOriginY = 0f,
            };
            textPanel.Append(description);

            var text = new UIText(ListItem.BountyFancyRequirements, 0.7f)
            {
                HAlign = 0f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Top = new StyleDimension(24 + 28, 0f),
                PaddingLeft = 4f,
                PaddingTop = 0f,
                TextOriginX = 0f,
                TextOriginY = 0f,
            };
            textPanel.Append(text);
            text.IsWrapped = true;

            string hintKey = bounty.LanguageKey + ".Hint";
            string hintText = Language.GetTextValue(hintKey);
            if (hintText == hintKey)
            {
                return;
            }

            int height = (int)(text.MinHeight.Pixels * 0.7f);
            text = new UIText(hintText, 0.66f)
            {
                HAlign = 0f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Top = new StyleDimension(32 + height, 0f),
                PaddingLeft = 4f,
                PaddingTop = 0f,
                TextOriginX = 0f,
                TextOriginY = 0f,
                TextColor = Color.Lerp(Color.Teal, Color.White, 0.8f),
            };
            textPanel.Append(text);
            text.IsWrapped = true;
        }

        public override void OnInitialize()
        {
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            bool old = hoverTick;
            hoverTick = IsMouseHovering;
            if (old != hoverTick)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                if (hoverTick)
                {
                    HoverColor(true);
                }
                else
                {
                    HoverColor(false);
                }
            }
            base.DrawSelf(spriteBatch);
        }

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            var rect = GetDimensions().ToRectangle();
            UpdatePanel(spriteBatch, rect);
            base.DrawChildren(spriteBatch);
            DrawItems(spriteBatch, rect);
        }

        public void UpdatePanel(SpriteBatch spriteBatch, Rectangle rect)
        {
            if (Main.mouseLeft && Main.mouseLeftRelease)
            {
                if (IsMouseHovering)
                {
                    bountyUICache.Select(this);
                    Main.mouseLeftRelease = false;
                    SoundEngine.PlaySound(SoundID.MenuOpen);
                    HoverColor(true);
                }
                else if (!bountyUICache.IsMouseHovering && bountyUICache.selected != null)
                {
                    bountyUICache.Select(null);
                    SoundEngine.PlaySound(SoundID.MenuClose);
                }
            }
        }

        public void DrawItems(SpriteBatch spriteBatch, Rectangle rect)
        {
            var itemSlotRect = new Rectangle(rect.X + 10, rect.Y + 10, (int)(64 * 0.8f), (int)(64 * 0.8f));
            string rewardText = AequusText.GetText("Chat.Carpenter.UI.Reward");
            var rewardTextOrigin = FontAssets.MouseText.Value.MeasureString(rewardText);
            rewardTextOrigin.X /= 2f;
            DrawItemSlot(spriteBatch, rect, itemSlotRect, rewardItem);
            if (itemSlotRect.Contains(Main.mouseX, Main.mouseY))
            {
                AequusUI.HoverItem(rewardItem);
            }
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, rewardText,
                new Vector2(itemSlotRect.X + itemSlotRect.Width / 2f, itemSlotRect.Y + 16), Color.White, 0f, rewardTextOrigin, Vector2.One * 0.75f);
        }

        public void DrawItemSlot(SpriteBatch spriteBatch, Rectangle rect, Rectangle itemSlotRect, Item item)
        {
            if (itemSlotRect.Contains(Main.mouseX, Main.mouseY))
            {
                AequusUI.HoverItem(rewardItem);
            }
            Utils.DrawInvBG(spriteBatch, itemSlotRect, new Color(40, 40, 100, 255) * AequusUI.invBackColorMultipler);

            Main.instance.LoadItem(item.type);
            var texture = TextureAssets.Item[item.type].Value;
            float scale = 1f;
            int largestAmt = texture.Width > texture.Height ? texture.Width : texture.Height;

            if (largestAmt > itemSlotRect.Width)
            {
                scale = largestAmt / (float)rect.Width;
            }

            item.GetItemDrawData(out var frame);
            spriteBatch.Draw(texture, itemSlotRect.Center(), frame, Color.White, 0f, frame.Size() / 2f, scale, SpriteEffects.None, 0f);

        }

        public void HoverColor(bool value)
        {
            if (value)
            {
                panel.BackgroundColor = BasePanelColor_Hover;
                panel.BorderColor = BasePanelColor * 3f;
            }
            else
            {
                panel.BackgroundColor = BasePanelColor;
                panel.BorderColor = (BasePanelColor * 0.5f).UseA(255);
            }
        }
    }
}