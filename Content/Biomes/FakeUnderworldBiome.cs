using Aequus.Tiles.Misc.AshTombstones;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Biomes {
    public class FakeUnderworldBiome : ModBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.None;

        public override bool IsBiomeActive(Player player)
        {
            return AshTombstonesTile.numAshTombstones > 5;
        }
    }
}