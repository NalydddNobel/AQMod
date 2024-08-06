using System;
using Terraria.Utilities;

namespace Aequus.Systems.WorldGeneration;

public static class GenTools {
    public static void Random(int Amount, Action<int, int, UnifiedRandom> Action, int? minX = null, int? maxX = null, int? minY = null, int? maxY = null) {
        int left = minX ?? 40;
        int right = maxX ?? Main.maxTilesX - 40;
        int top = minY ?? 40;
        int bottom = maxY ?? Main.maxTilesY - 40;
        UnifiedRandom rng = WorldGen.genRand;

        for (int i = 0; i < Amount; i++) {
            int x = rng.Next(left, right);
            int y = rng.Next(top, bottom);

            Action(x, y, rng);
        }
    }

    public static bool TryPlace(int I, int J, int Type, bool Forced = false, int Style = 0) {
        WorldGen.PlaceTile(I, J, Type, mute: false, forced: Forced, style: Style);

        Tile tile = Main.tile[I, J];
        return tile.TileType == Type;
    }

    public static bool TryPlace<T>(int I, int J, bool Forced = false, int Style = 0) where T : ModTile {
        return TryPlace(I, J, ModContent.TileType<T>(), Forced, Style);
    }

    internal static void TryPlace<T>(int x, int y, object Style) {
        throw new NotImplementedException();
    }
}
