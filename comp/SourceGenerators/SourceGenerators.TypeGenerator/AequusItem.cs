using System.Runtime.CompilerServices;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Aequus.Core.Structures;
using System.Collections.Generic;

namespace Aequus;

public partial class AequusItem {
    
    [CompilerGenerated]
    private void ModifyTooltipsInner(Item item, List<TooltipLine> tooltips) {
        Core.Debug.CheatCodes.ItemInternalNames.ShowInternalNames(item, tooltips);
    }
    
    [CompilerGenerated]
    private void UseItemInner(Item item, Player player, AequusPlayer aequusPlayer) {
        Old.Content.Items.Accessories.HyperCrystal.HyperCrystal.OnUseItem(item, player, aequusPlayer);
    }
}