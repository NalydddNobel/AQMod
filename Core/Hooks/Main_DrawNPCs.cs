using Aequu2.Core.Graphics;

namespace Aequu2.Core.Hooks;

public partial class TerrariaHooks {
    private static void On_Main_DrawNPCs(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles) {
        if (behindTiles) {
            DrawLayers.Instance.WorldBehindTiles?.Draw(Main.spriteBatch);
        }

        orig(self, behindTiles);
    }
}
