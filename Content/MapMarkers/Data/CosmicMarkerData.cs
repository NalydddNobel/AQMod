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
    public sealed class CosmicMarkerData : MapMarkerData, IGiveMapMarkerBuff
    {
        public CosmicMarkerData(string name, int item, bool setup = true) : base(name, item, setup)
        {
        }

        public override void NearbyEffects(TEGlobe globe)
        {
            Main.LocalPlayer.AddBuff(BuffType(), 16);
        }

        public bool BuffEnabled(Player player, AQPlayer aQPlayer)
        {
            return aQPlayer.showCosmicMap;
        }

        public int BuffType()
        {
            return ModContent.BuffType<Buffs.MapMarkers.CosmicMarker>();
        }

        public override void DrawMap(ref string mouseText, Player player, AQPlayer aQPlayer, MapMarkerLayerToggles toggles)
        {
            toggles.AddMapMarker(this);
            if (aQPlayer.showCosmicMap && GlimmerEvent.IsActive)
            {
                float alpha = (float)Math.Sin(Main.GlobalTime * 7f) + 1f;
                var texture = ModContent.GetTexture("AQMod/Assets/Map/GlimmerEvent");
                var frame = new Rectangle(0, 0, texture.Width, texture.Height);
                var drawPos = MapInterfaceManager.MapPos(new Vector2(GlimmerEvent.tileX + 0.5f, GlimmerEvent.tileY - 3f));
                var hitbox = Utils.CenteredRectangle(drawPos, new Vector2(texture.Width, texture.Height) * Main.UIScale);
                var scale = Main.UIScale;
                if (hitbox.Contains(Main.mouseX, Main.mouseY))
                {
                    string key = "Common.GlimmerEvent";
                    mouseText = AQText.ModText(key).Value;
                    scale += 0.5f;
                }
                Main.spriteBatch.Draw(texture, drawPos, frame, new Color(255, 255, 255, 255), 0f, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
                texture = ModContent.GetTexture("AQMod/Assets/Map/Starite");
                var stariteFrame = new Rectangle(0, 0, texture.Width, 22);
                var arrowUpFrame = new Rectangle(0, stariteFrame.Height + 2, texture.Width, 32);
                var arrowDownFrame = new Rectangle(0, arrowUpFrame.Y + arrowUpFrame.Height + 2, texture.Width, 32);
                var stariteOrig = stariteFrame.Size() / 2f;
                var arrowUpOrig = new Vector2(arrowUpFrame.Width / 2f, 0f);
                var arrowDownOrig = new Vector2(arrowUpOrig.X, arrowUpFrame.Height);
                float arrowBobbingY = alpha * 2f;
                for (int i = 0; i < GlimmerEvent.Layers.Count; i++)
                {
                    var layer = GlimmerEvent.Layers[i];
                    for (int j = 0; j < 2; j++)
                    {
                        int d = j == 1 ? -1 : 1;
                        var pos = new Vector2(GlimmerEvent.tileX + 0.5f + layer.Distance * d, 46f);
                        if (pos.X < 0f || pos.X > Main.maxTilesX)
                            continue;
                        for (int k = 0; k < 2; k++)
                        {
                            if (k == 1)
                                pos.Y = (float)Main.worldSurface;
                            drawPos = MapInterfaceManager.MapPos(pos);
                            hitbox = Utils.CenteredRectangle(drawPos, new Vector2(stariteFrame.Width, stariteFrame.Height) * Main.UIScale);
                            float stariteScale = Main.UIScale;
                            if (hitbox.Contains(Main.mouseX, Main.mouseY))
                            {
                                mouseText = string.Format(AQText.ModText("Common.SpawnAfterPoint").Value, Lang.GetNPCName(layer.NPCType));
                                stariteScale += 0.6f + alpha * 0.2f;
                            }
                            Main.spriteBatch.Draw(texture, drawPos, stariteFrame, new Color(255, 255, 255, 255), 0f, stariteOrig, stariteScale, SpriteEffects.None, 0f);
                            if (k == 0)
                            {
                                Main.spriteBatch.Draw(texture, drawPos + new Vector2(0f, (arrowDownFrame.Height + stariteFrame.Height + arrowBobbingY) * Main.UIScale), arrowDownFrame, new Color(255, 255, 255, 255), 0f, arrowDownOrig, Main.UIScale, SpriteEffects.None, 0f);
                            }
                            else
                            {
                                Main.spriteBatch.Draw(texture, drawPos - new Vector2(0f, (arrowUpFrame.Height + stariteFrame.Height + arrowBobbingY) * Main.UIScale), arrowUpFrame, new Color(255, 255, 255, 255), 0f, arrowUpOrig, Main.UIScale, SpriteEffects.None, 0f);
                            }
                        }
                    }
                }
            }
        }

        public void ToggleMapMarkerBuff(Player player, AQPlayer aQPlayer)
        {
            aQPlayer.showCosmicMap = !aQPlayer.showCosmicMap;
        }
    }
}