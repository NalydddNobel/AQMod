using AQMod.Assets;
using AQMod.Common.UserInterface;
using AQMod.Common.WorldGeneration;
using AQMod.Content.MapMarkers.Components;
using AQMod.Tiles.TileEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content.MapMarkers.Data
{
    public sealed class LihzahrdMarkerData : MapMarkerData, IGiveMapMarkerBuff
    {
        public LihzahrdMarkerData(string name, int item, bool setup = true) : base(name, item, setup)
        {
        }

        public override void NearbyEffects(TEGlobe globe)
        {
            Main.LocalPlayer.AddBuff(BuffType(), 16);
        }

        public bool BuffEnabled(Player player, AQPlayer aQPlayer)
        {
            return aQPlayer.showLihzahrdMap;
        }

        public int BuffType()
        {
            return ModContent.BuffType<Buffs.MapMarkers.LihzahrdMarker>();
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
            if (aQPlayer.lihzahrdMap)
            {
                if (aQPlayer.showLihzahrdMap && CommonStructureSearchMethods.LihzahrdAltar(out var position) && (Main.Map[position.X, position.Y].Light > 40 || NPC.downedPlantBoss))
                {
                    var mapIcon = TextureCache.MapIconDungeons.Value;
                    var frame = new Rectangle(MapInterfaceManager.MapIconWidth * 4, 0, MapInterfaceManager.TrueMapIconWidth, MapInterfaceManager.MapIconHeight);
                    var drawPos = MapInterfaceManager.MapPos(new Vector2(position.X + 1.5f, position.Y - 0.5f));
                    var hitbox = Utils.CenteredRectangle(drawPos, new Vector2(frame.Width, frame.Height) * Main.UIScale);
                    var scale = Main.UIScale;
                    bool hovering = hitbox.Contains(Main.mouseX, Main.mouseY);
                    if (hovering)
                        scale += 0.1f;
                    Main.spriteBatch.Draw(mapIcon, drawPos, frame, new Color(255, 255, 255, 255), 0f, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
                    if (player.GetModPlayer<AQPlayer>().unityMirror && NPC.downedPlantBoss && hovering && Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        Main.mapFullscreen = false;
                        player.UnityTeleport(new Vector2((position.X + 1f) * 16f, (position.Y + 2f) * 16f - Main.LocalPlayer.height));
                    }
                }
            }
        }

        public void ToggleMapMarkerBuff(Player player, AQPlayer aQPlayer)
        {
            aQPlayer.showLihzahrdMap = !aQPlayer.showLihzahrdMap;
        }
    }
}