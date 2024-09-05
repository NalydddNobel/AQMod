using Aequus.Common.CodeGeneration;
using System.Runtime.CompilerServices;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Aequus.Common.Structures;
using System.IO;
using System.Collections.Generic;

namespace Aequus;

public partial class AequusSystem {
    
    [CompilerGenerated]
    private void PostUpdateWorldInner() {
        Content.World.Generation.EOCOresGenerator.CheckEoCOres();
    }
}