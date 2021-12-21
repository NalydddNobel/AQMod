using AQMod.Assets;
using AQMod.Assets.LegacyItemOverlays;
using AQMod.Buffs.Debuffs.Temperature;
using AQMod.Common;
using AQMod.Common.CrossMod;
using AQMod.Common.DeveloperTools;
using AQMod.Common.Graphics;
using AQMod.Common.Graphics.PlayerEquips;
using AQMod.Common.Graphics.SceneLayers;
using AQMod.Common.NetCode;
using AQMod.Common.Skies;
using AQMod.Common.UserInterface;
using AQMod.Content;
using AQMod.Content.CursorDyes;
using AQMod.Content.MapMarkers;
using AQMod.Content.NameTags;
using AQMod.Content.Quest.Lobster;
using AQMod.Content.WorldEvents;
using AQMod.Content.WorldEvents.CrabSeason;
using AQMod.Content.WorldEvents.DemonSiege;
using AQMod.Content.WorldEvents.GaleStreams;
using AQMod.Content.WorldEvents.GlimmerEvent;
using AQMod.Content.WorldEvents.ProgressBars;
using AQMod.Effects.Dyes;
using AQMod.Effects.ScreenEffects;
using AQMod.Effects.Trails;
using AQMod.Effects.WorldEffects;
using AQMod.Items;
using AQMod.Items.Tools.Fishing.Bait;
using AQMod.Items.Vanities;
using AQMod.Localization;
using AQMod.NPCs;
using AQMod.NPCs.Boss.Crabson;
using AQMod.NPCs.Boss.Starite;
using AQMod.Sounds;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.GameInput;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.Utilities;

