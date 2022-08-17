using Aequus.Content.CarpenterBounties;
using Aequus.Graphics.RenderTargets;
using Aequus.Items.Misc;
using Aequus.Items.Tools.Camera;
using Aequus.NPCs.Friendly.Town;
using Aequus.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.UI.States
{
    public class CarpenterBountyUI : AequusUIState
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
                this.bounty = bounty;
                bountyUICache = bountyUI;
                listItem = bounty.ProvideBountyItem().Item;
                rewardItem = bounty.ProvideBountyRewardItem();
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
                            Main.mouseLeftRelease = false;
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
            public Action OnRecalculateTextureSize;

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

        public UIList bountyList;
        public UIScrollbar bountyListScrollBar;

        public UIPanel selectionPanel;
        public UIList selectionPanelList;
        public UIScrollbar selectionPanelListScrollBar;
        public ItemSlotElement submissionSlot;
        public UIText submissionSlotTextButton;
        public UIImageButton backButton;
        public CarpenterBountyUIElement selected;

        public string SubmitPhotoText => AequusText.GetText("Chat.Carpenter.UI.SubmitPhoto");

        public override void OnInitialize()
        {
            OverrideSamplerState = SamplerState.LinearClamp;

            Height.Set(0, 0.526f);
            MinWidth.Set(400, 0f);
            MaxWidth.Set(1000, 0f);
            MinHeight.Set(400, 0f);
            MaxHeight.Set(1000, 0f);
            Top.Set(100, 0f);
            HAlign = 0.5f;

            SetListPanel();
        }

        public override void OnDeactivate()
        {
            Clear();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Main.playerInventory = !Main.mouseItem.IsAir || selected != null;
            if (NotTalkingTo<Carpenter>())
            {
                Aequus.NPCTalkInterface.SetState(null);
                return;
            }

            if (selected != null)
            {
                if (selectionPanel == null)
                    SetViewBountyPanel();
            }
            else
            {
                if (bountyList == null)
                    SetListPanel();
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

            if (submissionSlot != null && submissionSlot.IsMouseHovering && submissionSlot.HasItem)
            {
                AequusUI.HoverItem(submissionSlot.item);
            }

            if (submissionSlotTextButton != null)
            {
                if (submissionSlotTextButton.IsMouseHovering && submissionSlot.HasItem)
                {
                    string t = SubmitPhotoText;
                    if (submissionSlotTextButton.Text == t)
                    {
                        submissionSlotTextButton.SetText(AequusText.ColorText(t, Color.Yellow));
                    }
                    if (Main.mouseLeft && Main.mouseLeftRelease && submissionSlot.item.ModItem is ShutterstockerClip clip)
                    {
                        Main.mouseLeftRelease = false;

                        bool completed = selected.bounty.CheckConditions(clip.tileMap, out string responseMessage, Main.npc[Main.LocalPlayer.talkNPC]);
                        ShutterstockerSceneRenderer.renderRequests.Add(clip);
                        clip.reviewed = true;
                        SoundEngine.PlaySound(SoundID.Chat);
                        Main.playerInventory = true;
                        Main.npcChatText = responseMessage;
                        Aequus.NPCTalkInterface.SetState(null);
                        if (completed)
                        {
                            Main.LocalPlayer.GetModPlayer<CarpenterBountyPlayer>().CompletedBounties.Add(selected.bounty.FullName);
                            selected.bounty.OnCompleteBounty(Main.LocalPlayer, Main.npc[Main.LocalPlayer.talkNPC]);
                        }
                    }
                }
                else
                {
                    string t = SubmitPhotoText;
                    if (submissionSlotTextButton.Text != t)
                    {
                        submissionSlotTextButton.SetText(t);
                    }
                }
            }

            if (bountyList != null)
            {
                var listBox = bountyList.GetDimensions().ToRectangle();
                var listBoxDraw = listBox;
                bountyListScrollBar.Height.Set(listBox.Height - 8 - bountyListScrollBar.Top.Pixels, 0f);
                listBoxDraw.Y -= 4;
                listBoxDraw.Height += 8;
                Utils.DrawInvBG(spriteBatch, listBoxDraw);
            }
            if (selectionPanelList != null)
            {
                ManageSelectionPanelScrollbarHeight();
            }
            base.DrawSelf(spriteBatch);
        }

        private void ManageSelectionPanelScrollbarHeight()
        {
            var listBox = selectionPanelList.GetDimensions().ToRectangle();
            var listBoxDraw = listBox;
            selectionPanelListScrollBar.Height.Set(listBox.Height - 8 - selectionPanelListScrollBar.Top.Pixels, 0f);
            listBoxDraw.Y -= 4;
            listBoxDraw.Height += 8;
        }

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            base.DrawChildren(spriteBatch);
        }

        public void Select(CarpenterBountyUIElement element)
        {
            if (element == selected)
                selected = null;
            selected = element;
        }

        public void SetListPanel()
        {
            Clear();
            selected = null;

            Width.Set(128, 0.4f);

            bountyList = new UIList();
            bountyList.Left.Set(20, 0f);
            bountyList.Top.Set(20, 0f);
            bountyList.Width.Set(-20, 1f);
            bountyList.Height.Set(-40, 1f);

            bountyListScrollBar = new UIScrollbar();
            bountyListScrollBar.Left.Set(-28, 1f);
            bountyListScrollBar.Top.Set(8, 0f);
            bountyListScrollBar.Height.Set(400, 0f);
            bountyList.SetScrollbar(bountyListScrollBar);
            bountyList.Append(bountyListScrollBar);

            Append(bountyList);
            Recalculate();

            var bountyPlayer = Main.LocalPlayer.GetModPlayer<CarpenterBountyPlayer>();
            foreach (var bounty in CarpenterSystem.BountiesByID)
            {
                if (!bounty.IsBountyAvailable() || bountyPlayer.CompletedBounties.Contains(bounty.FullName))
                    continue;

                var uiElement = new CarpenterBountyUIElement(this, bounty);
                uiElement.Width.Set(-48, 1f);
                uiElement.Height.Set(160, 0f);
                uiElement.Left.Set(10, 0f);
                bountyList.Add(uiElement);
                uiElement.Setup();
            }

            return;
        }

        public void SetViewBountyPanel()
        {
            Width = new StyleDimension(80, 0.35f);

            Clear();

            selectionPanel = new UIPanel();
            selectionPanel.Left.Set(20, 0f);
            selectionPanel.Top.Set(20, 0f);
            selectionPanel.Width.Set(-40, 1f);
            selectionPanel.Height.Set(-40, 1f);
            selectionPanel.BorderColor = selectionPanel.BackgroundColor * 2f;
            float colorMult = 1f / (selectionPanel.BackgroundColor.A / 255f);
            selectionPanel.BackgroundColor *= colorMult;

            Append(selectionPanel);

            selectionPanelList = new UIList();
            selectionPanelList.Left.Set(32, 0f);
            selectionPanelList.Top.Set(0, 0f);
            selectionPanelList.Width.Set(-selectionPanelList.Left.Pixels * 2, 1f);
            selectionPanelList.Height.Set(0, 1f);

            selectionPanelListScrollBar = new UIScrollbar();
            selectionPanelListScrollBar.Left.Set(-12, 1f);
            selectionPanelListScrollBar.Top.Set(8, 0f);
            selectionPanelListScrollBar.Width.Set(32, 0f);
            selectionPanelListScrollBar.Height.Set(460, 0f);
            selectionPanelList.SetScrollbar(selectionPanelListScrollBar);
            selectionPanel.Append(selectionPanelList);
            selectionPanel.Append(selectionPanelListScrollBar);

            backButton = new UIImageButton(ModContent.Request<Texture2D>(Aequus.VanillaTexture + "UI/Bestiary/Button_Back", AssetRequestMode.ImmediateLoad))
            {
                Top = new StyleDimension(0, 0f),
                Left = new StyleDimension(0, 0f),
                Width = new StyleDimension(28, 0f),
                Height = new StyleDimension(28, 0f),
            };
            backButton.OnClick += BackButton_OnClick;

            selectionPanel.Append(backButton);
            PopulateSelectPanelList(selected);
        }

        private void BackButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            selected = null;
        }

        public void Clear()
        {
            if (submissionSlot != null && submissionSlot.HasItem)
            {
                Main.LocalPlayer.QuickSpawnClonedItem(null, submissionSlot.item, submissionSlot.item.stack);
            }

            bountyList = null;
            bountyListScrollBar = null;
            selectionPanel = null;
            selectionPanelList = null;
            selectionPanelListScrollBar = null;
            backButton = null;
            submissionSlot = null;
            RemoveAllChildren();
        }

        public void PopulateSelectPanelList(CarpenterBountyUIElement element)
        {
            PopulateSideList_TitleAndDescription(element);
            PopulateSideList_Requirements(element);

            PopulateSideList_AddSeparator();

            PopulateSideList_Submission(element);

            PopulateSideList_AddSeparator();
            PopulateSideList_Blueprint(element);
        }
        public void PopulateSideList_TitleAndDescription(CarpenterBountyUIElement element)
        {
            var panel = new UIPanel()
            {
                Top = new StyleDimension(0, 0f),
                Left = new StyleDimension(0, 0f),
                Width = new StyleDimension(0, 1f),
                Height = new StyleDimension(56, 0f),
                BackgroundColor = (AequusUI.invBackColor * 0.85f).UseA(255),
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
                TextColor = Color.Lerp(Color.Yellow, Color.White, 0.45f),
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
        }
        public void PopulateSideList_Requirements(CarpenterBountyUIElement element)
        {
            var panel = new UIPanel()
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
            uiText.IsWrapped = true;
        }
        public void PopulateSideList_Submission(CarpenterBountyUIElement element)
        {
            var panel = new UIPanel()
            {
                Top = new StyleDimension(0, 0f),
                Left = new StyleDimension(0, 0f),
                Width = new StyleDimension(120, 0f),
                Height = new StyleDimension(1000, 0f),
                BackgroundColor = (AequusUI.invBackColor * 0.6f).UseA(255),
                BorderColor = Color.Transparent,
                HAlign = 0.5f,
            };
            selectionPanelList.Add(panel);

            submissionSlot = new ItemSlotElement(TextureAssets.InventoryBack.Value)
            {
                Top = new StyleDimension(0, 0f),
                HAlign = 0.5f,
                Width = new StyleDimension(64 * 0.8f, 0f),
                Height = new StyleDimension(64 * 0.8f, 0f),
            };
            submissionSlot.OnClick += SubmissionSlot_OnClick;
            panel.Append(submissionSlot);

            var text = SubmitPhotoText;
            submissionSlotTextButton = new UIText(text, 1f)
            {
                Width = panel.Width,
                Height = panel.Height,
                Top = new StyleDimension(submissionSlot.Height.Pixels + 8, 0f),
                TextOriginX = 0.5f,
                TextOriginY = 0f,
                HAlign = 0.5f,
                IgnoresMouseInteraction = false,
            };
            panel.Append(submissionSlotTextButton);
            var textMeasurement = FontAssets.MouseText.Value.MeasureString(text);

            panel.Width.Set(Math.Max((int)submissionSlot.Width.Pixels, (int)textMeasurement.X) + 20, 0f);
            panel.Height.Set((int)textMeasurement.Y + submissionSlot.Height.Pixels + 16, 0f);
        }

        private void SubmissionSlot_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (Main.mouseItem.IsAir)
            {
                if (submissionSlot.HasItem)
                {
                    Utils.Swap(ref Main.mouseItem, ref submissionSlot.item);
                    SoundEngine.PlaySound(SoundID.Grab);
                }
                return;
            }

            if (Main.mouseItem.ModItem is ShutterstockerClip clip && !clip.reviewed)
            {
                Utils.Swap(ref Main.mouseItem, ref submissionSlot.item);
                SoundEngine.PlaySound(SoundID.Grab);
            }
        }

        public void PopulateSideList_Blueprint(CarpenterBountyUIElement element)
        {
            string texture = element.ListItem.BountyTexture;
            if (!ModContent.HasAsset(texture))
            {
                return;
            }

            var panel = new UIPanel()
            {
                Width = new StyleDimension(0, 0.9f),
                BackgroundColor = (AequusUI.invBackColor * 0.6f).UseA(255),
                BorderColor = Color.Transparent,
                PaddingLeft = 0f,
                PaddingTop = 0f,
                HAlign = 0.5f,
            };

            panel.Append(new UIText(AequusText.Chat<Carpenter>("UI.Blueprint"), 0.85f)
            {
                Width = panel.Width,
                Height = panel.Height,
                TextOriginX = 0f,
                PaddingLeft = 6f,
                PaddingTop = 10f,
            });

            panel.Append(new UIText(AequusText.Chat<Carpenter>("UI.BlueprintFakeCopyright"), 0.6f)
            {
                Width = panel.Width,
                Height = panel.Height,
                TextOriginX = 0f,
                PaddingLeft = 6f,
                PaddingTop = 30f,
            });

            selectionPanelList.Add(panel);

            var textureElement = new CarpenterBountyTextureUIElement(ModContent.Request<Texture2D>(texture))
            {
                Width = new StyleDimension(0, 1f),
                Top = new StyleDimension(44, 0f),
            };
            textureElement.OnRecalculateTextureSize += () => panel.Height = new StyleDimension(textureElement.Height.Pixels + textureElement.Top.Pixels, 0f);
            panel.Append(textureElement);

            PopulateSideList_AddSeparator();
        }

        public void PopulateSideList_AddSeparator()
        {
            selectionPanelList.Add(new UIHorizontalSeparator()
            {
                Left = new StyleDimension(12f, 0f),
                Width = new StyleDimension(-24f, 1f),
                Color = AequusUI.invBackColor * 2f * AequusUI.invBackColorMultipler,
            });
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