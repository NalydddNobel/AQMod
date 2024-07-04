using System.Runtime.CompilerServices;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using AequusRemake.Core.Structures;
using System.Collections.Generic;

namespace AequusRemake;

public partial class AequusItem {
    
    [CompilerGenerated]
    private void ModifyTooltipsInner(Item item, List<TooltipLine> tooltips) {
        Core.Debug.CheatCodes.ItemInternalNames.ShowInternalNames(item, tooltips);
    }
}