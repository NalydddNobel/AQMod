namespace AequusRemake.Content.Biomes.PollutedOcean;

/// <summary>Main class for general things related to the Polluted Ocean biome. Also counts as the Underground Polluted Ocean.</summary>
public class PollutedOceanBiomeUnderground : PollutedOceanBiomeSurface {
    public static Vector3 CavernLight { get; set; } = Color.Cyan.ToVector3();

    public override bool IsBiomeActive(Player player) {
        return player.position.Y > Main.worldSurface * 16.0 && ModContent.GetInstance<PollutedOceanSystem>().CheckBiome(player);
    }
}