namespace AQMod
{
    public class AQMod : Mod
    {
        /// <summary>
        /// Gets an instance of the mod. This is the same as calling <see cref="ModContent.GetInstance{T}"/> and passing <see cref="AQMod"/> as T. Except it's slightly cooler?
        /// </summary>
        public static AQMod Instance => ModContent.GetInstance<AQMod>();
        public const int SpaceLayerTile = 200;
        public const int SpaceLayer = SpaceLayerTile * 16;
        /// <summary>
        /// Basically guesses if the game is still active, should only really use for drawing methods that do things like summon dust
        /// </summary>
        public static bool GameWorldActive => Main.instance.IsActive && !Main.gamePaused;
        public static bool CanUseAssets => !Loading && Main.netMode != NetmodeID.Server;
        /// <summary>
        /// If WoF or Omega Starite have been defeated
        /// </summary>
        public static bool SudoHardmode => Main.hardMode || WorldDefeats.DownedStarite;
        /// <summary>
        /// Gets the center of the screen's draw coordinates
        /// </summary>
        internal static Vector2 ScreenCenter => new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f);
        /// <summary>
        /// Gets the center of the screen's world coordinates
        /// </summary>
        internal static Vector2 WorldScreenCenter => new Vector2(Main.screenPosition.X + (Main.screenWidth / 2f), Main.screenPosition.Y + Main.screenHeight / 2f);
        /// <summary>
        /// The world view point matrix
        /// </summary>
        internal static Matrix WorldViewPoint
        {
            get
            {
                GraphicsDevice graphics = Main.graphics.GraphicsDevice;
                Vector2 zoom = Main.GameViewMatrix.Zoom;
                int width = graphics.Viewport.Width;
                int height = graphics.Viewport.Height;
                Matrix Zoom = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) * Matrix.CreateTranslation(width / 2, height / -2, 0) * Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(zoom.X, zoom.Y, 1f);
                Matrix Projection = Matrix.CreateOrthographic(width, height, 0, 1000);
                return Zoom * Projection;
            }
        }
        /// <summary>
        /// The zero for drawing tiles correctly
        /// </summary>
        internal static Vector2 TileZero => Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
        /// <summary>
        /// The color used for most menacing messages like "The Twins has awoken!".
        /// </summary>
        internal static Color BossMessage => new Color(175, 75, 255, 255);
        /// <summary>
        /// The color used for most event messages like "A meteorite has landed!".
        /// </summary>
        internal static Color EventMessage => new Color(50, 255, 130, 255);
        internal static string DebugFolderPath => Main.SavePath + Path.DirectorySeparatorChar + "Mods" + Path.DirectorySeparatorChar + "Cache" + Path.DirectorySeparatorChar + "AQMod";

        public static bool spawnStarite;
        public static int dayrateIncrease;

        /// <summary>
        /// This is normally used to prevent adding new content to arrays after the mod has loaded and blah blah. It's also used to prevent some drawing which might happen on the title screen before assets fully load.
        /// </summary>
        internal static bool Loading { get; private set; }
        /// <summary>
        /// This is normally used to prevent threaded assets from loading
        /// </summary>
        internal static bool Unloading { get; private set; }
        public static float StariteBGMult { get; private set; }
        public static bool EvilProgressionLock { get; private set; }
        public static bool Screenshakes { get; private set; }
        public static bool TonsofScreenShakes { get; private set; }
        /// <summary>
        /// Whether or not the background starites from the Glimmer Event should be shown. Default value is true
        /// </summary>
        public static bool ShowBackgroundStarites { get; private set; }
        public static Color MapBlipColor { get; private set; }
        public static Color StariteProjectileColor { get; private set; }
        public static Color StariteAuraColor { get; private set; }
        /// <summary>
        /// The active instance of the Cursor Dyes Loader.
        /// </summary>
        public static CursorDyeLoader CursorDyes { get; private set; }
        /// <summary>
        /// The active instance of the Robster Hunts Loader.
        /// </summary>
        public static RobsterHuntLoader RobsterHunts { get; private set; }
        public static MapMarkerManager MapMarkers { get; private set; }

        private static List<CachedTask> cachedLoadTasks;
        /// <summary>
        /// The active instance of World Layers
        /// </summary>
        public static SceneLayersManager WorldLayers { get; private set; }
        [Obsolete("Replaced with interfaces")]
        /// <summary>
        /// The active instance of Item Overlays, this is not initialized on the server
        /// </summary>
        public static DrawOverlayLoader<ItemOverlayData> ItemOverlays { get; private set; }
        /// <summary>
        /// The active instance of Armor Overlays, this is not initialized on the server
        /// </summary>
        public static EquipOverlayLoader ArmorOverlays { get; private set; }
        /// <summary>
        /// The active list of World Effects, this is not initialized on the server
        /// </summary>
        public static List<WorldVisualEffect> WorldEffects { get; private set; }
        public static ModifiableMusic CrabsonMusic { get; private set; }
        public static ModifiableMusic GlimmerEventMusic { get; private set; }
        public static ModifiableMusic OmegaStariteMusic { get; private set; }
        public static ModifiableMusic DemonSiegeMusic { get; private set; }
        public static ModifiableMusic GaleStreamsMusic { get; private set; }

        internal static CrossModType Split { get; private set; }

        private static Vector2 _lastScreenZoom;
        private static Vector2 _lastScreenView;

        public static bool RerollCursor;

        internal static class Debug
        {
            public static bool LogAutoload = false;
            public static bool LogDyeBinding = false;
            public static bool LogEffectLoading = false;
            public static bool LogNetcode = true;

            public struct DebugLogger
            {
                private readonly ILog _logger;

                public DebugLogger(ILog logger)
                {
                    _logger = logger;
                }

                public void Log(object message)
                {
                    _logger.Debug(message);
                }

                public void Log(object message, object arg0)
                {
                    _logger.Debug(string.Format(message.ToString(), arg0));
                }

                public void Log(object message, params object[] args)
                {
                    _logger.Debug(string.Format(message.ToString(), args));
                }

                public void Log(object message, Exception exception)
                {
                    _logger.Debug(message, exception);
                }
            }

            public static DebugLogger GetDebugLogger()
            {
                var logger = Instance.Logger;
                logger.Info("Accessed debug logger at: " + Environment.StackTrace);
                return new DebugLogger(logger);
            }
        }

        internal static class Autoloading
        {
            public static bool LoadingArmorSets;

            private static List<IAutoloadType> _autoloadCache;

            public static void Autoload(Assembly code)
            {
                _autoloadCache = new List<IAutoloadType>();
                if (Debug.LogAutoload)
                {
                    var logger = Debug.GetDebugLogger();
                    foreach (var t in code.GetTypes())
                    {
                        if (!t.IsAbstract && t.GetInterfaces().Contains(typeof(IAutoloadType)))
                        {
                            var instance = (IAutoloadType)Activator.CreateInstance(t);
                            instance.OnLoad();
                            logger.Log("Created autoload instance of: {0}", t.FullName);
                            _autoloadCache.Add(instance);
                        }
                    }
                }
                else
                {
                    foreach (var t in code.GetTypes())
                    {
                        if (!t.IsAbstract && t.GetInterfaces().Contains(typeof(IAutoloadType)))
                        {
                            var instance = (IAutoloadType)Activator.CreateInstance(t);
                            instance.OnLoad();
                            _autoloadCache.Add(instance);
                        }
                    }
                }
            }

            public static void SetupContent(Assembly code)
            {
                if (Debug.LogAutoload)
                {
                    var logger = Debug.GetDebugLogger();
                    foreach (var t in code.GetTypes())
                    {
                        if (!t.IsAbstract && t.GetInterfaces().Contains(typeof(ISetupContentType)))
                        {
                            var instance = (ISetupContentType)Activator.CreateInstance(t);
                            logger.Log("Created autoload instance of: {0}", t.FullName);
                            instance.SetupContent();
                        }
                    }
                }
                else
                {
                    foreach (var t in code.GetTypes())
                    {
                        if (!t.IsAbstract && t.GetInterfaces().Contains(typeof(ISetupContentType)))
                        {
                            var instance = (ISetupContentType)Activator.CreateInstance(t);
                            instance.SetupContent();
                        }
                    }
                }
            }

            public static void Unload()
            {
                if (_autoloadCache == null)
                    return;
                foreach (var autoload in _autoloadCache)
                {
                    autoload.Unload();
                }
            }
        }

        public static class Keys
        {
            public static ModHotKey CosmicanonToggle { get; private set; }
            public static ModHotKey EquivalenceMachineToggle { get; private set; }

            internal static void Load(AQMod mod)
            {
                CosmicanonToggle = mod.RegisterHotKey("Cosmicanon Toggle", "P");
                EquivalenceMachineToggle = mod.RegisterHotKey("Equivalence Machine Toggle", "O");
            }

            internal static void Unload()
            {
                EquivalenceMachineToggle = null;
                CosmicanonToggle = null;
            }
        }

        public static class Edits
        {
            internal static bool PreventChat { get; private set; }
            internal static bool PreventChatOnce { get; private set; }
            internal static bool CosmicanonActive { get; private set; }
            internal static bool UpdatingWorld { get; private set; }

            internal static void Load()
            {
                On.Terraria.UI.ItemSlot.OverrideHover += ItemSlot_OverrideHover;
                On.Terraria.Item.UpdateItem += Item_UpdateItem;
                On.Terraria.NetMessage.BroadcastChatMessage += NetMessage_BroadcastChatMessage;
                On.Terraria.GameContent.Achievements.AchievementsHelper.NotifyProgressionEvent += AchievementsHelper_NotifyProgressionEvent;
                On.Terraria.Chest.SetupShop += Chest_SetupShop;
                On.Terraria.Projectile.NewProjectile_float_float_float_float_int_int_float_int_float_float += Projectile_NewProjectile_float_float_float_float_int_int_float_int_float_float;
                On.Terraria.NPC.Collision_DecideFallThroughPlatforms += NPC_Collision_DecideFallThroughPlatforms;
                On.Terraria.Main.UpdateTime += Main_UpdateTime;
                On.Terraria.Main.UpdateSundial += Main_UpdateSundial;
                On.Terraria.Main.UpdateWeather += Main_UpdateWeather;
                On.Terraria.Main.UpdateDisplaySettings += Main_UpdateDisplaySettings;
                On.Terraria.Main.NewText_string_byte_byte_byte_bool += Main_NewText_string_byte_byte_byte_bool;
                On.Terraria.Main.DrawTiles += Main_DrawTiles;
                On.Terraria.Main.DrawPlayers += Main_DrawPlayers;
                On.Terraria.Main.CursorColor += Main_CursorColor;
                On.Terraria.Main.DrawCursor += Main_DrawCursor;
                On.Terraria.Main.DrawThickCursor += Main_DrawThickCursor;
                On.Terraria.Main.DrawInterface_36_Cursor += Main_DrawInterface_36_Cursor;
                On.Terraria.Player.FishingLevel += GetFishingLevel;
                On.Terraria.Player.AddBuff += Player_AddBuff;
                On.Terraria.Player.QuickBuff += Player_QuickBuff;
                On.Terraria.Player.PickTile += Player_PickTile;
                On.Terraria.Player.HorizontalMovement += Player_HorizontalMovement;
            }

            internal static Color _oldCursorColor;
            internal static Color _newCursorColor;
            public static bool OverrideColor { get; internal set; }

            internal static bool ShouldApplyCustomCursor()
            {
                return !AQMod.Loading && !Main.gameMenu && Main.myPlayer >= 0 && Main.LocalPlayer.active;
            }

            // ON edits
            private static Vector2 Main_DrawThickCursor(On.Terraria.Main.orig_DrawThickCursor orig, bool smart)
            {
                if (ShouldApplyCustomCursor())
                {
                    var type = Main.LocalPlayer.GetModPlayer<AQPlayer>().CursorDyeID;
                    if (type != CursorDyeLoader.ID.None)
                    {
                        var value = AQMod.CursorDyes.GetContent(type).DrawThickCursor(smart);
                        if (value != null)
                            return value.Value;
                    }
                }
                return orig(smart);
            }

            private static void Main_DrawCursor(On.Terraria.Main.orig_DrawCursor orig, Vector2 bonus, bool smart)
            {
                if (ShouldApplyCustomCursor() && !PlayerInput.UsingGamepad)
                {
                    var player = Main.LocalPlayer;
                    var drawingPlayer = player.GetModPlayer<AQPlayer>();
                    if (drawingPlayer.CursorDyeID != CursorDyeLoader.ID.None)
                    {
                        var cursorDye = CursorDyes.GetContent(drawingPlayer.CursorDyeID);
                        if (!cursorDye.PreDrawCursor(player, drawingPlayer, bonus, smart))
                            orig(bonus, smart);
                        cursorDye.PostDrawCursor(player, drawingPlayer, bonus, smart);
                    }
                    else
                    {
                        orig(bonus, smart);
                    }
                }
                else
                {
                    orig(bonus, smart);
                }
            }

            private static void Main_DrawInterface_36_Cursor(On.Terraria.Main.orig_DrawInterface_36_Cursor orig)
            {
                if (ShouldApplyCustomCursor())
                {
                    var player = Main.LocalPlayer;
                    var aQPlayer = player.GetModPlayer<AQPlayer>();
                    if (aQPlayer.CursorDyeID != CursorDyeLoader.ID.None)
                    {
                        var cursorDye = CursorDyes.GetContent(aQPlayer.CursorDyeID);
                        if (!cursorDye.PreDrawCursorOverrides(player, aQPlayer))
                        {
                            if (RerollCursor)
                            {
                                Main.spriteBatch.Draw(ModContent.GetTexture("AQMod/Assets/Cursors/Cursor_Reroll"), new Vector2(Main.mouseX, Main.mouseY), null, Color.White, 0f, default(Vector2), Main.cursorScale, SpriteEffects.None, 0f);
                            }
                            else
                            {
                                orig();
                            }
                        }
                        cursorDye.PostDrawCursorOverrides(player, aQPlayer);
                    }
                    else
                    {
                        if (RerollCursor)
                        {
                            Main.spriteBatch.Draw(ModContent.GetTexture("AQMod/Assets/Cursors/Cursor_Reroll"), new Vector2(Main.mouseX, Main.mouseY), null, Color.White, 0f, default(Vector2), Main.cursorScale, SpriteEffects.None, 0f);
                        }
                        else
                        {
                            orig();
                        }
                    }
                }
                else
                {
                    orig();
                }
                RerollCursor = false;
            }

            private static void Main_CursorColor(On.Terraria.Main.orig_CursorColor orig)
            {
                if (ShouldApplyCustomCursor() && OverrideColor)
                {
                    _oldCursorColor = Main.mouseColor;
                    Main.mouseColor = _newCursorColor;
                    orig();
                    Main.mouseColor = _oldCursorColor;
                    _newCursorColor = Main.mouseColor;
                }
                else
                {
                    orig();
                }
            }

            private static void ItemSlot_OverrideHover(On.Terraria.UI.ItemSlot.orig_OverrideHover orig, Item[] inv, int context, int slot)
            {
                if (inv[slot].type > Main.maxItemTypes && inv[slot].stack > 0 && inv[slot].modItem is Items.IInventoryHover hover)
                {
                    if (hover.CursorHover(inv, context, slot))
                    {
                        return;
                    }
                }
                orig(inv, context, slot);
            }

            private static void Item_UpdateItem(On.Terraria.Item.orig_UpdateItem orig, Item self, int i)
            {
                if (Main.itemLockoutTime[i] > 0)
                {
                    orig(self, i);
                    return;
                }

                EquivalenceMachineManager.UpdateItems();
                orig(self, i);
            }

            private static void AchievementsHelper_NotifyProgressionEvent(On.Terraria.GameContent.Achievements.AchievementsHelper.orig_NotifyProgressionEvent orig, int eventID)
            {
                if (UpdatingWorld && CosmicanonActive && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (eventID == AchievementHelperID.Events.BloodMoonStart)
                    {
                        Main.bloodMoon = false;
                        CosmicanonCounts.BloodMoonsPrevented++;
                        NetHelper.PreventedBloodMoon();
                        PreventChatOnce = true;
                    }
                    if (eventID == AchievementHelperID.Events.EclipseStart)
                    {
                        Main.eclipse = false;
                        CosmicanonCounts.EclipsesPrevented++;
                        NetHelper.PreventedEclipse();
                        PreventChatOnce = true;
                    }
                }
                orig(eventID);
            }

            private static void NetMessage_BroadcastChatMessage(On.Terraria.NetMessage.orig_BroadcastChatMessage orig, NetworkText text, Color color, int excludedPlayer)
            {
                if (PreventChatOnce)
                {
                    PreventChatOnce = false;
                    return;
                }
                if (PreventChat)
                {
                    return;
                }
                orig(text, color, excludedPlayer);
            }

            private static void Main_NewText_string_byte_byte_byte_bool(On.Terraria.Main.orig_NewText_string_byte_byte_byte_bool orig, string newText, byte R, byte G, byte B, bool force)
            {
                if (PreventChatOnce)
                {
                    PreventChatOnce = false;
                    return;
                }
                if (PreventChat)
                {
                    return;
                }
                orig(newText, R, G, B, force);
            }

            private static void Main_UpdateWeather(On.Terraria.Main.orig_UpdateWeather orig, Main self, GameTime gameTime)
            {
                if (GaleStreams.EndEvent)
                {
                    Main.windSpeedSet += -Math.Sign(Main.windSpeedSet) / 100f;
                    if (Main.windSpeedSet.Abs() < 0.1f)
                    {
                        GaleStreams.EndEvent = false;
                    }
                    Main.windSpeedTemp = Main.windSpeedSet;
                    return;
                }
                if (Main.netMode != NetmodeID.MultiplayerClient && (Main.netMode == NetmodeID.Server || !Main.gameMenu)
                    && GaleStreams.IsActive)
                {
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        if (Main.player[i].active && !Main.player[i].dead && GaleStreams.EventActive(Main.player[i]))
                        {
                            Main.cloudLimit = 200; // prevents the wind speed from naturally changing during the Gale Streams event
                            if (Main.windSpeed < Main.windSpeedSet)
                            {
                                Main.windSpeed += 0.001f * Main.dayRate;
                                if (Main.windSpeed > Main.windSpeedSet)
                                {
                                    Main.windSpeed = Main.windSpeedSet;
                                }
                            }
                            else if (Main.windSpeed > Main.windSpeedSet)
                            {
                                Main.windSpeed -= 0.001f * (float)Main.dayRate;
                                if (Main.windSpeed < Main.windSpeedSet)
                                {
                                    Main.windSpeed = Main.windSpeedSet;
                                }
                            }
                            Main.weatherCounter -= Main.dayRate;
                            if (Main.weatherCounter <= 0)
                            {
                                Main.weatherCounter = Main.rand.Next(3600, 18000);
                                if (Main.netMode == NetmodeID.Server)
                                {
                                    NetMessage.SendData(MessageID.WorldData);
                                }
                            }
                            return;
                        }
                    }
                }
                if (Main.windSpeedSet.Abs() > 1f)
                {
                    Main.windSpeedSet += -Math.Sign(Main.windSpeedSet) / 100f;
                    Main.windSpeedTemp = Main.windSpeedSet;
                }
                orig(self, gameTime);
            }

            private static void Player_HorizontalMovement(On.Terraria.Player.orig_HorizontalMovement orig, Player self)
            {
                orig(self);
                var aQPlayer = self.GetModPlayer<AQPlayer>();
                if (aQPlayer.redSpriteWind != 0 && !(self.mount.Active && self.velocity.Y == 0f && (self.controlLeft || self.controlRight)))
                {
                    float windDirection = Math.Sign(aQPlayer.redSpriteWind) * 0.07f;
                    if (Math.Abs(Main.windSpeed) > 0.5f)
                    {
                        windDirection *= 1.37f;
                    }
                    if (self.velocity.Y != 0f)
                    {
                        windDirection *= 1.5f;
                    }
                    if (self.controlLeft || self.controlRight)
                    {
                        windDirection *= 0.8f;
                    }
                    if (Math.Sign(self.direction) != Math.Sign(windDirection))
                    {
                        self.accRunSpeed -= Math.Abs(windDirection) * 20f;
                        self.maxRunSpeed -= Math.Abs(windDirection) * 20f;
                    }
                    if (windDirection < 0f && self.velocity.X > windDirection)
                    {
                        self.velocity.X += windDirection;
                        if (self.velocity.X < windDirection)
                        {
                            self.velocity.X = windDirection;
                        }
                    }
                    if (windDirection > 0f && self.velocity.X < windDirection)
                    {
                        self.velocity.X += windDirection;
                        if (self.velocity.X > windDirection)
                        {
                            self.velocity.X = windDirection;
                        }
                    }

                    if (!self.controlLeft && !self.controlRight)
                    {
                        self.legFrameCounter = -1.0;
                        self.legFrame.Y = 0;
                    }
                }
                aQPlayer.redSpriteWind = 0;
            }

            private static void Player_PickTile(On.Terraria.Player.orig_PickTile orig, Player self, int x, int y, int pickPower)
            {
                if (self.GetModPlayer<AQPlayer>().pickBreak)
                {
                    pickPower /= 2;
                }
                orig(self, x, y, pickPower);
            }

            private static void Main_UpdateDisplaySettings(On.Terraria.Main.orig_UpdateDisplaySettings orig, Main self)
            {
                orig(self);
                if (!Main.gameMenu && Main.graphics.GraphicsDevice != null && Main.spriteBatch != null)
                {
                    HotAndColdCurrentLayer.DrawTarget();
                }
            }

            private static int Projectile_NewProjectile_float_float_float_float_int_int_float_int_float_float(On.Terraria.Projectile.orig_NewProjectile_float_float_float_float_int_int_float_int_float_float orig, float X, float Y, float SpeedX, float SpeedY, int Type, int Damage, float KnockBack, int Owner, float ai0, float ai1)
            {
                int originalValue = orig(X, Y, SpeedX, SpeedY, Type, Damage, KnockBack, Owner, ai0, ai1);
                var projectile = Main.projectile[originalValue];
                if (projectile.coldDamage || (projectile.friendly && projectile.owner != 255 && Main.player[projectile.owner].frostArmor && (projectile.melee || projectile.ranged)))
                {
                    var aQProj = projectile.GetGlobalProjectile<AQProjectile>();
                    aQProj.canHeat = false;
                    aQProj.temperature = -15;
                }
                return originalValue;
            }

            private static void Player_QuickBuff(On.Terraria.Player.orig_QuickBuff orig, Player self)
            {
                AQPlayer.IsQuickBuffing = true;
                orig(self);
                AQPlayer.IsQuickBuffing = false;
            }

            private static void Player_AddBuff(On.Terraria.Player.orig_AddBuff orig, Player self, int type, int time1, bool quiet)
            {
                if (type >= Main.maxBuffTypes)
                {
                    var modBuff = ModContent.GetModBuff(type);
                    if (modBuff is temperatureDebuff)
                    {
                        for (int i = 0; i < Player.MaxBuffs; i++)
                        {
                            if (self.buffTime[i] > 0)
                            {
                                if (self.buffType[i] == type)
                                {
                                    orig(self, type, time1, quiet);
                                    return;
                                }
                                if (self.buffType[i] > Main.maxBuffTypes)
                                {
                                    var otherModBuff = ModContent.GetModBuff(self.buffType[i]);
                                    if (otherModBuff is temperatureDebuff)
                                    {
                                        self.DelBuff(i);
                                        orig(self, type, time1, quiet);
                                        return;
                                    }
                                }
                            }
                        }
                        orig(self, type, time1, quiet);
                        return;
                    }
                }
                if (AQBuff.Sets.IsFoodBuff[type])
                {
                    for (int i = 0; i < Player.MaxBuffs; i++)
                    {
                        if (self.buffTime[i] > 16 && self.buffType[i] != type && AQBuff.Sets.IsFoodBuff[self.buffType[i]])
                        {
                            self.DelBuff(i);
                            i--;
                        }
                    }
                }
                orig(self, type, time1, quiet);
            }

            private static int GetFishingLevel(On.Terraria.Player.orig_FishingLevel orig, Player player)
            {
                int regularLevel = orig(player);
                if (regularLevel <= 0)
                    return regularLevel;
                var aQPlayer = player.GetModPlayer<AQPlayer>();
                Item baitItem = null;
                for (int j = 0; j < 58; j++)
                {
                    if (player.inventory[j].stack > 0 && player.inventory[j].bait > 0)
                    {
                        baitItem = player.inventory[j];
                        break;
                    }
                }
                if (baitItem.modItem is PopperBaitItem popper)
                {
                    int popperPower = popper.GetExtraFishingPower(player, aQPlayer);
                    if (popperPower > 0)
                    {
                        aQPlayer.PopperType = baitItem.type;
                        aQPlayer.PopperBaitPower = popperPower;
                    }
                    else
                    {
                        aQPlayer.PopperType = 0;
                        aQPlayer.PopperBaitPower = 0;
                    }
                }
                else
                {
                    aQPlayer.PopperType = 0;
                    aQPlayer.PopperBaitPower = 0;
                }
                aQPlayer.FishingPowerCache = regularLevel + aQPlayer.PopperBaitPower;
                return aQPlayer.FishingPowerCache;
            }

            private static void Main_DrawPlayers(On.Terraria.Main.orig_DrawPlayers orig, Main self)
            {
                orig(self);
                BatcherMethods.GeneralEntities.Begin(Main.spriteBatch);
                SceneLayersManager.DrawLayer(SceneLayering.PostDrawPlayers);
                Main.spriteBatch.End();
            }

            private static void Main_UpdateSundial(On.Terraria.Main.orig_UpdateSundial orig)
            {
                orig();
                Main.dayRate += dayrateIncrease;
            }

            private static void Main_UpdateTime(On.Terraria.Main.orig_UpdateTime orig)
            {
                UpdatingWorld = true;
                Main.dayRate += dayrateIncrease;
                if (Main.dayTime)
                {
                    if (Main.time + Main.dayRate > Main.dayLength)
                    {
                        CosmicanonActive = AQPlayer.IgnoreMoons();
                        AprilFoolsJoke.UpdateActive();
                        GlimmerEvent.OnTurnNight();
                        if (Main.netMode != NetmodeID.Server)
                        {
                            GlimmerEventSky.InitNight();
                        }
                    }
                    orig();
                    CosmicanonActive = false;
                }
                else
                {
                    if (Main.time + Main.dayRate > Main.nightLength)
                    {
                        CosmicanonActive = AQPlayer.IgnoreMoons();
                    }
                    orig();
                    CosmicanonActive = false;
                }
                dayrateIncrease = 0;
                PreventChat = false;
                PreventChatOnce = false;
                UpdatingWorld = false;
            }

            private static void Chest_SetupShop(On.Terraria.Chest.orig_SetupShop orig, Chest self, int type)
            {
                var plr = Main.LocalPlayer;
                bool discount = plr.discount;
                plr.discount = false;

                orig(self, type);

                plr.discount = discount;
                if (discount)
                {
                    float discountPercentage = plr.GetModPlayer<AQPlayer>().discountPercentage;
                    for (int i = 0; i < Chest.maxItems; i++)
                    {
                        if (self.item[i] != null && self.item[i].type != ItemID.None)
                            self.item[i].value = (int)(self.item[i].value * discountPercentage);
                    }
                }
            }

            private static bool NPC_Collision_DecideFallThroughPlatforms(On.Terraria.NPC.orig_Collision_DecideFallThroughPlatforms orig, NPC self) =>
                self.type > Main.maxNPCTypes &&
                self.modNPC is IDecideFallThroughPlatforms decideToFallThroughPlatforms ?
                decideToFallThroughPlatforms.Decide() : orig(self);

            private static void Main_DrawTiles(On.Terraria.Main.orig_DrawTiles orig, Main self, bool solidOnly, int waterStyleOverride)
            {
                if (!solidOnly)
                {
                    GoreNestLayer.RefreshCoords();
                }
                orig(self, solidOnly, waterStyleOverride);
            }
        }

        public AQMod()
        {
            Properties = new ModProperties()
            {
                Autoload = true,
                AutoloadBackgrounds = true,
                AutoloadGores = true,
                AutoloadSounds = true
            };
            cachedLoadTasks = new List<CachedTask>();
            Loading = true;
        }

        public override void Load()
        {
            Loading = true;
            Unloading = false;
            Keys.Load(this);
            Edits.Load();
            AQText.Load();
            ImitatedWindyDay.Reset(resetNonUpdatedStatics: true);
            CursorDyes = new CursorDyeLoader();
            CursorDyes.Setup(setupStatics: true);
            RobsterHunts = new RobsterHuntLoader();
            RobsterHunts.Setup(setupStatics: true);
            MapMarkers = new MapMarkerManager();
            MoonlightWallHelper.Instance = new MoonlightWallHelper();
            ModCallHelper.SetupCalls();
            AprilFoolsJoke.UpdateActive();
            var server = AQConfigServer.Instance;
            ApplyServerConfig(server);
            if (!Main.dedServ)
            {
                var client = AQConfigClient.Instance;
                AQSound.rand = new UnifiedRandom();
                ApplyClientConfig(client);
                ItemOverlays = new DrawOverlayLoader<ItemOverlayData>(Main.maxItems, () => ItemLoader.ItemCount);
                ArmorOverlays = new EquipOverlayLoader();
                AQTextures.Load();
                EffectCache.Load(this);
                CrabsonMusic = new ModifiableMusic(MusicID.Boss1);
                GlimmerEventMusic = new ModifiableMusic(MusicID.MartianMadness);
                OmegaStariteMusic = new ModifiableMusic(MusicID.Boss4);
                DemonSiegeMusic = new ModifiableMusic(MusicID.PumpkinMoon);
                GaleStreamsMusic = new ModifiableMusic(MusicID.Sandstorm);
                SkyManager.Instance[GlimmerEventSky.Name] = new GlimmerEventSky();
                GlimmerEventSky.ModLoad();
                Trailshader.Setup();
                WorldLayers = new SceneLayersManager();
                SceneLayersManager.Setup();
                ScreenShakeManager.Load();
                StarbyteColorCache.Init();
                WorldEffects = new List<WorldVisualEffect>();
            }
            Autoloading.Autoload(Code);
        }

        public override void PostSetupContent()
        {
            AQBuff.Sets.Setup();
            MapMarkers.Setup(setupStatics: true);
            if (!Main.dedServ)
            {
                DyeBinder.LoadDyes();
            }
            Autoloading.SetupContent(Code);
            invokeTasks();
            cachedLoadTasks.Clear();
        }

        public override void AddRecipeGroups()
        {
            AQNPC.Sets.LoadSets();
            AQProjectile.Sets.LoadSets();
            AQRecipes.RecipeGroups.Setup();
        }

        public override void AddRecipes()
        {
            invokeTasks();
            cachedLoadTasks = null;

            Split = new CrossModType("Split");
            Split.Load();

            Loading = false; // Sets Loading to false, so that some things no longer accept new content.
            RobsterHunts.SetupHunts();
            if (!Main.dedServ)
            {
                ItemOverlays.Finish();
            }

            CelesitalEightBall.ResetStatics();

            AQRecipes.AddRecipes(this);

            FargosQOLStuff.Setup(this);
            AQItem.Sets.Clones.Setup();
        }

        public override void Unload()
        {
            // outside of AQMod
            Loading = true;
            Unloading = true;
            cachedLoadTasks = null;
            EventProgressBarManager.Unload();
            Autoloading.Unload();
            HuntSystem.Unload();

            // in: AddRecipes()
            AQItem.Sets.Clones.Unload();
            CelesitalEightBall.ResetStatics();
            ItemOverlays = null;
            if (Split != null)
            {
                Split.Unload();
                Split = null;
            }

            // in: PostSetupContent()
            DyeBinder.Unload();
            MapMarkers = null;
            DemonSiege.Unload();
            AQProjectile.Sets.UnloadSets();
            AQNPC.Sets.UnloadSets();
            AQBuff.Sets.Unload();

            // in: Load()
            // v doesn't load on server v
            if (!Main.dedServ)
            {
                AQSound.rand = null;
                WorldEffects = null;
                StarbyteColorCache.Unload();
                ScreenShakeManager.Unload();
                ArmorOverlays = null;
                if (WorldLayers != null)
                {
                    SceneLayersManager.Unload();
                    WorldLayers = null;
                }
                EffectCache.ParentPixelShader = null;
                //EffectCache.Instance = null;
                GlimmerEventSky.Unload();
                GaleStreamsMusic = null;
                DemonSiegeMusic = null;
                OmegaStariteMusic = null;
                GlimmerEventMusic = null;
                CrabsonMusic = null;
                AQTextures.Unload();
            }
            // ^ doesn't load on server ^

            ModCallHelper.Unload();
            MoonlightWallHelper.Instance = null;
            if (CursorDyes != null)
            {
                CursorDyes.Unload();
                CursorDyes = null;
            }
            AQText.Unload();
        }

        public override void PreUpdateEntities()
        {
            if (!Main.dedServ)
            {
                if ((_lastScreenView != Main.ViewSize || _lastScreenZoom != new Vector2(Main.screenWidth, Main.screenHeight)))
                {
                    HotAndColdCurrentLayer.Reset(Main.graphics.GraphicsDevice);
                }
                _lastScreenZoom = new Vector2(Main.screenWidth, Main.screenHeight);
                _lastScreenView = Main.ViewSize;
                for (int i = 0; i < WorldEffects.Count; i++)
                {
                    var v = WorldEffects[i];
                    if (!v.Update())
                    {
                        WorldEffects.RemoveAt(i);
                        i--;
                    }
                }
            }
            if (Main.netMode != NetmodeID.Server)
            {
                GlimmerEvent.stariteProjectileColor = GlimmerEvent.StariteDisco ? new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0) : StariteProjectileColor;
            }
            else
            {
                GlimmerEvent.stariteProjectileColor = GlimmerEvent.StariteDisco ? new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0) : GlimmerEvent.StariteProjectileColorOrig;
            }
        }

        public override void MidUpdateNPCGore()
        {
            DemonSiege.UpdateEvent();

            if (OmegaStariteScenes.OmegaStariteIndexCache > -1 && !Main.npc[OmegaStariteScenes.OmegaStariteIndexCache].active)
            {
                OmegaStariteScenes.OmegaStariteIndexCache = -1;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && spawnStarite)
            {
                OmegaStariteScenes.OmegaStariteIndexCache = (short)NPC.NewNPC(GlimmerEvent.tileX * 16 + 8, GlimmerEvent.tileY * 16 - 1600, ModContent.NPCType<OmegaStarite>(), 0, OmegaStarite.PHASE_NOVA, 0f, 0f, 0f, Main.myPlayer);
                OmegaStariteScenes.SceneType = 1;
                spawnStarite = false;
                BroadcastMessage("Mods.AQMod.Common.AwakenedOmegaStarite", BossMessage);
            }

            if (CrabSeason.CrabsonCachedID > -1 && !Main.npc[CrabSeason.CrabsonCachedID].active)
            {
                CrabSeason.CrabsonCachedID = -1;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                SceneLayersManager.UpdateLayers();
            }
        }

        public override void UpdateMusic(ref int music, ref MusicPriority priority)
        {
            AQText.UpdateCallback();

            if (Main.myPlayer == -1 || Main.gameMenu || !Main.LocalPlayer.active || Loading)
            {
                return;
            }

            var player = Main.LocalPlayer;
            if (DemonSiege.CloseEnoughToDemonSiege(player))
            {
                music = DemonSiegeMusic.GetMusicID();
                priority = MusicPriority.Event;
            }
            else if (GlimmerEvent.IsActive && player.position.Y < Main.worldSurface * 16.0)
            {
                int tileDistance = (int)(player.Center.X / 16 - GlimmerEvent.tileX).Abs();
                if (tileDistance < GlimmerEvent.MaxDistance)
                {
                    music = GlimmerEventMusic.GetMusicID();
                    priority = MusicPriority.Event;
                }
            }
            else if (GaleStreams.EventActive(Main.LocalPlayer))
            {
                music = GaleStreamsMusic.GetMusicID();
                priority = MusicPriority.Event;
            }
        }

        public static void SetupNewMusic()
        {
            if (Main.myPlayer == -1 || Main.gameMenu || !Main.LocalPlayer.active || Loading)
            {
                return;
            }
            if (Main.npc != null && Main.npc.Length >= Main.maxNPCs)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].modNPC is IModifiableMusicNPC modifiableMusicNPC)
                    {
                        Main.npc[i].modNPC.music = modifiableMusicNPC.GetMusic().GetMusicID();
                    }
                }
            }
        }

        public override void PostDrawFullscreenMap(ref string mouseText)
        {
            MapInterfaceManager.Apply(ref mouseText, drawGlobes: true);
        }

        private static Vector2 GetMapPosition(Vector2 map, Vector2 tileCoords, float mapScale)
        {
            return new Vector2(tileCoords.X * mapScale + map.X, tileCoords.Y * mapScale + map.Y);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            Edits.OverrideColor = false;
            var player = Main.LocalPlayer;
            var drawingPlayer = player.GetModPlayer<AQPlayer>();
            if (drawingPlayer.CursorDyeID != CursorDyeLoader.ID.None)
            {
                var cursorDye = CursorDyes.GetContent(drawingPlayer.CursorDyeID);
                Edits.OverrideColor = cursorDye.ApplyColor(player, drawingPlayer, out Edits._newCursorColor);
            }
            var index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer("AQMod: Rename Item Interface",
                    () => 
                    {
                        RenameItemInterface.Draw();
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
            index = layers.FindIndex((l) => l.Name.Equals("Vanilla: Invasion Progress Bars"));
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer("AQMod: Invasion Progress Bar", () =>
                {
                    EventProgressBarManager.Draw();
                    return true;
                }, InterfaceScaleType.UI));
            }
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            byte messageID = reader.ReadByte();

            switch (messageID)
            {
                case AQPacketID.SummonOmegaStarite:
                {
                    if (Main.netMode == NetmodeID.Server)
                    {
                        spawnStarite = true;
                    }

                    if (Debug.LogNetcode)
                    {
                        var l = Debug.GetDebugLogger();
                        l.Log("Summoning Omega Starite...");
                    }
                }
                break;

                case AQPacketID.UpdateGlimmerEvent:
                {
                    GlimmerEvent.tileX = reader.ReadUInt16();
                    GlimmerEvent.tileY = reader.ReadUInt16();
                    GlimmerEvent.spawnChance = reader.ReadInt32();
                    GlimmerEvent.StariteDisco = reader.ReadBoolean();
                    GlimmerEvent.deactivationTimer = reader.ReadInt32();

                    if (Debug.LogNetcode)
                    {
                        var l = Debug.GetDebugLogger();
                        l.Log("Updating Glimmer Event");
                        l.Log("x: " + GlimmerEvent.tileX);
                        l.Log("y: " + GlimmerEvent.tileY);
                        l.Log("spawn chance: " + GlimmerEvent.spawnChance);
                        l.Log("starite disco: " + GlimmerEvent.StariteDisco);
                        l.Log("deactivation timer: " + GlimmerEvent.deactivationTimer);
                    }
                }
                break;

                case AQPacketID.UpdateAQPlayerCelesteTorus:
                {
                    var player = Main.player[reader.ReadByte()];
                    var aQPlayer = player.GetModPlayer<AQPlayer>();
                    aQPlayer.celesteTorusX = reader.ReadSingle();
                    aQPlayer.celesteTorusY = reader.ReadSingle();
                    aQPlayer.celesteTorusZ = reader.ReadSingle();

                    if (Debug.LogNetcode)
                    {
                        var l = Debug.GetDebugLogger();
                        l.Log("Updating celeste torus positions for: (" + player.name + ")");
                        l.Log("x: " + aQPlayer.celesteTorusX);
                        l.Log("y: " + aQPlayer.celesteTorusY);
                        l.Log("z: " + aQPlayer.celesteTorusZ);
                    }
                }
                break;

                case AQPacketID.UpdateAQPlayerEncoreKills:
                {
                    var player = Main.player[reader.ReadByte()];
                    var aQPlayer = player.GetModPlayer<AQPlayer>();
                    byte[] buffer = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));
                    aQPlayer.DeserialzeBossKills(buffer);
                }
                break;

                case AQPacketID.PreventedBloodMoon:
                {
                    Debug.DebugLogger? l = null;
                    if (Debug.LogNetcode)
                    {
                        l = Debug.GetDebugLogger();
                        l.Value.Log("Old Blood Moons Prevented: " + CosmicanonCounts.EclipsesPrevented);
                    }

                    CosmicanonCounts.BloodMoonsPrevented = reader.ReadUInt16();

                    if (Debug.LogNetcode)
                    {
                        l.Value.Log("Updated Blood Moons Prevented: " + CosmicanonCounts.EclipsesPrevented);
                    }
                }
                break;

                case AQPacketID.PreventedGlimmer:
                {
                    Debug.DebugLogger? l = null;
                    if (Debug.LogNetcode)
                    {
                        l = Debug.GetDebugLogger();
                        l.Value.Log("Old Glimmers Prevented: " + CosmicanonCounts.EclipsesPrevented);
                    }

                    CosmicanonCounts.GlimmersPrevented = reader.ReadUInt16();

                    if (Debug.LogNetcode)
                    {
                        l.Value.Log("Updated Glimmers Prevented: " + CosmicanonCounts.EclipsesPrevented);
                    }
                }
                break;

                case AQPacketID.PreventedEclipse:
                {
                    Debug.DebugLogger? l = null;
                    if (Debug.LogNetcode)
                    {
                        l = Debug.GetDebugLogger();
                        l.Value.Log("Old Eclipses Prevented: " + CosmicanonCounts.EclipsesPrevented);
                    }

                    CosmicanonCounts.EclipsesPrevented = reader.ReadUInt16();

                    if (Debug.LogNetcode)
                    {
                        l.Value.Log("Updated Eclipses Prevented: " + CosmicanonCounts.EclipsesPrevented);
                    }
                }
                break;

                case AQPacketID.BeginDemonSiege:
                {
                    int x = reader.ReadInt32();
                    int y = reader.ReadInt32();
                    int player = reader.ReadInt32();
                    int itemType = reader.ReadInt32();
                    int itemStack = reader.ReadInt32();
                    int itemPrefix = reader.ReadByte();

                    Item item = new Item();

                    item.netDefaults(itemType);
                    item.stack = itemStack;
                    item.Prefix(itemPrefix);

                    if (itemType > Main.maxItemTypes)
                    {
                        item.modItem.NetRecieve(reader);
                    }

                    if (Debug.LogNetcode)
                    {
                        var l = Debug.GetDebugLogger();
                        l.Log("x: " + x);
                        l.Log("y: " + y);
                        l.Log("player activator: " + player + " (" + Main.player[player].name + ")");
                        l.Log("item Type: " + itemType + " (" + Lang.GetItemName(item.type) + ")");
                        l.Log("item Stack: " + itemStack);
                        l.Log("item Prefix: " + itemPrefix);
                    }

                    DemonSiege.Activate(x, y, player, item, server: true);
                }
                break;
            }
        }

        public override object Call(params object[] args)
        {
            if (ModCallHelper.VerifyCall(args))
            {
                return ModCallHelper.InvokeCall(args);
            }
            return null;
        }

        public static void ApplyClientConfig(AQConfigClient clientConfig)
        {
            ShowBackgroundStarites = clientConfig.BackgroundStarites;
            Screenshakes = clientConfig.Screenshakes;
            TonsofScreenShakes = clientConfig.TonsofScreenShakes;
            StariteProjectileColor = clientConfig.StariteProjColor;
            MapBlipColor = clientConfig.MapBlipColor;
            StariteAuraColor = clientConfig.StariteAuraColor;
            StariteBGMult = clientConfig.StariteBackgroundLight;
        }

        public static void ApplyServerConfig(AQConfigServer serverConfig)
        {
            EvilProgressionLock = serverConfig.evilProgressionLock;
        }

        internal static void BroadcastMessage(string key, Color color)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), color);
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(Language.GetTextValue(key), color);
            }
        }

        internal static int RandomSmokeGoreType(UnifiedRandom random)
        {
            return 61 + random.Next(3);
        }

        internal static bool VariableLuck(int chance)
        {
            return VariableLuck(chance, Main.rand);
        }
        internal static bool VariableLuck(int chance, UnifiedRandom rand)
        {
            return chance < 1 || rand.NextBool(chance);
        }

        internal static bool AnyBossDefeated()
        {
            return AnyVanillaBossDefeated() || AnyAequusBossDefeated();
        }

        internal static bool AnyAequusBossDefeated()
        {
            return WorldDefeats.DownedCrabson || WorldDefeats.DownedStarite;
        }

        internal static bool AnyVanillaBossDefeated()
        {
            return NPC.downedSlimeKing ||
                NPC.downedBoss1 ||
                NPC.downedBoss2 ||
                NPC.downedBoss3 ||
                NPC.downedQueenBee ||
                Main.hardMode;
        }

        internal static bool reduceSpawnrates()
        {
            return NPC.AnyNPCs(ModContent.NPCType<OmegaStarite>()) || NPC.AnyNPCs(ModContent.NPCType<JerryCrabson>());
        }

        public static bool ShouldRemoveSpawns()
        {
            return ModContent.GetInstance<AQConfigServer>().reduceSpawns && reduceSpawnrates();
        }

        public static int MultIntensity(int input)
        {
            return (int)(input * AQConfigClient.c_EffectIntensity);
        }

        public static TModProjectile NewModProjectile<TModProjectile>(Vector2 position, Vector2 velocity, int Type, int Damage, float KnockBack, int Owner, float ai0 = 0f, float ai1 = 0f) where TModProjectile : ModProjectile
        {
            return (TModProjectile)Main.projectile[Projectile.NewProjectile(position, velocity, Type, Damage, KnockBack, Owner, ai0, ai1)].modProjectile;
        }

        public static TModProjectile NewModProjectile<TModProjectile>(out int index, Vector2 position, Vector2 velocity, int Type, int Damage, float KnockBack, int Owner, float ai0 = 0f, float ai1 = 0f) where TModProjectile : ModProjectile
        {
            index = Projectile.NewProjectile(position, velocity, Type, Damage, KnockBack, Owner, ai0, ai1);
            return (TModProjectile)Main.projectile[index].modProjectile;
        }

        internal static void addLoadTask(CachedTask task)
        {
            cachedLoadTasks.Add(task);
        }

        private static void invokeTasks()
        {
            if (cachedLoadTasks == null)
                return;
            foreach (var task in cachedLoadTasks)
            {
                try
                {
                    task.Invoke();
                }
                catch (Exception e)
                {
                    var aQMod = Instance;
                    aQMod.Logger.Error("An error occured when invoking cached load tasks.");
                    aQMod.Logger.Error(e.Message);
                    aQMod.Logger.Error(e.StackTrace);
                }
            }
        }
    }
}