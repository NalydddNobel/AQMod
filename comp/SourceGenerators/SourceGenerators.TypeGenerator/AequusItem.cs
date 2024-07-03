using System.Runtime.CompilerServices;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Aequu2.Core.Structures;
using System.Collections.Generic;

namespace Aequu2;

public partial class AequusItem {
    
    [CompilerGenerated]
    private void ModifyTooltipsInner(Item item, List<TooltipLine> tooltips) {
        Core.Debug.CheatCodes.ItemInternalNames.ShowInternalNames(item, tooltips);
    }
}