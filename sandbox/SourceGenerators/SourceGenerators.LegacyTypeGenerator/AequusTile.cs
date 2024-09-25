using Aequus.Common.CodeGeneration;
using System.Runtime.CompilerServices;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Aequus.Common.Structures;
using System.IO;
using System.Collections.Generic;

namespace Aequus;

public partial class AequusTile {
    
    [CompilerGenerated]
    private void RandomUpdateInner(int i, int j, int type, int wall) {
        Content.Tiles.Herbs.Manacle.OnRandomUpdate(i, j, type, wall);
        Content.Tiles.Herbs.Mistral.OnRandomUpdate(i, j, type, wall);
    }
}