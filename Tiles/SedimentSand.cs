using AQMod.Tiles.Nature.CrabCrevice;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Tiles
{
    public class SedimentSand : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            this.MergeWith(TileID.Sand);
            this.MergeWith(TileID.HardenedSand);
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
                if (WeepingVine.GrowVine(i, j))
                {
                    WorldGen.TileFrame(i, j, resetFrame: true);
                }
            }
        }
    }
}