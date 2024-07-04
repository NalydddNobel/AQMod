namespace AequusRemake.Content.Items.PermaPowerups.NetherStar;

public class NetherStarAccessorySlot : ModAccessorySlot {
    public override bool IsEnabled() {
        if (Player?.TryGetModPlayer<AequusPlayer>(out var AequusRemakePlayer) != true) {
            return false;
        }
        return AequusRemakePlayer.usedConvergentHeart;
    }
}