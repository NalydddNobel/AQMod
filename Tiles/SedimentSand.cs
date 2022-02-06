using AQMod.Items.Placeable;
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
            drop = ModContent.ItemType<SedimentSandBlock>();
            soundType = SoundID.Dig;
            soundStyle = 1;
        }

        public override void RandomUpdate(int i, int j)
        {
            if (!Main.tile[i, j].halfBrick() && Main.tile[i, j].slope() == 0)
            {
                if (WorldGen.genRand.NextBool(2000))
                {
                    if (!Main.tile[i, j - 1].active() && Main.tile[i, j - 1].liquid > 0 && !Main.tile[i, j - 1].lava() && !Main.tile[i, j - 1].honey())
                    {
                        Main.tile[i, j - 1].active(active: true);
                        Main.tile[i, j - 1].halfBrick(halfBrick: false);
                        Main.tile[i, j - 1].slope(slope: 0);
                        if (WorldGen.genRand.NextBool(8))
                        {
                            Main.tile[i, j - 1].type = (ushort)ModContent.TileType<SeaPicklesTile>();
                            Main.tile[i, j - 1].frameX = 0;
                        }
                        else
                        {
                            Main.tile[i, j - 1].type = (ushort)ModContent.TileType<ExoticCoralNew>();
                            Main.tile[i, j - 1].frameX = (short)(22 * ExoticCoralNew.GetRandomStyle(WorldGen.genRand.Next(3)));
                        }
                        Main.tile[i, j - 1].frameY = 0;
                        WorldGen.SquareTileFrame(i, j, resetFrame: true);
                    }
                }
                if (WorldGen.genRand.NextBool(2000))
                {
                    if (!Main.tile[i - 1, j].active() && Main.tile[i - 1, j].liquid > 0 && !Main.tile[i - 1, j].lava() && !Main.tile[i - 1, j].honey())
                    {
                        Main.tile[i - 1, j].active(active: true);
                        Main.tile[i - 1, j].halfBrick(halfBrick: false);
                        Main.tile[i - 1, j].slope(slope: 0);
                        Main.tile[i - 1, j].type = (ushort)ModContent.TileType<SeaPicklesTile>();
                        Main.tile[i - 1, j].frameX = 0;
                        Main.tile[i - 1, j].frameY = 0;
                        WorldGen.SquareTileFrame(i, j, resetFrame: true);
                    }
                }
                if (WorldGen.genRand.NextBool(2000))
                {
                    if (!Main.tile[i + 1, j].active() && Main.tile[i + 1, j].liquid > 0 && !Main.tile[i + 1, j].lava() && !Main.tile[i + 1, j].honey())
                    {
                        Main.tile[i + 1, j].active(active: true);
                        Main.tile[i + 1, j].halfBrick(halfBrick: false);
                        Main.tile[i + 1, j].slope(slope: 0);
                        Main.tile[i + 1, j].type = (ushort)ModContent.TileType<SeaPicklesTile>();
                        Main.tile[i + 1, j].frameX = 0;
                        Main.tile[i + 1, j].frameY = 0;
                        WorldGen.SquareTileFrame(i, j, resetFrame: true);
                    }
                }
            }
            if (WorldGen.genRand.NextBool(4) && WeepingVine.GrowVine(i, j))
            {
                WorldGen.TileFrame(i, j, resetFrame: true);
            }
        }
    }
}