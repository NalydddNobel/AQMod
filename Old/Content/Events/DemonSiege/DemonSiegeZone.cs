using Aequus.Content.DataSets;
using Aequus.Core.Initialization;
using Aequus.Old.Common.EventBars;
using Aequus.Old.Content.Enemies.DemonSiege.CinderBat;
using Aequus.Old.Content.Enemies.DemonSiege.Keeper;
using Aequus.Old.Content.Enemies.DemonSiege.LavaLegs;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace Aequus.Old.Content.Events.DemonSiege;

public class DemonSiegeZone : ModBiome, IPostSetupContent {
    public const System.String ScreenFilterKey = "Aequus:DemonSiegeFilter";

    public override System.Int32 Music => MusicID.Monsoon;

    public override SceneEffectPriority Priority => SceneEffectPriority.Event;

    public override System.String BestiaryIcon => AequusTextures.DemonSiegeBestiaryIcon.Path;

    public override System.String BackgroundPath => "Terraria/Images/MapBG3";
    public override System.String MapBackground => BackgroundPath;

    public override void Load() {
        if (!Main.dedServ) {
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
    private static void Main_DrawUnderworldBackground(On_Main.orig_DrawUnderworldBackground orig, Main self, System.Boolean flat) {
        orig(self, flat);

        if (DemonSiegeSystem.ActiveSacrifices.Count > 0) {
            var texture = AequusTextures.Bloom;
            foreach (var sacrifice in DemonSiegeSystem.ActiveSacrifices) {
                var v = sacrifice.Value;
                var center = v.WorldCenter;
                if (!v.Renderable) {
                    continue;
                }
                var origin = texture.Size() / 2f;
                var drawCoords = (center - Main.screenPosition).Floor();
                System.Single scale = v.Range * 6f / texture.Width();
                Main.spriteBatch.Draw(texture, drawCoords, null, Color.Black * v.AuraScale,
                    0f, origin, scale, SpriteEffects.None, 0f);
            }
        }
        if (Filters.Scene[ScreenFilterKey].Opacity > 0f) {
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(-20, -20, Main.screenWidth + 20, Main.screenHeight + 20), new Color(20, 2, 2, 80) * Filters.Scene[ScreenFilterKey].Opacity);
        }
    }



    public override System.Boolean IsBiomeActive(Player player) {
        return player.GetModPlayer<DemonSiegePlayer>().GoreNest.X != 0;
    }

    public override void SpecialVisuals(Player player, System.Boolean isActive) {
        if (isActive) {
            if (!Filters.Scene[ScreenFilterKey].Active) {
                Filters.Scene.Activate(ScreenFilterKey, player.GetModPlayer<DemonSiegePlayer>().GoreNest.ToWorldCoordinates(), null);
            }
        }
        else if (Filters.Scene[ScreenFilterKey].Active) {
            Filters.Scene.Deactivate(ScreenFilterKey);
        }
    }

    public static void AddEnemies(IDictionary<System.Int32, System.Single> pool, NPCSpawnInfo spawnInfo) {
        pool.Clear();
        pool.Add(ModContent.NPCType<CinderBat>(), 1f);
        pool.Add(ModContent.NPCType<LeggedLava>(), 1f);
        pool.Add(ModContent.NPCType<KeeperImp>(), 1f);
    }

    void IPostSetupContent.PostSetupContent(Aequus aequus) {
        NPCSets.UnderworldTags.Add(ModBiomeBestiaryInfoElement);
    }
}