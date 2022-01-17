using AQMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Tiles.Nature
{
    public class CrustaciumShell : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBlendAll[Type] = true;

            //this.MergeWith(ModContent.TileType<CrustaciumFlesh>());
            AddMapEntry(new Color(160, 80, 40), CreateMapEntryName("Crustacium"));

            dustType = 95;
            soundType = SoundID.Tink;
            soundStyle = 1;
            mineResist = 1.8f;
            drop = ModContent.ItemType<CrustaciumOre>();
        }
    }
}
