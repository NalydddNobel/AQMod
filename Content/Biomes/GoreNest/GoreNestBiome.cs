using Aequus.Tiles.CraftingStations;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Biomes.GoreNest {
    public class GoreNestBiome : ModBiome {
        public override SceneEffectPriority Priority => SceneEffectPriority.None;

        public override bool IsBiomeActive(Player player) {
            return GoreNestTile.BiomeCount > 0 && player.townNPCs < 2f;
        }
    }
}