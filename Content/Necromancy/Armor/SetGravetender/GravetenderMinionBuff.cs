namespace Aequus.Content.Necromancy.Armor.SetGravetender;

public class GravetenderMinionBuff : ModBuff {
    public override void SetStaticDefaults() {
        Main.buffNoTimeDisplay[Type] = true;
    }

    public override bool RightClick(int buffIndex) {
        return false;
    }
}