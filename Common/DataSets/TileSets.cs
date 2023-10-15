using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.DataSets;
public class TileSets : DataSet {
    protected override ContentFileInfo ContentFileInfo => new(TileID.Search);

    public static readonly HashSet<int> Mechanical = new();

    public override void PostSetupContent() {
        for (int i = 0; i < TileLoader.TileCount; i++) {
            if (TileID.Sets.Torch[i] || TileID.Sets.OpenDoorID[i] > -1 || TileID.Sets.CloseDoorID[i] > -1) {
                Mechanical.Add(i);
            }
        }
    }
}