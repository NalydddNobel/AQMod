using Aequus.Content.CarpenterBounties;
using Aequus.Items.Tools.Misc;
using Aequus.NPCs.Friendly.Town;
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
using Terraria.UI.Chat;

namespace Aequus.UI.States
{
    public class CarpenterBountyUI : UIState
    {
        public class CarpenterBountyUIElement : UIElement
        {
            public CarpenterBountyUI bountyUICache;

            public CarpenterBounty bounty;
            public Item listItem;
            public CarpenterBountyItem ListItem => listItem.ModItem<CarpenterBountyItem>();
            public Item rewardItem;

            public UIPanel panel;
            public UIPanel textPanel;

            private bool hoverTick;

            public static Color BasePanelColor => (AequusUI.invBackColor * 0.8f).UseA(255) * AequusUI.invBackColorMultipler;
            public static Color BasePanelColor_Hover => AequusUI.invBackColor * 1.2f * AequusUI.invBackColorMultipler;

            public CarpenterBountyUIElement(CarpenterBountyUI bountyUI, CarpenterBounty bounty)
            {
                bountyUICache = bountyUI;
                listItem = bounty.ProvideBountyItem().Item;
                rewardItem = bounty.ProvideBountyRewardItem();
            }

            public override void OnInitialize()
            {
                listItem.buy = true;
                panel = new UIPanel();
                panel.Width.Set(Width.Pixels + 40, Width.Percent);
                panel.Height.Set(Height.Pixels, Height.Percent);
                Append(panel);

                textPanel = new UIPanel(Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Stat_Panel"), null, 12, 7)
                {
                    Top = new StyleDimension(10, 0f),
                    Left = new StyleDimension((int)(64 * 0.8f) + 20, 0f),
                    Width = new StyleDimension(-84, 1f),
                    BackgroundColor = new Color(43, 56, 101, 255),
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

                var requirements = new UIText(ListItem.BountyFancyRequirements, 0.7f)
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
                textPanel.Append(requirements);
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
                var itemSlotRect = new Rectangle(rect.X + 10, rect.Y + 10, (int)(64 * 0.8f), (int)(64 * 0.8f));
                if (itemSlotRect.Contains(Main.mouseX, Main.mouseY) && Main.mouseItem.IsAir)
                {
                    bool buyItem = Main.mouseLeft && Main.mouseLeftRelease;
                    if (Main.mouseRight && Main.mouseRightRelease)
                    {
                        buyItem = true;
                        Main.mouseRightRelease = false;
                    }
                    if (buyItem)
                    {
                        Main.LocalPlayer.GetItemExpectedPrice(listItem, out var calcForSelling, out var calcForBuying);

                        if (Main.LocalPlayer.CanBuyItem(calcForBuying) && Main.LocalPlayer.BuyItem(calcForBuying))
                        {
                            Main.mouseItem = listItem.Clone();
                            SoundEngine.PlaySound(SoundID.Coins);
                        }
                    }
                }

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
                DrawItemSlot(spriteBatch, rect, itemSlotRect, listItem);
                itemSlotRect.Y = rect.Y + rect.Height - 10 - itemSlotRect.Height;
                string rewardText = AequusText.GetText("Chat.Carpenter.UI.Reward");
                var rewardTextOrigin = FontAssets.MouseText.Value.MeasureString(rewardText);
                rewardTextOrigin.X /= 2f;
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, rewardText,
                    new Vector2(itemSlotRect.X + itemSlotRect.Width / 2f, itemSlotRect.Y), Color.White, 0f, rewardTextOrigin, Vector2.One * 0.75f);
                DrawItemSlot(spriteBatch, rect, itemSlotRect, rewardItem);
            }

            public void DrawItemSlot(SpriteBatch spriteBatch, Rectangle rect, Rectangle itemSlotRect, Item item)
            {
                Utils.DrawInvBG(spriteBatch, itemSlotRect, new Color(40, 40, 100, 255) * AequusUI.invBackColorMultipler);

                Main.instance.LoadItem(item.type);
                var texture = TextureAssets.Item[item.type].Value;
                float scale = 1f;
                int largestAmt = texture.Width > texture.Height ? texture.Width : texture.Height;

                if (largestAmt > itemSlotRect.Width)
                {
                    scale = largestAmt / (float)rect.Width;
                }

                //item.SetDefaults(ModContent.ItemType<Narrizuul>());
                item.GetItemDrawData(out var frame);
                spriteBatch.Draw(texture, itemSlotRect.Center(), frame, Color.White, 0f, frame.Size() / 2f, scale, SpriteEffects.None, 0f);

                if (itemSlotRect.Contains(Main.mouseX, Main.mouseY))
                {
                    AequusUI.HoverItem(item);
                }
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
                if (bountyUICache.selected == this)
                {
                    panel.BackgroundColor = panel.BackgroundColor.SaturationMultiply(0.5f) * 1.5f;
                    panel.BorderColor = Color.White * 0.8f;
                }
            }
        }

