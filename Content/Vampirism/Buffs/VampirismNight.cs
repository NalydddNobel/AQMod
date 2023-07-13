using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Vampirism.Buffs {
    public class VampirismNight : ModBuff {
        public override void SetStaticDefaults() {
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) {
            player.Aequus().vampireNight = true;
            player.GetDamage(DamageClass.Generic) += 0.1f;
            player.moveSpeed *= 1.5f;
            player.accRunSpeed *= 1.5f;
            player.pickSpeed *= 1.5f;
            player.jumpSpeedBoost *= 1.5f;
            player.Aequus().ghostSlotsMax += 2;
        }
    }

    public class VampirismNightEclipse : VampirismNight {
    }
}