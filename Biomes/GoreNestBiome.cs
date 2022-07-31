using Aequus.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Biomes
{
    public class GoreNestBiome : ModBiome
    {
        public override bool IsBiomeActive(Player player)
        {
            return GoreNestTile.BiomeCount > 0;
        }
    }
}
