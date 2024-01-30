namespace Aequus.Common.Buffs;

public abstract class BaseSpecialTimerBuff : ModBuff {
    public override void SetStaticDefaults() {
        BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
    }

    public abstract System.Int32 GetTick(Player player);

    public override void Update(Player player, ref System.Int32 buffIndex) {
        System.Int32 timer = this.GetTick(player);
        if (timer <= 0) {
            return;
        }
        player.buffTime[buffIndex] = timer + 2;
    }
}