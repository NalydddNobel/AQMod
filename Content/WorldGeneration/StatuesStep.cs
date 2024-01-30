using Aequus.Common.WorldGeneration;
using Aequus.Content.Tiles.Statues;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Aequus.Content.WorldGeneration;

public class StatuesStep : AequusGenStep {
    public override string InsertAfter => "Moss";

    public override void Apply(GenerationProgress progress, GameConfiguration config) {
        const int MaximumIterations = 100000;

        int statuesWanted = 2 + Main.maxTilesX / WorldGen.WorldSizeSmallX;
        int statuesPlaced = 0;
        int iterations = 0;

        int statueType = ModContent.TileType<MossStatues>();
        do {
            int x = Random.Next(50, Main.maxTilesX - 50);
            int y = Random.Next((int)Main.worldSurface, Main.UnderworldLayer);
            Tile tile = Main.tile[x, y];
            if (!tile.HasUnactuatedTile) {
                continue;
            }

            int style = tile.TileType switch {
                TileID.ArgonMoss => MossStatues.STYLE_ARGON,
                TileID.KryptonMoss => MossStatues.STYLE_KRYPTON,
                TileID.VioletMoss => MossStatues.STYLE_NEON,
                TileID.XenonMoss => MossStatues.STYLE_XENON,
                _ => -1,
            };

            Tile topTile = Main.tile[x, y - 1];
            if (topTile.HasTile && !Main.tileSolid[topTile.TileType] && Main.tileCut[topTile.TileType]) {
                WorldGen.KillTile(x, y - 1);
            }
            if (style > -1 && !topTile.HasTile) {
                WorldGen.PlaceTile(x, y - 1, statueType, style: style);
                if (topTile.HasTile && topTile.TileType == statueType) {
                    statuesPlaced++;
                }
            }
        }
        while (statuesPlaced < statuesWanted && ++iterations < MaximumIterations);
    }
}
