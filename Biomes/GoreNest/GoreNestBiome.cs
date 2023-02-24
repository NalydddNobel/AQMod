using Aequus.Biomes.GoreNest.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Biomes.GoreNest
{
    public class GoreNestBiome : ModBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.None;

        public override bool IsBiomeActive(Player player)
        {
            return GoreNestTile.BiomeCount > 0;
        }
    }
}