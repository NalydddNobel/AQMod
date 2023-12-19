using Aequus.Content.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria.GameContent.Drawing;

namespace Aequus.Core.Graphics.Tiles;

internal sealed class SpecialTileRenderer : ModSystem {
    public static Action PreDrawNonSolidTiles;
    public static Action UpdateTileEffects;
    public static Action ClearTileEffects;

    public static List<Point>[] DrawPoints { get; private set; }
    public static List<Point>[] SolidDrawPoints { get; private set; }
    public static Dictionary<int, int> ModHangingVines { get; private set; }

    private static FieldInfo _addSpecialPointSpecialPositions;
    private static FieldInfo _addSpecialPointSpecialsCount;
    private static TileDrawing _rendererCache;
    private static Point[][] _specialPositions;
    private static int[] _specialsCount;

    public override void Load() {
        if (Main.dedServ) {
            return;
        }

        _addSpecialPointSpecialPositions = typeof(TileDrawing).GetField("_specialPositions", BindingFlags.NonPublic | BindingFlags.Instance);
        _addSpecialPointSpecialsCount = typeof(TileDrawing).GetField("_specialsCount", BindingFlags.NonPublic | BindingFlags.Instance);

        SolidDrawPoints = new List<Point>[TileRenderLayerID.Count];
        for (int i = 0; i < TileRenderLayerID.Count; i++) {
            SolidDrawPoints[i] = new List<Point>();
        }
        DrawPoints = new List<Point>[TileRenderLayerID.Count];
        for (int i = 0; i < TileRenderLayerID.Count; i++) {
            DrawPoints[i] = new List<Point>();
        }
        ModHangingVines = new Dictionary<int, int>();
        On_TileDrawing.DrawMasterTrophies += TileDrawing_DrawMasterTrophies;
        On_TileDrawing.DrawReverseVines += TileDrawing_DrawReverseVines;
        On_TileDrawing.PreDrawTiles += TileDrawing_PreDrawTiles;
        On_Main.DoDraw_Tiles_NonSolid += Main_DoDraw_Tiles_NonSolid;
        On_Main.DoDraw_Tiles_Solid += Main_DoDraw_Tiles_Solid;
        On_Main.DoDraw_WallsAndBlacks += Main_DoDraw_WallsAndBlacks;
    }

    private void Main_DoDraw_Tiles_Solid(On_Main.orig_DoDraw_Tiles_Solid orig, Main self) {
        foreach (var batch in BatchedTileRenderer._batches.Values) {
            if (batch.Count <= 0 || batch.SolidLayer == false) {
                continue;
            }

            try {
                batch.Drawer.BatchedPreDraw(batch.Tiles, batch.Count);
            }
            catch {
            }
        }
        orig(self);
    }

    private static void Main_DoDraw_Tiles_NonSolid(On_Main.orig_DoDraw_Tiles_NonSolid orig, Main self) {
        foreach (var batch in BatchedTileRenderer._batches.Values) {
            if (batch.Count <= 0 || batch.SolidLayer == true) {
                continue;
            }

            try {
                batch.Drawer.BatchedPreDraw(batch.Tiles, batch.Count);
            }
            catch {
            }
        }
        orig(self);
    }

    private static void Main_DoDraw_WallsAndBlacks(On_Main.orig_DoDraw_WallsAndBlacks orig, Main self) {
        orig(self);
        Render(TileRenderLayerID.PostDrawWalls);
    }

    private static void CheckRenderer(TileDrawing otherRenderer) {
        if (_rendererCache != null && otherRenderer == _rendererCache) {
            return;
        }

        _specialPositions = (Point[][])_addSpecialPointSpecialPositions.GetValue(otherRenderer);
        _specialsCount = (int[])_addSpecialPointSpecialsCount.GetValue(otherRenderer);
    }
    public static void AddSpecialPoint(int x, int y, int type) {
        if (_specialPositions == null) {
            return;
        }

        _specialPositions[type][_specialsCount[type]++] = new Point(x, y);
    }

