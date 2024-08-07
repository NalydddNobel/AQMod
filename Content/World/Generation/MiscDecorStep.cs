using Aequus.Systems.WorldGeneration;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.ObjectData;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace Aequus.Content.World.Generation;

public class MiscDecorStep : AGenStep {
    public override string InsertAfter => "Pots";

    public static MiscDecorStep Instance => ModContent.GetInstance<MiscDecorStep>();

    public override void Apply(GenerationProgress progress, GameConfiguration config) {
        GenTools.Random(Main.maxTilesX * Main.maxTilesY / 200, TryPlaceAPainting);
    }

    void TryPlaceAPainting(int X, int Y, UnifiedRandom rng) {
        Tile tile = Main.tile[X, Y];
        int wall = tile.WallType;

        if (wall <= WallID.None || tile.TileType > 0) {
            return;
        }

        PaintingEntry choice = GetPaintingChoice(wall, rng);

        if (choice.tileType <= 0) {
            return;
        }

        TileObjectData data = TileObjectData.GetTileData(choice.tileType, choice.style);

        int width = 1;
        int height = 1;

        if (data != null) {
            width = data.Width;
            height = data.Height;
        }

        int left = X - width / 2;
        int top = X - height / 2;

        if (TileHelper.ScanDown(new Point(X, top + height), 3, out _, TileHelper.IsSolid)) {
            return;
        }

        bool placed = GenTools.TryPlace(X, Y, choice.tileType, Style: choice.style);
#if DEBUG
        if (placed) {
            Aequus.Instance.Logger.Debug($"Placed choice {choice.tileType}, {choice.style}");
        }
#endif
    }

    PaintingEntry GetPaintingChoice(int wall, UnifiedRandom rng) {
        // Per-ID cases.
        switch (wall) {
            case WallID.MarbleUnsafe:
            case WallID.GraniteUnsafe:
            case WallID.Cave2Unsafe:
            case WallID.Cave3Unsafe:
            case WallID.Cave4Unsafe:
            case WallID.Cave5Unsafe:
            case WallID.Cave6Unsafe:
            case WallID.Cave7Unsafe:
            case WallID.Cave8Unsafe:
            case WallID.CaveUnsafe:
            case WallID.CaveWall:
            case WallID.CaveWall2:
            case WallID.RocksUnsafe1:
            case WallID.RocksUnsafe2:
            case WallID.RocksUnsafe3:
            case WallID.RocksUnsafe4: {
                    if (rng.NextBool(8)) {
                        return rng.Next(Tiles.Paintings.Paintings.Instance.Sets.RockPictures);
                    }
                }
                break;
        }

        return default;
    }

    public override void SetStaticDefaults() {

    }
}
