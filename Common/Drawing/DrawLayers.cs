using System;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Renderers;

namespace Aequus.Common.Drawing;

[Autoload(Side = ModSide.Client)]
public class DrawLayers : ILoadable {
    /// <summary>Invoked before anything has begun rendering, but after the screen position has been determined.</summary>
    public DrawLayer PostUpdateScreenPosition = new();
    /// <summary>Invoked before NPCs behind tiles are drawn.</summary>
    public DrawLayer WorldBehindTiles = new();
    /// <summary>Invoked after Master Relics are drawn.</summary>
    public DrawLayer PostDrawMasterRelics = new();
    /// <summary>Invoked after NPCs are drawn.</summary>
    public DrawLayer PostDrawNPCs = new();
    /// <summary>Invoked after Dusts have been drawn.</summary>
    public DrawLayer PostDrawDust = new();
    /// <summary>Invoked after Liquids and Inferno Rings have been drawn, but before Wire Overlays are drawn.</summary>
    public DrawLayer PostDrawLiquids = new();

    /// <summary></summary>
    public Action<SpriteBatch, Player>? PreDrawPlayer;

    public static DrawLayers Instance => ModContent.GetInstance<DrawLayers>();

    static void On_LegacyPlayerRenderer_DrawPlayers(On_LegacyPlayerRenderer.orig_DrawPlayers orig, LegacyPlayerRenderer self, Terraria.Graphics.Camera camera, System.Collections.Generic.IEnumerable<Player> players) {
        DrawLayers layers = Instance<DrawLayers>();
        if (layers.PreDrawPlayer != null) {
            Main.spriteBatch.Begin_World();
            foreach (Player p in players) {
                layers.PreDrawPlayer?.Invoke(Main.spriteBatch, p);
            }
            Main.spriteBatch.End();
        }

        orig(self, camera, players);
    }

    static void On_TileDrawing_DrawMasterTrophies(On_TileDrawing.orig_DrawMasterTrophies orig, TileDrawing self) {
        orig(self);
        Instance<DrawLayers>().PostDrawMasterRelics.Draw(Main.spriteBatch);
    }

    static void On_Main_CheckMonoliths(On_Main.orig_CheckMonoliths orig) {
        if (!Main.gameMenu) {
            Instance.PostUpdateScreenPosition.Draw(Main.spriteBatch);
        }
        orig();
    }

    static void On_Main_DrawNPCs(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles) {
        if (behindTiles) {
            Instance.WorldBehindTiles.Draw(Main.spriteBatch);
        }

        orig(self, behindTiles);
    }

    static void On_Main_DrawDust(On_Main.orig_DrawDust orig, Main main) {
        orig(main);
        Instance.PostDrawDust.Draw(Main.spriteBatch);
    }

    static void On_Main_DrawInfernoRings(On_Main.orig_DrawInfernoRings orig, Main self) {
        orig(self);

        Instance.PostDrawLiquids.Draw(Main.spriteBatch);
    }

    void ILoadable.Load(Mod mod) {
        On_TileDrawing.DrawMasterTrophies += On_TileDrawing_DrawMasterTrophies;
        On_Main.CheckMonoliths += On_Main_CheckMonoliths;
        On_Main.DrawNPCs += On_Main_DrawNPCs;
        On_Main.DrawDust += On_Main_DrawDust;
        On_Main.DrawInfernoRings += On_Main_DrawInfernoRings;
        On_LegacyPlayerRenderer.DrawPlayers += On_LegacyPlayerRenderer_DrawPlayers;
    }

    void ILoadable.Unload() { }
}
