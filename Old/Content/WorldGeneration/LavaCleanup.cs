using Aequus.Common.WorldGeneration;
using Aequus.Old.Content.Events.DemonSiege.Tiles;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Aequus.Old.Content.WorldGeneration;
public class LavaCleanup : AequusGenStep {
    public override string InsertAfter => "Tile Cleanup";

    public override void Apply(GenerationProgress progress, GameConfiguration config) {
        GoreNestsGeneration.GetGenerationValues(out int minY, out int maxY, out _);
        for (int i = 0; i < Main.maxTilesX; i++) {
            for (int j = minY; j < maxY; j++) {
                if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == ModContent.TileType<OblivionAltar>()) {
                    CleanLava(i, j);
                }
            }
        }
    }

    private static void CleanLava(int x, int y) {
        for (int i = x - 60; i < x + 60; i++) {
            for (int j = y - 50; j < Main.maxTilesY && !Main.tile[i, j].IsSolid(); j++) {
                Main.tile[i, j].LiquidAmount = 0;
            }
        }
    }

}
