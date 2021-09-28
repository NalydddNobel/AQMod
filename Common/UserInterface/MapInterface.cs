using AQMod.Assets.Textures;
using AQMod.Common.Config;
using AQMod.Common.Utilities;
using AQMod.Content;
using AQMod.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace AQMod.Common.UserInterface
{
    public static class MapInterface
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
            return new Vector2((int)(tileCoords.X * _mapScale + _map.X), (int)(tileCoords.Y * _mapScale + _map.Y));
        }

        private static Vector2 mapPosSingle(Vector2 tileCoords)
        {
            return new Vector2(tileCoords.X * _mapScale + _map.X, tileCoords.Y * _mapScale + _map.Y);
        }

        public static bool UnityTeleport(float x, float y, Vector2 drawPosition, Player player, bool allowedToTeleport)
        {
            var texture = DrawUtils.Textures.Extras[ExtraID.UnityTeleportable];
            float scale = Main.UIScale * 0.8f;
            drawPosition += new Vector2(-texture.Width / 2f, -texture.Height / 2f) * scale;
            Main.spriteBatch.Draw(texture, drawPosition + new Vector2(2f, 0f), null, Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPosition + new Vector2(-2f, 0f), null, Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPosition + new Vector2(0f, 2f), null, Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPosition + new Vector2(0f, -2f), null, Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            if (player.GetModPlayer<AQPlayer>().unityMirror && allowedToTeleport && Main.mouseLeft && Main.mouseLeftRelease)
            {
                Main.mapFullscreen = false;
                player.UnityTeleport(new Vector2(x, y));
                return true;
            }
            return false;
        }

        private const int defaultMarkerMapIconType = 1;

        private static void DrawMarkerMapIcons(ref string mouseText)
        {
            float oldScale = Main.inventoryScale;
            Main.inventoryScale = 0.6f;
            var mapPlayer = Main.player[Main.myPlayer].GetModPlayer<MapMarkerPlayer>();
            int x = 8;
            int add = (int)(56 * Main.inventoryScale);
            int y = 8;
            int markerMapIconType = AQConfigClient.Instance.MarkerMapUIType;
            if (markerMapIconType == -1)
                markerMapIconType = defaultMarkerMapIconType;
            for (int i = 0; i < MapMarkerPlayer.MapMarkerCount; i++)
            {
                if (mapPlayer.MarkersObtained[i])
                {
                    drawMarkerSlot(mapPlayer, x, y, i, ref mouseText);
                    if (markerMapIconType == 0)
                        y += add;
                    else
                        x += add;
                }
            }
            Main.inventoryScale = oldScale;
        }

        private static void drawMarkerSlot(MapMarkerPlayer mapPlayer, int x, int y, int type, ref string mouseText)
        {
            var marker = MapMarkerPlayer.GetMarker(type);
            UIHelper.DrawItemSlot(new Vector2(x, y), Main.inventoryBackTexture);
            if (new Rectangle(x, y, (int)(Main.inventoryBackTexture.Width * Main.inventoryScale), (int)(Main.inventoryBackTexture.Height * Main.inventoryScale)).Contains(Main.mouseX, Main.mouseY))
            {
                if (!mapPlayer.MarkersHidden[type])
                {
                    mouseText = AQText.DisableThing(AQText.AequusItemName(marker.Name));
                    if (Main.mouseLeft && Main.mouseLeftRelease || Main.mouseRight && Main.mouseRightRelease)
                    {
                        Main.PlaySound(SoundID.MenuClose);
                        mapPlayer.MarkersHidden[type] = true;
                    }
                }
                else
                {
                    mouseText = AQText.EnableThing(AQText.AequusItemName(marker.Name));
                    if (Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        Main.PlaySound(SoundID.MenuOpen);
                        mapPlayer.MarkersHidden[type] = false;
                    }
                }
            }
            if (mapPlayer.MarkersHidden[type])
            {
                UIHelper.DrawItemInv(new Vector2(x, y), marker.item, new Color(0, 0, 0, 255));
            }
            else
            {
                UIHelper.DrawItemInv(new Vector2(x, y), marker.item);
            }
        }

        public static void Apply(ref string mouseText)
        {
            if (Main.myPlayer == -1 || !Main.LocalPlayer.active)
                return;
            _mapScale = Main.mapFullscreenScale / Main.UIScale;
            _map = new Vector2(-(Main.mapFullscreenPos.X * _mapScale) + Main.screenWidth / 2, -(Main.mapFullscreenPos.Y * _mapScale) + Main.screenHeight / 2);
            DrawMarkerMapIcons(ref mouseText);
            mouseText = Main.player[Main.myPlayer].GetModPlayer<MapMarkerPlayer>().ApplyMarkers(Main.player[Main.myPlayer].GetModPlayer<AQPlayer>(), mouseText);
        }
    }
}