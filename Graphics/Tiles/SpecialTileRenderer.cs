using Aequus.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader;

namespace Aequus.Graphics.Tiles
{
    public class SpecialTileRenderer : ILoadable
    {
        public static List<Action<bool>> AdjustTileTarget { get; private set; }
        public static TextureColorData TileTargetColors { get; private set; }

        public static int DrawTileDelay;

        public static Action PreDrawTiles;
        public static Dictionary<TileRenderLayer, List<Point>> DrawPoints { get; private set; }
        public static Dictionary<int, int> ModHangingVines { get; private set; }

        private static FieldInfo _addSpecialPointSpecialPositions;
        private static FieldInfo _addSpecialPointSpecialsCount;

        void ILoadable.Load(Mod mod)
        {
            if (Main.dedServ)
                return;

            _addSpecialPointSpecialPositions = typeof(TileDrawing).GetField("_specialPositions", BindingFlags.NonPublic | BindingFlags.Instance);
            _addSpecialPointSpecialsCount = typeof(TileDrawing).GetField("_specialsCount", BindingFlags.NonPublic | BindingFlags.Instance);

            DrawPoints = new Dictionary<TileRenderLayer, List<Point>>();
            for (int i = 0; i < (int)TileRenderLayer.Count; i++)
            {
                DrawPoints[(TileRenderLayer)i] = new List<Point>();
            }
            AdjustTileTarget = new List<Action<bool>>();
            ModHangingVines = new Dictionary<int, int>();
            On.Terraria.GameContent.Drawing.TileDrawing.DrawMultiTileVinesInWind += TileDrawing_DrawMultiTileVinesInWind; ;
            On.Terraria.Main.RenderTiles += Main_RenderTiles;
            On.Terraria.GameContent.Drawing.TileDrawing.DrawMasterTrophies += TileDrawing_DrawMasterTrophies;
            On.Terraria.GameContent.Drawing.TileDrawing.DrawReverseVines += TileDrawing_DrawReverseVines;
            On.Terraria.GameContent.Drawing.TileDrawing.PreDrawTiles += TileDrawing_PreDrawTiles;
        }

        private static void TileDrawing_DrawMultiTileVinesInWind(On.Terraria.GameContent.Drawing.TileDrawing.orig_DrawMultiTileVinesInWind orig, TileDrawing self, Vector2 screenPosition, Vector2 offSet, int topLeftX, int topLeftY, int sizeX, int sizeY)
        {
            if (ModHangingVines.TryGetValue(Main.tile[topLeftX, topLeftY].TileType, out int value))
                sizeY = value;
            orig(self, screenPosition, offSet, topLeftX, topLeftY, sizeX, sizeY);
        }

        private static void Main_RenderTiles(On.Terraria.Main.orig_RenderTiles orig, Main self)
        {
            orig(self);
            if (AdjustTileTarget.Count <= 0)
            {
                DrawTileDelay = 0;
                return;
            }

            if (AdjustTileTarget.Count > 240)
            {
                AdjustTileTarget.Clear();
                return;
            }
            if (self.tileTarget == null || self.tileTarget.IsDisposed)
            {
                return;
            }

            DrawTileDelay--;
            bool draw = false;
            if (DrawTileDelay <= 0)
            {
                draw = true;
                var s = new Stopwatch();
                s.Start();
                if (TileTargetColors == null || !TileTargetColors.CheckTexture(self.tileTarget))
                {
                    TileTargetColors = new TextureColorData(self.tileTarget);
                }
                else
                {
                    TileTargetColors.RefreshTexture(Main.instance.tileTarget);
                }
                s.Stop();
                DrawTileDelay = (int)Math.Clamp(s.ElapsedMilliseconds, 0, 50);
                if (DrawTileDelay > 10)
                {
                    var tileDrawPos = new Vector2(Main.sceneTilePos.X + Main.offScreenRange, Main.sceneTilePos.Y + Main.offScreenRange);
                    DrawTileDelay = (int)Math.Max(DrawTileDelay - (Main.screenPosition - tileDrawPos).Length().UnNaN() / 2f, 10f);
                }
            }

            foreach (var r in AdjustTileTarget)
            {
                r.Invoke(draw);
            }
            AdjustTileTarget?.Clear();

            //TileTargetColors.ApplyChanges();
        }

        public static void AddSpecialPoint(TileDrawing renderer, int x, int y, int type)
        {
            if (_addSpecialPointSpecialPositions?.GetValue(renderer) is Point[][] _specialPositions)
            {
                if (_addSpecialPointSpecialsCount?.GetValue(renderer) is int[] _specialsCount)
                {
                    _specialPositions[type][_specialsCount[type]++] = new Point(x, y);
                }
            }
        }

        public static void Add(Point p, TileRenderLayer renderLayer)
        {
            DrawPoints[renderLayer].Add(p);
        }

        public static void Add(int i, int j, TileRenderLayer renderLayer)
        {
            Add(new Point(i, j), renderLayer);
        }

        private static void TileDrawing_DrawMasterTrophies(On.Terraria.GameContent.Drawing.TileDrawing.orig_DrawMasterTrophies orig, Terraria.GameContent.Drawing.TileDrawing self)
        {
            DrawRender(TileRenderLayer.PreDrawMasterRelics);
            orig(self);
            DrawRender(TileRenderLayer.PostDrawMasterRelics);
        }

        private static void TileDrawing_DrawReverseVines(On.Terraria.GameContent.Drawing.TileDrawing.orig_DrawReverseVines orig, Terraria.GameContent.Drawing.TileDrawing self)
        {
            DrawRender(TileRenderLayer.PreDrawVines);
            orig(self);
            DrawRender(TileRenderLayer.PostDrawVines);
        }

        private static void TileDrawing_PreDrawTiles(On.Terraria.GameContent.Drawing.TileDrawing.orig_PreDrawTiles orig, Terraria.GameContent.Drawing.TileDrawing self, bool solidLayer, bool forRenderTargets, bool intoRenderTargets)
        {
            orig(self, solidLayer, forRenderTargets, intoRenderTargets);
            bool flag = intoRenderTargets || Lighting.UpdateEveryFrame;
            if (!solidLayer && flag)
            {
                PreDrawTiles?.Invoke();

                foreach (var l in DrawPoints.Values)
                    l.Clear();
            }
        }
        public static void DrawRender(TileRenderLayer layer)
        {
            foreach (var p in DrawPoints[layer])
            {
                if (Main.tile[p].HasTile && ModContent.GetModTile(Main.tile[p].TileType) is ISpecialTileRenderer renderer)
                {
                    renderer.Render(p.X, p.Y, layer);
                }
            }
        }

        void ILoadable.Unload()
        {
            if (DrawPoints != null)
            {
                foreach (var l in DrawPoints.Values)
                {
                    l.Clear();
                }
                DrawPoints?.Clear();
                DrawPoints = null;
            }
            PreDrawTiles = null;
        }
    }
}