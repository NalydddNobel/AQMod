using Aequus.Items.Placeable.CrabCrevice;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.CrabCrevice
{
    public class CrustaciumEmbed : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMergeDirt[Type] = true;
            AddMapEntry(new Color(180, 160, 50));
            DustType = DustID.Sand;
            ItemDrop = ModContent.ItemType<SedimentaryRock>();
            HitSound = SoundID.Tink;
            MineResist = 2f;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}