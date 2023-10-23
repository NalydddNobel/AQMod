using Aequus.Core;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Tiles.Conductive.Items;

[WorkInProgress]
public class ConductiveBlockTin : ConductiveBlock {
    public override int BarItem => ItemID.TinBar;
    public override int TileId => ModContent.TileType<ConductiveBlockTileTin>();
}