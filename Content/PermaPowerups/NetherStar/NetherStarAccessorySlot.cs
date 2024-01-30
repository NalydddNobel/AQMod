namespace Aequus.Content.PermaPowerups.NetherStar;

public class NetherStarAccessorySlot : ModAccessorySlot {
    public override System.Boolean IsEnabled() {
        if (Player?.TryGetModPlayer<AequusPlayer>(out var aequusPlayer) != true) {
            return false;
        }
        return aequusPlayer.usedConvergentHeart;
    }
}