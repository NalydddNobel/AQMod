using Aequus.Common.Entities.Bestiary;
using Aequus.Content.Biomes.PollutedOcean.Background;
using Aequus.Content.Biomes.PollutedOcean.Water;

namespace Aequus.Content.Biomes.PollutedOcean;

public abstract class PollutedOceanBiomes : ModBiome {
    public override ModWaterStyle WaterStyle => ModContent.GetInstance<PollutedWater>();
    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<PollutedSurfaceBG>();
    public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<PollutedUndergroundBG>();

    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

    private static int? _musicSlotCache;
    public override int Music => _musicSlotCache ??= MusicLoader.GetMusicSlot("AequusMusic/Assets/Music/PollutedOcean");

#if POLLUTED_OCEAN_TODO
    //public override int BiomeTorchItemType => ModContent.GetInstance<SeaPickleTorch>().Item.Type;
    //public override int BiomeCampfireItemType => ModContent.GetInstance<TrashFurniture>().CampfireItem.Type;
#else
    public override int BiomeTorchItemType => ItemID.CoralTorch;
    public override int BiomeCampfireItemType => ItemID.CoralCampfire;
#endif

    public override string MapBackground => BackgroundPath;

    public override float GetWeight(Player player) {
        // Increase weight depending on how many tiles are nearby.
        var tileCount = Instance<PollutedOceanSystem>();
        return tileCount.TileCount / (float)PollutedOceanSystem.TileCountPriorityThreshold;
    }

    public override bool IsBiomeActive(Player player) {
        var tileCount = Instance<PollutedOceanSystem>();
        return tileCount.TileCount > PollutedOceanSystem.TileCountBiome;
    }
}

public class PollutedOceanSurface : PollutedOceanBiomes, IPostSetupContent {
    public override string BackgroundPath => AequusTextures.MapBG_PollutedSurface.FullPath;
    public override string BestiaryIcon => AequusTextures.Biome_PollutedSurface.FullPath;

    public override bool IsBiomeActive(Player player) {
        return player.position.Y < Main.worldSurface * 16.0 && base.IsBiomeActive(player);
    }

    void IPostSetupContent.PostSetupContent() {
        BestiaryTagCollection.Ocean.Add(this);
    }
}

public class PollutedOceanUnderground : PollutedOceanBiomes, IPostSetupContent {
    public Vector3 CavernLight { get; set; } = Color.Cyan.ToVector3();
    public override string BackgroundPath => AequusTextures.MapBG_PollutedUnderground.FullPath;
    public override string BestiaryIcon => AequusTextures.Biome_PollutedUnderground.FullPath;

    public override bool IsBiomeActive(Player player) {
        return player.position.Y >= Main.worldSurface * 16.0 && base.IsBiomeActive(player);
    }

    void IPostSetupContent.PostSetupContent() {
        BestiaryTagCollection.Ocean.Add(this);
    }
}
