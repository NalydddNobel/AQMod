using Aequus.Content.Biomes.MossBiomes;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Aequus.Content.Biomes {
    public class AequusBiomes : ILoadable {
        public static readonly List<GlowingMossBiome> MossBiomes = new();

        public void Load(Mod mod) {
        }

        public void Unload() {
            MossBiomes.Clear();
        }
    }
}