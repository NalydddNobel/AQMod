using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.MossCaves.Radon.Brick;

public class RadonMossBrickCraftedTile : ModTile {
    public override void SetStaticDefaults() {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        AddMapEntry(Color.SaddleBrown);
        HitSound = SoundID.Tink;
        MineResist = 1f;
        DustType = DustID.Ambient_DarkBrown;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
}