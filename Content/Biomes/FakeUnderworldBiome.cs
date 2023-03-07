using Aequus.Tiles.Furniture;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Biomes
{
    public class FakeUnderworldBiome : ModBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.None;

        public override bool IsBiomeActive(Player player)
        {
            return AshTombstones.numAshTombstones > 5;
        }
    }
}