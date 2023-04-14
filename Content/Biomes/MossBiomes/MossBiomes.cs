using Aequus.Content.Biomes.MossBiomes.Tiles.Radon;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Biomes.MossBiomes {
    public abstract class GlowingMossBiome : ModBiome {
        public int tileCount;

        public int MossTileID { get; protected set; }
        public int MossBrickTileID { get; protected set; }

        public override void Load() {
            AequusBiomes.MossBiomes.Add(this);
        }

        public override bool IsBiomeActive(Player player) {
            return tileCount > 50;
        }
    }

    public class RadonMossBiome : GlowingMossBiome {
        public override void SetStaticDefaults() {
            MossTileID = ModContent.TileType<RadonMossTile>();
            MossBrickTileID = ModContent.TileType<RadonMossBrickTile>();
        }
    }
}
