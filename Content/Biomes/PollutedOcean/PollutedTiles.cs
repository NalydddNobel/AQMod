using Aequus.Common;
using System.Collections.Generic;

namespace Aequus.Content.Biomes.PollutedOcean;

public class PollutedTiles : LoadedType {
    public HashSet<int> IsPolluted = [];
    public HashSet<int> RemoveFromGen = [TileID.Pots, TileID.Coral];
}
