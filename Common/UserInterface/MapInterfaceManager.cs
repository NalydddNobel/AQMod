using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace AQMod.Common.UserInterface
{
    public static class MapInterfaceManager
    {
        public const int MapIconFrames = 4;
        public const int MapIconWidth = 34;
        public const int MapIconWidthPadding = 2;
        public const int TrueMapIconWidth = MapIconWidth - MapIconWidthPadding;
        public const int MapIconHeight = 32;

        private static Vector2 _map;
        private static float _mapScale;

        public static Vector2 MapPos(Vector2 tileCoords)
        {
            var v = mapPosSingle(tileCoords);
            return new Vector2((int)v.X, (int)v.Y);
        }

        private static Vector2 mapPosSingle(Vector2 tileCoords)
        {
            return new Vector2(tileCoords.X * _mapScale + _map.X, tileCoords.Y * _mapScale + _map.Y);
        }

        public static Vector2 MapPosToTilePos(Vector2 mapPos)
        {
            return new Vector2((mapPos.X - _map.X) / _mapScale, (mapPos.Y - _map.Y) / _mapScale);
        }

        public static void DrawUI(ref string mouseText, bool drawGlobes = true)
        {
            _mapScale = Main.mapFullscreenScale / Main.UIScale;
            _map = new Vector2(-(Main.mapFullscreenPos.X * _mapScale) + Main.screenWidth / 2, -(Main.mapFullscreenPos.Y * _mapScale) + Main.screenHeight / 2);
            float alpha = (float)Math.Sin(Main.GlobalTime * 7f) + 1f;
            var plr = Main.player[Main.myPlayer];
            var aQPlayer = plr.GetModPlayer<AQPlayer>();

            if (aQPlayer.showCosmicMap)
            {

            }

            //var layerToggles = new MapMarkerLayerToggles(Main.player[Main.myPlayer], aQPlayer);
            //TEGlobe globe2 = (TEGlobe)TileEntity.ByID[index];
            //foreach (var m in globe2.LegacyMarkers)
            //{
            //    m.DrawMap(ref mouseText, plr, aQPlayer, layerToggles);
            //}
            //layerToggles.ApplyInterface(ref mouseText);
        }

        private static void Draw_DungeonTP()
        {

        }

        private static void Draw_LihzahrdAltarTP()
        {

        }

        private static void Draw_PlanteraBulbTP()
        {

        }

        private static void Draw_OldGenMapBlips()
        {

        }
    }
}