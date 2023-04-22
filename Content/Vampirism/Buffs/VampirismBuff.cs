using Aequus.Buffs;
using Terraria;

namespace Aequus.Content.Vampirism.Buffs {
    public class VampirismBuff : BaseSpecialTimerBuff {
        public override int GetTick(Player player) {
            return player.GetModPlayer<AequusPlayer>()._vampirismData;
        }

        public override void SetStaticDefaults() {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            AequusBuff.ConcoctibleBuffIDsBlacklist.Add(Type);
        }

        public override void Update(Player player, ref int buffIndex) {
            if (player.GetModPlayer<AequusPlayer>().IsVampire) {
                player.DelBuff(buffIndex);
                buffIndex--;
                return;
            }
            base.Update(player, ref buffIndex);
        }
    }
}