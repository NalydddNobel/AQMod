using Terraria.ModLoader;

namespace Aequus.Content.Items.Misc.PermaPowerups.NetherStar;

public class NetherStarAccessorySlot : ModAccessorySlot {
    public override bool IsEnabled() {
        if (Player?.TryGetModPlayer<AequusPlayer>(out var aequusPlayer) != true) {
            return false;
        }
        return aequusPlayer.yinYangBonusSlot;
    }
}