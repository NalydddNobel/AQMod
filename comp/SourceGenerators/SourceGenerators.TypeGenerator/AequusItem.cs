using System.Runtime.CompilerServices;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Aequus.Core.Structures;

namespace Aequus;

public partial class AequusItem {
    
    [CompilerGenerated]
    private void UseItemInner(Item item, Player player, AequusPlayer aequusPlayer) {
        Old.Content.Items.Accessories.HyperCrystal.HyperCrystal.OnUseItem(item, player, aequusPlayer);
    }
}