using Aequus.Common.Tiles.Global;
using Aequus.Items.Accessories.Life;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
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

            TileID.Sets.SwaysInWindBasic[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.CoordinateHeights = new int[1] { 32 };
            TileObjectData.newTile.CoordinateWidth = 26;
            TileObjectData.newTile.DrawYOffset = -12;

            TileObjectData.addTile(Type);

            DustType = DustID.OasisCactus;
            HitSound = SoundID.Grass;

            AddMapEntry(Color.Teal, TextHelper.GetItemName<AloeVera>());
        }

        public static void TrySpawn(in GlobalRandomTileUpdateParams info) {
            var groundTile = Main.tile[info.X, info.Y];
            var aboveTile = Framing.GetTileSafely(info.X, info.Y - 1);
            if (WorldGen.oceanDepths(info.X, info.Y) || !groundTile.HasTile || groundTile.IsActuated || !Main.tileSand[groundTile.TileType] || aboveTile.HasTile || groundTile.HasAnyLiquid() || aboveTile.HasAnyLiquid() || TileHelper.ScanTiles(new(info.X - 150, info.Y - 25, 300, 50), TileHelper.HasTileAction(ModContent.TileType<AloeVeraTile>()))) {
                return;
            }

            Main.NewText(AequusWorld.aloeFrenzy);
            Main.LocalPlayer.Teleport(new Vector2(info.X, info.Y).ToWorldCoordinates());
            WorldGen.PlaceTile(info.X, info.Y - 1, ModContent.TileType<AloeVeraTile>(), mute: true);
            AequusWorld.aloeFrenzy = false;
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j) {
            return new Item[] { new(ModContent.ItemType<AloeVera>()) };
        }
    }
}