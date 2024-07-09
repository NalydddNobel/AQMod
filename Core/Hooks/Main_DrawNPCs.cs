using AequusRemake.Core.Graphics;

namespace AequusRemake.Core.Hooks;

public partial class TerrariaHooks {
    private static void On_Main_DrawNPCs(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles) {
        if (behindTiles) {
            DrawLayers.Instance.BehindTiles?.Draw(Main.spriteBatch);
        }

        orig(self, behindTiles);

        if (!behindTiles) {
            DrawLayers.Instance.PostDrawNPCs?.Draw(Main.spriteBatch);
        }
    }
}
