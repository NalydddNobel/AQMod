using Aequus.Content.Biomes.PollutedOcean.Background;
using Aequus.Content.Biomes.PollutedOcean.Water;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Biomes.PollutedOcean;

public class PollutedOceanBiome : ModBiome {
    public override string BestiaryIcon => AequusTextures.PollutedOceanBestiaryIcon.Path;

    public override string BackgroundPath => AequusTextures.MapBG.Path;
    public override string MapBackground => BackgroundPath;

    public override ModWaterStyle WaterStyle => ModContent.GetInstance<PollutedOceanWater>();
    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<PollutedOceanSurfaceBG>();
    public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<PollutedOceanUndergroundBG>();

    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

    public override int Music => MusicLoader.GetMusicSlot(Mod, AequusSounds.PollutedOcean.ModPath);

    public override bool IsBiomeActive(Player player) {
        return WorldGen.oceanDepths((int)player.position.X / 16, (int)player.position.Y / 16);
    }
}