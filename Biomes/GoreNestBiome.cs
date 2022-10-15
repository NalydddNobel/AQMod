using Aequus.Tiles.Misc;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Biomes
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