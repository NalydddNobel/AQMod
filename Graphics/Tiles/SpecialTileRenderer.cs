using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Graphics.Tiles
{
    public class SpecialTileRenderer : ILoadable
    {
        internal static Action PreDrawTiles;
        public static Dictionary<TileRenderLayer, List<Point>> DrawPoints { get; private set; }

        void ILoadable.Load(Mod mod)
        {
            if (Main.dedServ)
                return;

            DrawPoints = new Dictionary<TileRenderLayer, List<Point>>();
            for (int i = 0; i < (int)TileRenderLayer.Count; i++)
            {
                DrawPoints[(TileRenderLayer)i] = new List<Point>();
            }
            On.Terraria.GameContent.Drawing.TileDrawing.DrawMasterTrophies += TileDrawing_DrawMasterTrophies;
            On.Terraria.GameContent.Drawing.TileDrawing.DrawReverseVines += TileDrawing_DrawReverseVines;
            On.Terraria.GameContent.Drawing.TileDrawing.PreDrawTiles += TileDrawing_PreDrawTiles;
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