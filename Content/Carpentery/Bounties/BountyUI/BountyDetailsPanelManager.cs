using Aequus.Content.Carpentery.Bounties;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Aequus.Content.Carpentery.Bounties.BountyUI
{
    public class BountyDetailsPanelManager : UIElement
    {
        public readonly BountyUIState parentState;
        public readonly UIPanel uiPanel;

        public CarpenterBounty bounty;

        public BountyDetailsPanelManager(BountyUIState parent, UIPanel panel)
        {
            parentState = parent;
            uiPanel = panel;
        }

        public void SetBounty(CarpenterBounty bounty)
        {
            Clear();
            var uiText = new UIText(bounty.DisplayName, 1.2f);
            uiText.HAlign = 0.5f;
            uiText.Top.Set(10, 0f);
            uiPanel.Append(uiText);

            var descriptionPanel = new UIPanel();
            descriptionPanel.BackgroundColor = new Color(91, 124, 193) * 0.9f;
            descriptionPanel.BorderColor = uiPanel.BackgroundColor * 0.5f;
            descriptionPanel.Top.Set(42f, 0f);
            descriptionPanel.Width.Set(0f, 1f);
            descriptionPanel.Height.Set(100f, 0.1f);
            uiPanel.Append(descriptionPanel);
            uiText = new UIText(bounty.Description, 1f);
            uiText.Width.Set(0f, 1f);
            uiText.Height.Set(0f, 1f);
            uiText.TextOriginX = 0f;
            uiText.IsWrapped = true;
            descriptionPanel.Append(uiText);

            var npcHead = TextureAssets.NpcHead[NPC.TypeToDefaultHeadIndex(bounty.GetBountyNPCID())];
            var uiImage = new UIImage(npcHead);
            uiImage.ImageScale = 1.2f;
            uiImage.Width.Set(npcHead.Value.Width, 0f);
            uiImage.Height.Set(npcHead.Value.Height, 0f);
            uiImage.Left.Set(32 - npcHead.Value.Width / 2, 0f);
            uiImage.Top.Set(20 - npcHead.Value.Height / 2, 0f);
            uiPanel.Append(uiImage);
        }

        public void Clear()
        {
            bounty = null;
            uiPanel.RemoveAllChildren();
        }
    }
}