using AQMod.Assets;
using AQMod.Common.Configuration;
using AQMod.Common.WorldGeneration;
using AQMod.Content.Players;
using AQMod.Content.World;
using AQMod.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Content
{
    public static class InterfaceMap
    {
        public const byte Icon_DungeonPink = 0;
        public const byte Icon_DungeonBlue = 1;
        public const byte Icon_DungeonGreen = 2;
        public const byte Icon_DungeonColorless = 3;
        public const byte Icon_LihzahrdAltar = 4;
        public const byte Icon_PlanteraBulb = 5;

        public const int SpriteWidth = 36;
        public const int SpriteHeight = 34;
        public const int SpritePadding = 2;
        public static readonly Vector2 SpriteOrigin = new Vector2(SpriteWidth / 2f, SpriteHeight / 2f);

        private static Vector2 _map;
        private static float _mapScale;
        private static float _mapTogglesAlpha = 0.1f;

        public static Action<Player, List<MapLayerToggle>> AddMapLayerToggles;

        private static byte _mapRefresh = 0;
        private static List<Point> _planteraBulbsPositionCache;

        public struct MapLayerToggle
        {
            private readonly Func<bool> isActive;
            private readonly Action toggle;
            public string HoverKey;
            public readonly Texture2D Texture;

            public MapLayerToggle(Texture2D texture, string key, Func<bool> isActive, Action toggle)
            {
                HoverKey = key;
                this.isActive = isActive;
                this.toggle = toggle;
                Texture = texture;
            }

            public bool IsActive()
            {
                return isActive();
            }

            public void Toggle()
            {
                toggle();
            }

            public string HoverText()
            {
                return Language.GetTextValue(HoverKey);
            }
        }

        public static Vector2 MapDrawPosition(Vector2 tileCoords)
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

        internal static void RenderOnMap(ref string mouseText)
        {
            _mapScale = Main.mapFullscreenScale / Main.UIScale;
            _map = new Vector2(-(Main.mapFullscreenPos.X * _mapScale) + Main.screenWidth / 2, -(Main.mapFullscreenPos.Y * _mapScale) + Main.screenHeight / 2);

            float alpha = (float)Math.Sin(Main.GlobalTime * 7f) + 1f;

            var player = Main.LocalPlayer;
            var upgrades = player.GetModPlayer<PlayerMapUpgrades>();

            if (PlayerMapUpgrades.MapUpgradeVisible(upgrades.Beeswax))
            {
                if (_planteraBulbsPositionCache == null)
                {
                    _planteraBulbsPositionCache = new List<Point>();
                    _mapRefresh = 0;
                }
                if (_mapRefresh < 16)
                {
                    int startX = 50 + Main.maxTilesX / 16 * _mapRefresh;
                    int endX = Math.Min(50 + Main.maxTilesX / 16 * (_mapRefresh + 1), Main.maxTilesX - 50);
                    if (_planteraBulbsPositionCache.Count == 1 && startX <= _planteraBulbsPositionCache[0].X && endX > _planteraBulbsPositionCache[0].X)
                    {
                        _planteraBulbsPositionCache = new List<Point>();
                    }
                    for (int k = 0; k < _planteraBulbsPositionCache.Count; k++)
                    {
                        if (_planteraBulbsPositionCache[k].X >= startX && _planteraBulbsPositionCache[k].X < endX)
                        {
                            _planteraBulbsPositionCache.RemoveAt(k);
                            k--;
                        }
                    }
                    for (int i = startX; i < endX; i++)
                    {
                        for (int j = 50; j < Main.maxTilesY - 50; j++)
                        {
                            if (Main.tile[i, j] == null)
                            {
                                Main.tile[i, j] = new Tile();
                                continue;
                            }
                            if (!Main.tile[i, j].active() || Main.Map[i, j].Light < 40)
                            {
                                continue;
                            }
                            if (Main.tile[i, j].type == TileID.PlanteraBulb && Main.tile[i, j].frameX == 0 && Main.tile[i, j].frameY == 0)
                            {
                                _planteraBulbsPositionCache.Add(new Point(i, j));
                                j++;
                            }
                        }
                    }
                }
                //Main.NewText(_planteraBulbsPositionCache.Count);
                foreach (var p in _planteraBulbsPositionCache)
                {
                    DrawMapIcon(out bool hovering, Icon_PlanteraBulb, p.X + 1f, p.Y, interactable: Main.hardMode);
                    if (hovering)
                    {
                        if (Main.mouseLeft && Main.mouseLeftRelease)
                        {
                            //TeleportPlayer(new Vector2(p.X * 16f + 16f, p.Y * 16f - 16f));
                            //return;
                        }
                        mouseText = Language.GetTextValue("MapObject.PlanterasBulb");
                    }
                }
            }
            else
            {
                _planteraBulbsPositionCache = null;
            }

            if (PlayerMapUpgrades.MapUpgradeVisible(upgrades.VialOfBlood) && (Main.Map[Main.dungeonX, Main.dungeonY].Light > 40 || NPC.downedBoss3 || Main.hardMode))
            {
                byte icon = Icon_DungeonColorless;
                for (int l = 0; l < 4; l++)
                {
                    if (Main.tile[Main.dungeonX, Main.dungeonY + l].active())
                    {
                        if (Main.tile[Main.dungeonX, Main.dungeonY + l].type == TileID.PinkDungeonBrick)
                        {
                            icon = Icon_DungeonPink;
                            break;
                        }
                        if (Main.tile[Main.dungeonX, Main.dungeonY + l].type == TileID.BlueDungeonBrick)
                        {
                            icon = Icon_DungeonBlue;
                            break;
                        }
                        if (Main.tile[Main.dungeonX, Main.dungeonY + l].type == TileID.GreenDungeonBrick)
                        {
                            icon = Icon_DungeonGreen;
                            break;
                        }
                    }
                }
                DrawMapIcon(out bool hovering, icon, Main.dungeonX + 0.5f, Main.dungeonY - 2.5f, interactable: true, ((Main.dungeonX > Main.maxTilesX / 2) ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
                if (hovering)
                {
                    if (Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        TeleportPlayer(new Vector2(Main.dungeonX * 16f + 8f, Main.dungeonY * 16f - 48f));
                        return;
                    }
                    mouseText = Language.GetTextValue("Mods.AQMod.MapObject.Dungeon");
                }
            }

            if (PlayerMapUpgrades.MapUpgradeVisible(upgrades.Cabbage))
            {
                if (CommonStructureSearchMethods.LihzahrdAltar(out var altarLocation) && (NPC.downedPlantBoss || Main.Map[altarLocation.X, altarLocation.Y].Light > 40))
                {
                    DrawMapIcon(out bool hovering, Icon_LihzahrdAltar, altarLocation.X + 1.5f, altarLocation.Y + 0.5f, interactable: NPC.downedPlantBoss);
                    if (hovering && NPC.downedPlantBoss)
                    {
                        if (Main.mouseLeft && Main.mouseLeftRelease)
                        {
                            TeleportPlayer(altarLocation.ToWorldCoordinates(new Vector2(24f, -8f)));
                            return;
                        }
                        mouseText = Language.GetTextValue("ItemName.LihzahrdAltar");
                    }
                }
            }

            if (PlayerMapUpgrades.MapUpgradeVisible(upgrades.CosmicTelescope) && EventGlimmer.IsGlimmerEventCurrentlyActive())
            {
                var texture = ModContent.GetTexture(TexturePaths.MapUI + "ultimatesword");
                var frame = new Rectangle(0, 0, texture.Width, texture.Height);
                var drawPos = MapDrawPosition(new Vector2(EventGlimmer.tileX + 0.5f, EventGlimmer.tileY - 3f));
                var hitbox = Utils.CenteredRectangle(drawPos, new Vector2(texture.Width, texture.Height) * Main.UIScale);
                var scale = Main.UIScale;
                if (hitbox.Contains(Main.mouseX, Main.mouseY))
                {
                    mouseText = Language.GetTextValue("Mods.AQMod.EventName.GlimmerEvent");
                    scale += 0.5f;
                }
                Main.spriteBatch.Draw(texture, drawPos, frame, new Color(255, 255, 255, 255), 0f, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
                texture = ModContent.GetTexture(TexturePaths.MapUI + "staritelayers");
                var stariteFrame = new Rectangle(0, 0, texture.Width, 22);
                var arrowUpFrame = new Rectangle(0, stariteFrame.Height + 2, texture.Width, 32);
                var arrowDownFrame = new Rectangle(0, arrowUpFrame.Y + arrowUpFrame.Height + 2, texture.Width, 32);
                var stariteOrig = stariteFrame.Size() / 2f;
                var arrowUpOrig = new Vector2(arrowUpFrame.Width / 2f, 0f);
                var arrowDownOrig = new Vector2(arrowUpOrig.X, arrowUpFrame.Height);
                float arrowBobbingY = alpha * 2f;
                for (int i = 0; i < EventGlimmer.Layers.Count; i++)
                {
                    var layer = EventGlimmer.Layers[i];
                    for (int j = 0; j < 2; j++)
                    {
                        int d = j == 1 ? -1 : 1;
                        var pos = new Vector2(EventGlimmer.tileX + 0.5f + layer.Distance * d, 46f);
                        if (pos.X < 0f || pos.X > Main.maxTilesX)
                            continue;
                        for (int k = 0; k < 2; k++)
                        {
                            if (k == 1)
                                pos.Y = (float)Main.worldSurface;
                            drawPos = MapDrawPosition(pos);
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

            if (PlayerMapUpgrades.MapUpgradeVisible(upgrades.BlightedSoul))
            {
                var color = ModContent.GetInstance<AQConfigClient>().MapBlipColor;
                var texture = ModContent.GetTexture(TexturePaths.MapUI + "enemyblip");
                int frameHeight = texture.Height / 2;
                int frameNumber = (int)(Main.GameUpdateCount % 24 / 12);
                var frame = new Rectangle(0, frameHeight * frameNumber, texture.Width, frameHeight);
                Vector2 origin = frameNumber == 1 ? new Vector2(3f, 5f) : new Vector2(4f, 4f);
                var scale = Main.UIScale * 4f;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && !AQNPC.Sets.NoMapBlip.Contains(Main.npc[i].type) && Main.npc[i].GetBossHeadTextureIndex() == -1 && !Main.npc[i].townNPC)
                    {
                        var drawPos = MapDrawPosition(Main.npc[i].Center / 16f);
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

            _mapRefresh++;
            if (_mapRefresh > 60)
                _mapRefresh = 0;
        }

        private static void DrawMapIcon(byte frame, float x, float y, bool teleportable = false)
        {
            DrawMapIcon(out bool _, frame, x, y, teleportable);
        }

        private static void DrawMapIcon(out bool hovering, byte frame, float x, float y, bool interactable = false, SpriteEffects effects = SpriteEffects.None)
        {
            var mapIcon = ModContent.GetTexture(TexturePaths.MapUI + "structures");
            var spriteFrame = new Rectangle((SpriteWidth + SpritePadding) * frame, 0, SpriteWidth, SpriteHeight);
            var drawPos = MapDrawPosition(new Vector2(x, y));
            var hitbox = Utils.CenteredRectangle(drawPos, new Vector2(spriteFrame.Width, spriteFrame.Height) * Main.UIScale);
            var scale = Main.UIScale;
            hovering = hitbox.Contains(Main.mouseX, Main.mouseY);
            if (hovering && interactable)
            {
                scale *= 2f;
            }
            Main.spriteBatch.Draw(mapIcon, drawPos, spriteFrame, new Color(255, 255, 255, 255), 0f, SpriteOrigin, scale, effects, 0f);
        }

        private static void TeleportPlayer(Vector2 location)
        {
            Main.mouseLeftRelease = false;
            Main.mapFullscreen = false;
            Main.player[Main.myPlayer].Teleport(location, -1, -1);
            Main.PlaySound(SoundID.MenuClose);
            NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, Main.myPlayer, location.X, location.Y, -1);
        }

        internal static void RenderOverlayingUI(ref string mouseText)
        {
            var player = Main.LocalPlayer;
            var upgrades = player.GetModPlayer<PlayerMapUpgrades>();

            List<MapLayerToggle> mapLayerToggles = new List<MapLayerToggle>();

            if (PlayerMapUpgrades.HasMapUpgrade(upgrades.CosmicTelescope))
            {
                mapLayerToggles.Add(new MapLayerToggle(ModContent.GetTexture(TexturePaths.MapUIToggles + "cosmictelescope"), "Mods.AQMod.MapLayerToggle.CosmicTelescope",
                    () => PlayerMapUpgrades.MapUpgradeVisible(upgrades.CosmicTelescope), () => PlayerMapUpgrades.Visibility(ref upgrades.CosmicTelescope)));
            }

            if (PlayerMapUpgrades.HasMapUpgrade(upgrades.VialOfBlood))
            {
                mapLayerToggles.Add(new MapLayerToggle(ModContent.GetTexture(TexturePaths.MapUIToggles + "vialofblood"), "Mods.AQMod.MapLayerToggle.VialOfBlood",
                    () => PlayerMapUpgrades.MapUpgradeVisible(upgrades.VialOfBlood), () => PlayerMapUpgrades.Visibility(ref upgrades.VialOfBlood)));
            }

            if (PlayerMapUpgrades.HasMapUpgrade(upgrades.Beeswax))
            {
                mapLayerToggles.Add(new MapLayerToggle(ModContent.GetTexture(TexturePaths.MapUIToggles + "beeswax"), "Mods.AQMod.MapLayerToggle.Beeswax",
                    () => PlayerMapUpgrades.MapUpgradeVisible(upgrades.Beeswax), () => PlayerMapUpgrades.Visibility(ref upgrades.Beeswax)));
            }

            if (PlayerMapUpgrades.HasMapUpgrade(upgrades.Cabbage))
            {
                mapLayerToggles.Add(new MapLayerToggle(ModContent.GetTexture(TexturePaths.MapUIToggles + "cabbage"), "Mods.AQMod.MapLayerToggle.Cabbage",
                    () => PlayerMapUpgrades.MapUpgradeVisible(upgrades.Cabbage), () => PlayerMapUpgrades.Visibility(ref upgrades.Cabbage)));
            }

            if (PlayerMapUpgrades.HasMapUpgrade(upgrades.BlightedSoul))
            {
                mapLayerToggles.Add(new MapLayerToggle(ModContent.GetTexture(TexturePaths.MapUIToggles + "blightedsoul"), "Mods.AQMod.MapLayerToggle.BlightedSoul",
                    () => PlayerMapUpgrades.MapUpgradeVisible(upgrades.BlightedSoul), () => PlayerMapUpgrades.Visibility(ref upgrades.BlightedSoul)));
            }

            if (AddMapLayerToggles != null)
            {
                AddMapLayerToggles(player, mapLayerToggles);
            }

            if (mapLayerToggles.Count > 0)
            {
                int buffIconSeparation = 26 * 2;
                int slotsUntilWrap = 4;
                int width = 12 + Math.Min(mapLayerToggles.Count, slotsUntilWrap) * buffIconSeparation;
                var uiConfig = ModContent.GetInstance<UIConfiguration>();
                int x = (int)uiConfig.MapUITogglesPosition.X;
                //x = Main.mouseX; For testing
                if (x + width > Main.screenWidth - 60)
                {
                    x = Main.screenWidth - 60 - width;
                }
                int y = (int)uiConfig.MapUITogglesPosition.Y;

                int height = 64;
                if (mapLayerToggles.Count > slotsUntilWrap)
                {
                    height += buffIconSeparation * ((mapLayerToggles.Count - 1) / slotsUntilWrap);
                }
                if (y + height > Main.screenHeight - 20)
                {
                    y = Main.screenHeight - 20 - height;
                }

                Utils.DrawInvBG(Main.spriteBatch, new Rectangle(x, y, width, height), new Color(45, 51, 111, 255) * _mapTogglesAlpha);

                if (new Rectangle(x, y, width, height).Contains(Main.mouseX, Main.mouseY))
                {
                    Main.LocalPlayer.mouseInterface = true;
                    if (_mapTogglesAlpha < 1f)
                    {
                        _mapTogglesAlpha += 0.1f;
                        if (_mapTogglesAlpha > 1f)
                        {
                            _mapTogglesAlpha = 1f;
                        }
                    }
                }
                else
                {
                    if (_mapTogglesAlpha > 0.33f)
                    {
                        _mapTogglesAlpha -= 0.02f;
                        if (_mapTogglesAlpha < 0.33f)
                        {
                            _mapTogglesAlpha = 0.33f;
                        }
                    }
                }
                byte b = (byte)(int)(255f * _mapTogglesAlpha);
                for (int i = 0; i < mapLayerToggles.Count; i++)
                {
                    byte clr = b;
                    if (!mapLayerToggles[i].IsActive())
                    {
                        clr /= 2;
                    }
                    byte a = b;
                    int toggleX = x + 10 + i % slotsUntilWrap * buffIconSeparation;
                    int toggleY = y + 10 + buffIconSeparation * (i / slotsUntilWrap);

                    var destinationRectangle = new Rectangle(toggleX, toggleY, buffIconSeparation - 8, buffIconSeparation - 8);

                    bool hovering = destinationRectangle.Contains(Main.mouseX, Main.mouseY);
                    if (hovering)
                    {
                        mouseText = mapLayerToggles[i].HoverText();
                        if (Main.mouseLeft && Main.mouseLeftRelease)
                        {
                            Main.mouseLeftRelease = false;
                            mapLayerToggles[i].Toggle();
                            if (mapLayerToggles[i].IsActive())
                            {
                                Main.PlaySound(SoundID.MenuOpen);
                            }
                            else
                            {
                                Main.PlaySound(SoundID.MenuClose);
                            }
                        }
                        if (_mapTogglesAlpha < 1f)
                        {
                            _mapTogglesAlpha = 1f;
                            a = 255;
                            clr = 255;
                        }
                        Main.spriteBatch.Draw(mapLayerToggles[i].Texture, new Rectangle(destinationRectangle.X + 2, destinationRectangle.Y, destinationRectangle.Width, destinationRectangle.Height), new Color(0, 0, 0, a));
                        Main.spriteBatch.Draw(mapLayerToggles[i].Texture, new Rectangle(destinationRectangle.X - 2, destinationRectangle.Y, destinationRectangle.Width, destinationRectangle.Height), new Color(0, 0, 0, a));
                        Main.spriteBatch.Draw(mapLayerToggles[i].Texture, new Rectangle(destinationRectangle.X, destinationRectangle.Y + 2, destinationRectangle.Width, destinationRectangle.Height), new Color(0, 0, 0, a));
                        Main.spriteBatch.Draw(mapLayerToggles[i].Texture, new Rectangle(destinationRectangle.X, destinationRectangle.Y - 2, destinationRectangle.Width, destinationRectangle.Height), new Color(0, 0, 0, a));
                    }

                    Main.spriteBatch.Draw(mapLayerToggles[i].Texture, destinationRectangle, new Color(clr, clr, clr, a));
                    if (hovering)
                        Main.spriteBatch.Draw(mapLayerToggles[i].Texture, destinationRectangle, new Color(clr, clr, clr, 0));
                }
            }
        }
    }
}