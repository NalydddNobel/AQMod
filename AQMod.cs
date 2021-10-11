using AQMod.Assets;
using AQMod.Assets.SceneLayers;
using AQMod.Common;
using AQMod.Common.Commands;
using AQMod.Common.Config;
using AQMod.Common.CrossMod;
using AQMod.Common.NPCIMethods;
using AQMod.Common.UI;
using AQMod.Common.Utilities;
using AQMod.Content;
using AQMod.Content.CrossMod;
using AQMod.Content.CursorDyes;
using AQMod.Content.SceneLayers;
using AQMod.Content.Skies;
using AQMod.Content.WorldEvents;
using AQMod.Content.WorldEvents.Glimmer;
using AQMod.Content.WorldEvents.Siege;
using AQMod.Effects;
using AQMod.Effects.Screen;
using AQMod.Effects.WorldEffects;
using AQMod.Items;
using AQMod.Items.Accessories.ShopCards;
using AQMod.Items.Placeable;
using AQMod.Items.TagItems.Starbyte;
using AQMod.Items.Vanities;
using AQMod.Items.Vanities.Dyes;
using AQMod.Localization;
using AQMod.NPCs;
using AQMod.NPCs.Starite;
using AQMod.NPCs.Town.Robster;
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
        /// <summary>
        /// Basically guesses if the game is still active, should only really use for drawing methods that do things like summon dust
        /// </summary>
        public static bool GameWorldActive => Main.instance.IsActive && !Main.gamePaused;
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
        internal static bool DebugKeysPressed => Main.netMode == NetmodeID.SinglePlayer && AQConfigServer.Instance.debugCommand && Main.keyState.IsKeyDown(Keys.LeftShift) && Main.keyState.IsKeyDown(Keys.Q);

        public static bool spawnStarite;
        public static int dayrateIncrease;

        /// <summary>
        /// This is normally used to prevent adding new content to arrays after the mod has loaded and blah blah. It's also used to prevent some drawing which might happen on the title screen before assets fully load.
        /// </summary>
        internal static bool Loading { get; private set; }
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
        public static int MultIntensity(int input)
        {
            return (int)(input * EffectIntensity);
        }
        public static bool TonsofScreenShakes { get; private set; }
        /// <summary>
        /// Whether or not the background starites from the Glimmer Event should be shown. Default value is true
        /// </summary>
        public static bool ShowBackgroundStarites { get; private set; }

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
            Loading = true;
        }

        /// <summary>
        /// The active instance of the Cursor Dyes Loader.
        /// </summary>
        public static CursorDyeLoader CursorDyes { get; private set; }
        /// <summary>
        /// The active instance of the Robster Hunts Loader.
        /// </summary>
        public static RobsterHuntLoader RobsterHunts { get; private set; }
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
        public static ArmorOverlayLoader ArmorOverlays { get; private set; }
        /// <summary>
        /// The active list of World Effects, this is not initialized on the server
        /// </summary>
        public static List<WorldVisualEffect> WorldEffects { get; private set; }
        /// <summary>
        /// The active instance of the Glimmer Event
        /// </summary>
        public static GlimmerEvent glimmerEvent { get; private set; }

        public static ModifiableMusic CrabsonMusic { get; private set; }
        public static ModifiableMusic GlimmerEventMusic { get; private set; }
        public static ModifiableMusic OmegaStariteMusic { get; private set; }
        public static ModifiableMusic DemonSiegeMusic { get; private set; }

        public override void Load()
        {
            Loading = true;
            AssetManager.AssetsLoaded = false;
            AQText.Load();
            CursorDyes = new CursorDyeLoader();
            CursorDyes.Setup();
            RobsterHunts = new RobsterHuntLoader();
            RobsterHunts.Setup();
            glimmerEvent = new GlimmerEvent();
            AQPlayer.Setup();
            AQCommand.LoadCommands();
            MoonlightWallHelper.Instance = new MoonlightWallHelper();
            On.Terraria.Chest.SetupShop += Chest_SetupShop;
            //On.Terraria.Projectile.NewProjectile_float_float_float_float_int_int_float_int_float_float += Projectile_NewProjectile_float_float_float_float_int_int_float_int_float_float;
            On.Terraria.NPC.Collision_DecideFallThroughPlatforms += NPC_Collision_DecideFallThroughPlatforms;
            On.Terraria.Main.UpdateTime += Main_UpdateTime;
            On.Terraria.Main.UpdateSundial += Main_UpdateSundial;
            On.Terraria.Main.DrawTiles += Main_DrawTiles;
            if (!Main.dedServ)
            {
                var client = AQConfigClient.Instance;
                ApplyClientConfig(client);
                ItemOverlays = new DrawOverlayLoader<ItemOverlayData>(Main.maxItems, () => ItemLoader.ItemCount);
                ArmorOverlays = new ArmorOverlayLoader();
                TextureCache.Load();
                CrabsonMusic = new ModifiableMusic(MusicID.Boss1);
                GlimmerEventMusic = new ModifiableMusic(MusicID.MartianMadness);
                OmegaStariteMusic = new ModifiableMusic(MusicID.Boss4);
                DemonSiegeMusic = new ModifiableMusic(MusicID.PumpkinMoon);
                Parralax.RefreshParralax();
                SkyManager.Instance[GlimmerEventSky.Name] = new GlimmerEventSky();
                GlimmerEventSky.Initialize();
                Trailshader.Setup();
                DrawUtils.LegacyTextureCache.Setup();
                EffectCache.Instance = new EffectCache(this, client, Logger);
                WorldLayers = new SceneLayersManager();
                WorldLayers.Setup(loadHooks: true);
                WorldLayers.AddLayer("GoreNest", new GoreNestLayer(test: false), SceneLayering.InfrontNPCs);
                WorldLayers.AddLayer("UltimateSword", new UltimateSwordWorldOverlay(), SceneLayering.InfrontNPCs);
                WorldLayers.AddLayer("ImpChains", new ImpChainLayer(), SceneLayering.BehindNPCs);
                WorldLayers.AddLayer("CrabsonChains", new JerryCrabsonLayer(), SceneLayering.BehindTiles_BehindNPCs);
                GameScreenManager.Load();
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
                WorldEffects = new List<WorldVisualEffect>();
                AssetManager.AssetsLoaded = true;
            }
        }

        private static void Main_UpdateSundial(On.Terraria.Main.orig_UpdateSundial orig)
        {
            orig();
            Main.dayRate += dayrateIncrease;
        }

        private static void Main_UpdateTime(On.Terraria.Main.orig_UpdateTime orig)
        {
            bool settingUpNight = false;
            if (Main.time + Main.dayRate > Main.dayLength)
                settingUpNight = true;
            Main.dayRate += dayrateIncrease;
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
            self.type > Main.maxNPCTypes && self.modNPC is IDecideFallThroughPlatforms decideToFallThroughPlatforms ? decideToFallThroughPlatforms.Decide() : orig(self);

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
            AQNPC.Sets.Setup(); // Initializes sets for npcs
            AQProjectile.Sets.Setup(); // Initializes sets for projectiles
            DemonSiege.Setup(); // Sets up the Demon Siege event
            GlimmerEvent.Setup();
            BossChecklistHelper.Setup(this); // Sets up boss checklist entries for events and bosses
            FargosQOLStuff.Setup(this); // Sets up boss summons for Fargowiltas,
            AQItem.Sets.Setup();
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
            // TODO: Add content method instances that run here, which other mods can add to, so that adding to specific things is less hellish.
            Loading = false; // Sets Loading to false, so that some things no longer accept new content.
            RobsterHunts.SetupHunts();
            if (!Main.dedServ)
            {
                ItemOverlays.Finish();
            }
            CelesitalEightBall.Text = Language.GetTextValue(AQText.Key + "Common.EightballAnswer20");

            var r = new ModRecipe(this);
            r.AddIngredient(ItemID.Bottle);
            r.AddIngredient(ModContent.ItemType<AtmosphericEnergy>());
            r.AddIngredient(ItemID.Cloud, 20);
            r.SetResult(ItemID.CloudinaBottle);
            r.AddRecipe();
            r = new ModRecipe(this);
            r.AddIngredient(ItemID.Bottle);
            r.AddIngredient(ModContent.ItemType<AtmosphericEnergy>());
            r.AddIngredient(ItemID.RainCloud, 20);
            r.SetResult(ItemID.TsunamiInABottle);
            r.AddRecipe();
            r = new ModRecipe(this);
            r.AddIngredient(ItemID.CloudinaBottle);
            r.AddIngredient(ModContent.ItemType<AtmosphericEnergy>(), 3);
            r.AddIngredient(ItemID.SnowCloudBlock, 40);
            r.SetResult(ItemID.BlizzardinaBottle);
            r.AddRecipe();
            r = new ModRecipe(this);
            r.AddIngredient(ItemID.CloudinaBottle);
            r.AddIngredient(ModContent.ItemType<AtmosphericEnergy>(), 3);
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
            r.AddIngredient(ModContent.ItemType<AtmosphericEnergy>());
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

            if (FargosQOLStuff.FargowiltasActive)
            {
                int[] itemArray = new int[] { ModContent.ItemType<EnchantedDye>(), ModContent.ItemType<RainbowOutlineDye>(), ModContent.ItemType<DiscoDye>(), };
                int item = ModContent.ItemType<OmegaStariteTrophy>();
                for (int i = 0; i < itemArray.Length; i++)
                {
                    for (int j = 0; j < itemArray.Length; j++)
                    {
                        if (j != i)
                        {
                            r = new ModRecipe(this);
                            r.AddIngredient(itemArray[i]);
                            r.AddIngredient(item);
                            r.AddTile(TileID.Solidifier);
                            r.SetResult(itemArray[j]);
                            r.AddRecipe();
                        }
                    }
                }
            }
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
            HuntSystem.Unload();

            // in: AddRecipes()
            CelesitalEightBall.Text = null;
            ItemOverlays = null;

            // in: PostSetupContent()
            AQItem.Sets.Unload();
            DemonSiege.Unload();
            AQNPC.Sets.Setup();

            // in: Load()
            // v doesn't load on server v
            if (Main.dedServ)
            {
                AssetManager.AssetsLoaded = false; // set assets loaded to false here so that anything that is using assets at the title screen knows to stop before it unloads
                DrawUtils.UnloadAssets();
                if (WorldEffects != null)
                {
                    WorldEffects.Clear();
                    WorldEffects = null;
                }
                StarbyteColorCache.Unload();
                GameScreenManager.Unload();
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
            MoonlightWallHelper.Instance = null;
            AQCommand.UnloadCommands();
            if (CursorDyes != null)
            {
                CursorDyes.Unload();
                CursorDyes = null;
            }
            AQText.Unload();
        }

        public override void PreUpdateEntities()
        {
            if (Main.netMode != NetmodeID.Server)
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
            if (Main.netMode != NetmodeID.Server)
            {
                glimmerEvent.stariteProjectileColor = glimmerEvent.StariteDisco ? new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0) : AQConfigClient.Instance.StariteProjColor;
            }
            else
            {
                glimmerEvent.stariteProjectileColor = glimmerEvent.StariteDisco ? new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0) : GlimmerEvent.StariteProjectileColorOrig;
            }
        }

        public override void MidUpdateNPCGore()
        {
            DemonSiege.UpdateEvent();

            if (OmegaStariteSceneManager.OmegaStariteIndexCache > -1 && !Main.npc[OmegaStariteSceneManager.OmegaStariteIndexCache].active)
            {
                OmegaStariteSceneManager.OmegaStariteIndexCache = -1;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && spawnStarite)
            {
                OmegaStariteSceneManager.OmegaStariteIndexCache = (short)NPC.NewNPC(AQMod.glimmerEvent.tileX * 16 + 8, AQMod.glimmerEvent.tileY * 16 - 1600, ModContent.NPCType<OmegaStarite>(), 0, OmegaStarite.PHASE_NOVA, 0f, 0f, 0f, Main.myPlayer);
                OmegaStariteSceneManager.Scene = 1;
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

            if (Main.myPlayer == -1 || Main.gameMenu || !Main.LocalPlayer.active || !AssetManager.AssetsLoaded)
            {
                return;
            }

            var player = Main.LocalPlayer;
            if (DemonSiege.CloseEnoughToDemonSiege(player))
            {
                music = DemonSiegeMusic.GetMusicID();
                priority = MusicPriority.Event;
            }
            else if (glimmerEvent.IsActive && player.position.Y < Main.worldSurface * 16.0)
            {
                int tileDistance = (int)(player.Center.X / 16 - glimmerEvent.tileX).Abs();
                if (tileDistance < GlimmerEvent.MaxDistance)
                {
                    music = GlimmerEventMusic.GetMusicID();
                    priority = MusicPriority.Event;
                }
            }
        }

        public static void SetupNewMusic()
        {
            if (Main.myPlayer == -1 || Main.gameMenu || !Main.LocalPlayer.active || !AssetManager.AssetsLoaded)
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
            MapInterface.Apply(ref mouseText);
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
            var messageID = Networking.GetMessage(reader);

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
                    glimmerEvent.tileX = reader.ReadUInt16();
                    glimmerEvent.tileY = reader.ReadUInt16();
                    glimmerEvent.spawnChance = reader.ReadInt32();
                    glimmerEvent.StariteDisco = reader.ReadBoolean();
                    glimmerEvent.deactivationTimer = reader.ReadInt32();
                }
                break;
            }
        }

        public static void ApplyClientConfig(AQConfigClient clientConfig)
        {
            EffectQuality = clientConfig.EffectQuality;
            EffectIntensity = clientConfig.EffectIntensity;
            ShowBackgroundStarites = clientConfig.BackgroundStarites;
            TonsofScreenShakes = clientConfig.TonsofScreenShakes;
        }

        public static void OnTurnNight()
        {
            glimmerEvent.OnTurnNight();
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
    }
}