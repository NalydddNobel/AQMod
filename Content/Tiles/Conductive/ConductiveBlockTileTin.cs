using Aequus.Content.Tiles.Conductive.Items;
using Terraria.ID;

namespace Aequus.Content.Tiles.Conductive;

public class ConductiveBlockTileTin : ConductiveBlockTile {
    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        DustType = DustID.Tin;
    }

    protected override void AddMapEntries() {
        AddMapEntry(new(187, 165, 124), TextHelper.GetDisplayName<ConductiveBlockTin>());
    }
}