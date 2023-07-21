using Aequus.Common;
using Aequus.Common.UI.EventBars;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Events.DemonSiege {
    public class DemonSiegeZone : ModBiome {
        public const string ScreenFilterKey = "Aequus:DemonSiegeFilter";

        public static ConfiguredMusicData music { get; private set; }

        public override int Music => music.GetID();

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override string BestiaryIcon => AequusTextures.DemonSiegeBestiaryIcon.Path;

        public override string BackgroundPath => Aequus.VanillaTexture + "MapBG3";
        public override string MapBackground => BackgroundPath;

        public override void Load() {
            if (!Main.dedServ) {
                music = new(MusicID.Monsoon, MusicID.OtherworldlyPlantera);
                Filters.Scene[ScreenFilterKey] = new(new ScreenShaderData("FilterBloodMoon").UseColor(1f, -0.46f, -0.2f), EffectPriority.High);
                if (!Main.dedServ) {
                    AequusEventBarLoader.AddBar(new DemonSiegeBar() {
                        DisplayName = DisplayName,
                        Icon = AequusTextures.DemonSiegeEventIcon,
                        backgroundColor = new Color(180, 100, 20, 128),
                    });
                }
            }
            On_Main.DrawUnderworldBackground += Main_DrawUnderworldBackground;
        }
        private static void Main_DrawUnderworldBackground(On_Main.orig_DrawUnderworldBackground orig, Main self, bool flat) {
            orig(self, flat);

            if (DemonSiegeSystem.ActiveSacrifices.Count > 0) {
                var texture = AequusTextures.Bloom0;
                foreach (var sacrifice in DemonSiegeSystem.ActiveSacrifices) {
                    var v = sacrifice.Value;
                    var center = v.WorldCenter;
                    if (!v.Renderable) {
                        continue;
                    }
                    var origin = texture.Size() / 2f;
                    var drawCoords = (center - Main.screenPosition).Floor();
                    float scale = v.Range * 6f / texture.Width;
                    Main.spriteBatch.Draw(texture, drawCoords, null, Color.Black * v.AuraScale,
                        0f, origin, scale, SpriteEffects.None, 0f);
                }
            }
            if (Filters.Scene[ScreenFilterKey].Opacity > 0f) {
                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(-20, -20, Main.screenWidth + 20, Main.screenHeight + 20), new Color(20, 2, 2, 80) * Filters.Scene[ScreenFilterKey].Opacity);
            }
        }

        public override void Unload() {
            music = null;
        }

        public override bool IsBiomeActive(Player player) {
            return player.Aequus().eventDemonSiege.X != 0;
        }

        public override void SpecialVisuals(Player player, bool isActive) {
            if (isActive) {
                if (!Filters.Scene[ScreenFilterKey].Active) {
                    Filters.Scene.Activate(ScreenFilterKey, player.Aequus().eventDemonSiege.ToWorldCoordinates(), null);
                }
            }
            else if (Filters.Scene[ScreenFilterKey].Active) {
                Filters.Scene.Deactivate(ScreenFilterKey);
            }
        }
    }
}