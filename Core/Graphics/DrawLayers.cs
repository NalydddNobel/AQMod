using System;

namespace Aequus.Core.Graphics;

[Autoload(Side = ModSide.Client)]
public class DrawLayers : ILoadable {
    public event Action<SpriteBatch> PostDrawDust;

    public static DrawLayers Instance { get; private set; }

    public void Load(Mod mod) {
        Instance = this;
        On_Main.DrawDust += On_Main_DrawDust;
    }

    public void Unload() {
    }

    private static void On_Main_DrawDust(On_Main.orig_DrawDust orig, Main main) {
        orig(main);
        Instance.PostDrawDust?.Invoke(Main.spriteBatch);
    }
}
