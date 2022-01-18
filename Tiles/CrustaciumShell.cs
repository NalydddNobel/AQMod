using AQMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Tiles
{
    public class CrustaciumShell : ModTile
    {
        public override void SetDefaults()
        {
            TileID.Sets.Ore[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileValue[Type] = 415;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 975;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBlendAll[Type] = true;

            AddMapEntry(new Color(160, 80, 40), CreateMapEntryName("Crustacium"));

            dustType = 95;
            soundType = SoundID.Tink;
            soundStyle = 1;
            mineResist = 1.8f;
            drop = ModContent.ItemType<CrustaciumOre>();
        }
    }
}