        public class CarpenterBountyTextureUIElement : UIElement
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

            public CarpenterBountyTextureUIElement(Asset<Texture2D> texture)
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
                maxSize = maxSize > maxSize2 ? maxSize : maxSize2;
                maxSize /= 2;
                if (largestSide > maxSize)
                {
                    DrawScale = maxSize / (float)largestSide;
                }
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
                    spriteBatch.Draw(texture.Value, drawCoords + new Vector2(4f), null, Color.Black * 0.35f, 0f, drawOrigin, DrawScale, SpriteEffects.None, 0f);

                    if (Aequus.HQ)
                    {
                        foreach (var c in AequusHelpers.CircularVector(4))
                        {
                            spriteBatch.Draw(texture.Value, drawCoords + new Vector2(4f) + c * 2f, null, Color.Black * 0.1f, 0f, drawOrigin, DrawScale, SpriteEffects.None, 0f);
                        }
                    }
                    spriteBatch.Draw(texture.Value, drawCoords, null, Color.White, 0f, drawOrigin, DrawScale, SpriteEffects.None, 0f);
                }
                base.DrawSelf(spriteBatch);
            }
        }

        public UIList bountyList;
        public UIScrollbar bountyListScrollBar;
        public UIPanel selectionPanel;
        public UIList selectionPanelList;
        public CarpenterBountyUIElement selected;

        public override void OnInitialize()
        {
            var back = TextureAssets.InventoryBack3.Value;

            OverrideSamplerState = SamplerState.LinearClamp;

            Width.Set(100, 0.4f);
            Height.Set(0, 0.526f);
            MinWidth.Set(400, 0f);
            MaxWidth.Set(1000, 0f);
            MinHeight.Set(400, 0f);
            MaxHeight.Set(1000, 0f);
            Top.Set(0, 0.33f - Height.Precent / 2f);
            Left.Set(0, 0.5f - Width.Precent / 2f);

            bountyList = new UIList();
            bountyList.Left.Set(20, 0f);
            bountyList.Top.Set(20, 0f);
            bountyList.Width.Set(0, 0.6f);
            bountyList.Height.Set(-40, 1f);
            bountyListScrollBar = new UIScrollbar();
            bountyListScrollBar.Left.Set(-28, 1f);
            bountyListScrollBar.Top.Set(8, 0f);
            bountyListScrollBar.Height.Set(400, 0f);
            bountyList.SetScrollbar(bountyListScrollBar);
            bountyList.Append(bountyListScrollBar);

            Append(bountyList);
            Recalculate();

            selectionPanel = new UIPanel();
            selectionPanel.Left.Set(bountyList.Left.Pixels + bountyList.Width.Pixels + 4, bountyList.Left.Percent + bountyList.Width.Percent);
            selectionPanel.Top.Set(20 - 4, bountyList.Top.Percent);
            selectionPanel.Width.Set(-bountyList.Left.Pixels - 12, 1f - bountyList.Width.Percent);
            selectionPanel.Height.Set(bountyList.Height.Pixels + 8, bountyList.Height.Percent);
            selectionPanel.BorderColor = selectionPanel.BackgroundColor * 2f;
            float colorMult = 1f / (selectionPanel.BackgroundColor.A / 255f);
            selectionPanel.BackgroundColor *= colorMult;

            Append(selectionPanel);

            selectionPanelList = new UIList();
            selectionPanelList.Left.Set(0, 0f);
            selectionPanelList.Top.Set(0, 0f);
            selectionPanelList.Width.Set(0, 1f);
            selectionPanelList.Height.Set(0, 1f);

            var selectionPanelListScrollBar = new UIScrollbar();
            selectionPanelListScrollBar.Left.Set(-28, 1f);
            selectionPanelListScrollBar.Top.Set(8, 0f);
            selectionPanelListScrollBar.Height.Set(400, 0f);
            selectionPanelList.SetScrollbar(selectionPanelListScrollBar);

            selectionPanel.Append(selectionPanelList);

            int bountiesAdded = 0;
            foreach (var bounty in CarpenterSystem.BountiesByID)
            {
                if (!bounty.IsBountyAvailable())
                    continue;

                var uiElement = new CarpenterBountyUIElement(this, bounty);
                uiElement.Width.Set(0, 0.9f);
                uiElement.Height.Set(160, 0f);
                uiElement.Left.Set(10, 0f);
                bountyList.Add(uiElement);

                bountiesAdded++;
            }
        }

        public override void OnDeactivate()
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Main.LocalPlayer.talkNPC == -1 || Main.npc[Main.LocalPlayer.talkNPC].type != ModContent.NPCType<Carpenter>())
            {
                Aequus.NPCTalkInterface.SetState(null);
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            var d = GetDimensions();
            var rect = d.ToRectangle();
            if (rect.Contains(Main.mouseX, Main.mouseY))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            Utils.DrawInvBG(spriteBatch, rect, (AequusUI.invBackColor * 0.5f).UseA(255) * AequusUI.invBackColorMultipler);
            var listBox = bountyList.GetDimensions().ToRectangle();
            var listBoxDraw = listBox;
            bountyListScrollBar.Height.Set(listBox.Height - 8 - bountyListScrollBar.Top.Pixels, 0f);
            listBoxDraw.Y -= 4;
            listBoxDraw.Height += 8;
            Utils.DrawInvBG(spriteBatch, listBoxDraw);
            base.DrawSelf(spriteBatch);
        }

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            base.DrawChildren(spriteBatch);
        }

        public void Select(CarpenterBountyUIElement element)
        {
            selectionPanelList.Clear();

            if (selected == element)
            {
                element = null;
            }
            selected = element;
            foreach (var b in bountyList)
            {
                if (b is CarpenterBountyUIElement e)
                {
                    e.HoverColor(false);
                }
            }
            if (selected == null)
            {
                return;
            }
            element.HoverColor(true);
            var panel = new UIPanel()
            {
                Top = new StyleDimension(0, 0f),
                Left = new StyleDimension(0, 0f),
                Width = new StyleDimension(0, 1f),
                Height = new StyleDimension(64, 0f),
                BackgroundColor = (AequusUI.invBackColor * 0.6f).UseA(255),
                BorderColor = Color.Transparent,
                PaddingLeft = 0f,
                PaddingTop = 0f,
            };
            panel.Append(new UIText(element.listItem.ModItem<CarpenterBountyItem>().BountyName)
            {
                Width = panel.Width,
                Height = panel.Height,
                TextOriginX = 0.5f,
                TextOriginY = 0.25f,
                TextColor = Color.Lerp(Color.Yellow, Color.White, 0.66f),
            });
            panel.Append(new UIText(element.listItem.ModItem<CarpenterBountyItem>().BountyDescription, textScale: 0.85f)
            {
                Width = panel.Width,
                Height = panel.Height,
                Top = new StyleDimension(24, 0f),
                TextOriginX = 0.5f,
                TextOriginY = 0.25f,
            });
            selectionPanelList.Add(panel);

            panel = new UIPanel()
            {
                Top = new StyleDimension(0, 0f),
                Left = new StyleDimension(0, 0f),
                Width = new StyleDimension(0, 1f),
                Height = new StyleDimension(1000, 0f),
                BackgroundColor = (AequusUI.invBackColor * 0.6f).UseA(255),
                BorderColor = Color.Transparent,
                PaddingLeft = 0f,
                PaddingTop = 0f,
            };

            selectionPanelList.Add(panel);

            string requirementText = element.listItem.ModItem<CarpenterBountyItem>().BountyRequirements;
            var split = requirementText.Split('\n');
            requirementText = "";
            foreach (string s in split)
            {
                if (requirementText != "")
                    requirementText += "\n";
                requirementText += "• " + FontAssets.MouseText.Value.CreateWrappedText(s, panel.GetInnerDimensions().Width);
            }

            var uiText = new UIText(requirementText, textScale: 0.8f)
            {
                Width = panel.Width,
                Height = panel.Height,
                Top = new StyleDimension(10, 0f),
                TextOriginX = 0f,
                TextOriginY = 0f,
                PaddingLeft = 4f,
            };
            panel.Append(uiText);
            panel.Height.Set((int)FontAssets.MouseText.Value.MeasureString(uiText.Text).Y * 0.8f + uiText.Top.Pixels, 0f);

            string texture = element.ListItem.BountyTexture;
            if (!ModContent.HasAsset(texture))
            {
                texture = "Aequus/Assets/UI/Carpenter/Blueprints/Error";
            }
            selectionPanelList.Add(new CarpenterBountyTextureUIElement(ModContent.Request<Texture2D>(texture))
            {
                Width = new StyleDimension(0, 1f),
            });

            uiText.IsWrapped = true;

            selected = element;
        }
    }
}