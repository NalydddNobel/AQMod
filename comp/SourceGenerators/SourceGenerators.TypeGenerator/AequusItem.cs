using System.Runtime.CompilerServices;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Aequus.Core.Structures;
using System.Collections.Generic;

namespace Aequus;

public partial class AequusItem {
    
    [CompilerGenerated]
    private void ModifyTooltipsInner(Item item, List<TooltipLine> tooltips) {
        Core.Debugging.CheatCodes.CheatCode.ShowInternalNames(item, tooltips);
    }
}