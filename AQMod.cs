using AQMod.Assets.ItemOverlays;
using AQMod.Assets.Sounds.Music;
using AQMod.Common;
using AQMod.Common.Commands;
using AQMod.Common.Config;
using AQMod.Common.CursorDyes;
using AQMod.Common.DeveloperTools;
using AQMod.Common.Skies;
using AQMod.Common.UserInterface;
using AQMod.Common.WorldEvents;
using AQMod.Content;
using AQMod.Effects;
using AQMod.Items.Misc.Energies;
using AQMod.Items.Placeable.Mushrooms;
using AQMod.Items.Vanities;
using AQMod.Localization;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace AQMod
{
    public class AQMod : Mod
    {
        public const string ModName = nameof(AQMod);
        public static bool GameWorldActive => Main.instance.IsActive && !Main.gamePaused;

        public static AQMod Instance => ModContent.GetInstance<AQMod>();

        internal const float TwoPiOver5 = MathHelper.TwoPi / 5f;
        internal const float TwoPiOver8 = MathHelper.TwoPi / 8f;

        internal static bool Loading { get; private set; }

        public static short OmegaStariteIndexCache { get; set; } = -1;
        public static byte omegaStariteScene;

        public static bool AprilFools { get; internal set; }

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

        public override void Load()
        {
            Loading = true;
            AQText.Load();
            CursorDyeManager.Setup();
            NPCShopManager.Setup();
            NPCIMethods.Setup();
            WorldTimeManager.Setup();
            AQPlayer.Setup();
            AQCommand.LoadCommands();
            AQNPC.StariteProjectileColor = AQNPC.StariteProjectileColorOrig;
            AQNPC.MoonlightWall = new MoonlightWallHelper();
            AQUtils.Setup();
            if (!Main.dedServ)
            {
                var client = AQConfigClient.Instance;
                AQMusicManager.Initialize();
                Parralax.RefreshParralax();
                SkyManager.Instance[GlimmerEventSky.Name] = new GlimmerEventSky();
                GlimmerEventSky.Initialize();
                Trailshader.Setup();
                SpriteUtils.LoadAssets(this, client, Logger);
            }
        }

        private void Unload_Load()
        {
            SpriteUtils.UnloadAssets();
            GlimmerEventSky.Unload();
            AQUtils.Unload();
            AQNPC.MoonlightWall = null;
            AQCommand.UnloadCommands();
            AQText.Unload();
        }

        public override void PostSetupContent()
        {
            AQNPC.Sets.Setup();
            MapMarkerPlayer.Setup();
            ChestTypeAndStyleToItemID.Setup();
            ModHelpers.BossChecklist.SetupBossChecklistEntries(this);
            ModHelpers.Fargowiltas.SetupBossSummons(this);
            CursorDyeManager.InitializeDyes(this);
            AQItem.Sets.Setup();
        }

        private void Unload_PostSetupContent()
        {
            AQItem.Sets.Unload();
            CursorDyeManager.Unload();
            ChestTypeAndStyleToItemID.Unload();
            MapMarkerPlayer.Unload();
            AQNPC.Sets.Setup();
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

        private void Unload_AddRecipeGroups()
        {
        }

        public override void AddRecipes()
        {
            Loading = false;
            ItemOverlayLoader.Finish();
            AQRecipes.AddRecipes(this);
            CelesitalEightBall.Text = Language.GetTextValue(AQText.Key + "Common.EightballAnswer20");
        }

        private void Unload_AddRecipes()
        {
            CelesitalEightBall.Text = null;
            ItemOverlayLoader.Unload();
        }

        public override void Unload()
        {
            Unload_AddRecipes();
            Unload_AddRecipeGroups();
            Unload_PostSetupContent();
            Unload_Load();
        }

        public override object Call(params object[] args)
        {
            return CallManager.Call(args);
        }

        public override void PreUpdateEntities()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < SpriteUtils.WorldEffects.Count; i++)
                {
                    var v = SpriteUtils.WorldEffects[i];
                    if (!v.Update())
                    {
                        SpriteUtils.WorldEffects.RemoveAt(i);
                        i--;
                    }
                }
            }
            AQNPC.StariteProjectileColor = AQNPC.StariteDisco ? new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0) : AQNPC.StariteProjectileColorOrig;
        }

        public override void MidUpdateNPCGore()
        {
            if (OmegaStariteIndexCache > -1 && !Main.npc[OmegaStariteIndexCache].active)
            {
                OmegaStariteIndexCache = -1;
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
            if (GlimmerEvent.ActuallyActive)
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

        private const byte _testType = 0;

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            CursorDyeManager.UpdateColor();
            var index = layers.FindIndex((l) => l.Name.Equals("Vanilla: Invasion Progress Bars"));
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer("AQMod: Invasion Progress Bar", delegate ()
                {
                    InvasionUI.Apply();
                    return true;
                }, InterfaceScaleType.UI));
            }
            index = layers.FindIndex((l) => l.Name.Equals("Vanilla: Inventory"));
            if (index != -1)
            {
                //layers.Insert(index, new LegacyGameInterfaceLayer("AQMod: Inventory", delegate ()
                //{
                //    return true;
                //}, InterfaceScaleType.UI));
            }
            index = layers.FindIndex((l) => l.Name.Equals("Vanilla: Cursor"));
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer("AQMod: MouseUI", delegate ()
                {
                    switch (_testType)
                    {
                        case 1:
                        {
                            AQTests.TestFabrikMethod2D.Apply();
                        }
                        break;

                        case 2:
                        {
                            AQTests.TestFabrikMethod3D.Apply();
                        }
                        break;
                    }
                    return true;
                }, InterfaceScaleType.UI));
            }
        }

        internal static void BroadcastMessage(string text, Color color)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.BroadcastChatMessage(NetworkText.FromKey(text), color);
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(Language.GetTextValue(text), color);
            }
        }
    }
}