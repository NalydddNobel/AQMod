using AQMod.Assets;
using AQMod.Common.UserInterface;
using AQMod.Content.MapMarkers.Components;
using AQMod.Content.WorldEvents.GlimmerEvent;
using AQMod.Localization;
using AQMod.Tiles.TileEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content.MapMarkers.Data
{
    public sealed class RetroMarkerData : MapMarkerData, IGiveMapMarkerBuff
    {
        public RetroMarkerData(string name, int item, bool setup = true) : base(name, item, setup)
        {
        }

        public override void NearbyEffects(TEGlobe globe)
        {
            Main.LocalPlayer.AddBuff(BuffType(), 16);
        }

        public bool BuffEnabled(Player player, AQPlayer aQPlayer)
        {
            return aQPlayer.showRetroMap;
        }

        public int BuffType()
        {
            return ModContent.BuffType<Buffs.MapMarkers.RetroMarker>();
        }

        public override void OnAddToGlobe(Player player, AQPlayer aQPlayer, TEGlobe globe)
        {
            var rectangle = new Rectangle(globe.Position.X * 16, globe.Position.Y * 16, 32, 32);
            for (int i = 0; i < 12; i++)
            {
                int d = Dust.NewDust(new Vector2(rectangle.X, rectangle.Y), rectangle.Width, rectangle.Height, 75);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity.X *= 0.2f;
                Main.dust[d].velocity.Y = -Main.rand.NextFloat(1f, 3f);
            }
        }

        public override void DrawMap(ref string mouseText, Player player, AQPlayer aQPlayer, MapMarkerLayerToggles toggles)
        {
            toggles.AddMapMarker(this);
            if (aQPlayer.showRetroMap)
            {
                var texture = TextureCache.MapIconEnemyBlip.Value;
                int frameHeight = texture.Height / 2;
                int frameNumber = (int)(Main.GameUpdateCount % 24 / 12);
                var frame = new Rectangle(0, frameHeight * frameNumber, texture.Width, frameHeight);
                Vector2 origin = frameNumber == 1 ? new Vector2(3f, 5f) : new Vector2(4f, 4f);
                var scale = Main.UIScale * 4f;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && !AQNPC.Sets.NoMapBlip[Main.npc[i].type] && Main.npc[i].GetBossHeadTextureIndex() == -1 && !Main.npc[i].townNPC)
                    {
                        var drawPos = MapInterfaceManager.MapPos(Main.npc[i].Center / 16f);
                        var color = AQMod.MapBlipColor;
                        byte r = color.R;
                        if (r < 10)
                            r = 10;
                        byte g = color.G;
                        if (g < 10)
                            g = 10;
                        byte b = color.B;
                        if (b < 10)
                            b = 10;
                        Main.spriteBatch.Draw(texture, drawPos, frame, new Color(r, g, b, 150), 0f, origin, scale, SpriteEffects.None, 0f);
                    }
                }
            }
        }

        public void ToggleMapMarkerBuff(Player player, AQPlayer aQPlayer)
        {
            aQPlayer.showRetroMap = !aQPlayer.showRetroMap;
        }
    }
}