using Aequu2.Core.Debug;
using Aequu2.Core.Graphics;

namespace Aequu2.Core.Hooks;

public partial class TerrariaHooks {
    private static void On_Main_DrawInfernoRings(On_Main.orig_DrawInfernoRings orig, Main self) {
        orig(self);
        DiagnosticsMenu.StartStopwatch();

        DrawLayers.Instance.PostDrawLiquids?.Draw(Main.spriteBatch);

        DiagnosticsMenu.EndStopwatch(DiagnosticsMenu.TimerType.PostDrawLiquids);
    }
}
