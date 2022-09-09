using Aequus.Items.Placeable.CrabCrevice;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.CrabCrevice
{
    public class SedimentaryRockTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[Type][TileID.Sand] = true;
            Main.tileMerge[TileID.Sand][Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.Conversion.HardenedSand[Type] = true;
            AddMapEntry(new Color(160, 149, 97));
            DustType = DustID.Sand;
            ItemDrop = ModContent.ItemType<SedimentaryRock>();
            HitSound = SoundID.Tink;
            MineResist = 1.5f;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFramingHelper.MergeWithFrame(i, j, Type, TileID.Sand);
            return false;
        }

        public override void RandomUpdate(int i, int j)
        {
            if (Main.tile[i, j - 1].HasTile)
                return;

            if (Main.tile[i, j - 1].LiquidAmount == 255 && Main.tile[i, j - 1].LiquidType == LiquidID.Water && WorldGen.genRand.NextBool(2))
            {
                WorldGen.PlaceTile(i, j, ModContent.TileType<CrabFloorPlants>(), mute: true, style: WorldGen.genRand.Next(15));
            }
            else if (Main.tile[i, j - 1].LiquidAmount > 128 && Main.tile[i, j - 1].LiquidType == LiquidID.Water && WorldGen.genRand.NextBool(2))
            {
                if (Main.rand.NextBool())
                {
                    WorldGen.PlaceTile(i, j - 1, TileID.BeachPiles, mute: true, style: WorldGen.genRand.Next(15));
                }
                else
                {
                    WorldGen.PlaceTile(i, j - 1, TileID.Coral, mute: true);
                }
            }
            else if (WorldGen.genRand.NextBool(2))
            {
                if (Main.tile[i - 1, j - 1].HasTile || Main.tile[i + 1, j - 1].HasTile)
                    return;
                WorldGen.PlaceTile(i, j - 1, ModContent.TileType<CrabGrassBig>(), mute: true);
            }

            if (WorldGen.genRand.NextBool(2) && Main.tile[i, j - 1].LiquidAmount > 0 && Main.tile[i, j - 1].LiquidType == LiquidID.Water)
            {
                for (int l = j - 2; l > 50; l--)
                {
                    if (Main.tile[i, l].LiquidAmount <= 0)
                    {
                        if (!Main.tile[i, l].HasTile)
                        {
                            WorldGen.PlaceTile(i, l + 1, ModContent.TileType<CrabAlgae>(), mute: true);
                        }
                        break;
                    }
                }
            }
        }
    }
}