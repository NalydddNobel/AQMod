using AequusRemake.Core.Graphics;

namespace AequusRemake.Core.Hooks;

public partial class TerrariaHooks {
    private static void On_Main_CheckMonoliths(On_Main.orig_CheckMonoliths orig) {
        if (!Main.gameMenu) {
            DrawLayers.Instance.PostUpdateScreenPosition?.Draw(Main.spriteBatch);
        }
        orig();
    }
}
