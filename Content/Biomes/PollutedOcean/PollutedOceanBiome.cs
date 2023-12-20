using Aequus.Content.Biomes.PollutedOcean.Background;
using Aequus.Content.Biomes.PollutedOcean.Water;

namespace Aequus.Content.Biomes.PollutedOcean;

public class PollutedOceanBiome : ModBiome {
    public static int BlockPresenceNeeded { get; set; } = 300;

    public static int BlockPresence { get; set; }

    public override string BestiaryIcon => AequusTextures.PollutedOceanBestiaryIcon.Path;

    public override string BackgroundPath => AequusTextures.MapBG.Path;
    public override string MapBackground => BackgroundPath;

    public override ModWaterStyle WaterStyle => ModContent.GetInstance<PollutedOceanWater>();
    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<PollutedOceanSurfaceBG>();
    public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<PollutedOceanUndergroundBG>();

    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

    private int? MusicSlotId;
    public override int Music => MusicSlotId ??= MusicLoader.GetMusicSlot(Mod, AequusSounds.PollutedOcean.ModPath());

    public override bool IsBiomeActive(Player player) {
        return (player.position.Y > Main.worldSurface * 16.0 || WorldGen.oceanDepths((int)player.position.X / 16, (int)Main.worldSurface)) && BlockPresence >= BlockPresenceNeeded;
    }
}