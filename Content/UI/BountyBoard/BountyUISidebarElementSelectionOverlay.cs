using Aequus.NPCs.Town.CarpenterNPC.Quest.Bounties;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.Content.UI.BountyBoard {
    public class BountyUISidebarElementSelectionOverlay : UIElement {
        public readonly CarpenterBounty bounty;

        public BountyUISidebarElementSelectionOverlay(CarpenterBounty bounty) {
            this.bounty = bounty;
        }

        public override void OnInitialize() {
            UpdateSelection();
        }

        public void UpdateSelection() {
            RemoveAllChildren();
            var carpenter = Main.LocalPlayer.GetModPlayer<CarpenterBountyPlayer>();
            if (carpenter.SelectedBounty == bounty.Type) {
                var uiImage = new UIImage(AequusTextures.BountyUIArrow.Asset);
                uiImage.Left.Set(8f, 0f);
                uiImage.VAlign = 0.5f;
                Append(uiImage);
            }
        }
    }
}