using Aequus.Core.Components;
using Aequus.Old.Content.Tiles.Ambient.BigGems;
using System.Collections.Generic;
using Terraria.IO;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace Aequus.Old.Content.WorldGeneration;

internal class BigGemsGeneration : AGenStep {
    private int _bigGemId;
    private List<BigGem.GemInfo> _gems;

    public override string InsertAfter => "Gem Caves";

    public override void Apply(GenerationProgress progress, GameConfiguration config) {
        _bigGemId = ModContent.TileType<BigGem>();
        _gems = ModContent.GetInstance<BigGem>()._gems;

        SetMessage(progress);
        var rand = WorldGen.genRand;
        int attempts = Main.maxTilesX * Main.maxTilesY / 80;
        if (Main.remixWorld) {
            attempts *= 6;
        }
        for (int i = 0; i < attempts; i++) {
            SetProgress(progress, i / (double)attempts);
            int x = rand.Next(100, Main.maxTilesX - 100);
            int y = rand.Next((int)Main.worldSurface, Main.maxTilesY - 100);
            TryPlaceBigGem(x, y, Random);
        }
    }

    public bool TryPlaceBigGem(int x, int y, UnifiedRandom random) {
        Tile tile = Main.tile[x, y];

        if (tile.HasTile && Main.tile[x + 1, y].IsFullySolid() && TileID.Sets.IcesSnow[tile.TileType]) {
            WorldGen.PlaceTile(x, y - 1, _bigGemId, mute: true, style: GetValidGemStyle(random));
            return Main.tile[x, y - 1].TileType == _bigGemId;
        }

        return false;
    }

    private int GetValidGemStyle(UnifiedRandom random) {
        int style;
        do {
            style = random.Next(_gems.Count);
        }
        while (!_gems[style].Loaded);
        return style;
    }
}
