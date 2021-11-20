using AQMod.Assets;
using AQMod.Assets.Graphics;
using AQMod.Assets.Graphics.ParticlesLayers;
using AQMod.Assets.Graphics.SceneLayers;
using AQMod.Assets.ItemOverlays;
using AQMod.Assets.PlayerLayers.EquipOverlays;
using AQMod.Common;
using AQMod.Common.Config;
using AQMod.Common.CrossMod;
using AQMod.Common.DeveloperTools;
using AQMod.Common.NetCode;
using AQMod.Common.SceneLayers;
using AQMod.Common.Skies;
using AQMod.Common.UserInterface;
using AQMod.Common.Utilities;
using AQMod.Content;
using AQMod.Content.CursorDyes;
using AQMod.Content.MapMarkers;
using AQMod.Content.NoHitting;
using AQMod.Content.Quest.Lobster;
using AQMod.Content.WorldEvents;
using AQMod.Content.WorldEvents.AquaticEvent;
using AQMod.Content.WorldEvents.AtmosphericEvent;
using AQMod.Content.WorldEvents.CosmicEvent;
using AQMod.Content.WorldEvents.DemonicEvent;
using AQMod.Effects;
using AQMod.Effects.WorldEffects;
using AQMod.Items.Accessories;
using AQMod.Items.BuffItems.Foods;
using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Placeable;
using AQMod.Items.Tools.Bait;
using AQMod.Items.Vanities;
using AQMod.Localization;
using AQMod.NPCs;
using AQMod.NPCs.Boss.Crabson;
using AQMod.NPCs.Boss.Starite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
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
        /// <summary>
        /// The name of the mod, to be honest I need to remove all references of this and replace it with "AQMod" since like... why
        /// </summary>
        public const string ModName = nameof(AQMod);
        /// <summary>
        /// The key for the Any Noble Mushrooms recipe group
        /// </summary>
        public const string AnyNobleMushroom = "AQMod:AnyNobleMushroom";
        /// <summary>
        /// The key for the Any Energy recipe group
        /// </summary>
        public const string AnyEnergy = "AQMod:AnyEnergy";
        public const int SpaceLayerTile = 200;
        public const int SpaceLayer = SpaceLayerTile * 16;
        /// <summary>
        /// Basically guesses if the game is still active, should only really use for drawing methods that do things like summon dust
        /// </summary>
        public static bool GameWorldActive => Main.instance.IsActive && !Main.gamePaused;
        public static bool CanUseAssets => !Loading && Main.netMode != NetmodeID.Server;
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
        internal static bool DebugKeysPressed => Main.netMode == NetmodeID.SinglePlayer && AQConfigServer.Instance.debugCommand && Main.keyState.IsKeyDown(Keys.LeftShift) && Main.keyState.IsKeyDown(Keys.Q);
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
        /// <summary>
        /// A flag which gets raised if the mod detects that it's april fools. Use this for fun :)
        /// </summary>
        public static bool AprilFools { get; internal set; }
        /// <summary>
        /// Use this value to check if you should actually run unnecessary drawcode operations. Default value is 1.
        /// </summary>
        public static float EffectQuality { get; private set; }
        /// <summary>
        /// Use this value to tune down bright, flashy things. Default value is 1.
        /// </summary>
        public static float EffectIntensity { get; private set; }
        public static float EffectIntensityMinus => 2f - EffectIntensity;
        public static float Effect3Dness { get; private set; }
        public static float StariteBGMult { get; private set; }
        public static bool HarderOmegaStarite { get; private set; }
        public static bool EvilProgressionLock { get; private set; }
        public static bool ConfigReduceSpawnsWhenYouShould { get; private set; }
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
        /// <summary>
        /// The active instance of the Glimmer Event (event)
        /// </summary>
        public static GlimmerEvent CosmicEvent { get; set; }
        /// <summary>
        /// The active instance of the Azure Currents event
        /// </summary>
        public static AzureCurrents AtmosphericEvent { get; set; }

        public static ModifiableMusic CrabsonMusic { get; private set; }
        public static ModifiableMusic GlimmerEventMusic { get; private set; }
        public static ModifiableMusic OmegaStariteMusic { get; private set; }
        public static ModifiableMusic DemonSiegeMusic { get; private set; }

        internal static CrossModType Split { get; private set; }

        private static Vector2 _lastScreenZoom;
        private static Vector2 _lastScreenView;

        public AQMod()
        {
            Properties = new ModProperties()
            {
                Autoload = true,
                AutoloadBackgrounds = true,
                AutoloadGores = true,
                AutoloadSounds = true
            };
            AprilFools = false;
            if (DateTime.Now.Month == 4 && DateTime.Now.Day == 1)
            {
                AprilFools = true;
            }
            cachedLoadTasks = new List<CachedTask>();
            Loading = true;
        }

        public override void Load()
        {
            Loading = true;
            Unloading = false;
            AQText.Load();
            ImitatedWindyDay.Reset(resetNonUpdatedStatics: true);
            CursorDyes = new CursorDyeLoader();
            CursorDyes.Setup(setupStatics: true);
            RobsterHunts = new RobsterHuntLoader();
            RobsterHunts.Setup(setupStatics: true);
            CosmicEvent = new GlimmerEvent();
            AtmosphericEvent = new AzureCurrents();
            MapMarkers = new MapMarkerManager();
            MoonlightWallHelper.Instance = new MoonlightWallHelper();
            ModCallHelper.SetupCalls();

            On.Terraria.Chest.SetupShop += Chest_SetupShop;
            On.Terraria.NPC.Collision_DecideFallThroughPlatforms += NPC_Collision_DecideFallThroughPlatforms;
            On.Terraria.Main.UpdateTime += Main_UpdateTime;
            On.Terraria.Main.UpdateSundial += Main_UpdateSundial;
            On.Terraria.Player.FishingLevel += GetFishingLevel;
            On.Terraria.Player.AddBuff += Player_AddBuff;
            On.Terraria.Player.QuickBuff += Player_QuickBuff;

            var server = AQConfigServer.Instance;
            ApplyServerConfig(server);
            if (!Main.dedServ)
            {
                Load_ClientSide();
            }
        }

        private void Player_QuickBuff(On.Terraria.Player.orig_QuickBuff orig, Player self)
        {
            AQPlayer.IsQuickBuffing = true;
            orig(self);
            AQPlayer.IsQuickBuffing = false;
        }

        private void Player_AddBuff(On.Terraria.Player.orig_AddBuff orig, Player self, int type, int time1, bool quiet)
        {
            if (AQBuff.Sets.FoodBuff[type])
            {
                for (int i = 0; i < Player.MaxBuffs; i++)
                {
                    if (self.buffTime[i] > 16 && self.buffType[i] != type && AQBuff.Sets.FoodBuff[self.buffType[i]])
                    {
                        self.DelBuff(i);
                        i--;
                    }
                }
            }
            orig(self, type, time1, quiet);
        }

        private void Load_ClientSide()
        {
            var client = AQConfigClient.Instance;
            ApplyClientConfig(client);
            On.Terraria.ItemText.NewText += ItemText_NewText;
            On.Terraria.Main.DrawTiles += Main_DrawTiles;
            On.Terraria.Main.DrawPlayers += Main_DrawPlayers;
            ItemOverlays = new DrawOverlayLoader<ItemOverlayData>(Main.maxItems, () => ItemLoader.ItemCount);
            ArmorOverlays = new EquipOverlayLoader();
            TextureCache.Load();
            CrabsonMusic = new ModifiableMusic(MusicID.Boss1);
            GlimmerEventMusic = new ModifiableMusic(MusicID.MartianMadness);
            OmegaStariteMusic = new ModifiableMusic(MusicID.Boss4);
            DemonSiegeMusic = new ModifiableMusic(MusicID.PumpkinMoon);
            SkyManager.Instance[GlimmerEventSky.Name] = new GlimmerEventSky();
            GlimmerEventSky.Initialize();
            Trailshader.Setup();
            DrawUtils.LegacyTextureCache.Setup();
            EffectCache.Instance = new EffectCache(this, client, Logger, newInstance: true);
            WorldLayers = new SceneLayersManager();
            WorldLayers.Setup(loadHooks: true);
            WorldLayers.Register("GoreNest", new GoreNestLayer(test: false), SceneLayering.InfrontNPCs);
            WorldLayers.Register("UltimateSword", new UltimateSwordWorldOverlay(), SceneLayering.InfrontNPCs);
            WorldLayers.Register("ImpChains", new ImpChainLayer(), SceneLayering.BehindNPCs);
            WorldLayers.Register("CrabsonChains", new JerryCrabsonLayer(), SceneLayering.BehindTiles_BehindNPCs);
            WorldLayers.Register("ParticleLayer_PostDrawPlayer", new ParticleLayer_PostDrawPlayers(), SceneLayering.PostDrawPlayers);
            WorldLayers.Register(HotAndColdCurrentLayer.Name, new HotAndColdCurrentLayer(), HotAndColdCurrentLayer.Layer);
            WorldLayers.Register(CustomPickupTextLayer.Name, new CustomPickupTextLayer(), CustomPickupTextLayer.Layer);
            ScreenShakeManager.Load();
            StarbyteColorCache.Init();
            if (client.OutlineShader)
            {
                GameShaders.Misc["AQMod:Outline"] = new MiscShaderData(new Ref<Effect>(EffectCache.Instance.Outline), "OutlinePass");
                GameShaders.Misc["AQMod:OutlineColor"] = new MiscShaderData(new Ref<Effect>(EffectCache.Instance.Outline), "OutlineColorPass");
            }
            if (client.PortalShader)
            {
                GameShaders.Misc["AQMod:GoreNestPortal"] = new MiscShaderData(new Ref<Effect>(EffectCache.Instance.Portal), "DemonicPortalPass");
            }
            if (client.SpotlightShader)
            {
                GameShaders.Misc["AQMod:Spotlight"] = new MiscShaderData(new Ref<Effect>(EffectCache.Instance.Spotlight), "SpotlightPass");
                GameShaders.Misc["AQMod:FadeYProgressAlpha"] = new MiscShaderData(new Ref<Effect>(EffectCache.Instance.Spotlight), "FadeYProgressAlphaPass");
                GameShaders.Misc["AQMod:SpikeFade"] = new MiscShaderData(new Ref<Effect>(EffectCache.Instance.Spotlight), "SpikeFadePass");
            }
            Main.OnPreDraw += Main_OnPreDraw;
            WorldEffects = new List<WorldVisualEffect>();
        }

        private static void ItemText_NewText(On.Terraria.ItemText.orig_NewText orig, Item newItem, int stack, bool noStack, bool longText)
        {
            if (newItem != null && newItem.type > Main.maxItemTypes && newItem.modItem is Items.ICustomPickupText customPickupText)
            {
                if (customPickupText.OnSpawnText(newItem, stack, noStack, longText))
                    return;
            }
            orig(newItem, stack, noStack, longText);
        }

        private void Main_OnPreDraw(GameTime obj) // this is based greatly on the Stargoop from Spirit Mod, thank you, whoever coded that.
        {
            if (!Main.gameMenu && Main.graphics.GraphicsDevice != null && Main.spriteBatch != null)
            {
                HotAndColdCurrentLayer.DrawTarget();
            }
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

        private void Main_DrawPlayers(On.Terraria.Main.orig_DrawPlayers orig, Main self)
        {
            orig(self);
            BatcherMethods.StartBatch_GeneralEntities(Main.spriteBatch);
            WorldLayers.DrawLayer(SceneLayering.PostDrawPlayers);
            Main.spriteBatch.End();
        }

        private static void Main_UpdateSundial(On.Terraria.Main.orig_UpdateSundial orig)
        {
            orig();
            Main.dayRate += dayrateIncrease;
        }

        private static void Main_UpdateTime(On.Terraria.Main.orig_UpdateTime orig)
        {
            bool settingUpNight = false;
            Main.dayRate += dayrateIncrease;
            if (Main.time + Main.dayRate > Main.dayLength)
                settingUpNight = true;
            orig();
            dayrateIncrease = 0;
            if (settingUpNight)
            {
                OnTurnNight();
            }
        }

        /// <summary>
        /// Modifies <see cref="Chest.SetupShop(int)"/> to make the <see cref="BusinessCard"/> and <see cref="BlurryDiscountCard"/> work by modifying <see cref="AQPlayer.discountPercentage"/>
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="type"></param>
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

        /// <summary>
        /// Modifies NPC.Collision_DecideFallThroughPlatforms so that some npcs can... actually fall through platforms... is this implented in ModNPC at all? Since I can't find it :(
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <returns></returns>
        private static bool NPC_Collision_DecideFallThroughPlatforms(On.Terraria.NPC.orig_Collision_DecideFallThroughPlatforms orig, NPC self) =>
            self.type > Main.maxNPCTypes &&
            self.modNPC is IDecideFallThroughPlatforms decideToFallThroughPlatforms ?
            decideToFallThroughPlatforms.Decide() : orig(self);

        /// <summary>
        /// Modifies <see cref="Main.DrawTiles(bool, int)"/> so that special tile draw coordinates can be refreshed.
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="solidOnly"></param>
        /// <param name="waterStyleOverride"></param>
        private static void Main_DrawTiles(On.Terraria.Main.orig_DrawTiles orig, Main self, bool solidOnly, int waterStyleOverride)
        {
            if (!solidOnly)
            {
                GoreNestLayer.RefreshCoords();
            }
            orig(self, solidOnly, waterStyleOverride);
        }

        public override void PostSetupContent()
        {
            AQBuff.Sets.Setup();
            AQNPC.Sets.LoadSets(); // Initializes sets for npcs
            NoHitManager.Setup();
            AQProjectile.Sets.LoadSets(); // Initializes sets for projectiles
            DemonSiege.Setup(); // Sets up the Demon Siege event
            GlimmerEvent.Setup();
            BossChecklistHelper.Setup(this); // Sets up boss checklist entries for events and bosses
            AQItem.Sets.Setup();
            MapMarkers.Setup(setupStatics: true);
            invokeTasks();
            cachedLoadTasks.Clear();
        }

        public override void AddRecipeGroups()
        {
            var r = new RecipeGroup(() => Language.GetTextValue(AQText.Key + "Common.RecipeGroup_AnyNobleMushroom"), new[]
            {
                ModContent.ItemType<ArgonMushroom>(),
                ModContent.ItemType<KryptonMushroom>(),
                ModContent.ItemType<XenonMushroom>(),
            });
            RecipeGroup.RegisterGroup(AnyNobleMushroom, r);
            r = new RecipeGroup(() => Language.GetTextValue(AQText.Key + "Common.RecipeGroup_AnyEnergy"), new[]
            {
                ModContent.ItemType<UltimateEnergy>(),
                ModContent.ItemType<AquaticEnergy>(),
                ModContent.ItemType<AtmosphericEnergy>(),
                ModContent.ItemType<OrganicEnergy>(),
                ModContent.ItemType<DemonicEnergy>(),
                ModContent.ItemType<CosmicEnergy>(),
            });
            RecipeGroup.RegisterGroup(AnyEnergy, r);
        }

        public override void AddRecipes()
        {
            invokeTasks();
            cachedLoadTasks = null;

            Split = new CrossModType("Split");
            Split.Load();

            // TODO: Add content method instances that run here, which other mods can add to, so that adding to specific things is less hellish.
            Loading = false; // Sets Loading to false, so that some things no longer accept new content.
            RobsterHunts.SetupHunts();
            if (!Main.dedServ)
            {
                ItemOverlays.Finish();
            }
            CelesitalEightBall.ResetStatics();

            var r = new ModRecipe(this);
            r.AddIngredient(ItemID.Bottle);
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>());
            r.AddIngredient(ItemID.Cloud, 20);
            r.SetResult(ItemID.CloudinaBottle);
            r.AddRecipe();
            r = new ModRecipe(this);
            r.AddIngredient(ItemID.CloudinaBottle);
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>(), 3);
            r.AddIngredient(ItemID.SnowCloudBlock, 40);
            r.SetResult(ItemID.BlizzardinaBottle);
            r.AddRecipe();
            r = new ModRecipe(this);
            r.AddIngredient(ItemID.CloudinaBottle);
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>(), 3);
            r.AddIngredient(ItemID.SandBlock, 40);
            r.SetResult(ItemID.SandstorminaBottle);
            r.AddRecipe();

            r = new ModRecipe(this);
            r.AddIngredient(ItemID.HermesBoots);
            r.AddIngredient(ItemID.WaterWalkingPotion);
            r.AddIngredient(ModContent.ItemType<AquaticEnergy>(), 3);
            r.AddIngredient(ModContent.ItemType<CrabShell>(), 10);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(ItemID.WaterWalkingBoots);
            r.AddRecipe();

            r = new ModRecipe(this);
            r.AddIngredient(ItemID.GreenThread);
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>());
            r.AddTile(TileID.WorkBenches);
            r.SetResult(ItemID.HermesBoots);
            r.AddRecipe();
            r = new ModRecipe(this);
            r.AddIngredient(ItemID.HermesBoots);
            r.AddIngredient(ItemID.Snowball, 100);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(ItemID.FlurryBoots);
            r.AddRecipe();
            r = new ModRecipe(this);
            r.AddIngredient(ItemID.HermesBoots);
            r.AddIngredient(ItemID.Goldfish, 5);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(ItemID.SailfishBoots);
            r.AddRecipe();

            r = new ModRecipe(this);
            r.AddIngredient(ModContent.ItemType<BlurryDiscountCard>());
            r.AddIngredient(ModContent.ItemType<DemonicEnergy>(), 5);
            r.AddIngredient(ItemID.SoulofNight, 8);
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.DiscountCard);
            r.AddRecipe();

            foreach (var u in DemonSiege._upgrades)
            {
                DowngradeSiegeWeaponRecipe(u);
            }

            FargosQOLStuff.Setup(this);
        }

        /// <summary>
        /// Automatically creates and registers a recipe used to downgrade a Demon Siege weapon back into its original item <para>TODO: Make the recipe keep the reforge of the item when you craft the downgrade</para>
        /// </summary>
        /// <param name="u"></param>
        private void DowngradeSiegeWeaponRecipe(DemonSiegeUpgrade u)
        {
            var r = new ModRecipe(this);
            r.AddIngredient(u.rewardItem);
            r.AddIngredient(ModContent.ItemType<DemonicEnergy>());
            r.AddTile(TileID.DemonAltar);
            r.SetResult(u.baseItem);
            r.AddRecipe();
        }

        public override void Unload()
        {
            // outside of AQMod
            Loading = true;
            Unloading = true;
            HuntSystem.Unload();

            // in: AddRecipes()
            CelesitalEightBall.ResetStatics();
            ItemOverlays = null;
            if (Split != null)
            {
                Split.Unload();
                Split = null;
            }

            // in: PostSetupContent()
            cachedLoadTasks = null;
            MapMarkers = null;
            AQItem.Sets.Unload();
            DemonSiege.Unload();
            AQProjectile.Sets.UnloadSets();
            NoHitManager.Unload();
            AQNPC.Sets.UnloadSets();
            AQBuff.Sets.Unload();

            // in: Load()
            // v doesn't load on server v
            if (Main.dedServ)
            {
                DrawUtils.UnloadAssets();
                WorldEffects = null;
                StarbyteColorCache.Unload();
                ScreenShakeManager.Unload();
                ArmorOverlays = null;
                if (WorldLayers != null)
                {
                    WorldLayers.Unload();
                    WorldLayers = null;
                }
                EffectCache.Instance = null;
                GlimmerEventSky.Unload();
                DemonSiegeMusic = null;
                OmegaStariteMusic = null;
                GlimmerEventMusic = null;
                CrabsonMusic = null;
                TextureCache.Unload();
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

        public override void MidUpdateInvasionNet()
        {
            ImitatedWindyDay.Reset(resetNonUpdatedStatics: false);
            ImitatedWindyDay.UpdateWindyDayFlags();
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
                CosmicEvent.stariteProjectileColor = CosmicEvent.StariteDisco ? new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0) : StariteProjectileColor;
            }
            else
            {
                CosmicEvent.stariteProjectileColor = CosmicEvent.StariteDisco ? new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0) : GlimmerEvent.StariteProjectileColorOrig;
            }
        }

        public override void MidUpdateNPCGore()
        {
            DemonSiege.UpdateEvent();

            if (OmegaStariteScene.OmegaStariteIndexCache > -1 && !Main.npc[OmegaStariteScene.OmegaStariteIndexCache].active)
            {
                OmegaStariteScene.OmegaStariteIndexCache = -1;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && spawnStarite)
            {
                OmegaStariteScene.OmegaStariteIndexCache = (short)NPC.NewNPC(AQMod.CosmicEvent.tileX * 16 + 8, AQMod.CosmicEvent.tileY * 16 - 1600, ModContent.NPCType<OmegaStarite>(), 0, OmegaStarite.PHASE_NOVA, 0f, 0f, 0f, Main.myPlayer);
                OmegaStariteScene.SceneType = 1;
                spawnStarite = false;
                BroadcastMessage("Mods.AQMod.Common.AwakenedOmegaStarite", BossMessage);
            }

            if (CrabSeason.CrabsonCachedID > -1 && !Main.npc[CrabSeason.CrabsonCachedID].active)
            {
                CrabSeason.CrabsonCachedID = -1;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                WorldLayers.UpdateLayers();
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
            else if (CosmicEvent.IsActive && player.position.Y < Main.worldSurface * 16.0)
            {
                int tileDistance = (int)(player.Center.X / 16 - CosmicEvent.tileX).Abs();
                if (tileDistance < GlimmerEvent.MaxDistance)
                {
                    music = GlimmerEventMusic.GetMusicID();
                    priority = MusicPriority.Event;
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
            CursorDyeLoader.UpdateColor();
            var index = layers.FindIndex((l) => l.Name.Equals("Vanilla: Invasion Progress Bars"));
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer("AQMod: Invasion Progress Bar", delegate ()
                {
                    InvasionUI.Apply();
                    return true;
                }, InterfaceScaleType.UI));
            }
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            var messageID = NetworkingMethods.GetMessage(reader);

            switch (messageID)
            {
                case NetType.SummonOmegaStarite:
                {
                    if (Main.netMode == NetmodeID.Server)
                    {
                        spawnStarite = true;
                    }
                }
                break;

                case NetType.UpdateGlimmerEvent:
                {
                    CosmicEvent.tileX = reader.ReadUInt16();
                    CosmicEvent.tileY = reader.ReadUInt16();
                    CosmicEvent.spawnChance = reader.ReadInt32();
                    CosmicEvent.StariteDisco = reader.ReadBoolean();
                    CosmicEvent.deactivationTimer = reader.ReadInt32();
                }
                break;

                case NetType.UpdateAQPlayerCelesteTorus:
                {
                    var player = Main.player[reader.ReadByte()];
                    var aQPlayer = player.GetModPlayer<AQPlayer>();
                    aQPlayer.celesteTorusX = reader.ReadSingle();
                    aQPlayer.celesteTorusY = reader.ReadSingle();
                    aQPlayer.celesteTorusZ = reader.ReadSingle();
                }
                break;

                case NetType.UpdateAQPlayerEncoreKills:
                {
                    var player = Main.player[reader.ReadByte()];
                    var aQPlayer = player.GetModPlayer<AQPlayer>();
                    byte[] buffer = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));
                    aQPlayer.DeserialzeBossKills(buffer);
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
            EffectQuality = clientConfig.EffectQuality;
            EffectIntensity = clientConfig.EffectIntensity;
            Effect3Dness = clientConfig.Effect3D;
            ShowBackgroundStarites = clientConfig.BackgroundStarites;
            TonsofScreenShakes = clientConfig.TonsofScreenShakes;
            StariteProjectileColor = clientConfig.StariteProjColor;
            MapBlipColor = clientConfig.MapBlipColor;
            StariteAuraColor = clientConfig.StariteAuraColor;
            StariteBGMult = clientConfig.StariteBackgroundLight;
        }

        public static void ApplyServerConfig(AQConfigServer serverConfig)
        {
            HarderOmegaStarite = serverConfig.harderOmegaStarite;
            EvilProgressionLock = serverConfig.evilProgressionLock;
            ConfigReduceSpawnsWhenYouShould = serverConfig.reduceSpawns;
        }

        public static void OnTurnNight()
        {
            CosmicEvent.OnTurnNight();
            if (Main.netMode != NetmodeID.Server)
            {
                GlimmerEventSky.InitNight();
            }
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

        internal static bool DryadCanMoveIn()
        {
            return NPC.downedSlimeKing || NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3 || NPC.downedQueenBee || Main.hardMode;
        }

        internal static bool reduceSpawnrates()
        {
            return NPC.AnyNPCs(ModContent.NPCType<OmegaStarite>()) || NPC.AnyNPCs(ModContent.NPCType<JerryCrabson>());
        }

        public static bool ShouldReduceSpawns()
        {
            return ConfigReduceSpawnsWhenYouShould && reduceSpawnrates();
        }

        public static int MultIntensity(int input)
        {
            return (int)(input * EffectIntensity);
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