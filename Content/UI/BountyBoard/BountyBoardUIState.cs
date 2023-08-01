using Aequus.Common.Carpentry;
using Aequus.Common.UI;
using Aequus.NPCs.Town.CarpenterNPC;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace Aequus.Content.UI.BountyBoard;

public class BountyBoardUIState : AequusUIState {
    public override void OnInitialize() {
        OverrideSamplerState = SamplerState.LinearClamp;

        Width.Set(300, 0.2f);
        Height.Set(200, 0.2f);
        Top.Set(100, 0f);
        HAlign = 0.5f;

        var uiPanel = new UIPanel {
            BackgroundColor = new Color(68, 99, 164) * 0.825f
        };
        uiPanel.Width.Set(0, 1f);
        uiPanel.Height.Set(0, 1f);
        Append(uiPanel);

        BountyDetailsUIElement details = new();
        Append(details);
        Append(new BountyPostsUIElement(BuildChallengeLoader.registeredBuildChallenges, details));
    }

    public override void Update(GameTime gameTime) {
        if (IsMouseHovering) {
            Main.LocalPlayer.mouseInterface = true;
        }
        if (NotTalkingTo<Carpenter>()) {
            Aequus.UserInterface.SetState(null);
            return;
        }
    }
}
