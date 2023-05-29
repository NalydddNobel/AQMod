using Aequus.Common;
using Aequus.Content.Events.GlimmerEvent.Sky;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.MainMenu {
    public class GlimmerMenu : ModMenu {
        public override Asset<Texture2D> MoonTexture => ModContent.Request<Texture2D>(Aequus.BlankTexture);

        public static float Intro;
        public static float Opacity;

        public static ConfiguredMusicData music = new(MusicID.Title, MusicID.Title);
        public override int Music => music.GetID();

        public override void OnSelected() {
            Opacity = 0f;
            Intro = 0f;
        }
        public override void OnDeselected() {
            Opacity = 0f;
            Intro = 0f;
        }
        public override void Update(bool isOnTitleScreen) {
            //DebugRefreshBiomes();
            if (!Main.lockMenuBGChange) {
                if (!SkyManager.Instance[GlimmerSky.Key].IsActive()) {
                    SkyManager.Instance.Activate(GlimmerSky.Key);
                    Opacity = 0f;
                }

                Opacity = Math.Clamp(Opacity + 0.033f, 0.1f, 0.75f);
                SkyManager.Instance[GlimmerSky.Key].Opacity = Opacity;
            }
            else if (Opacity > 0f && SkyManager.Instance[GlimmerSky.Key].IsActive()) {
                if (SkyManager.Instance[GlimmerSky.Key].IsActive()) {
                    SkyManager.Instance.Deactivate(GlimmerSky.Key);
                }

                Opacity = Math.Max(Opacity - 0.015f, 0f);
                SkyManager.Instance[GlimmerSky.Key].Opacity = Opacity;
            }
            if (!Main.lockMenuBGChange) {
                Main.dayTime = false;
                Main.time = Main.nightLength / 2;
            }
            Intro++;
            CleanLayers(Main.bgAlphaFarBackLayer);
            CleanLayers(Main.bgAlphaFrontLayer);
            Main.bgAlphaFarBackLayer[0] = 1f;
            Main.bgAlphaFrontLayer[0] = 1f;
        }
        private void DebugRefreshBiomes() {
            if (Main.mouseLeftRelease) {
                while (Main.alreadyGrabbingSunOrMoon
                    || WorldGen.treeBG1 == 5 || WorldGen.treeBG1 == 9 || WorldGen.treeBG2 == 5 || WorldGen.treeBG2 == 0 || WorldGen.treeBG1 == 2
                    || WorldGen.treeBG1 == 31 || WorldGen.treeBG1 == 51 || WorldGen.treeBG1 == 71 || WorldGen.treeBG1 == 73) {
                    WorldGen.RandomizeBackgrounds(Main.rand);
                    Main.alreadyGrabbingSunOrMoon = false;
                }
            }
        }
        private void CleanLayers(float[] layerAlpha) {
            for (int i = 0; i < layerAlpha.Length; i++) {
                layerAlpha[i] = 0f;
            }
        }

        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor) {
            //DebugDrawBiomes();
            var texture = AequusTextures.GlimmerMenu;
            Main.moonPhase = (int)MoonPhase.Empty;
            for (float progress = 0f;; progress += 0.01f) {
                logoScale = MathF.Pow(Math.Clamp((Intro - 30f * (1f - progress)) / 40f, 0f, 1f), 8f);
                var clr = Color.White;
                if (progress < 1f) {
                    if (logoScale >= 1f) {
                        continue;
                    }

                    clr.A = 0;
                    clr *= progress;
                }
                else {
                    if (Intro > 40f && Intro < 55f) {
                        logoScale += MathF.Sin((1f - MathF.Pow(1f - (Intro - 40f) / 15f, 2f)) * MathHelper.Pi) * 0.1f;
                    }
                }
                Main.spriteBatch.Draw(
                    texture,
                    logoDrawCenter,
                    null,
                    clr,
                    0f,
                    texture.Size() / 2f,
                    logoScale,
                    SpriteEffects.None,
                    0f
                );

                if (progress > 1f) {
                    break;
                }
            }
            return false;
        }
    }
}