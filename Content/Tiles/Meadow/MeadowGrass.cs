using Aequus.Common.JourneyMode;
using Aequus.Common.Tiles.Components;
using Aequus.Core.ContentGeneration;
using Terraria.Audio;
using Terraria.GameContent.Metadata;

namespace Aequus.Content.Tiles.Meadow;

public class MeadowGrass : ModTile, IOverridePlacement, IOnPlaceTile {
    public override void Load() {
        ModItem item = new InstancedTileItem(this, value: Item.buyPrice(copper: 20), researchSacrificeCount: 25, journeyOverride: new JourneySortByTileId(TileID.Grass), TileIdOverride: TileID.Grass);
        Mod.AddContent(item);
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

    public bool? PlaceTile(int i, int j, bool mute, bool forced, int plr, int style) {
        Tile tile = Main.tile[i, j];
        if (!tile.HasTile || tile.TileType != TileID.Dirt) {
            return false;
        }

        tile.TileType = Type;
        SoundEngine.PlaySound(SoundID.Dig, new Vector2(i, j).ToWorldCoordinates());
        return null;
    }

    public void OverridePlacementCheck(Player player, Tile targetTile, Item item, ref int tileToCreate, ref int previewPlaceStyle, ref bool? overrideCanPlace, ref int? forcedRandom) {
        tileToCreate = Type;
    }
}
