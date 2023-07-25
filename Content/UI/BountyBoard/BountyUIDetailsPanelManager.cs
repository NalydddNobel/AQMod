using Aequus.Common.UI.Elements;
using Aequus.Content.Building.old.Quest.Bounties;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Aequus.Content.UI.BountyBoard {
    public class BountyUIDetailsPanelManager : UIElement {
        public readonly BountyUIState parentState;
        public readonly UIPanel uiPanel;

        public CarpenterBounty bounty;

        public BountyUIDetailsPanelManager(BountyUIState parent, UIPanel panel) {
            parentState = parent;
            uiPanel = panel;
        }

        public void SetBounty(CarpenterBounty bounty) {
            Clear();
            var uiText = new UIText(bounty.DisplayName, 0.7f, large: true) {
                HAlign = 0.5f
            };
            uiText.Top.Set(8, 0f);
            uiPanel.Append(uiText);

            var descriptionPanel = new UIPanel {
                BackgroundColor = new Color(91, 124, 193) * 0.9f,
                BorderColor = uiPanel.BackgroundColor * 0.5f
            };
            descriptionPanel.Top.Set(42f, 0f);
            descriptionPanel.Width.Set(0f, 1f);
            descriptionPanel.Height.Set(100f, 0.1f);
            uiPanel.Append(descriptionPanel);
            uiText = new UIText($"{bounty.Description}", 1f) {
                HAlign = 0f,
                VAlign = 0f,
                TextOriginX = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
                IgnoresMouseInteraction = true,
                IsWrapped = true,
            };
            descriptionPanel.Append(uiText);
            uiText.FixUIText();

            var npcHead = TextureAssets.NpcHead[NPC.TypeToDefaultHeadIndex(bounty.GetBountyNPCID())];
            UIImage uiImage = new(npcHead) {
                ImageScale = 1.05f
            };
            uiImage.Width.Set(npcHead.Value.Width, 0f);
            uiImage.Height.Set(npcHead.Value.Height, 0f);
            uiImage.Left.Set(32 - npcHead.Value.Width / 2, 0f);
            uiImage.Top.Set(20 - npcHead.Value.Height / 2, 0f);
            uiPanel.Append(uiImage);

            var stepsPanel = new UIPanel {
                BackgroundColor = new Color(91, 124, 193) * 0.9f
            };
            stepsPanel.BorderColor = stepsPanel.BackgroundColor * 1.3f;
            stepsPanel.Top.Set(42f + 100f + 8f, descriptionPanel.Height.Percent);
            stepsPanel.Width.Set(-10f, 0.5f);
            stepsPanel.Height.Set(0f, 0.45f);
            uiPanel.Append(stepsPanel);

            var text = "";
            foreach (var t in bounty.StepsToString()) {
                if (!string.IsNullOrEmpty(text))
                    text += "\n";
                text += $"- {t}";
            }
            uiText = new UIText(text, 0.8f) {
                HAlign = 0f,
                VAlign = 0f,
                TextOriginX = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
                IgnoresMouseInteraction = true,
                IsWrapped = true,
            };
            stepsPanel.Append(uiText);
            uiText.FixUIText();

            var blueprintPanel = new UIPanel {
                BackgroundColor = (new Color(91, 124, 193) * 0.5f).UseA(255) * 0.9f,
                BorderColor = stepsPanel.BackgroundColor * 1.3f,
                Left = stepsPanel.Width
            };
            blueprintPanel.Left.Pixels += 16f;
            blueprintPanel.Top = stepsPanel.Top;
            blueprintPanel.Width.Set(-10f, 0.5f);
            blueprintPanel.Height.Set(0f, 0.45f);
            uiPanel.Append(blueprintPanel);

            var top = stepsPanel.Top;
            top.Pixels += 10f;
            top.Percent += 0.45f;
            //var buttonPanel = new UIPanel
            //{
            //    BackgroundColor = stepsPanel.BackgroundColor,
            //    BorderColor = stepsPanel.BackgroundColor * 1.3f,
            //    Left = stepsPanel.Left,
            //    Top = top
            //};
            //buttonPanel.Width.Set(-10f, 0.33f);
            //buttonPanel.Height.Set(64f, 0f);
            //uiPanel.Append(buttonPanel);

            //uiText = new UIText("Buy Materials")
            //{
            //    DynamicallyScaleDownToWidth = true,
            //    HAlign = 0.5f,
            //    VAlign = 0.5f
            //};
            //buttonPanel.Append(uiText);

            //buttonPanel = new UIPanel
            //{
            //    BackgroundColor = stepsPanel.BackgroundColor,
            //    BorderColor = stepsPanel.BackgroundColor * 1.3f,
            //    Left = stepsPanel.Left
            //};
            //buttonPanel.Left.Precent += 0.33f;
            //buttonPanel.Top = top;
            //buttonPanel.Width.Set(-10f, 0.33f);
            //buttonPanel.Height.Set(64f, 0f);
            //uiPanel.Append(buttonPanel);

            //uiText = new UIText("Set as Quest")
            //{
            //    DynamicallyScaleDownToWidth = true,
            //    HAlign = 0.5f,
            //    VAlign = 0.5f
            //};
            //buttonPanel.Append(uiText);

            var rewardPanel = new UIPanel {
                BackgroundColor = stepsPanel.BackgroundColor,
                BorderColor = stepsPanel.BackgroundColor * 1.3f,
                Left = stepsPanel.Left
            };
            rewardPanel.Left.Precent += 0.66f;
            rewardPanel.Top = top;
            rewardPanel.Width.Set(-10f, 0.33f);
            rewardPanel.Height.Set(-rewardPanel.Top.Pixels, 1f - rewardPanel.Top.Precent);
            uiPanel.Append(rewardPanel);

            var reward = bounty.ProvideBountyRewardItems()[0];
            uiText = new UIText("Reward:", 0.56f, large: true) {
                DynamicallyScaleDownToWidth = true
            };
            uiText.Top.Set(6f, 0f);
            uiText.HAlign = 0.5f;
            rewardPanel.Append(uiText);
            uiText = new UIText(Lang.GetItemNameValue(reward.type), 0.4f, large: true) {
                DynamicallyScaleDownToWidth = true,
                TextColor = Color.Lerp(Color.Yellow, Color.White, 0.8f)
            };
            uiText.Top.Set(6f, 0.74f);
            uiText.HAlign = 0.5f;
            rewardPanel.Append(uiText);

            AequusItemSlotElement itemSlot = new(TextureAssets.InventoryBack.Value);
            itemSlot.Width.Set(0f, 0.4f);
            itemSlot.Height.Set(0f, 0.4f);
            itemSlot.HAlign = 0.5f;
            itemSlot.VAlign = 0.5f;
            itemSlot.item = reward;
            itemSlot.showItemTooltipOnHover = true;
            itemSlot.canHover = true;
            rewardPanel.Append(itemSlot);
        }

        public void Clear() {
            bounty = null;
            uiPanel.RemoveAllChildren();
        }
    }
}