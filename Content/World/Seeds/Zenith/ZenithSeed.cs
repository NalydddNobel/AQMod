using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Aequus.Content.World.Seeds.Zenith {
    public class ZenithSeed : ModSystem {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight) {
            //AequusWorldGenerator.AddPass("Micro Biomes", "Zenith Seed", (progress, configuration) => {
            //    var gravBlock = (ushort)ModContent.TileType<GravityBlockTile>();
            //    var antiGravBlock = (ushort)ModContent.TileType<AntiGravityBlockTile>();
            //    for (int i = 0; i < Main.maxTilesX; i++) {
            //        for (int j = 0; j < Main.maxTilesY; j++) {
            //            var tile = Main.tile[i, j];
            //            if (Main.tileDungeon[tile.TileType]) {
            //                if (Framing.GetTileSafely(i, j + 1).IsNotSolid()) {
            //                    tile.TileType = antiGravBlock;
            //                }
            //                else if (Framing.GetTileSafely(i, j - 1).IsNotSolid()) {
            //                    tile.TileType = gravBlock;
            //                }
            //            }
            //        }
            //    }
            //}, tasks);
        }
    }
}