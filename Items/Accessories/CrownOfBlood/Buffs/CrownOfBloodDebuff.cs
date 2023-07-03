using Terraria;

namespace Aequus.Items.Accessories.CrownOfBlood.Buffs {
    public class CrownOfBloodDebuff : CrownOfBloodBuff {
        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            Main.debuff[Type] = true;
        }
    }
}