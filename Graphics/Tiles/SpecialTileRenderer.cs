﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader;

namespace Aequus.Graphics.Tiles
{
    public class SpecialTileRenderer : ILoadable
    {
        public static List<Action<bool>> AdjustTileTarget { get; private set; }

        public static Action PreDrawTiles;
        public static Dictionary<TileRenderLayer, List<Point>> DrawPoints { get; private set; }
        public static Dictionary<TileRenderLayer, List<Point>> SolidDrawPoints { get; private set; }
        public static Dictionary<int, int> ModHangingVines { get; private set; }

        private static FieldInfo _addSpecialPointSpecialPositions;
        private static FieldInfo _addSpecialPointSpecialsCount;

        void ILoadable.Load(Mod mod)
        {
            if (Main.dedServ)
                return;

            _addSpecialPointSpecialPositions = typeof(TileDrawing).GetField("_specialPositions", BindingFlags.NonPublic | BindingFlags.Instance);
            _addSpecialPointSpecialsCount = typeof(TileDrawing).GetField("_specialsCount", BindingFlags.NonPublic | BindingFlags.Instance);

            SolidDrawPoints = new Dictionary<TileRenderLayer, List<Point>>();
            for (int i = 0; i < (int)TileRenderLayer.Count; i++)
            {
                SolidDrawPoints[(TileRenderLayer)i] = new List<Point>();
            }
            DrawPoints = new Dictionary<TileRenderLayer, List<Point>>();
            for (int i = 0; i < (int)TileRenderLayer.Count; i++)
            {
                DrawPoints[(TileRenderLayer)i] = new List<Point>();
            }
            AdjustTileTarget = new List<Action<bool>>();
            ModHangingVines = new Dictionary<int, int>();
            On.Terraria.GameContent.Drawing.TileDrawing.DrawMultiTileVinesInWind += TileDrawing_DrawMultiTileVinesInWind;
            On.Terraria.GameContent.Drawing.TileDrawing.DrawMasterTrophies += TileDrawing_DrawMasterTrophies;
            On.Terraria.GameContent.Drawing.TileDrawing.DrawReverseVines += TileDrawing_DrawReverseVines;
            On.Terraria.GameContent.Drawing.TileDrawing.PreDrawTiles += TileDrawing_PreDrawTiles;
            On.Terraria.Main.DoDraw_WallsAndBlacks += Main_DoDraw_WallsAndBlacks;
        }

        private static void Main_DoDraw_WallsAndBlacks(On.Terraria.Main.orig_DoDraw_WallsAndBlacks orig, Main self)
        {
            orig(self);
            Render(TileRenderLayer.PostDrawWalls);
        }

        private static void TileDrawing_DrawMultiTileVinesInWind(On.Terraria.GameContent.Drawing.TileDrawing.orig_DrawMultiTileVinesInWind orig, TileDrawing self, Vector2 screenPosition, Vector2 offSet, int topLeftX, int topLeftY, int sizeX, int sizeY)
        {
            if (ModHangingVines.TryGetValue(Main.tile[topLeftX, topLeftY].TileType, out int value))
                sizeY = value;
            orig(self, screenPosition, offSet, topLeftX, topLeftY, sizeX, sizeY);
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

        public static void AddSolid(Point p, TileRenderLayer renderLayer)
        {
            SolidDrawPoints[renderLayer].Add(p);
        }
        public static void AddSolid(int i, int j, TileRenderLayer renderLayer)
        {
            AddSolid(new Point(i, j), renderLayer);
        }

        private static void TileDrawing_DrawMasterTrophies(On.Terraria.GameContent.Drawing.TileDrawing.orig_DrawMasterTrophies orig, Terraria.GameContent.Drawing.TileDrawing self)
        {
            Render(TileRenderLayer.PreDrawMasterRelics);
            orig(self);
            Render(TileRenderLayer.PostDrawMasterRelics);
        }

        private static void TileDrawing_DrawReverseVines(On.Terraria.GameContent.Drawing.TileDrawing.orig_DrawReverseVines orig, Terraria.GameContent.Drawing.TileDrawing self)
        {
            Render(TileRenderLayer.PreDrawVines);
            orig(self);
            Render(TileRenderLayer.PostDrawVines);
        }

        private static void TileDrawing_PreDrawTiles(On.Terraria.GameContent.Drawing.TileDrawing.orig_PreDrawTiles orig, Terraria.GameContent.Drawing.TileDrawing self, bool solidLayer, bool forRenderTargets, bool intoRenderTargets)
        {
            orig(self, solidLayer, forRenderTargets, intoRenderTargets);
            if (intoRenderTargets || Lighting.UpdateEveryFrame)
            {
                if (!solidLayer)
                {
                    PreDrawTiles?.Invoke();
                    foreach (var l in DrawPoints.Values)
                    {
                        l.Clear();
                    }
                }
                else
                {
                    foreach (var l in SolidDrawPoints.Values)
                    {
                        l.Clear();
                    }
                }
            }
        }
        public static void Render(TileRenderLayer layer)
        {
            if (SolidDrawPoints == null || DrawPoints == null)
                return;

            try
            {
                foreach (var p in SolidDrawPoints[layer])
                {
                    if (Main.tile[p].HasTile && ModContent.GetModTile(Main.tile[p].TileType) is ISpecialTileRenderer renderer)
                    {
                        renderer.Render(p.X, p.Y, layer);
                    }
                }
                foreach (var p in DrawPoints[layer])
                {
                    if (Main.tile[p].HasTile && ModContent.GetModTile(Main.tile[p].TileType) is ISpecialTileRenderer renderer)
                    {
                        renderer.Render(p.X, p.Y, layer);
                    }
                }
            }
            catch
            {

            }
        }

        void ILoadable.Unload()
        {
            if (SolidDrawPoints != null)
            {
                foreach (var l in SolidDrawPoints.Values)
                {
                    l.Clear();
                }
                SolidDrawPoints?.Clear();
                SolidDrawPoints = null;
            }
            if (DrawPoints != null)
            {
                foreach (var l in DrawPoints.Values)
                {
                    l.Clear();
                }
                DrawPoints?.Clear();
                DrawPoints = null;
            }
            _addSpecialPointSpecialsCount = null;
            _addSpecialPointSpecialPositions = null;
            PreDrawTiles = null;
        }
    }
}