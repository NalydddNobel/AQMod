using Aequus.Common.ContentTemplates.Generic;
using Aequus.Common.Entities.Tiles;
using Aequus.Common.Tiles;
using Terraria.Audio;
using Terraria.GameContent.Metadata;

namespace Aequus.Content.Tiles.Meadows;

public class MeadowGrass : ModTile, IOverridePlacement, IPlaceTile {
    public readonly ModItem MeadowGrassSeeds;
    public MeadowGrass() {
        MeadowGrassSeeds = new InstancedTileItem(this, Settings: new TileItemSettings() {
            Value = Item.buyPrice(copper: 20),
            Research = 25,
            TileID = TileID.Grass,
        });
    }

    public override void Load() {
        Mod.AddContent(MeadowGrassSeeds);
    }

    public override void SetStaticDefaults() {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileBrick[Type] = true;
        TileID.Sets.Grass[Type] = true;
        TileID.Sets.NeedsGrassFraming[Type] = true;
        TileID.Sets.CanBeDugByShovel[Type] = true;
        TileID.Sets.ResetsHalfBrickPlacementAttempt[Type] = true;
        TileID.Sets.ChecksForMerge[Type] = true;
        TileID.Sets.SpreadOverground[Type] = true;
        TileID.Sets.DoesntPlaceWithTileReplacement[Type] = true;
        TileID.Sets.Conversion.MergesWithDirtInASpecialWay[Type] = true;
        TileID.Sets.Conversion.Grass[Type] = true;
        TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Grass"]);

        MineResist = 100f;
        AddMapEntry(new Color(106, 188, 170));
        RegisterItemDrop(ItemID.DirtBlock);
    }

    public override void RandomUpdate(int i, int j) {
        if (Main.tile[i, j - 1].HasTile || !WorldGen.genRand.NextBool(20)) {
            return;
        }

        // Stupid hack.
        Main.tile[i, j].TileType = TileID.Grass;
        WorldGen.PlaceTile(i, j - 1, TileID.Plants, mute: true);

        Main.tile[i, j].TileType = Type;
        Main.tile[i, j - 1].TileType = (ushort)ModContent.TileType<MeadowPlants>();
    }

    public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem) {
        if (fail && !effectOnly) {
            Main.tile[i, j].TileType = TileID.Dirt;
        }
    }

    bool? IPlaceTile.ModifyPlaceTile(ref PlaceTileInfo info) {
        Tile tile = info.Tile;
        if (!tile.HasTile || tile.TileType != TileID.Dirt) {
            return false;
        }

        tile.TileType = Type;
        SoundEngine.PlaySound(SoundID.Dig, new Vector2(info.X, info.Y).ToWorldCoordinates());
        return null;
    }

    public void OverridePlacementCheck(Player player, Tile targetTile, Item item, ref int tileToCreate, ref int previewPlaceStyle, ref bool? overrideCanPlace, ref int? forcedRandom) {
        tileToCreate = Type;
    }
}
