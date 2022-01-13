using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Tiles.Nature.CrabCrevice
{
    public class SedimentSand : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            this.MergeWith(TileID.Sand);
            AddMapEntry(new Color(150, 180, 110));
            TileID.Sets.ChecksForMerge[Type] = true;

            dustType = 32;
            drop = ItemID.SandBlock;
            soundType = SoundID.Dig;
            soundStyle = 1;
        }

        public override void RandomUpdate(int i, int j)
        {
            if (WorldGen.genRand.NextBool(4))
            {
                WeepingVine.GrowVine(i, j);
            }
        }
    }
}