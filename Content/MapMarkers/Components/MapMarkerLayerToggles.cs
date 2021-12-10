using AQMod.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace AQMod.Content.MapMarkers.Components
{
    public sealed class MapMarkerLayerToggles
    {
        private Player _player;
        private AQPlayer _aQPlayer;
        private readonly List<IGiveMapMarkerBuff> MapMarkerBuffs;

        public MapMarkerLayerToggles(Player player, AQPlayer aQPlayer)
        {
            MapMarkerBuffs = new List<IGiveMapMarkerBuff>();
            _player = player;
            _aQPlayer = aQPlayer;
        }

        public void AddMapMarker(IGiveMapMarkerBuff mapMarkerBuff)
        {
            MapMarkerBuffs.Add(mapMarkerBuff);
        }
        
        public void ApplyInterface(ref string mouseText)
        {
            if (MapMarkerBuffs.Count == 0)
                return;
            for (int i = 0; i < MapMarkerBuffs.Count; i++)
            {
                IGiveMapMarkerBuff marker = MapMarkerBuffs[i];
                int buffType = marker.BuffType();
                var buffTexture = OldTextureCache.GetBuff(buffType);
                float scale = Main.UIScale;
                var drawPosition = new Vector2((8f + ((buffTexture.Width + 8f) * i)) * scale, (8f + buffTexture.Height / 2f) * scale);
                var drawColor = new Color(255, 255, 255, 255);
                if (!marker.BuffEnabled(_player, _aQPlayer))
                {
                    drawColor *= 0.5f;
                    drawColor.A = 200;
                }
                var hitbox = new Rectangle((int)drawPosition.X, (int)drawPosition.Y, (int)(buffTexture.Width * scale), (int)(buffTexture.Height * scale));
                if (hitbox.Contains(Main.mouseX, Main.mouseY))
                {
                    if (Main.mouseLeft && Main.mouseLeftRelease)
                        marker.ToggleMapMarkerBuff(_player, _aQPlayer);
                    mouseText = Lang.GetBuffName(buffType);
                    Main.spriteBatch.Draw(OldTextureCache.BuffOutline.Value, drawPosition, null, drawColor, 0f, new Vector2(4f, 4f), scale, SpriteEffects.None, 0f);
                }
                Main.spriteBatch.Draw(buffTexture, drawPosition, null, drawColor, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, 0f);
            }
        }
    }
}