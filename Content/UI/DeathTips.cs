using Aequus.Common.UI;
using Aequus.Content.Configuration;
using Microsoft.Xna.Framework.Graphics;

namespace Aequus.Content.UI;

public class DeathTips : UILayer {
    public override string Layer => InterfaceLayers.PlayerChat_35;

    public bool resetGameTips;

    public override bool Draw(SpriteBatch spriteBatch) {
        if (Main.gameMenu || !ClientConfig.Instance.ShowDeathTips) {
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
}