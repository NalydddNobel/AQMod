using Aequus.Common.Tiles.Global;
using Aequus.Items.Accessories.Life;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.Misc {
    public class AloeVeraTile : ModTile {
        public static int spawnChance;

        public override void SetStaticDefaults() {
            Main.tileNoAttach[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.CoordinateHeights = new int[2] { 16, 18 };

            TileObjectData.addTile(Type);

            DustType = DustID.OasisCactus;
            HitSound = SoundID.Grass;

            AddMapEntry(Color.Teal, TextHelper.GetItemName<AloeVera>());
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
            return true;
        }

        public static void TrySpawn(in GlobalRandomTileUpdateParams info) {
            var groundTile = Main.tile[info.X, info.Y];
            var aboveTile = Framing.GetTileSafely(info.X, info.Y);
            if (!groundTile.HasTile || groundTile.IsActuated || !Main.tileSand[groundTile.TileType] || aboveTile.HasTile) {
                return;
            }

            WorldGen.PlaceTile(info.X, info.Y - 1, ModContent.TileType<AloeVeraTile>(), mute: true);
        }
    }
}
