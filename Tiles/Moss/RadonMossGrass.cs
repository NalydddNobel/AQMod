using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.Moss
{
    public class RadonMossGrass : ModTile
    {
        public static int[] anchorTiles;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileCut[Type] = true;

            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.SwaysInWindBasic[Type] = true;

            anchorTiles = new int[]
            {
                ModContent.TileType<RadonMossTile>(),
                ModContent.TileType<RadonMossBrickTile>(),
            };

            AddMapEntry(new Color(55, 65, 65));
            HitSound = SoundID.Grass;
            DustType = DustID.Ambient_DarkBrown;
            //ItemDrop = ModContent.ItemType<SeaPickle>();
        }

        public override bool CanPlace(int i, int j)
        {
            int[] radonMoss = anchorTiles;
            var top = Framing.GetTileSafely(i, j - 1);
            if (top.HasTile && !top.BottomSlope && radonMoss.ContainsAny(top.TileType) && Main.tileSolid[top.TileType] && !Main.tileSolidTop[top.TileType])
            {
                return true;
            }
            var bottom = Framing.GetTileSafely(i, j + 1);
            if (bottom.HasTile && !bottom.IsHalfBlock && !bottom.TopSlope && radonMoss.ContainsAny(bottom.TileType) && (Main.tileSolid[bottom.TileType] || Main.tileSolidTop[bottom.TileType]))
            {
                return true;
            }
            var left = Framing.GetTileSafely(i - 1, j);
            if (left.HasTile && radonMoss.ContainsAny(left.TileType) && Main.tileSolid[left.TileType] && !Main.tileSolidTop[left.TileType])
            {
                return true;
            }
            var right = Framing.GetTileSafely(i + 1, j);
            if (right.HasTile && radonMoss.ContainsAny(right.TileType) && Main.tileSolid[right.TileType] && !Main.tileSolidTop[right.TileType])
            {
                return true;
            }
            return false;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            AequusTile.GemFrame(i, j, anchorTiles);
            return false;
        }
    }
}