using Aequus.Common.UI;
using Aequus.Content.Configuration;
using Aequus.Core.UI;
using Terraria.UI;

namespace Aequus.Content.UI;

public class DeathTips : UILayer {
    public bool resetGameTips;

    public override void OnPreUpdatePlayers() {
        if (ClientConfig.Instance.ShowDeathTips && !Active && Main.LocalPlayer.dead && !Main.LocalPlayer.ghost) {
            Activate();
        }
    }

    public override bool OnUIUpdate(GameTime gameTime) {
        return Main.LocalPlayer.dead && ClientConfig.Instance.ShowDeathTips;
    }

    protected override bool DrawSelf() {
        if (Main.gameMenu) {
            return true;
        }

        if (!Main.LocalPlayer.dead || Main.LocalPlayer.ghost) {
            resetGameTips = true;
            return true;
        }

        if (resetGameTips) {
            Main.gameTips.ClearTips();
            resetGameTips = false;
        }
        Main.gameTips.Update();
        Main.gameTips.Draw();
        return true;
    }

    public DeathTips() : base("Death Tips", InterfaceLayers.PlayerChat_35, InterfaceScaleType.UI) { }
}