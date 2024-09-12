#if POLLUTED_OCEAN
using Aequus.Common.ContentTemplates.Tiles.Rubblemaker;
using Aequus.Content.Tiles.PollutedOcean.PolymerSands;
using Terraria.GameContent;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.PollutedOcean.Ambient.SeaPickles;

internal class SeaPickles1x1 : SeaPicklesTileBase {
    public SeaPickles1x1() : base() { }
    public SeaPickles1x1(string name, bool natural) : base(name, natural) { }

    public override int UseItem => Instance<PolymerSand>().Item.Type;

    public override int[] Styles => [0, 1, 2];

    public override FlexibleTileWand Size => FlexibleTileWand.RubblePlacementSmall;

    public override void Load(RubblemakerTile rubblemakerCopy) {
#if POLLUTED_OCEAN_TODO
        ModTypeLookup<ModTile>.RegisterLegacyNames(this, "SeaPickle", "SeaPickleTile");
#endif
    }

    public override void SafeSetStaticDefaults() {
        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
        base.SafeSetStaticDefaults();
        TileObjectData.addTile(Type);

        LightMagnitudeMultiplier = 0.5f;
    }
}
#endif