namespace Aequus.Common.Buffs {
    public abstract class BaseSpecialTimerBuff : ModBuff {
        public override void SetStaticDefaults() {
            BuffSets.NurseCannotRemoveDebuff[Type] = true;
        }

        public abstract int GetTick(Player player);

        public override void Update(Player player, ref int buffIndex) {
            int timer = GetTick(player);
            if (timer <= 0) {
                return;
            }
            player.buffTime[buffIndex] = timer + 2;
        }
    }
}