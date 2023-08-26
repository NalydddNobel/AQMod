using Aequus.Content.WorldEvents.DemonSiege;
using Aequus.Content.WorldEvents.Glimmer;
using Aequus.Content.WorldEvents.SpaceStorm;
using Terraria;

namespace Aequus;

public static class PlayerExtensions {
    #region Biomes
    public static bool ZoneGlimmer(this Player player) {
        return player.InModBiome<GlimmerZone>();
    }
    public static bool ZoneDemonSiege(this Player player) {
        return player.InModBiome<DemonSiegeZone>();
    }
    public static bool ZoneSpaceStorm(this Player player) {
        return player.InModBiome<SpaceStormZone>();
    }
    #endregion
}