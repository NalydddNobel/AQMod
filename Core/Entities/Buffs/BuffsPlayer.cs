namespace Aequus.Core.Entities.Buffs;

public class BuffsPlayer : ModPlayer {
    public override void PreUpdateBuffs() {
        if (Player.whoAmI == Main.myPlayer) {
            BuffUI.DisableRightClick.Clear();
        }
    }
}
