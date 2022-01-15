using AQMod.Common.WorldGeneration;
using AQMod.Content.Players;
using AQMod.Content.World.Events.GlimmerEvent;
using AQMod.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Content
{
    public static class MapInterface
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

        public struct MapLayerToggle
        {
            private readonly Func<bool> isActive;
            private readonly Action<Player> toggle;
            public string HoverKey;
            public readonly Texture2D Texture;

            public MapLayerToggle(Texture2D texture, string key, Func<bool> isActive, Action<Player> toggle)
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

            public void Toggle(Player player)
            {
                toggle(player);
            }

            public string HoverText()
            {
                return Language.GetTextValue(HoverKey);
            }
        }

        private static Vector2 _map;
        private static float _mapScale;

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

            Main.NewText(upgrades.VialOfBlood.ToString());

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
                DrawMapIcon(out bool hovering, icon, Main.dungeonX + 0.5f, Main.dungeonY - 2.5f, interactable: true, Main.dungeonX > Main.maxTilesX / 2 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
                if (hovering)
                {
                    if (Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        TeleportPlayer(new Vector2(Main.dungeonX + 8f, Main.dungeonY - 16f));
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
                            TeleportPlayer(altarLocation.ToWorldCoordinates(new Vector2(24f, 8f)));
                        }
                        mouseText = Language.GetTextValue("ItemName.LihzahrdAltar");
                    }
                }
            }

            if (PlayerMapUpgrades.MapUpgradeVisible(upgrades.CosmicTelescope) && GlimmerEvent.IsGlimmerEventCurrentlyActive())
            {
                var texture = ModContent.GetTexture("AQMod/Assets/UI/map_ultimatesword");
                var frame = new Rectangle(0, 0, texture.Width, texture.Height);
                var drawPos = MapDrawPosition(new Vector2(GlimmerEvent.tileX + 0.5f, GlimmerEvent.tileY - 3f));
                var hitbox = Utils.CenteredRectangle(drawPos, new Vector2(texture.Width, texture.Height) * Main.UIScale);
                var scale = Main.UIScale;
                if (hitbox.Contains(Main.mouseX, Main.mouseY))
                {
                    mouseText = Language.GetTextValue("Mods.AQMod.EventName.GlimmerEvent");
                    scale += 0.5f;
                }
                Main.spriteBatch.Draw(texture, drawPos, frame, new Color(255, 255, 255, 255), 0f, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
                texture = ModContent.GetTexture("AQMod/Assets/UI/map_cosmictelescope");
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
                var texture = ModContent.GetTexture("AQMod/Assets/UI/map_enemyblip");
                int frameHeight = texture.Height / 2;
                int frameNumber = (int)(Main.GameUpdateCount % 24 / 12);
                var frame = new Rectangle(0, frameHeight * frameNumber, texture.Width, frameHeight);
                Vector2 origin = frameNumber == 1 ? new Vector2(3f, 5f) : new Vector2(4f, 4f);
                var scale = Main.UIScale * 4f;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && !AQNPC.Sets.NoMapBlip[Main.npc[i].type] && Main.npc[i].GetBossHeadTextureIndex() == -1 && !Main.npc[i].townNPC)
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
        }

        private static void DrawMapIcon(byte frame, float x, float y, bool teleportable = false)
        {
            DrawMapIcon(out bool _, frame, x, y, teleportable);
        }

        private static void DrawMapIcon(out bool hovering, byte frame, float x, float y, bool interactable = false, SpriteEffects effects = SpriteEffects.None)
        {
            var mapIcon = ModContent.GetTexture("AQMod/Assets/UI/map_structures");
            var spriteFrame = new Rectangle((SpriteWidth + SpritePadding) * frame, 0, SpriteWidth, SpriteHeight);
            var drawPos = MapDrawPosition(new Vector2(x, y));
            var hitbox = Utils.CenteredRectangle(drawPos, new Vector2(spriteFrame.Width, spriteFrame.Height) * Main.UIScale);
            var scale = Main.UIScale;
            hovering = hitbox.Contains(Main.mouseX, Main.mouseY);
            if (hovering && interactable)
            {
                scale *= 2f;
            }
            Main.spriteBatch.Draw(mapIcon, drawPos, spriteFrame, new Color(255, 255, 255, 255), 0f, SpriteOrigin, scale, SpriteEffects.None, 0f);
        }

        private static void TeleportPlayer(Vector2 location)
        {
            Main.mouseLeftRelease = false;
            Main.mapFullscreen = false;
            Main.player[Main.myPlayer].Teleport(location, -1, -1);
            Main.PlaySound(SoundID.MenuClose);
            NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, Main.myPlayer, location.X, location.Y, -1);
        }

        internal static void RenderOverlayingUI()
        {
            var player = Main.LocalPlayer;
            var upgrades = player.GetModPlayer<PlayerMapUpgrades>();
        }
    }
}