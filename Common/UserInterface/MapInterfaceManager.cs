using AQMod.Content.MapMarkers.Components;
using AQMod.Items.Placeable.Furniture;
using AQMod.Tiles.TileEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

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

        public static void Apply(ref string mouseText, bool drawGlobes = true)
        {
            _mapScale = Main.mapFullscreenScale / Main.UIScale;
            _map = new Vector2(-(Main.mapFullscreenPos.X * _mapScale) + Main.screenWidth / 2, -(Main.mapFullscreenPos.Y * _mapScale) + Main.screenHeight / 2);
            float alpha = (float)Math.Sin(Main.GlobalTime * 7f) + 1f;
            var plr = Main.player[Main.myPlayer];
            var aQPlayer = plr.GetModPlayer<AQPlayer>();

            if (!drawGlobes || aQPlayer.nearGlobe <= 0)
                return;

            foreach (var t in TileEntity.ByID)
            {
                var texture = ModContent.GetTexture("AQMod/Assets/Map/Globe");
                var frame = new Rectangle(0, 0, texture.Width, texture.Height);
                var origin = frame.Size() / 2f;
                if (t.Value is TEGlobe globe && globe.Discovered)
                {
                    var pos = MapPos(new Vector2(globe.Position.X + 1f, globe.Position.Y + 1f));
                    if (AQUtils.PositionOnScreen(pos, 8f))
                    {
                        var scale = Main.UIScale;
                        var hitbox = Utils.CenteredRectangle(pos, new Vector2(texture.Width, texture.Height) * scale);
                        if (hitbox.Contains(Main.mouseX, Main.mouseY))
                        {
                            if (string.IsNullOrEmpty(mouseText))
                                mouseText = Lang.GetItemName(ModContent.ItemType<GlobeItem>()).Value;
                            scale += 0.5f;
                        }
                        if (aQPlayer.globeX == globe.Position.X && aQPlayer.globeY == globe.Position.Y)
                            scale += alpha * 0.1f;
                        Main.spriteBatch.Draw(texture, pos, frame, new Color(255, 255, 255, 255), 0f, origin, scale, SpriteEffects.None, 0f);
                    }
                }
            }

            int index = ModContent.GetInstance<TEGlobe>().Find(aQPlayer.globeX, aQPlayer.globeY);
            if (index == -1)
            {
                return;
            }
            var layerToggles = new MapMarkerLayerToggles(Main.player[Main.myPlayer], aQPlayer);
            TEGlobe globe2 = (TEGlobe)TileEntity.ByID[index];
            foreach (var m in globe2.Markers)
            {
                m.DrawMap(ref mouseText, plr, aQPlayer, layerToggles);
            }
            layerToggles.ApplyInterface(ref mouseText);
        }
    }
}