using Aequus.Tiles.Moss;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Biomes
{
    public abstract class GlowingMossBiome : ModBiome 
    {
        public static List<GlowingMossBiome> MossBiomes { get; private set; }

        public int tileCount;

        public int MossTileID { get; protected set; }
        public int MossBrickTileID { get; protected set; }

        public override void Load()
        {
            if (MossBiomes == null)
            {
                MossBiomes = new List<GlowingMossBiome>();
            }
            MossBiomes.Add(this);
        }

        public override bool IsBiomeActive(Player player)
        {
            return tileCount > 50;
        }
    }

    public class RadonMossBiome : GlowingMossBiome
    {
        public override void SetStaticDefaults()
        {
            MossTileID = ModContent.TileType<RadonMossTile>();
            MossBrickTileID = ModContent.TileType<RadonMossBrickTile>();
        }
    }
}
