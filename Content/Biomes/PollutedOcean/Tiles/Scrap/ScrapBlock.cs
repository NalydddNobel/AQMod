using Aequus.Common.Tiles;
using Aequus.Common.Tiles.Components;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Biomes.PollutedOcean.Tiles.Scrap;

public class ScrapBlock : AequusModTile, ITouchEffects {
    public override void SetStaticDefaults() {
        Main.tileSolid[Type] = false;
        Main.tileBlockLight[Type] = true;
        Main.tileMerge[Type][TileID.Sand] = true;
        Main.tileMerge[Type][TileID.HardenedSand] = true;
        Main.tileMerge[TileID.Sand][Type] = true;
        Main.tileMerge[TileID.HardenedSand][Type] = true;
        TileID.Sets.DoesntPlaceWithTileReplacement[Type] = true;
        TileID.Sets.CanPlaceNextToNonSolidTile[Type] = true;
        var name = this.GetLocalization("DisplayName", () => "Scrap");
        AddMapEntry(new(169, 73, 43), name);
        AddMapEntry(new(141, 87, 70), name);
        AddMapEntry(new(90, 109, 71), name);
        AddMapEntry(new(77, 86, 70), name);
        AddMapEntry(new(110, 92, 87), name);
        DustType = DustID.Sand;
        HitSound = SoundID.Tink;
        MineResist = 0.5f;
    }

    public override ushort GetMapOption(int i, int j) {
        var seed = Helper.TileSeed(i, j);
        return (ushort)Utils.RandomInt(ref seed, 0, 5);
    }

    public void Touch(int i, int j, Player player, AequusPlayer aequusPlayer) {
        aequusPlayer.touchingScrapBlock = true;
    }
}