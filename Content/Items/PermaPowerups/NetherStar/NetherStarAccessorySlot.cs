using Aequus.Core;
using Terraria.ModLoader;

namespace Aequus.Content.Items.PermaPowerups.NetherStar;

public class NetherStarAccessorySlot : ModAccessorySlot {
    public override bool IsLoadingEnabled(Mod mod) {
        return LiteAttribute.LiteCheck(this);
    }

    public override bool IsEnabled() {
        if (Player?.TryGetModPlayer<AequusPlayer>(out var aequusPlayer) != true) {
            return false;
        }
        return aequusPlayer.usedConvergentHeart;
    }
}