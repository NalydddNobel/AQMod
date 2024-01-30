using Aequus.Common.WorldGeneration;
using Aequus.Content.Tiles.Statues;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Aequus.Content.WorldGeneration;

public class StatuesStep : AequusGenStep {
    public override System.String InsertAfter => "Moss";

    public override void Apply(GenerationProgress progress, GameConfiguration config) {
        const System.Int32 MaximumIterations = 100000;

        System.Int32 statuesWanted = 2 + Main.maxTilesX / WorldGen.WorldSizeSmallX;
        System.Int32 statuesPlaced = 0;
        System.Int32 iterations = 0;

        System.Int32 statueType = ModContent.TileType<MossStatues>();
        do {
            System.Int32 x = Random.Next(50, Main.maxTilesX - 50);
            System.Int32 y = Random.Next((System.Int32)Main.worldSurface, Main.UnderworldLayer);
            Tile tile = Main.tile[x, y];
            if (!tile.HasUnactuatedTile) {
                continue;
            }

            System.Int32 style = tile.TileType switch {
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
