using Aequus.Core.Debug;
using System;

namespace Aequus.Core.Graphics;

[Autoload(Side = ModSide.Client)]
public class DrawLayers : ILoadable {
    /// <summary>Invoked before NPCs behind tiles are drawn.</summary>
    public event Action<SpriteBatch> WorldBehindTiles;
    /// <summary>Invoked after Dusts have been drawn.</summary>
    public event Action<SpriteBatch> PostDrawDust;
    /// <summary>Invoked after Liquids and Inferno Rings have been drawn, but before Wire Overlays are drawn.</summary>
    public event Action<SpriteBatch> PostDrawLiquids;

    public static DrawLayers Instance { get; private set; }

    public void Load(Mod mod) {
        Instance = this;
        On_Main.DrawNPCs += On_Main_DrawNPCs;
        On_Main.DrawDust += On_Main_DrawDust;
        On_Main.DrawInfernoRings += On_Main_DrawInfernoRings;
    }

    private static void On_Main_DrawNPCs(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles) {
        if (behindTiles) {
            Instance.WorldBehindTiles?.Invoke(Main.spriteBatch);
        }

        orig(self, behindTiles);
    }

    public void Unload() {
    }

    private static void On_Main_DrawDust(On_Main.orig_DrawDust orig, Main main) {
        orig(main);
        Instance.PostDrawDust?.Invoke(Main.spriteBatch);
    }


    private static void On_Main_DrawInfernoRings(On_Main.orig_DrawInfernoRings orig, Main self) {
        orig(self);
        DiagnosticsMenu.StartStopwatch();

        Instance.PostDrawLiquids?.Invoke(Main.spriteBatch);

        DiagnosticsMenu.EndStopwatch(DiagnosticsMenu.TimerType.PostDrawLiquids);
    }
}
