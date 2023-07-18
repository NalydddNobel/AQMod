using Terraria;

namespace Aequus.Items.Equipment.Accessories.CrownOfBlood.Buffs {
    public class CrownOfBloodDebuff : CrownOfBloodBuff {
        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            Main.debuff[Type] = true;
        }
    }
}