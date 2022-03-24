using AQMod.Assets;
using AQMod.Common;
using AQMod.Common.CrossMod;
using AQMod.Common.Utilities;
using AQMod.Common.Utilities.Debugging;
using AQMod.Content;
using AQMod.Content.Concoctions;
using AQMod.Content.Entities;
using AQMod.Content.Players;
using AQMod.Content.World.Events;
using AQMod.Effects;
using AQMod.Effects.Dyes;
using AQMod.Effects.Particles;
using AQMod.Effects.Trails;
using AQMod.Items;
using AQMod.Items.Accessories.Wings;
using AQMod.Items.Dyes;
using AQMod.Items.Misc;
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
        public static Color MysteriousGuideTooltip => new Color(225, 100, 255, 255);
        public static Color DemonSiegeTooltip => new Color(255, 170, 150, 255);
        public static Vector2 Zero => Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
        public static Vector2 ScreenCenter => new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f);
        public static Vector2 WorldScreenCenter => new Vector2(Main.screenPosition.X + (Main.screenWidth / 2f), Main.screenPosition.Y + Main.screenHeight / 2f);
        public static bool UseAssets => !Loading && Main.netMode != NetmodeID.Server;
        public static bool GameWorldActive => Main.instance.IsActive && !Main.gamePaused;

        public static bool LowQ => !Lighting.NotRetro || ModContent.GetInstance<AQConfigClient>().EffectQuality <= 0.5f;

        public static bool spawnStarite;

        public static byte NearGlobe;

        internal static bool Loading { get; private set; }
        internal static bool IsUnloading { get; private set; }

        public static AQMod Instance { get; private set; }
        public static ConcoctionsSystem Concoctions { get; private set; }
        public static AQSets Sets { get; private set; }
        public static ParticleSystem Particles { get; private set; }
        public static TrailSystem Trails { get; private set; }
        public static EquipOverlaysManager ArmorOverlays { get; private set; }
        public UserInterface NPCTalkUI { get; private set; }
        public InGameHookablesUI HookablesUI { get; private set; }

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
                AQWorld.Hooks.Unload();

                TimeActions.Hooks.Main_UpdateTime_SpawnTownNPCs = null;
            }
            else
            {
                TimeActions.Hooks.Main_UpdateTime_SpawnTownNPCs = typeof(Main).GetMethod("UpdateTime_SpawnTownNPCs", BindingFlags.NonPublic | BindingFlags.Static);
                On.Terraria.Main.UpdateTime += TimeActions.Hooks.Main_UpdateTime;

                On.Terraria.UI.ItemSlot.OverrideHover += InvSlotData.Hooks.ItemSlot_OverrideHover;

                On.Terraria.Main.CursorColor += CursorDyeManager.Hooks.Main_CursorColor;
                On.Terraria.Main.DrawCursor += CursorDyeManager.Hooks.Main_DrawCursor;
                On.Terraria.Main.DrawThickCursor += CursorDyeManager.Hooks.Main_DrawThickCursor;
                On.Terraria.Main.DrawInterface_36_Cursor += CursorDyeManager.Hooks.Main_DrawInterface_36_Cursor;

                On.Terraria.Projectile.NewProjectile_float_float_float_float_int_int_float_int_float_float += AQProjectile.Hooks.Projectile_NewProjectile_float_float_float_float_int_int_float_int_float_float;

                On.Terraria.UI.ItemSlot.MouseHover_ItemArray_int_int += PlayerStorage.Hooks.ItemSlot_MouseHover_ItemArray_int_int;

                AQWorld.Hooks.Apply();
                InvUI.Hooks.Apply();
                DrawHelper.Hooks.Apply();
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
            Instance = this;
            Sets = new AQSets();
            Keybinds.Load();
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
                HookablesUI = new InGameHookablesUI();
                DrawHelper.Load();

                LegacyTextureCache.Load();

                CrabPot.frame = new Rectangle(0, 0, GetTexture("Assets/CrabPot").Width, GetTexture("Assets/CrabPot").Height / CrabPot.FrameCount - 2);
                CrabPot.origin = CrabPot.frame.Size() / 2f;

                LegacyEffectCache.Load(this);
                FX.InternalSetup();
                PrimitivesRenderer.Setup();

                SkyManager.Instance[SkyGlimmerEvent.Name] = new SkyGlimmerEvent();

                AQSound.rand = new UnifiedRandom();
                ArmorOverlays = new EquipOverlaysManager();
                LoadMusic(unload: false);
                NPCTalkUI = new UserInterface();
                Particles = new ParticleSystem();
                Trails = new TrailSystem();
            }

            LoadCrossMod(unload: false);

            MonoModHooks.RequestNativeAccess();

            LoadHooks(unload: false);

            CelesitalEightBall.Initalize();

            AQBuff.Sets.Instance = new AQBuff.Sets();
            AQItem.Sets.Instance = new AQItem.Sets();
            AQProjectile.Sets.Instance = new AQProjectile.Sets();
            AQNPC.Sets.Instance = new AQNPC.Sets();
            AQTile.Sets.Instance = new AQTile.Sets();
            AQConfigClient.LoadTranslations();
            AQConfigServer.LoadTranslations();
            BatchData.Load();

            Concoctions = new ConcoctionsSystem();
            Autoloading.Autoload(Code);
        }

        public override void PostSetupContent()
        {
            LootDrops.LootTables();
            AQItem.Sets.Instance.SetupContent();
            AQProjectile.Sets.Instance.SetupContent();
            AQNPC.Sets.Instance.SetupContent();
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
            Loading = true;
            IsUnloading = true;
            Instance = null;
            cachedLoadTasks?.Clear();
            cachedLoadTasks = null;
            LoadHooks(unload: true);
            Autoloading.Unload();

            HookablesUI = null;
            Concoctions = null;
            Sets = null;
            LootDrops.Unload();
            BatchData.Unload();

            NoHitting.CurrentlyDamaged?.Clear();
            NoHitting.CurrentlyDamaged = null;
            AutoDyeBinder.Unload();
            DemonSiege.Unload();
            AQProjectile.Sets.Instance = null;
            AQNPC.Sets.Instance = null;
            AQItem.Sets.Instance = null;
            GlowmaskData.ItemToGlowmask?.Clear();
            GlowmaskData.ItemToGlowmask = null;
            AQBuff.Sets.Instance = null;

            LoadCrossMod(unload: true);

            if (!Main.dedServ)
            {
                AQSound.rand = null;
                ArmorOverlays?.Dispose();
                ArmorOverlays = null;
                LegacyEffectCache.Unload();
                SkyGlimmerEvent.BGStarite._texture = null;
                NPCTalkUI = null;
                LoadMusic(unload: true);
                PrimitivesRenderer.Unload();
                FX.Unload();
                LegacyEffectCache.Unload();
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
                Particles.PreDrawProjectiles.UpdateParticles();
                Particles.PostDrawPlayers.UpdateParticles();

                Trails.PreDrawProjectiles.UpdateTrails();
            }

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
            NPCTalkUI.Update(gameTime);
            HookablesUI.Update(gameTime);
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
                    HookablesUI.Draw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.Game));
            }

            index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer("AQMod: Rename Item Interface",
                    () =>
                    {
                        NPCTalkUI.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
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
                    var aQMod = Instance;
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
        public static string GetText(string key, object arg)
        {
            return string.Format(GetText(key), arg);
        }
        public static string GetText(string key, params object[] args)
        {
            return string.Format(GetText(key), args);
        }
        public static ModTranslation GetTranslation(string key)
        {
            return AQText.modTranslations["Mods.AQMod." + key];
        }
        public static Texture2D Texture(string key)
        {
            return ModContent.GetInstance<AQMod>().GetTexture(key);
        }
    }
}