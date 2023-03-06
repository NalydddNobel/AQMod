using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Events.DemonSiege
{
    public class DemonSiegeBiome : ModBiome
    {
        public const string ScreenFilterKey = "Aequus:DemonSiegeFilter";

        public static ConfiguredMusicData music { get; private set; }

        public override int Music => music.GetID();

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override string BestiaryIcon => Aequus.AssetsPath + "UI/BestiaryIcons/DemonSiege";

        public override string BackgroundPath => Aequus.VanillaTexture + "MapBG3";
        public override string MapBackground => BackgroundPath;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                music = new ConfiguredMusicData(MusicID.Monsoon, MusicID.OtherworldlyPlantera);
                Filters.Scene[ScreenFilterKey] = new Filter(new ScreenShaderData("FilterBloodMoon").UseColor(1f, -0.46f, -0.2f), EffectPriority.High); ;
            }
            On.Terraria.Main.DrawUnderworldBackground += Main_DrawUnderworldBackground;
        }
        private static void Main_DrawUnderworldBackground(On.Terraria.Main.orig_DrawUnderworldBackground orig, Main self, bool flat)
        {
            orig(self, flat);
            if (Filters.Scene[ScreenFilterKey].Opacity > 0f)
            {
                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(-20, -20, Main.screenWidth + 20, Main.screenHeight + 20), new Color(20, 2, 2, 180) * Filters.Scene[ScreenFilterKey].Opacity);
            }
        }

        public override void Unload()
        {
            music = null;
        }

        public override bool IsBiomeActive(Player player)
        {
            return player.Aequus().eventDemonSiege.X != 0;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (isActive)
            {
                if (!Filters.Scene[ScreenFilterKey].Active)
                {
                    Filters.Scene.Activate(ScreenFilterKey, player.Aequus().eventDemonSiege.ToWorldCoordinates(), null);
                }
            }
            else if (Filters.Scene[ScreenFilterKey].Active)
            {
                Filters.Scene.Deactivate(ScreenFilterKey);
            }
        }
    }
}