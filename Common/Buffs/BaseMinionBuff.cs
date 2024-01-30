namespace Aequus.Common.Buffs;

public abstract class BaseMinionBuff : ModBuff {
    protected abstract System.Int32 MinionProj { get; }

    public override void SetStaticDefaults() {
        Main.buffNoSave[this.Type] = true;
        Main.buffNoTimeDisplay[this.Type] = true;
    }

    public override void Update(Player player, ref System.Int32 buffIndex) {
        if (player.ownedProjectileCounts[MinionProj] > 0) {
            player.buffTime[buffIndex] = 18000;
        }
        else {
            player.DelBuff(buffIndex);
            buffIndex--;
        }
    }
}