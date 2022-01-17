using AQMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Tiles.Nature
{
    public class CrustaciumFlesh : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBlendAll[Type] = true;

            AddMapEntry(new Color(120, 20, 40), CreateMapEntryName("Crustacium"));

            dustType = 95;
            soundType = SoundID.Tink;
            soundStyle = 1;
            drop = ModContent.ItemType<CrustaciumBlob>();
            mineResist = 0.7f;
            minPick = 36;
        }

        public override void RandomUpdate(int i, int j)
        {
            if (WorldGen.genRand.NextBool(25) && ShouldConvertToShell(i, j))
            {
                Main.tile[i, j].type = (ushort)ModContent.TileType<CrustaciumShell>();
            }
        }

        public static bool ShouldConvertToShell(int i, int j)
        {
            for (int k = -1; k < 2; k++)
            {
                for (int l = -1; l < 2; l++)
                {
                    if (k == 0 && l == 0)
                        continue;
                    if (Main.tile[i + k, j + l] == null)
                    {
                        Main.tile[i + k, j + l] = new Tile();
                    }
                    if (Main.tile[i + k, j + l].type != ModContent.TileType<CrustaciumFlesh>() &&
                        Main.tile[i + k, j + l].type != ModContent.TileType<CrustaciumShell>())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}