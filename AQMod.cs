using AQMod.Assets;
using AQMod.Assets.LegacyItemOverlays;
using AQMod.Buffs.Temperature;
using AQMod.Common;
using AQMod.Common.Configuration;
using AQMod.Common.CrossMod;
using AQMod.Common.DeveloperTools;
using AQMod.Common.Graphics;
using AQMod.Common.Graphics.PlayerEquips;
using AQMod.Common.Graphics.SceneLayers;
using AQMod.Common.ID;
using AQMod.Common.UserInterface;
using AQMod.Content;
using AQMod.Content.CursorDyes;
using AQMod.Content.Entities;
using AQMod.Content.MapMarkers;
using AQMod.Content.NameTags;
using AQMod.Content.Quest.Lobster;
using AQMod.Content.Seasonal.Christmas;
using AQMod.Content.World.Events;
using AQMod.Content.World.Events.DemonSiege;
using AQMod.Content.World.Events.GaleStreams;
using AQMod.Content.World.Events.GlimmerEvent;
using AQMod.Content.World.Events.ProgressBars;
using AQMod.Effects;
using AQMod.Effects.Dyes;
using AQMod.Effects.ScreenEffects;
using AQMod.Effects.Trails.Rendering;
using AQMod.Effects.WorldEffects;
using AQMod.Items.Tools.Fishing.Bait;
using AQMod.Localization;
using AQMod.NPCs;
using AQMod.NPCs.Boss;
using AQMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameInput;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.Utilities;
using Terraria.World.Generation;

namespace AQMod
{
    public class AQMod : Mod
    {
        public static AQMod GetInstance()
        {
            return ModContent.GetInstance<AQMod>();
        }

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
        public static MapMarkerManager MapMarkers => ModContent.GetInstance<MapMarkerManager>();

        internal static List<CachedTask> cachedLoadTasks;
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

        internal static int _lastScreenWidth;
        internal static int _lastScreenHeight;

        public static class Keybinds
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
            internal static void Load()
            {
                if (ModContent.GetInstance<AQConfigClient>().XmasProgressMeterOverride)
                {
                    On.Terraria.GameContent.UI.States.UIWorldLoad.ctor += UIWorldLoad_ctor_Xmas;
                }
                On.Terraria.UI.ItemSlot.OverrideHover += ItemSlot_OverrideHover;
                On.Terraria.GameContent.Achievements.AchievementsHelper.NotifyProgressionEvent += AchievementsHelper_NotifyProgressionEvent;
                On.Terraria.Chest.SetupShop += Chest_SetupShop;
                On.Terraria.NPC.Collision_DecideFallThroughPlatforms += NPC_Collision_DecideFallThroughPlatforms;
                if (ModContent.GetInstance<AQConfigClient>().XmasBackground)
                {
                    On.Terraria.Main.DrawBG += Main_DrawBG_XMasBG;
                }
                On.Terraria.Main.UpdateSundial += Main_UpdateSundial;
                On.Terraria.Main.UpdateWeather += Main_UpdateWeather;
                On.Terraria.Main.UpdateDisplaySettings += Main_UpdateDisplaySettings;
                On.Terraria.Main.DrawProjectiles += Main_DrawProjectiles;
                On.Terraria.Main.DrawPlayers += Main_DrawPlayers;
                On.Terraria.Player.FishingLevel += GetFishingLevel;
                On.Terraria.Player.AddBuff += Player_AddBuff;
                On.Terraria.Player.QuickBuff += Player_QuickBuff;
                On.Terraria.Player.PickTile += Player_PickTile;
                On.Terraria.Player.HorizontalMovement += Player_HorizontalMovement;
            }

            private static void Main_DrawProjectiles(On.Terraria.Main.orig_DrawProjectiles orig, Main self)
            {
                BatcherMethods.GeneralEntities.Begin(Main.spriteBatch);
                Particle.PreDrawProjectiles.Render();
                Trail.PreDrawProjectiles.Render();
                Main.spriteBatch.End();
                orig(self);
            }

