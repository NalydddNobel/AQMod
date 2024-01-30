using Aequus.Common.WorldGeneration;
using Aequus.Old.Content.Events.DemonSiege.Tiles;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Aequus.Old.Content.WorldGeneration;
public class LavaCleanup : AequusGenStep {
    public override System.String InsertAfter => "Tile Cleanup";

    public override void Apply(GenerationProgress progress, GameConfiguration config) {
        GoreNestsGeneration.GetGenerationValues(out System.Int32 minY, out System.Int32 maxY, out _);
        for (System.Int32 i = 0; i < Main.maxTilesX; i++) {
            for (System.Int32 j = minY; j < maxY; j++) {
                if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == ModContent.TileType<OblivionAltar>()) {
                    CleanLava(i, j);
                }
            }
        }
    }

    private static void CleanLava(System.Int32 x, System.Int32 y) {
        for (System.Int32 i = x - 60; i < x + 60; i++) {
            for (System.Int32 j = y - 50; j < Main.maxTilesY && !Main.tile[i, j].IsSolid(); j++) {
                Main.tile[i, j].LiquidAmount = 0;
            }
        }
    }

}
