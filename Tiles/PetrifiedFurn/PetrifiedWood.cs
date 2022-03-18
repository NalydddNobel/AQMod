using AQMod.Tiles.CrabCrevice;
using AQMod.Tiles.Walls;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Tiles.PetrifiedFurn
{
    public class PetrifiedWood : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMergeDirt[Type] = true;

            AddMapEntry(new Color(100, 90, 10, 255));

            dustType = DustID.Dirt;
            soundType = SoundID.Tink;
            soundStyle = 1;
            drop = ModContent.ItemType<Items.Placeable.PetrifiedWood>();
        }

        public override void RandomUpdate(int i, int j)
        {
            if (WorldGen.genRand.NextBool(80) && Main.tile[i, j + 1] != null
                && (Main.tile[i, j + 1].wall == ModContent.WallType<OceanRavineWall>() || Main.tile[i, j + 1].wall == ModContent.WallType<PetrifiedWoodWall>()))
            {
                if (WeepingVine.GrowVine(i, j))
                {
                    WorldGen.TileFrame(i, j, resetFrame: true);
                }
            }
        }
    }
}