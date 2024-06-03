namespace Aequus.Common.Buffs;

public class StatusEffectsPlayer : ModPlayer {
    public override void PreUpdateBuffs() {
        if (Player.whoAmI == Main.myPlayer) {
            BuffUI.DisableRightClick.Clear();
        }
    }
}
