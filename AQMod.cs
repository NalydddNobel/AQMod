using AQMod.Assets;
using AQMod.Common;
using AQMod.Common.CrossMod;
using AQMod.Common.Graphics;
using AQMod.Common.Utilities;
using AQMod.Common.Utilities.Debugging;
using AQMod.Content;
using AQMod.Content.Entities;
using AQMod.Content.Players;
using AQMod.Content.Seasonal.Christmas;
using AQMod.Content.World.Events;
using AQMod.Effects;
using AQMod.Effects.Dyes;
using AQMod.Effects.Trails.Rendering;
using AQMod.Items;
using AQMod.Items.Accessories.Wings;
using AQMod.Items.Dyes;
using AQMod.Items.Potions.Foods;
using AQMod.Items.Recipes;
using AQMod.Localization;
using AQMod.NPCs;
using AQMod.NPCs.Bosses;
using AQMod.Sounds;
using AQMod.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
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
        public const string TextureNone = "AQMod/Assets/None";
        public static Color MysteriousGuideTooltip => Color.CornflowerBlue * 4f;
        public static Color DemonSiegeTooltip => new Color(255, 170, 150, 255);
        public static Vector2 Zero => Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
        public static Vector2 ScreenCenter => new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f);
        public static Vector2 WorldScreenCenter => new Vector2(Main.screenPosition.X + (Main.screenWidth / 2f), Main.screenPosition.Y + Main.screenHeight / 2f);

        public static AQMod GetInstance()
        {
            return ModContent.GetInstance<AQMod>();
        }

        public static bool LowQ => !Lighting.NotRetro || ModContent.GetInstance<AQConfigClient>().EffectQuality <= 0.5f;

        public static bool spawnStarite;

        internal static bool Loading { get; private set; }
        internal static bool IsUnloading { get; private set; }

        public static EquipOverlayLoader ArmorOverlays { get; private set; }
        public static ModifiableMusic CrabCavesMusic { get; private set; }
        public static ModifiableMusic CrabsonMusic { get; private set; }
        public static ModifiableMusic GlimmerEventMusic { get; private set; }
        public static ModifiableMusic OmegaStariteMusic { get; private set; }
        public static ModifiableMusic DemonSiegeMusic { get; private set; }
        public static ModifiableMusic GaleStreamsMusic { get; private set; }
        internal List<CachedTask> cachedLoadTasks;

        internal static ModData calamityMod;
        internal static ModData catalyst;
        internal static ModData thoriumMod;
        internal static ModData fargowiltas;
        internal static ModData polarities;
        internal static ModData split;
        internal static ModData sOTS;
        internal static ModData shaderLib;
        internal static ModData discordRP;
        internal static ModData bossChecklist;
        internal static ModData census;

        public UserInterface NPCTalkState { get; private set; }

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

        private void LoadHooks(bool unload = false)
        {
            if (unload)
            {
                TimeActions.Hooks.Main_UpdateTime_SpawnTownNPCs = null;
            }
            else
            {
                if (ModContent.GetInstance<AQConfigClient>().XmasProgressMeterOverride)
                {
                    On.Terraria.GameContent.UI.States.UIWorldLoad.ctor += XmasSeeds.Hooks.UIWorldLoad_ctor_Xmas;
                }
                if (ModContent.GetInstance<AQConfigClient>().XmasBackground)
                {
                    On.Terraria.Main.DrawBG += XmasSeeds.Hooks.Main_DrawBG_XMasBG;
                }

                TimeActions.Hooks.Main_UpdateTime_SpawnTownNPCs = typeof(Main).GetMethod("UpdateTime_SpawnTownNPCs", BindingFlags.NonPublic | BindingFlags.Static);
                On.Terraria.Main.UpdateTime += TimeActions.Hooks.Main_UpdateTime;
                On.Terraria.Main.DrawProjectiles += DrawHelper.Hooks.Main_DrawProjectiles;
                On.Terraria.Main.DrawPlayers += DrawHelper.Hooks.Main_DrawPlayers;

                On.Terraria.Main.UpdateSundial += AQSystem.Hooks.Main_UpdateSundial;
                On.Terraria.Main.UpdateWeather += AQSystem.Hooks.Main_UpdateWeather;

                On.Terraria.GameContent.Achievements.AchievementsHelper.NotifyProgressionEvent += CosmicanonWorldData.Hooks.AchievementsHelper_NotifyProgressionEvent;

                On.Terraria.UI.ItemSlot.OverrideHover += InvSlotData.Hooks.ItemSlot_OverrideHover;

                On.Terraria.Main.DrawNPCs += DrawHelper.Hooks.Main_DrawNPCs;
                On.Terraria.Main.DrawTiles += DrawHelper.Hooks.Main_DrawTiles;
                On.Terraria.Main.UpdateDisplaySettings += DrawHelper.Hooks.Main_UpdateDisplaySettings;

                On.Terraria.Main.CursorColor += CursorDyeManager.Hooks.Main_CursorColor;
                On.Terraria.Main.DrawCursor += CursorDyeManager.Hooks.Main_DrawCursor;
                On.Terraria.Main.DrawThickCursor += CursorDyeManager.Hooks.Main_DrawThickCursor;
                On.Terraria.Main.DrawInterface_36_Cursor += CursorDyeManager.Hooks.Main_DrawInterface_36_Cursor;

                On.Terraria.Player.DropTombstone += TombstonesPlayer.Hooks.Player_DropTombstone;
                AQPlayer.Hooks.Apply();

                On.Terraria.NetMessage.BroadcastChatMessage += MessageBroadcast.Hooks.NetMessage_BroadcastChatMessage;
                On.Terraria.Main.NewText_string_byte_byte_byte_bool += MessageBroadcast.Hooks.Main_NewText_string_byte_byte_byte_bool;

                On.Terraria.Projectile.NewProjectile_float_float_float_float_int_int_float_int_float_float += AQProjectile.Hooks.Projectile_NewProjectile_float_float_float_float_int_int_float_int_float_float;

                On.Terraria.UI.ItemSlot.MouseHover_ItemArray_int_int += PlayerStorage.Hooks.ItemSlot_MouseHover_ItemArray_int_int;

                On.Terraria.NPC.Collision_DecideFallThroughPlatforms += AQNPC.Hooks.NPC_Collision_DecideFallThroughPlatforms;
            }

        }
        private void LoadMusic(bool unload = false)
        {
            if (unload)
            {
                CrabCavesMusic?.Dispose();
                CrabCavesMusic = null;
                CrabsonMusic?.Dispose();
                CrabsonMusic = null;
                GlimmerEventMusic?.Dispose();
                GlimmerEventMusic = null;
                OmegaStariteMusic?.Dispose();
                OmegaStariteMusic = null;
                DemonSiegeMusic?.Dispose();
                DemonSiegeMusic = null;
                GaleStreamsMusic?.Dispose();
                GaleStreamsMusic = null;
            }
            else
            {
                CrabCavesMusic = new ModifiableMusic(MusicID.Ocean);
                CrabsonMusic = new ModifiableMusic(MusicID.Boss1);
                GlimmerEventMusic = new ModifiableMusic(MusicID.MartianMadness);
                OmegaStariteMusic = new ModifiableMusic(MusicID.Boss4);
                DemonSiegeMusic = new ModifiableMusic(MusicID.PumpkinMoon);
                GaleStreamsMusic = new ModifiableMusic(MusicID.Sandstorm);
            }
        }
        private void LoadCrossMod(bool unload = false)
        {
            if (unload)
            {
                calamityMod.Dispose();
                catalyst.Dispose();
                thoriumMod.Dispose();
                fargowiltas.Dispose();
                polarities.Dispose();
                split.Dispose();
                sOTS.Dispose();
                shaderLib.Dispose();
                discordRP.Dispose();
                bossChecklist.Dispose();
                census.Dispose();
            }
            else
            {
                calamityMod = new ModData("CalamityMod");
                catalyst = new ModData("Catalyst");
                thoriumMod = new ModData("ThoriumMod");
                fargowiltas = new ModData("Fargowiltas");
                polarities = new ModData("Polarities");
                split = new ModData("Split");
                sOTS = new ModData("SOTS");
                shaderLib = new ModData("ShaderLib");
                discordRP = new ModData("DiscordRP");
                bossChecklist = new ModData("BossChecklist");
                census = new ModData("Census");
            }
        }
        public override void Load()
        {
            Loading = true;
            IsUnloading = false;
            Keybinds.Load();
            LoadHooks(unload: false);
            AQText.Load();
            ImitatedWindyDay.Reset(resetNonUpdatedStatics: true);
            DemonSiege.Load();
            ModCallDictionary.Load();
            CursorDyeManager.Load();
            AprilFoolsJoke.Check();
            Coloring.Load();

            var server = ModContent.GetInstance<AQConfigServer>();
            if (!Main.dedServ)
            {
                DrawHelper.Load();

                LegacyTextureCache.Load();
                Tex.Load(this);
                CrabPot.frame = new Rectangle(0, 0, Tex.CrabPot.Texture.Value.Width, Tex.CrabPot.Texture.Value.Height / CrabPot.FrameCount - 2);
                CrabPot.origin = CrabPot.frame.Size() / 2f;

                LegacyEffectCache.Load(this);
                FX.InternalSetup();
                PrimitivesRenderer.Setup();
                BuffColorCache.Init();

                SkyManager.Instance[SkyGlimmerEvent.Name] = new SkyGlimmerEvent();

                AQSound.rand = new UnifiedRandom();
                ArmorOverlays = new EquipOverlayLoader();
                LoadMusic(unload: false);
                NPCTalkState = new UserInterface();
            }

            LoadCrossMod(unload: false);

            AQBuff.Sets.Load();
            AQItem.Load();
            AQTile.Sets.Load();
            AQConfigClient.LoadTranslations();
            AQConfigServer.LoadTranslations();

            Autoloading.Autoload(Code);
        }

        public override void PostSetupContent()
        {
            AQNPC.Sets.Setup();
            AQProjectile.Sets.Setup();
            BossChecklistSupport.SetupContent(this);
            CensusSupport.SetupContent(this);
            if (!Main.dedServ)
            {
                DiscordRichPresenceSupport.SetupContent();
                AutoDyeBinder.SetupDyes();
            }
            Autoloading.SetupContent(Code);
            invokeTasks();
            cachedLoadTasks.Clear();

            AQItem.SetupContent();
        }

        public override void AddRecipeGroups()
        {
            AQRecipeGroups.Setup();
        }

        public override void AddRecipes()
        {
            invokeTasks();
            cachedLoadTasks = null;

            Loading = false; // Sets Loading to false, so that some things no longer accept new content.

            AQRecipes.VanillaRecipeAddons(this);

            FargowiltasSupport.Setup(this);
        }

        public override void Unload()
        {
            // outside of AQMod
            Loading = true;
            IsUnloading = true;
            cachedLoadTasks?.Clear();
            cachedLoadTasks = null;
            LoadHooks(unload: true);
            Autoloading.Unload();

            NoHitting.CurrentlyDamaged?.Clear();
            NoHitting.CurrentlyDamaged = null;
            AutoDyeBinder.Unload();
            DemonSiege.Unload();
            AQProjectile.Sets.Unload();
            AQNPC.Sets.Unload();
            AQItem.Unload();
            GlowmaskData.ItemToGlowmask?.Clear();
            GlowmaskData.ItemToGlowmask = null;
            AQBuff.Sets.Unload();

            LoadCrossMod(unload: true);

            if (!Main.dedServ)
            {
                AQSound.rand = null;
                BuffColorCache.Unload();
                ArmorOverlays?.Dispose();
                ArmorOverlays = null;
                LegacyEffectCache.Unload();
                SkyGlimmerEvent.BGStarite._texture = null;
                NPCTalkState = null;
                LoadMusic(unload: true);
                PrimitivesRenderer.Unload();
                FX.Unload();
                LegacyEffectCache.Unload();
                Tex.Unload();
                LegacyTextureCache.Unload();
                DrawHelper.Unload();
            }

            Coloring.Unload();
            CursorDyeManager.Unload();
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

            AQGraphics.TimerBasedOnTimeOfDay = (float)Main.time;
            if (!Main.dayTime)
            {
                AQGraphics.TimerBasedOnTimeOfDay += (float)Main.dayLength;
            }
            AQGraphics.TimerBasedOnTimeOfDay /= 60f;

            if (Glimmer.stariteDiscoParty)
            {
                Glimmer.stariteProjectileColoring = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0);
            }
            else
            {
                Glimmer.stariteProjectileColoring = Glimmer.StariteProjectileColorOrig;
            }
        }

        private void UpdateNoHitStatus()
        {
            try
            {
                NoHitting.CurrentlyDamaged.Clear();
                for (byte i = 0; i < Main.maxPlayers; i++)
                {
                    if (Main.player[i].active && Main.player[i].statLife < Main.player[i].statLifeMax2)
                    {
                        NoHitting.CurrentlyDamaged.Add(i);
                    }
                }
            }
            catch
            {
                NoHitting.CurrentlyDamaged?.Clear();
                NoHitting.CurrentlyDamaged = new List<byte>();
            }
        }
        public override void MidUpdatePlayerNPC()
        {
            UpdateNoHitStatus();
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && !Main.player[i].dead)
                {
                    var aQPlayer = Main.player[i].GetModPlayer<AQPlayer>();
                    if (aQPlayer.shade && (Main.player[i].velocity == Vector2.Zero || Main.player[i].velocity.Length() < 0.2f))
                    {
                        aQPlayer.undetectable = true;
                    }
                    if (aQPlayer.undetectable)
                    {
                        Main.player[i].dead = true;
                    }
                }
            }
        }

        public override void MidUpdateNPCGore()
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && Main.player[i].dead)
                {
                    var aQPlayer = Main.player[i].GetModPlayer<AQPlayer>();
                    if (aQPlayer.undetectable)
                    {
                        Main.player[i].dead = false;
                        aQPlayer.undetectable = false;
                    }
                }
            }

            DemonSiege.UpdateEvent();

            if (Glimmer.omegaStarite > -1 && !Main.npc[Glimmer.omegaStarite].active)
            {
                Glimmer.omegaStarite = -1;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && spawnStarite)
            {
                int n = Glimmer.omegaStarite =
                    (short)NPC.NewNPC(Glimmer.tileX * 16 + 8, Glimmer.tileY * 16 - 1600, ModContent.NPCType<OmegaStarite>(),
                    0, OmegaStarite.PHASE_NOVA, 0f, 0f, 0f, Player.FindClosest(new Vector2(Glimmer.tileX * 16f, Glimmer.tileY * 16f), 16, 16));
                if (n != -1)
                {
                    Main.npc[n].netUpdate = true;
                    BroadcastMessage("Mods.AQMod.Common.AwakenedOmegaStarite", Coloring.BossMessage);
                }
                spawnStarite = false;
            }
            if (Main.netMode != NetmodeID.Server)
            {
                DrawHelper.DoUpdate();
            }
        }

        public override void UpdateMusic(ref int music, ref MusicPriority priority)
        {
            if (Main.myPlayer == -1 || Main.gameMenu || !Main.LocalPlayer.active || Loading)
            {
                return;
            }

            var player = Main.LocalPlayer;
            if (player.Biomes().zoneDemonSiege)
            {
                music = DemonSiegeMusic.GetMusicID();
                priority = MusicPriority.Event;
            }
            else if (Glimmer.IsGlimmerEventCurrentlyActive() && player.position.Y < Main.worldSurface * 16.0)
            {
                int tileDistance = (int)(player.Center.X / 16 - Glimmer.tileX).Abs();
                if (tileDistance < Glimmer.MaxDistance)
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
            else if (player.Biomes().zoneCrabCrevice)
            {
                music = CrabCavesMusic.GetMusicID();
                priority = MusicPriority.BiomeMedium;
            }
        }

        public override void PostDrawFullscreenMap(ref string mouseText)
        {
            if (WorldGen.gen)
                return;
            MapUI.RenderOnMap(ref mouseText);
            MapUI.RenderOverlayingUI(ref mouseText);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            NPCTalkState.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            layers.Insert(0, new LegacyGameInterfaceLayer("AQMod: UpdateUtilities",
                () =>
                {
                    CursorDyeManager.Update();
                    return true;
                },
                InterfaceScaleType.None));

            var index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Gamepad Lock On"));
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer("AQMod: NPC Lock Ons", () =>
                {
                    if (Main.gamePaused || !Main.instance.IsActive)
                    {
                        return true;
                    }
                    var player = Main.LocalPlayer;
                    var aQPlayer = player.GetModPlayer<AQPlayer>();
                    if (!aQPlayer.meathookUI)
                    {
                        return true;
                    }

                    float grappleDistance = (ProjectileLoader.GetProjectile(player.miscEquips[4].type)?.GrappleRange()).GetValueOrDefault(480f);

                    int meathookChoice = -1;
                    float meathookDistance = 320f;

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].active && AQNPC.CanBeMeathooked(Main.npc[i]))
                        {
                            float distanceFromPlayer = Main.npc[i].Distance(player.Center);
                            if (distanceFromPlayer < grappleDistance - Main.npc[i].Size.Length())
                            {
                                float distanceFromCursor = Main.npc[i].Distance(Main.MouseWorld);
                                if (distanceFromCursor < meathookDistance)
                                {
                                    meathookChoice = i;
                                    meathookDistance = distanceFromCursor;
                                }
                            }
                        }
                    }

                    if (meathookChoice == -1)
                    {
                        return true;
                    }
                    InterfaceHookableNPC.RenderUI(meathookChoice);
                    return true;
                }, InterfaceScaleType.Game));
            }

            index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer("AQMod: Rename Item Interface",
                    () =>
                    {
                        NPCTalkState.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
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

        private void invokeTasks()
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
            if (DebugUtilities.LogTextureLoading)
            {
                DebugUtilities.GetDebugLogger().Log("Loading Texture: " + path);
            }
            return ModContent.GetTexture(path);
        }

        public static bool UnderworldCheck()
        {
            return !PolaritiesModSupport.InFractalDimension();
        }

        public static void AequusDeveloperItems(Player player, bool hardmode)
        {
            if (Main.rand.Next(7) > 0)
            {
                return;
            }
            List<int> items = new List<int>()
            {
                ModContent.ItemType<Baguette>(),
            };
            if (hardmode)
            {
                items.Add(ModContent.ItemType<NalydDye>());
                items.Add(ModContent.ItemType<Thunderbird>());
            }
            if (items.Count == 0)
            {
                player.QuickSpawnItem(items[0]);
            }
            else
            {
                player.QuickSpawnItem(items[Main.rand.Next(items.Count)]);
            }
        }

        public static Matrix GetWorldViewPoint()
        {
            GraphicsDevice graphics = Main.graphics.GraphicsDevice;
            Vector2 screenZoom = Main.GameViewMatrix.Zoom;
            int width = graphics.Viewport.Width;
            int height = graphics.Viewport.Height;

            var zoom = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) *
                Matrix.CreateTranslation(width / 2f, height / -2f, 0) *
                Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(screenZoom.X, screenZoom.Y, 1f);
            var projection = Matrix.CreateOrthographic(width, height, 0, 1000);
            return zoom * projection;
        }

        public static string GetText(string key)
        {
            return AQText.modTranslations["Mods.AQMod." + key].GetTranslation(Language.ActiveCulture);
        }
        public static ModTranslation GetTranslation(string key)
        {
            return AQText.modTranslations["Mods.AQMod." + key];
        }
    }
}