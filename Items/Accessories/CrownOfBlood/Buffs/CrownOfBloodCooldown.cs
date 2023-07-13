using Aequus.Common.Buffs;
using Terraria;

namespace Aequus.Items.Accessories.CrownOfBlood.Buffs {
    public class CrownOfBloodCooldown : BaseSpecialTimerBuff {
        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            Main.buffNoSave[Type] = true;
        }

        public override int GetTick(Player player) {
            return player.Aequus().crownOfBloodCD;
        }

        public override bool RightClick(int buffIndex) {
            return false;
        }
    }
}