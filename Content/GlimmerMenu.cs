﻿using Aequus.Biomes.Glimmer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Aequus.Content
{
    public class GlimmerMenu : ModMenu
    {
        public override Asset<Texture2D> MoonTexture => ModContent.Request<Texture2D>(Aequus.BlankTexture);

        public float Opacity;

        public override void OnSelected()
        {
            Opacity = 0f;
        }
        public override void OnDeselected()
        {
            Opacity = 0f;
        }
        public override void Update(bool isOnTitleScreen)
        {
            //DebugRefreshBiomes();
            if (!Main.lockMenuBGChange)
            {
                if (!SkyManager.Instance[GlimmerSky.Key].IsActive())
                {
                    SkyManager.Instance.Activate(GlimmerSky.Key);
                    Opacity = 0f;
                }

                Opacity = Math.Clamp(Opacity + 0.033f, 0.1f, 0.75f);
                SkyManager.Instance[GlimmerSky.Key].Opacity = Opacity;
            }
            else if (Opacity > 0f && SkyManager.Instance[GlimmerSky.Key].IsActive())
            {
                if (!SkyManager.Instance[GlimmerSky.Key].IsActive())
                {
                    SkyManager.Instance.Activate(GlimmerSky.Key);
                    Opacity = 0f;
                }

                Opacity = Math.Max(Opacity - 0.015f, 0f);
                SkyManager.Instance[GlimmerSky.Key].Opacity = Opacity;
            }
            CleanLayers(Main.bgAlphaFarBackLayer);
            CleanLayers(Main.bgAlphaFrontLayer);
            Main.bgAlphaFarBackLayer[0] = 1f;
            Main.bgAlphaFrontLayer[0] = 1f;
        }
        private void DebugRefreshBiomes()
        {
            if (Main.mouseLeftRelease)
            {
                while (Main.alreadyGrabbingSunOrMoon
                    || WorldGen.treeBG1 == 5 || WorldGen.treeBG1 == 9 || WorldGen.treeBG2 == 5 || WorldGen.treeBG2 == 0 || WorldGen.treeBG1 == 2
                    || WorldGen.treeBG1 == 31 || WorldGen.treeBG1 == 51 || WorldGen.treeBG1 == 71 || WorldGen.treeBG1 == 73)
                {
                    WorldGen.RandomizeBackgrounds(Main.rand);
                    Main.alreadyGrabbingSunOrMoon = false;
                }
            }
        }
        private void CleanLayers(float[] layerAlpha)
        {
            for (int i = 0; i < layerAlpha.Length; i++)
            {
                layerAlpha[i] = 0f;
            }
        }

        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            //DebugDrawBiomes();
            Main.dayTime = false;
            Main.time = Main.nightLength / 2;
            return true;
        }
        public void DebugDrawBiomes()
        {
            AequusHelpers.debugTextDraw(WorldGen.treeBG1.ToString(), Vector2.Zero);
            AequusHelpers.debugTextDraw(WorldGen.treeBG2.ToString(), new Vector2(0f, 20f));
            AequusHelpers.debugTextDraw(WorldGen.treeBG3.ToString(), new Vector2(0f, 40f));
            AequusHelpers.debugTextDraw(WorldGen.treeBG4.ToString(), new Vector2(0f, 60f));
            AequusHelpers.debugTextDraw(Opacity.ToString(), new Vector2(0f, 80f));
        }
    }
}