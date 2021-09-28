using AQMod.Assets;
using AQMod.Assets.ItemOverlays;
using AQMod.Assets.SceneLayers;
using AQMod.Common;
using AQMod.Common.Commands;
using AQMod.Common.Config;
using AQMod.Common.NPCIMethods;
using AQMod.Common.UI;
using AQMod.Common.UserInterface;
using AQMod.Common.Utilities;
using AQMod.Content;
using AQMod.Content.CursorDyes;
using AQMod.Content.Skies;
using AQMod.Content.WorldEvents;
using AQMod.Content.WorldEvents.DemonSiege;
using AQMod.Effects;
using AQMod.Effects.Screen;
using AQMod.Items;
using AQMod.Items.Accessories.ShopCards;
using AQMod.Items.Energies;
using AQMod.Items.GrapplingHooks;
using AQMod.Items.Placeable.Mushrooms;
using AQMod.Items.TagItems.Starbyte;
using AQMod.Items.Vanities;
using AQMod.Items.Weapons.Melee.Flails;
using AQMod.Localization;
using AQMod.NPCs.SiegeEvent;
using AQMod.NPCs.Starite;
using AQMod.NPCs.Town.Robster;
using AQMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

namespace AQMod
{
    public class AQMod : Mod
    {
        public static AQMod Instance => ModContent.GetInstance<AQMod>();
        public const string ModName = nameof(AQMod);
        public static bool GameWorldActive => Main.instance.IsActive && !Main.gamePaused;
        internal static bool Loading { get; private set; }


        internal const float TwoPiOver5 = MathHelper.TwoPi / 5f;
        internal const float TwoPiOver8 = MathHelper.TwoPi / 8f;

        public static short omegaStariteIndexCache = -1;
        public static byte omegaStariteScene;
        public static bool summonOmegaStarite;

        public static bool AprilFools { get; internal set; }

        internal static Color BossMessage => new Color(175, 75, 255, 255);

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

        internal static CursorDyeLoader CursorDyes { get; private set; }
        internal static RobsterHuntLoader RobsterHunts { get; private set; }

        public override void Load()
        {
            Loading = true;
            AssetManager.AssetsLoaded = false;
            AQText.Load();
            CursorDyes = new CursorDyeLoader();
            CursorDyes.Setup();
            RobsterHunts = new RobsterHuntLoader();
            RobsterHunts.Setup();
            AQPlayer.Setup();
            AQCommand.LoadCommands();
            GlimmerEvent.StariteProjectileColor = GlimmerEvent.StariteProjectileColorOrig;
            MoonlightWallHelper.Instance = new MoonlightWallHelper();
            AQUtils.Setup();
            WorldTimeManager.Setup();
            On.Terraria.Chest.SetupShop += Chest_SetupShop;
            //On.Terraria.Projectile.NewProjectile_float_float_float_float_int_int_float_int_float_float += Projectile_NewProjectile_float_float_float_float_int_int_float_int_float_float;
            On.Terraria.NPC.Collision_DecideFallThroughPlatforms += NPC_Collision_DecideFallThroughPlatforms;
            On.Terraria.Main.DrawTiles += Main_DrawTiles;
            if (!Main.dedServ)
            {
                var client = AQConfigClient.Instance;
                TextureCache.Load();
                AQMusicManager.Initialize();
                Parralax.RefreshParralax();
                SkyManager.Instance[GlimmerEventSky.Name] = new GlimmerEventSky();
                GlimmerEventSky.Initialize();
                Trailshader.Setup();
                DrawUtils.Textures.Setup();
                ChainTextures.Setup();
                EffectCache.Instance = new EffectCache(this, client, Logger);
                WorldDrawLayers.Setup();
                GameScreenManager.Load();
                StarbyteColorCache.Load();
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
                DrawUtils.FinalSetup();
                AssetManager.AssetsLoaded = true;
            }
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

        private int Projectile_NewProjectile_float_float_float_float_int_int_float_int_float_float(On.Terraria.Projectile.orig_NewProjectile_float_float_float_float_int_int_float_int_float_float orig, float X, float Y, float SpeedX, float SpeedY, int Type, int Damage, float KnockBack, int Owner, float ai0, float ai1)
        {
            int p = orig(X, Y, SpeedX, SpeedY, Type, Damage, KnockBack, Owner, ai0, ai1);
            if (Owner >= 0 && Owner < 255 && Main.player[Owner].active && !Main.player[Owner].dead)
            {
                Main.player[Owner].GetModPlayer<AQPlayer>().TagProjectile(Main.projectile[p]);
            }
            return p;
        }

        private static bool NPC_Collision_DecideFallThroughPlatforms(On.Terraria.NPC.orig_Collision_DecideFallThroughPlatforms orig, NPC self) =>
            self.type > Main.maxNPCTypes && self.modNPC is IDecideFallThroughPlatforms decideToFallThroughPlatforms ? decideToFallThroughPlatforms.Decide() : orig(self);

        private static void Main_DrawTiles(On.Terraria.Main.orig_DrawTiles orig, Terraria.Main self, bool solidOnly, int waterStyleOverride)
        {
            if (!solidOnly)
            {
                GoreNestWorldOverlay.RefreshCoords();
            }
            orig(self, solidOnly, waterStyleOverride);
        }

        public override void PostSetupContent()
        {
            AQNPC.Sets.Setup();
            AQProjectile.Sets.Setup();
            MapMarkerPlayer.Setup();
            DemonSiege.Setup();
            bosschecklist.setup(this);
            ModHelpers.Fargowiltas.SetupBossSummons(this);
            AQItem.Sets.Setup();
        }

        public const string AnyNobleMushroom = "AQMod:AnyNobleMushroom";
        public const string AnyEnergy = "AQMod:AnyEnergy";

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
            Loading = false;
            RobsterHunts.SetupHunts();
            ItemOverlayLoader.Finish();
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
        }

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
            ItemOverlayLoader.Unload();

