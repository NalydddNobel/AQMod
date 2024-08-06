using Aequus.Systems.Chests;
using Aequus.Systems.Chests.DropRules;
using Aequus.Systems.WorldGeneration;
using Aequus.Tiles.Paintings.Canvas2x2;
using Aequus.Tiles.Paintings.Canvas2x3;
using Aequus.Tiles.Paintings.Canvas3x2;
using Aequus.Tiles.Paintings.Canvas3x3;
using Aequus.Tiles.Paintings.Canvas6x4;
using System.Collections.Generic;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.ObjectData;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace Aequus.Content.World.Generation;

public class MiscDecorStep : AGenStep {
    public override string InsertAfter => "Pots";

    public static MiscDecorStep Instance => ModContent.GetInstance<MiscDecorStep>();

    public readonly List<PaintingEntry> RockPictures = [];

    public readonly List<PaintingEntry> GenericPictures = [];

    public readonly List<PaintingEntry> DesertPictures = [];

    public readonly List<PaintingEntry> DungeonPictures = [];

    public readonly List<PaintingEntry> HellPictures = [];

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
                        return rng.Next(RockPictures);
                    }
                }
                break;
        }

        return default;
    }

    public override void Load() {
        On_WorldGen.RandHousePicture += On_WorldGen_RandHousePicture;
        On_WorldGen.RandHousePictureDesert += On_WorldGen_RandHousePictureDesert;
        On_WorldGen.RandBonePicture += On_WorldGen_RandBonePicture;
        On_WorldGen.RandHellPicture += On_WorldGen_RandHellPicture;
    }

    private PaintingEntry On_WorldGen_RandHellPicture(On_WorldGen.orig_RandHellPicture orig) {
        if (WorldGen.genRand.NextBool(5)) {
            return WorldGen.genRand.Next(Instance.HellPictures);
        }

        return orig();
    }

    private static PaintingEntry On_WorldGen_RandBonePicture(On_WorldGen.orig_RandBonePicture orig) {
        if (WorldGen.genRand.NextBool(3)) {
            return WorldGen.genRand.Next(Instance.DungeonPictures);
        }

        return orig();
    }

    private static PaintingEntry On_WorldGen_RandHousePictureDesert(On_WorldGen.orig_RandHousePictureDesert orig) {
        if (WorldGen.genRand.NextBool(3)) {
            return WorldGen.genRand.Next(Instance.DesertPictures);
        }

        return orig();
    }

    private static PaintingEntry On_WorldGen_RandHousePicture(On_WorldGen.orig_RandHousePicture orig) {
        if (WorldGen.genRand.NextBool(4)) {
            return WorldGen.genRand.Next(Instance.GenericPictures);
        }

        return orig();
    }

    public override void SetStaticDefaults() {
        // Add sky paintings to skyware chest loot pool.
        ChestLootDatabase.Instance.RegisterIndexed(2, ChestPool.Sky, [
            new CommonChestRule(ModContent.ItemType<ExLydSpacePainting>()),
            new CommonChestRule(ModContent.ItemType<OmegaStaritePainting>()),
            new CommonChestRule(ModContent.ItemType<OmegaStaritePainting2>()),
            new CommonChestRule(ModContent.ItemType<HomeworldPainting>())
        ]);

        // Generic paintings which spawn in underground houses.
        GenericPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings3x2>(), style = WallPaintings3x2.BongBongPainting });
        GenericPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings3x2>(), style = WallPaintings3x2.OliverPainting });
        GenericPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings6x4>(), style = WallPaintings6x4.BreadRoachPainting });

        // Paintings which spawn on rock walls.
        RockPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings3x2>(), style = WallPaintings3x2.Fus });
        RockPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings3x2>(), style = WallPaintings3x2.Ro });
        RockPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings3x2>(), style = WallPaintings3x2.DAH });

        // Paintings which spawn in the Desert.
        DesertPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings3x2>(), style = WallPaintings3x2.YinYangPainting });
        DesertPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings2x2>(), style = WallPaintings2x2.YinPainting });
        DesertPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings2x2>(), style = WallPaintings2x2.YangPainting });

        // Paintings which spawn in the Dungeon.
        DungeonPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings2x3>(), style = WallPaintings2x3.NarryPainting });
        DungeonPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings3x3>(), style = WallPaintings3x3.RockFromAnAlternateUniversePainting });

        // Paintings which spawn in the Underworld.
        HellPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings3x3>(), style = WallPaintings3x3.GoreNestPainting });
        HellPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings3x3>(), style = WallPaintings3x3.GoreNest2Painting });
        HellPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings3x3>(), style = WallPaintings3x3.InsurgentPainting });
    }
}