    public static void Add(Point p, byte renderLayer) {
        DrawPoints[renderLayer].Add(p);
    }
    public static void Add(int i, int j, byte renderLayer) {
        Add(new Point(i, j), renderLayer);
    }

    public static void AddSolid(Point p, byte renderLayer) {
        SolidDrawPoints[renderLayer].Add(p);
    }
    public static void AddSolid(int i, int j, byte renderLayer) {
        AddSolid(new Point(i, j), renderLayer);
    }

    public static bool AnyInLayer(byte layerId) {
        return DrawPoints[layerId].Count > 0 || SolidDrawPoints[layerId].Count > 0;
    }

    private static void TileDrawing_DrawMasterTrophies(On_TileDrawing.orig_DrawMasterTrophies orig, TileDrawing self) {
        Render(TileRenderLayerID.PreDrawMasterRelics);
        orig(self);
        Render(TileRenderLayerID.PostDrawMasterRelics);
    }

    private static void TileDrawing_DrawReverseVines(On_TileDrawing.orig_DrawReverseVines orig, TileDrawing self) {
        Render(TileRenderLayerID.PreDrawVines);
        orig(self);
        Render(TileRenderLayerID.PostDrawVines);
    }

    private static void TileDrawing_PreDrawTiles(On_TileDrawing.orig_PreDrawTiles orig, TileDrawing tileRenderer, bool solidLayer, bool forRenderTargets, bool intoRenderTargets) {
        CheckRenderer(tileRenderer);
        orig(tileRenderer, solidLayer, forRenderTargets, intoRenderTargets);
        if (intoRenderTargets || Lighting.UpdateEveryFrame) {
            if (!solidLayer) {
                PreDrawNonSolidTiles?.Invoke();
                for (int i = 0; i < DrawPoints.Length; i++) {
                    DrawPoints[i].Clear();
                }
            }
            else {
                RadonMossFogRenderer.Tiles.Clear();
                RadonMossFogRenderer.DrawInfoCache.Clear();
                for (int i = 0; i < SolidDrawPoints.Length; i++) {
                    SolidDrawPoints[i].Clear();
                }
            }

            foreach (var batch in BatchedTileRenderer._batches.Values) {
                if (batch.SolidLayer == solidLayer) {
                    batch.Count = 0;
                }
            }
        }
    }

    public static void Render(byte layer) {
        if (SolidDrawPoints == null || DrawPoints == null) {
            return;
        }

        try {
            for (int i = 0; i < SolidDrawPoints[layer].Count; i++) {
                Point p = SolidDrawPoints[layer][i];
                if (Main.tile[p].HasTile && ModContent.GetModTile(Main.tile[p].TileType) is ISpecialTileRenderer renderer) {
                    renderer.Render(p.X, p.Y, layer);
                }
            }
            for (int i = 0; i < DrawPoints[layer].Count; i++) {
                Point p = DrawPoints[layer][i];
                if (Main.tile[p].HasTile && ModContent.GetModTile(Main.tile[p].TileType) is ISpecialTileRenderer renderer) {
                    renderer.Render(p.X, p.Y, layer);
                }
            }
        }
        catch {
        }
    }

    public override void PreUpdateGores() {
        if (!Main.dedServ && UpdateTileEffects != null) {
            UpdateTileEffects();
        }
    }

    public override void OnWorldLoad() {
        if (!Main.dedServ && ClearTileEffects != null) {
            ClearTileEffects();
        }
    }

    public override void OnWorldUnload() {
        if (!Main.dedServ && ClearTileEffects != null) {
            ClearTileEffects();
        }
    }

    public override void Unload() {
        if (SolidDrawPoints != null) {
            foreach (var l in SolidDrawPoints) {
                l.Clear();
            }
            SolidDrawPoints = null;
        }
        if (DrawPoints != null) {
            foreach (var l in DrawPoints) {
                l.Clear();
            }
            DrawPoints = null;
        }
        _addSpecialPointSpecialsCount = null;
        _addSpecialPointSpecialPositions = null;
        PreDrawNonSolidTiles = null;
        UpdateTileEffects = null;
        ClearTileEffects = null;
    }
}