using Aequus.Content.Configuration;
using Aequus.Core.UI;
using Terraria.UI;

namespace Aequus.Content.UI;

public class DeathTips : UILayer {
    public System.Boolean resetGameTips;

    public override void OnPreUpdatePlayers() {
        if (ClientConfig.Instance.ShowDeathTips && !Active && Main.LocalPlayer.dead && !Main.LocalPlayer.ghost) {
            Activate();
        }
    }

    public override System.Boolean OnUIUpdate(GameTime gameTime) {
        return Main.LocalPlayer.dead && ClientConfig.Instance.ShowDeathTips;
    }

    protected override System.Boolean DrawSelf() {
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

    public DeathTips() : base("Death Tips", InterfaceLayerNames.PlayerChat_35, InterfaceScaleType.UI) { }
}