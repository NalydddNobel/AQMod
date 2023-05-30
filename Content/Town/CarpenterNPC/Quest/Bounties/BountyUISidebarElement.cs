using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;

namespace Aequus.Content.Town.CarpenterNPC.Quest.Bounties {
    public class BountyUISidebarElement : UIElement
    {
        public static Color UIPanelColor = new Color(122, 168, 226);

        public readonly BountyUIState parentState;
        public readonly CarpenterBounty bounty;
        public UIPanel uiPanel;

        public BountyUISidebarElement(BountyUIState parent, CarpenterBounty bounty)
        {
            parentState = parent;
            this.bounty = bounty;
        }

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(50, 0f);

            uiPanel = new UIPanel
            {
                BackgroundColor = UIPanelColor
            };
            uiPanel.BorderColor = uiPanel.BackgroundColor * 1.2f;
            uiPanel.Left.Set(2, 0f);
            uiPanel.Top.Set(2, 0f);
            uiPanel.Width.Set(-4, 1f);
            uiPanel.Height.Set(-4, 1f);
            Append(uiPanel);

            int head = NPC.TypeToDefaultHeadIndex(bounty.GetBountyNPCID());
            string name = bounty.DisplayName;
            bool completed = CarpenterSystem.CompletedBounties.Contains(bounty.FullName);
            if (!bounty.IsNPCPresent() && !completed)
            {
                head = 0;
                name = "Not Unlocked!";
                byte a = uiPanel.BackgroundColor.A;
                uiPanel.BackgroundColor *= 0.6f;
                uiPanel.BackgroundColor = uiPanel.BackgroundColor.UseA(a);
            }
            if (completed)
            {
                var checkMark = new UIText("✓")
                {
                    VAlign = 0.5f
                };
                checkMark.Left.Set(54, 0f);
                checkMark.TextColor = Color.Lime;
                Append(checkMark);
            }
            else
            {
                var npcHead = TextureAssets.NpcHead[head];
                var uiImage = new UIImage(npcHead)
                {
                    VAlign = 0.5f
                };
                uiImage.Width.Set(npcHead.Value.Width, 0f);
                uiImage.Height.Set(npcHead.Value.Height, 0f);
                uiImage.Left.Set(60 - npcHead.Value.Width / 2, 0f);
                Append(uiImage);
            }
            var uiText = new UIText(name)
            {
                VAlign = 0.5f
            };
            uiText.Left.Set(80, 0f);
            if (completed)
            {
                uiText.TextColor = Color.Lime;
            }
            Append(uiText);

            var overlay = new BountyUISidebarElementSelectionOverlay(bounty);
            overlay.Width.Set(0f, 1f);
            overlay.Height.Set(0f, 1f);
            Append(overlay);
            parentState.selectionOverlays.Add(overlay);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
        }

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            base.DrawChildren(spriteBatch);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (uiPanel.BackgroundColor.R < UIPanelColor.R)
                return;

            var carpenter = Main.LocalPlayer.GetModPlayer<CarpenterBountyPlayer>();
            if (carpenter.SelectedBounty == bounty.Type)
            {
                carpenter.SelectedBounty = -1;
                SoundEngine.PlaySound(SoundID.MenuClose);
            }
            else
            {
                carpenter.SelectedBounty = bounty.Type;
                SoundEngine.PlaySound(SoundID.MenuOpen);
            }
            CarpenterBountyPlayer.LastPhotoTakenResults = null;
            foreach (var overlay in parentState.selectionOverlays)
            {
                overlay.UpdateSelection();
            }
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            if (uiPanel.BackgroundColor.R < UIPanelColor.R)
                return;
            uiPanel.BackgroundColor *= 1.2f;
            parentState.detailsManager.SetBounty(bounty);
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            if (uiPanel.BackgroundColor.R < UIPanelColor.R)
                return;
            uiPanel.BackgroundColor = UIPanelColor;
        }
    }
}