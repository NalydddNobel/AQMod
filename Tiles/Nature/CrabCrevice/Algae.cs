using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Tiles.Nature.CrabCrevice
{
    public class Algae : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBlendAll[Type] = true;

            AddMapEntry(new Color(10, 180, 80));

            dustType = 167;
            soundType = SoundID.Grass;
            soundStyle = 1;
        }
    }
}
