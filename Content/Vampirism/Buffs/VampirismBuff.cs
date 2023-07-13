using Aequus.Common.Buffs;
using Aequus.Common.DataSets;
using Terraria;
using Terraria.ID;

namespace Aequus.Content.Vampirism.Buffs {
    public class VampirismBuff : BaseSpecialTimerBuff {
        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;

            BuffSets.ClearableDebuff.Add(Type);
            BuffSets.PotionPrefixBlacklist.Add(Type);
        }

        public override int GetTick(Player player) {
            return player.GetModPlayer<AequusPlayer>()._vampirismData;
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