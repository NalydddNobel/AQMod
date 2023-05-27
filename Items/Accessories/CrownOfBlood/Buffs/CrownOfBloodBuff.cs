using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.CrownOfBlood.Buffs {
    public class CrownOfBloodBuff : ModBuff {
        public override void SetStaticDefaults() {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare) {
            rare = ItemRarityID.LightRed;
            var item = Main.LocalPlayer.Aequus().accCrownOfBloodItemClone;
            if (item == null || item.IsAir
                || item.ToolTip == null || item.ToolTip.Lines <= 0
                || CrownOfBloodItem.NoBoost.Contains(item.type)
                || !CrownOfBloodItem.GetCrownOfBloodTooltip(item, out string tooltip)) {
                tip = string.Format(tip, AequusLocalization.NoItem);
                return;
            }

            tip = string.Format(tip, tooltip);
        }

        public override bool RightClick(int buffIndex) {
            return false;
        }
    }
    public class CrownOfBloodDebuff : CrownOfBloodBuff {
        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            Main.debuff[Type] = true;
        }
    }
}