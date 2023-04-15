using Terraria;
using Terraria.ID;

namespace Aequus.Content.Biomes.MossBiomes.Radon.Tiles {
    public class RadonMossBrickTile : RadonMossTile {
        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            TileID.Sets.tileMossBrick[Type] = true;
            Main.tileMoss[Type] = false;
            ItemDrop = ItemID.GrayBrick;
        }

        public override bool? OnPlaceTile(int i, int j, bool mute, bool forced, int plr, int style) {
            return null;
        }
    }
}