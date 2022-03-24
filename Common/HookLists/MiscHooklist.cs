using AQMod.Content.XMas;
using AQMod.Effects.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.World.Generation;

namespace AQMod.Common.HookLists
{
    public sealed class MiscHooklist : HookList
    {
        protected override void PreLoadHooks()
        {
            if (ModContent.GetInstance<AQConfigClient>().XmasProgressMeterOverride)
            {
                On.Terraria.GameContent.UI.States.UIWorldLoad.ctor += WorldLoadUIManipulation;
            }
            if (ModContent.GetInstance<AQConfigClient>().XmasBackground)
            {
                On.Terraria.Main.DrawBG += DrawXmasBG;
            }
        }

        internal static void WorldLoadUIManipulation(On.Terraria.GameContent.UI.States.UIWorldLoad.orig_ctor orig, Terraria.GameContent.UI.States.UIWorldLoad self, Terraria.World.Generation.GenerationProgress progress)
        {
            if (XmasSeed.XmasWorld)
            {
                XmasSeed.realGenerationProgress = progress;
                XmasSeed.generationProgress = new GenerationProgress
                {
                    Value = progress.Value,
                    TotalWeight = progress.TotalWeight,
                    CurrentPassWeight = 1f,
                };
                progress = XmasSeed.generationProgress;
            }
            orig(self, progress);
        }

        internal static void DrawXmasBG(On.Terraria.Main.orig_DrawBG orig, Main self)
        {
            bool christmasBackground = XmasSeed.XmasWorld && WorldGen.gen; // originally this also ran on the title screen,
                                                                  // but for some reason there were conflicts with Modder's Toolkit
            bool snowflakes = XmasSeed.XmasWorld; // I like the snowflakes on the title screen :)
            if (AQMod.Loading || AQMod.IsUnloading)
            {
                christmasBackground = false;
                snowflakes = false;
            }
            if (christmasBackground)
            {
                if (XmasSeed.generationProgress != null)
                {
                    XmasSeed.generationProgress.Value = Main.rand.NextFloat(0f, 1f);
                    if (!XmasSeed.generatingSnowBiomeText)
                        XmasSeed.generationProgress.Message = Language.GetTextValue("Mods.AQMod.WorldGen.ChristmasSpirit") + ", " + Language.GetTextValue("Mods.AQMod.WorldGen.ChristmasSpiritProgress" + XmasSeed.snowflakeRandom.Next(16));
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
                if (XmasSeed.farSnowflakes == null)
                {
                    XmasSeed.farSnowflakes = new ParticleLayer<FarBGSnowflake>();
                }

                if (XmasSeed.snowflakeRandom == null)
                {
                    XmasSeed.snowflakeRandom = new UnifiedRandom();
                }

                XmasSeed.farSnowflakes.AddParticle(new FarBGSnowflake(new Vector2(XmasSeed.snowflakeRandom.Next(-200, Main.screenWidth + 200), -XmasSeed.snowflakeRandom.Next(100, 250))));
                XmasSeed.farSnowflakes.UpdateParticles();
                XmasSeed.farSnowflakes.Render();
            }
            else
            {
                XmasSeed.farSnowflakes = null;
            }
            orig(self);
            if (snowflakes)
            {
                if (XmasSeed.closeSnowflakes == null)
                {
                    XmasSeed.closeSnowflakes = new ParticleLayer<CloseBGSnowflake>();
                }
                if (XmasSeed.snowflakeRandom.NextBool(10))
                {
                    XmasSeed.closeSnowflakes.AddParticle(new CloseBGSnowflake(new Vector2(Main.screenPosition.X + XmasSeed.snowflakeRandom.Next(-200, Main.screenWidth + 200), Main.screenPosition.Y - XmasSeed.snowflakeRandom.Next(100, 250))));
                }
                XmasSeed.farSnowflakes.UpdateParticles();
                XmasSeed.farSnowflakes.Render();
            }
            else
            {
                XmasSeed.generatingSnowBiomeText = false;
                XmasSeed.realGenerationProgress = null;
                XmasSeed.generationProgress = null;
                XmasSeed.snowflakeRandom = null;
                XmasSeed.closeSnowflakes = null;
            }
            Main.gameMenu = oldGameMenu;
        }
    }
}