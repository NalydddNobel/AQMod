using Aequus.Content.Biomes.Oblivion.Tiles;

namespace Aequus.Content.Biomes.Oblivion;

[LegacyName("GoreNestBiome")]
public class OblivionAltarBiome : ModBiome {
    public override SceneEffectPriority Priority => SceneEffectPriority.None;

    public override bool IsBiomeActive(Player player) {
        return OblivionAltarCount.TileCount > 0 && player.townNPCs < 2f;
    }
}