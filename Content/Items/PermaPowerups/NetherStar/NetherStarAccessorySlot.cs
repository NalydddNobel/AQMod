namespace Aequu2.Content.Items.PermaPowerups.NetherStar;

public class NetherStarAccessorySlot : ModAccessorySlot {
    public override bool IsEnabled() {
        if (Player?.TryGetModPlayer<AequusPlayer>(out var Aequu2Player) != true) {
            return false;
        }
        return Aequu2Player.usedConvergentHeart;
    }
}