            private static void Main_DrawPlayers(On.Terraria.Main.orig_DrawPlayers orig, Main self)
            {
                orig(self);
                BatcherMethods.GeneralEntities.Begin(Main.spriteBatch);
                for (int i = 0; i < CrabPot.maxCrabPots; i++)
                {
                    CrabPot.crabPots[i].Render();
                }
                Particle.PostDrawPlayers.Render();
                SceneLayersManager.DrawLayer(SceneLayering.PostDrawPlayers);
                Main.spriteBatch.End();
            }


            private static void UIWorldLoad_ctor_Xmas(On.Terraria.GameContent.UI.States.UIWorldLoad.orig_ctor orig, Terraria.GameContent.UI.States.UIWorldLoad self, Terraria.World.Generation.GenerationProgress progress)
            {
                if (XmasSeeds.XmasWorld)
                {
                    XmasSeeds.realGenerationProgress = progress;
                    XmasSeeds.generationProgress = new GenerationProgress
                    {
                        Value = progress.Value,
                        TotalWeight = progress.TotalWeight,
                        CurrentPassWeight = 1f,
                    };
                    progress = XmasSeeds.generationProgress;
                }
                orig(self, progress);
            }

            private static void Main_DrawBG_XMasBG(On.Terraria.Main.orig_DrawBG orig, Main self)
            {
                bool christmasBackground = XmasSeeds.XmasWorld && WorldGen.gen; // originally this also ran on the title screen,
                                                                                // but for some reason there were conflicts with Modder's Toolkit
                bool snowflakes = XmasSeeds.XmasWorld; // I like the snowflakes on the title screen :)
                if (Loading || Unloading)
                {
                    christmasBackground = false;
                    snowflakes = false;
                }
                if (christmasBackground)
                {
                    if (XmasSeeds.generationProgress != null)
                    {
                        XmasSeeds.generationProgress.Value = Main.rand.NextFloat(0f, 1f);
                        if (!XmasSeeds.generatingSnowBiomeText)
                            XmasSeeds.generationProgress.Message = Language.GetTextValue("Mods.AQMod.WorldGen.ChristmasSpirit") + ", " + Language.GetTextValue("Mods.AQMod.WorldGen.ChristmasSpiritProgress" + XmasSeeds.snowflakeRandom.Next(16));
                    }
                }
                if (Main.mapFullscreen)
                {
                    orig(self);
                    return;
                }
                bool oldGameMenu = Main.gameMenu;
                if (christmasBackground)
                {
                    Main.snowTiles = 10000;
                    if (Main.myPlayer > -1 || Main.player[Main.myPlayer] != null)
                    {
                        var plr = Main.LocalPlayer;
                        plr.ZoneGlowshroom = false;
                        plr.ZoneDesert = false;
                        plr.ZoneBeach = false;
                        plr.ZoneJungle = false;
                        plr.ZoneHoly = false;
                        plr.ZoneCrimson = false;
                        plr.ZoneCorrupt = false;
                        plr.ZoneSnow = true;
                        plr.position.X = Main.maxTilesX * 8f;
                    }
                    Main.screenPosition.X = Main.maxTilesX * 8f + (float)Math.Sin(Main.GlobalTime * 0.2f) * 1250f;
                    Main.screenPosition.Y = 2200f + (float)Math.Sin(Main.GlobalTime) * 80f;
                    Main.gameMenu = false;
                }
                if (snowflakes)
                {
                    if (XmasSeeds.farBGSnowflakes == null)
                    {
                        XmasSeeds.farBGSnowflakes = new List<FarBGSnowflake>();
                    }

                    if (XmasSeeds.snowflakeRandom == null)
                    {
                        XmasSeeds.snowflakeRandom = new UnifiedRandom();
                    }

                    var snowflake = new FarBGSnowflake(new Vector2(XmasSeeds.snowflakeRandom.Next(-200, Main.screenWidth + 200), -XmasSeeds.snowflakeRandom.Next(100, 250)));
                    snowflake.OnAdd();
                    XmasSeeds.farBGSnowflakes.Add(snowflake);
                    Particle.UpdateParticles(XmasSeeds.farBGSnowflakes);
                    Particle.DrawParticles(XmasSeeds.farBGSnowflakes);
                }
                else
                {
                    XmasSeeds.farBGSnowflakes = null;
                }
                orig(self);
                if (snowflakes)
                {
                    if (XmasSeeds.closeBGSnowflakes == null)
                    {
                        XmasSeeds.closeBGSnowflakes = new List<CloseBGSnowflake>();
                    }
                    if (XmasSeeds.snowflakeRandom.NextBool(10))
                    {
                        var snowflake = new CloseBGSnowflake(new Vector2(Main.screenPosition.X + XmasSeeds.snowflakeRandom.Next(-200, Main.screenWidth + 200), Main.screenPosition.Y - XmasSeeds.snowflakeRandom.Next(100, 250)));
                        snowflake.OnAdd();
                        XmasSeeds.closeBGSnowflakes.Add(snowflake);
                    }
                    Particle.UpdateParticles(XmasSeeds.closeBGSnowflakes);
                    Particle.DrawParticles(XmasSeeds.closeBGSnowflakes);
                }
                else
                {
                    XmasSeeds.generatingSnowBiomeText = false;
                    XmasSeeds.realGenerationProgress = null;
                    XmasSeeds.generationProgress = null;
                    XmasSeeds.snowflakeRandom = null;
                    XmasSeeds.closeBGSnowflakes = null;
                }
                Main.gameMenu = oldGameMenu;
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

            private static void AchievementsHelper_NotifyProgressionEvent(On.Terraria.GameContent.Achievements.AchievementsHelper.orig_NotifyProgressionEvent orig, int eventID)
            {
                if (AQSystem.UpdatingWorld && AQSystem.CosmicanonActive && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (eventID == AchievementHelperID.Events.BloodMoonStart)
                    {
                        Main.bloodMoon = false;
                        CosmicanonCounts.BloodMoonsPrevented++;
                        if (Main.netMode == NetmodeID.Server)
                            NetHelper.PreventedBloodMoon();
                        MessageBroadcast.PreventChatOnce = true;
                    }
                    if (eventID == AchievementHelperID.Events.EclipseStart)
                    {
                        Main.eclipse = false;
                        CosmicanonCounts.EclipsesPrevented++;
                        if (Main.netMode == NetmodeID.Server)
                            NetHelper.PreventedEclipse();
                        MessageBroadcast.PreventChatOnce = true;
                    }
                }
                orig(eventID);
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
                                Main.windSpeed -= 0.001f * Main.dayRate;
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
                    SceneLayersManager.DrawLayer(SceneLayering.PreRender);
                    SceneLayersManager.RenderTargetLayers.PreRender();
                    AQGraphics.renderBox = new Rectangle(-20, -20, Main.screenWidth + 20, Main.screenHeight + 20);
                    _lastScreenWidth = Main.screenWidth;
                    _lastScreenHeight = Main.screenHeight;
                }
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

            private static void Main_UpdateSundial(On.Terraria.Main.orig_UpdateSundial orig)
            {
                orig();
                Main.dayRate += AQSystem.DayrateIncrease;
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
            _lastScreenWidth = 0;
            _lastScreenHeight = 0;
            Keybinds.Load(this);
            Common.Edits.LoadHooks();
            Edits.Load();
            AQText.Load();
            ImitatedWindyDay.Reset(resetNonUpdatedStatics: true);

            ModCallDictionary.Load();
            AprilFoolsJoke.UpdateActive();
            var server = ModContent.GetInstance<AQConfigServer>();
            if (!Main.dedServ)
            {
                var client = AQConfigClient.Instance;
                AQSound.rand = new UnifiedRandom();
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
                PrimitivesRenderer.Setup();
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
            AQItem.Sets.Setup();
            CensusSupport.AddSupport(this);
            if (!Main.dedServ)
            {
                DiscordRichPresenceSupport.AddSupport(this);
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

            Loading = false; // Sets Loading to false, so that some things no longer accept new content.
            RobsterHuntLoader.Instance.SetupHunts();
            if (!Main.dedServ)
            {
                ItemOverlays.Finish();
            }

            AQRecipes.AddRecipes(this);

            FargowiltasSupport.Setup(this);
            AQItem.Sets.Clones.Setup();
        }

        public override void Unload()
        {
            // outside of AQMod
            Loading = true;
            Unloading = true;
            cachedLoadTasks = null;
            Autoloading.Unload();
            Common.Edits.UnloadHooks();

            AQItem.Sets.Clones.Unload();
            ItemOverlays = null;

            DyeBinder.Unload();
            DemonSiege.Unload();
            AQProjectile.Sets.UnloadSets();
            AQNPC.Sets.UnloadSets();
            AQItem.Sets.Unload();
            AQBuff.Sets.Unload();

            if (!Main.dedServ)
            {
                AQSound.rand = null;
                WorldEffects = null;
                StarbyteColorCache.Unload();
                ScreenShakeManager.Unload();
                ArmorOverlays = null;
                SceneLayersManager.Unload();
                EffectCache.Unload();
                GlimmerEventSky.BGStarite._texture = null;
                GaleStreamsMusic = null;
                DemonSiegeMusic = null;
                OmegaStariteMusic = null;
                GlimmerEventMusic = null;
                CrabsonMusic = null;
                AQTextures.Unload();
            }

            ModCallDictionary.Unload();
            AQText.Unload();
        }

        public override void PreUpdateEntities()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Particle.PreDrawProjectiles.UpdateParticles();
                Particle.PostDrawPlayers.UpdateParticles();

                Trail.PreDrawProjectiles.UpdateTrails();
            }
            if (!Main.dedServ)
            {
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

            AQGraphics.TimerBasedOnTimeOfDay = (float)Main.time;
            if (!Main.dayTime)
            {
                AQGraphics.TimerBasedOnTimeOfDay += (float)Main.dayLength;
            }
            AQGraphics.TimerBasedOnTimeOfDay /= 60f;

            if (GlimmerEvent.stariteDiscoParty)
            {
                GlimmerEvent.stariteProjectileColoring = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0);
            }
            else
            {
                GlimmerEvent.stariteProjectileColoring = Main.netMode != NetmodeID.Server
                    ? ModContent.GetInstance<StariteConfig>().StariteProjectileColoring
                    : GlimmerEvent.StariteProjectileColorOrig;
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
                int n = OmegaStariteScenes.OmegaStariteIndexCache =
                    (short)NPC.NewNPC(GlimmerEvent.tileX * 16 + 8, GlimmerEvent.tileY * 16 - 1600, ModContent.NPCType<OmegaStarite>(),
                    0, OmegaStarite.PHASE_NOVA, 0f, 0f, 0f, Player.FindClosest(new Vector2(GlimmerEvent.tileX * 16f, GlimmerEvent.tileY * 16f), 16, 16));
                if (n != -1)
                {
                    Main.npc[n].netUpdate = true;
                    OmegaStariteScenes.SceneType = 1;
                    BroadcastMessage("Mods.AQMod.Common.AwakenedOmegaStarite", CommonColors.BossMessage);
                }
                spawnStarite = false;
            }

            if (CrabSeason.CrabsonCachedID > -1 && !Main.npc[CrabSeason.CrabsonCachedID].active)
            {
                CrabSeason.CrabsonCachedID = -1;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                CustomRenderUltimateSword.UpdateUltimateSword();
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
            else if (GlimmerEvent.IsGlimmerEventCurrentlyActive() && player.position.Y < Main.worldSurface * 16.0)
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

        public override void PostDrawFullscreenMap(ref string mouseText)
        {
            MapInterfaceManager.Apply(ref mouseText, drawGlobes: true);
        }

        public override void UpdateUI(GameTime gameTime)
        {
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            CursorDyeManager.Update();
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
                    try
                    {
                        EventProgressBarLoader.Draw();
                    }
                    catch
                    {
                    }
                    return true;
                }, InterfaceScaleType.UI));
            }
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            NetHelper.ReadPacket(reader, whoAmI);
        }

        public override object Call(params object[] args)
        {
            if (ModCallDictionary.VerifyCall(args))
            {
                return ModCallDictionary.InvokeCall(args);
            }
            return null;
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

        internal static void addLoadTask(CachedTask task)
        {
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
                    var aQMod = GetInstance();
                    aQMod.Logger.Error("An error occured when invoking cached load tasks.");
                    aQMod.Logger.Error(e.Message);
                    aQMod.Logger.Error(e.StackTrace);
                }
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

        public static Texture2D LoggableTexture(string path)
        {
            if (aqdebug.LogTextureLoading)
            {
                aqdebug.GetDebugLogger().Log("Loading Texture: " + path);
            }
            return ModContent.GetTexture(path);
        }
    }
}