            // in: PostSetupContent()
            AQItem.Sets.Unload();
            DemonSiege.Unload();
            MapMarkerPlayer.Unload();
            AQNPC.Sets.Setup();

            // in: Load()
            // v doesn't load on server v
            AssetManager.AssetsLoaded = false; // set assets loaded to false here so that anything that is using assets at the title screen knows to stop before it unloads
            DrawUtils.UnloadAssets();
            StarbyteColorCache.Unload();
            GameScreenManager.Unload();
            WorldDrawLayers.Unload();
            EffectCache.Instance = null;
            GlimmerEventSky.Unload();
            AQMusicManager.Unload();
            TextureCache.Unload();
            // ^ doesn't load on server ^
            AQUtils.Unload();
            MoonlightWallHelper.Instance = null;
            AQCommand.UnloadCommands();
            if (CursorDyes != null)
            {
                CursorDyes.Unload();
                CursorDyes = null;
            }
            AQText.Unload();
        }

        public override object Call(params object[] args)
        {
            if (args.Length == 0 || !(args[0] is string callType))
                return null;
            switch (callType)
            {
                case "GlimmerEvent_GlimmerChance":
                if (args.Length > 1)
                {
                    try
                    {
                        GlimmerEvent.GlimmerChance = (int)args[1];
                    }
                    catch
                    {
                    }
                }
                return GlimmerEvent.GlimmerChance;

                case "GlimmerEvent_X":
                if (args.Length > 1)
                {
                    try
                    {
                        GlimmerEvent.X = (ushort)args[1];
                    }
                    catch
                    {
                    }
                }
                return GlimmerEvent.X;

                case "GlimmerEvent_Y":
                if (args.Length > 1)
                {
                    try
                    {
                        GlimmerEvent.Y = (ushort)args[1];
                    }
                    catch
                    {
                    }
                }
                return GlimmerEvent.Y;

                case "GlimmerEvent_FakeActive":
                return GlimmerEvent.FakeActive;

                case "GlimmerEvent_ActuallyActive":
                return GlimmerEvent.ActuallyActive;

                case "GlimmerEvent_Active":
                {
                    if (args.Length > 1 && args[1] is bool flag)
                    {
                        if (flag)
                        {
                            if (args.Length > 2 && args[2] is bool flag2)
                            {
                                GlimmerEvent.Activate(genuine: flag2);
                            }
                            else
                            {
                                GlimmerEvent.Activate(genuine: true);
                            }
                        }
                        else
                        {
                            GlimmerEvent.Deactivate();
                        }
                    }
                    return GlimmerEvent.IsActive;
                }
            }
            return null;
        }

        public override void PreUpdateEntities()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < DrawUtils.WorldEffects.Count; i++)
                {
                    var v = DrawUtils.WorldEffects[i];
                    if (!v.Update())
                    {
                        DrawUtils.WorldEffects.RemoveAt(i);
                        i--;
                    }
                }
            }
            GlimmerEvent.StariteProjectileColor = GlimmerEvent.StariteDisco ? new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0) : GlimmerEvent.StariteProjectileColorOrig;
        }

        public override void MidUpdateNPCGore()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && summonOmegaStarite && omegaStariteIndexCache == -1)
            {
                SummonOmegaStarite();
            }
            if (Main.netMode != NetmodeID.Server && Main.drawToScreen)
            {
                WorldDrawLayers.Update();
            }
            DemonSiege.UpdateEvent();
            if (omegaStariteIndexCache > -1 && !Main.npc[omegaStariteIndexCache].active)
            {
                omegaStariteIndexCache = -1;
            }
            if (CrabSeason.CrabsonCachedID > -1 && !Main.npc[CrabSeason.CrabsonCachedID].active)
            {
                CrabSeason.CrabsonCachedID = -1;
            }
        }

        public override void UpdateMusic(ref int music, ref MusicPriority priority)
        {
            AQText.UpdateCallback();

            if (Main.myPlayer == -1 || Main.gameMenu || !Main.LocalPlayer.active)
            {
                return;
            }

            var player = Main.LocalPlayer;
            if (DemonSiege.CloseEnoughToDemonSiege(player))
            {
                music = AQMusicManager.GetMusic(AQMusicManager.DemonSiege);
                priority = MusicPriority.Event;
            }
            else if (GlimmerEvent.ActuallyActive && player.position.Y < Main.worldSurface * 16.0)
            {
                int tileDistance = (int)(player.Center.X / 16 - GlimmerEvent.X).Abs();
                if (tileDistance < GlimmerEvent.MaxDistance)
                {
                    music = AQMusicManager.GetMusic(AQMusicManager.GlimmerEvent);
                    priority = MusicPriority.Event;
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
            byte messageID = reader.ReadByte();

            switch (messageID)
            {
                case 0:
                {
                    if (Main.netMode == NetmodeID.Server)
                    {
                        summonOmegaStarite = true;
                    }
                }
                break;
            }
        }

        private static void SummonOmegaStarite()
        {
            omegaStariteIndexCache = (short)NPC.NewNPC(GlimmerEvent.X * 16 + 8, GlimmerEvent.Y * 16 - 1600, ModContent.NPCType<OmegaStarite>(), 0, OmegaStarite.PHASE_NOVA, 0f, 0f, 0f, Main.myPlayer);
            Main.npc[omegaStariteIndexCache].netUpdate = true;
            omegaStariteScene = 1;
            summonOmegaStarite = false;
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
    }
}