using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Vampirism.Buffs {
    public class VampirismDay : ModBuff {
        public override void SetStaticDefaults() {
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) {
            player.Aequus().vampireDay = true;
        }
    }

    public class VampirismDayRain : ModBuff {
        public override void SetStaticDefaults() {
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) {
            player.Aequus().vampireDay = true;
            player.statDefense *= 0.4f;
            player.GetDamage(DamageClass.Generic) *= 0.5f;
            player.GetKnockback(DamageClass.Generic) *= 0.5f;
        }
    }
}