using Aequus.Content.Tiles.Meadow;
using Aequus.Core.WorldGeneration;
using System;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Aequus.Content.WorldGeneration;

public class SkyMeadowsStep : AequusGenStep {
    public override string InsertAfter => "Floating Islands";

    public override void Apply(GenerationProgress progress, GameConfiguration config) {
        int length = Math.Min(GenVars.floatingIslandHouseX.Length, GenVars.floatingIslandHouseY.Length);

        ushort skyGrass = (ushort)ModContent.TileType<MeadowGrass>();
        float inWorldPercent = 0.33f;
        int grassBlocksGenerated = 0;
        while (grassBlocksGenerated == 0) {
            for (int i = 0; i < length; i++) {
                int x = GenVars.floatingIslandHouseX[i];
                int y = GenVars.floatingIslandHouseY[i];

                if ((x == 0 && y == 0) || !Helper.InOuterPercentOfWorld(x, inWorldPercent)) {
                    continue;
                }

                int startX = Math.Max(x - 50, 0);
                int endX = Math.Min(x + 50, Main.maxTilesX);
                int startY = Math.Max(y - 50, 0);
                int endY = Math.Min(y + 50, Main.maxTilesY);

                int left = endX;
                int right = startX;
                int top = endY;
                int bottom = startY;

                for (int k = startX; k < endX; k++) {
                    for (int l = startY; l < endY; l++) {
                        Tile tile = Main.tile[k, l];
                        if (tile.TileType != TileID.Cloud && tile.TileType != TileID.RainCloud && tile.TileType != TileID.SnowCloud) {
                            continue;
                        }

                        right = Math.Max(k, right);
                        left = Math.Min(k, left);
                        bottom = Math.Max(l, bottom);
                        top = Math.Min(l, top);
                    }
                }
                for (int k = left; k < right; k++) {
                    for (int l = top; l < bottom; l++) {
                        Tile tile = Main.tile[k, l];
                        if (tile.TileType == TileID.Dirt) {
                            WorldGen.SpreadGrass(k, l, TileID.Dirt, skyGrass, repeat: false);
                            if (tile.TileType == skyGrass) {
                                grassBlocksGenerated++;
                            }
                        }
                    }
                }

                //WorldGen.PlaceTile(left, top, TileID.SnowCloud);
                //WorldGen.PlaceTile(right, top, TileID.SnowCloud);
                //WorldGen.PlaceTile(left, bottom, TileID.SnowCloud);
                //WorldGen.PlaceTile(right, bottom, TileID.SnowCloud);
            }

            inWorldPercent += 0.02f;
            if (inWorldPercent >= 0.5f) {
                break;
            }
        }
    }
}
