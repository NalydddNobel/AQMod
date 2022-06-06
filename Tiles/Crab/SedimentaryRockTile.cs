using Aequus.Items.Placeable.Crab;
using Aequus.Tiles.Ambience;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Crab
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
			TileID.Sets.Conversion.HardenedSand[Type] = true;
			AddMapEntry(new Color(160, 149, 97));
			DustType = DustID.Sand;
			ItemDrop = ModContent.ItemType<SedimentaryRock>();
			HitSound = SoundID.Tink;
			MineResist = 1.5f;
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override void RandomUpdate(int i, int j)
        {
            if (WorldGen.genRand.NextBool(2))
            {
				WorldGen.PlaceTile(i, j - 1, ModContent.TileType<CrabGrass3x2>(), mute: true);
            }
            else if (Main.tile[i, j - 1].LiquidAmount > 128 && Main.tile[i, j - 1].LiquidType == LiquidID.Water)
            {
				if (WorldGen.genRand.NextBool(18))
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
			}
        }
    }
}